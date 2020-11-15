using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    /*
    This calculation of Excite-O-Meter level is performed once every time that
    a session has reached an end. It consists on the z-normalization of
    the RMSSD and RRi data.
    */
    public class ExciteOMeterCalculation : MonoBehaviour
    {
        public static bool Calculate(SessionVariables sessionData)
        {
            // Check that both arrays are the same length
            int N = sessionData.RMSSD.timestamp.Count;

            // Assess length of each timeseries, by default are assummed equal
            bool equalLength = true, useRRi=false, useRMSSD=false;
            if(sessionData.RRi.timestamp.Count != sessionData.RMSSD.timestamp.Count)
            {
                // timeseries are different length
                equalLength = false;
                N = Math.Min(sessionData.RRi.timestamp.Count, sessionData.RMSSD.timestamp.Count);

                // Which is the shorter signal?
                if (N == sessionData.RRi.timestamp.Count)
                    useRRi=true;
                else if (N == sessionData.RMSSD.timestamp.Count)
                    useRMSSD = true;
            }
            
            // Mean values
            float meanRRi = 0f;
            float meanRMSSD = 0f;
            for (int i = 0; i < N; i++)
            {
                // Check timestamps are similar :: +/-10ms offset
                if(Math.Abs(sessionData.RRi.timestamp[i]-sessionData.RMSSD.timestamp[i]) > 0.1f)
                {
                    ExciteOMeterManager.DebugLog("WARNING: Two timestamps differ in more than 0.1s");
                }

                // Cumulative sum
                meanRRi += sessionData.RRi.value[i];
                meanRMSSD += sessionData.RMSSD.value[i];
            }
            meanRRi = meanRRi/N;
            meanRMSSD = meanRMSSD/N;

            // Standard deviation
            float stdRRi = 0f;
            float stdRMSSD = 0f;
            for (int i = 0; i < N; i++)
            {
                stdRRi += (float)Math.Pow(sessionData.RRi.value[i] - meanRRi, 2);
                stdRMSSD += (float)Math.Pow(sessionData.RMSSD.value[i] - meanRMSSD, 2);
            }
            stdRRi = (float)Math.Sqrt(stdRRi/N);
            stdRMSSD = (float)Math.Sqrt(stdRMSSD/N);

            // Placeholder for z-score
            double zScoreRRi = 0f, percentileRRi = 0f;
            double zScoreRMSSD = 0f, percentileRMSSD = 0f;

            // Final EoM level
            float timestampEOM = 0.0f;
            float valueEOM = 0.0f;

            // Calculate final EoM level
            for (int i = 0; i < N; i++)
            {
                zScoreRRi   = (double)((sessionData.RRi.value[i] - meanRRi)/stdRRi);
                zScoreRMSSD = (double)((sessionData.RMSSD.value[i] - meanRMSSD)/stdRMSSD);

                percentileRRi = Phi(zScoreRRi);
                percentileRMSSD = Phi(zScoreRMSSD);

                // Average of percentile of RRI and percentile of RMSSD
                valueEOM = (1 - (float)(percentileRRi + percentileRMSSD)/2.0f);

                if(equalLength)
                    timestampEOM = (sessionData.RRi.timestamp[i]);
                else if(useRRi)
                    timestampEOM = (sessionData.RRi.timestamp[i]);
                else if(useRMSSD)
                    timestampEOM = (sessionData.RMSSD.timestamp[i]);

                // Add to class that is exported as JSON
                SessionVariablesController.instance.WritePostProcessedExciteOMeterIndex(timestampEOM, valueEOM);
                // Add to CSV file
                LoggerController.instance.WriteLine(LogName.EOM, timestampEOM.ToString("F6") + "," + valueEOM.ToString("F5"));
            }
            
            return true;
        }

        // CDF Gaussian random variable
        static double Phi(double x)
        {
            // SOURCE: https://www.johndcook.com/blog/csharp_phi/
            /*
            The function Φ(x) is the cumulative density function 
            (CDF) of a standard normal (Gaussian) random variable. 
            It is closely related to the error function erf(x).
            */

            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
                
            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);
                
            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p*x);
            double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x);
                
            return 0.5 * (1.0 + sign*y);
        }
    }
}


