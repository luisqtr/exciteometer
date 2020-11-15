using System;
using UnityEngine;

namespace ExciteOMeter
{
    public class RMSSD : EoM_Base_FeatureCalculation
    {
        int length, N;
        float diff;
        float cumsum;
        float result;

        protected override void SetupStart()
        {
            isTimeBasedFeature = !SettingsManager.Values.featureSettings.RMSSD.isSampleBased;
            windowTime = SettingsManager.Values.featureSettings.RMSSD.windowTime;
            overlappingFraction = SettingsManager.Values.featureSettings.RMSSD.overlapPercentageTime;

            isSampleBasedFeature = SettingsManager.Values.featureSettings.RMSSD.isSampleBased;
            sampleBuffer = SettingsManager.Values.featureSettings.RMSSD.sampleBufferLength;
            overlappingSamples = SettingsManager.Values.featureSettings.RMSSD.overlapSamplesLength;
            offsetSamplesTimestamp = SettingsManager.Values.featureSettings.RMSSD.offsetSamplesInTimestamp;
        }

        protected override float CalculateFeature(float[] timestamps, float[] values)
        {
            /*
            REFERENCE: https://www.frontiersin.org/articles/10.3389/fpubh.2017.00258/full
            
            RMSSD
            The root mean square of successive differences between normal heartbeats (RMSSD) is obtained by first calculating each successive time difference between heartbeats in ms. Then, each of the values is squared and the result is averaged before the square root of the total is obtained. While the conventional minimum recording is 5 min, researchers have proposed ultra-short-term periods of 10 s (30), 30 s (31), and 60 s (36).
            */
            
            int length = values.Length;
            if (length < 2)
            {
                ExciteOMeterManager.DebugLog("It is not possible to calculate RMSSD with less than 2 values");
                return 0;
            }

            // Calculation
            N = length-1; // Size of final vector of differences

            cumsum = 0;
            for (int i = 0; i < N; i++)
            {
                diff = values[i+1] - values[i];
                cumsum += (float)Math.Pow(diff, 2);
            };

            result = (float)Math.Sqrt(cumsum/N);

            return result;
        }
    }
}

