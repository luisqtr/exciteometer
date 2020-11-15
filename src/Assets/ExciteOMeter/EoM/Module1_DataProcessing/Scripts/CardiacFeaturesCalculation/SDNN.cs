using System;
using UnityEngine;

namespace ExciteOMeter
{
    public class SDNN : EoM_Base_FeatureCalculation
    {
        int length;
        float cumsum;
        float mean;
        float result;

        protected override void SetupStart()
        {
            isTimeBasedFeature = !SettingsManager.Values.featureSettings.SDNN.isSampleBased;
            windowTime = SettingsManager.Values.featureSettings.SDNN.windowTime;
            overlappingFraction = SettingsManager.Values.featureSettings.SDNN.overlapPercentageTime;

            isSampleBasedFeature = SettingsManager.Values.featureSettings.SDNN.isSampleBased;
            sampleBuffer = SettingsManager.Values.featureSettings.SDNN.sampleBufferLength;
            overlappingSamples = SettingsManager.Values.featureSettings.SDNN.overlapSamplesLength;
            offsetSamplesTimestamp = SettingsManager.Values.featureSettings.SDNN.offsetSamplesInTimestamp;
        }

        protected override float CalculateFeature(float[] timestamps, float[] values)
        {
            /*
            REFERENCE: https://www.frontiersin.org/articles/10.3389/fpubh.2017.00258/full
            
            SDNN
            The standard deviation of the IBI of normal sinus beats (SDNN) is measured in ms. "Normal" means that abnormal beats, like ectopic beats (heartbeats that originate outside the right atrium’s sinoatrial node), have been removed. While the conventional short-term recording standard is 5 min (11), researchers have proposed ultra-short-term recording periods from 60 s (30) to 240 s (31). The related standard deviation of successive RR interval differences (SDSD) only represents short-term variability (9).
            */
            length = values.Length;
            cumsum = 0;
            for (int i = 0; i < length; i++)
            {
                cumsum += values[i];
            }
            mean = cumsum/length;

            // Difference respect to mean
            cumsum = 0;
            for (int i = 0; i < length; i++)
            {
                cumsum += (float)Math.Pow(values[i] - mean, 2);
            }

            // Average over N-1 to try to generate an unbiased estimator of STD.
            // Which is how the Python lib: NeuroKit2 calculates SDNN
            // See NK source code: https://github.com/neuropsychology/NeuroKit/blob/master/neurokit2/hrv/hrv_time.py
            // Ultimately it does not provide an unbiased estimator because of the sqrt
            // Read more at: https://docs.scipy.org/doc/numpy-1.16.1/reference/generated/numpy.nanstd.html
            result = (float)Math.Sqrt(cumsum/(length - 1) ); 

            return result;
        }
    }
}


