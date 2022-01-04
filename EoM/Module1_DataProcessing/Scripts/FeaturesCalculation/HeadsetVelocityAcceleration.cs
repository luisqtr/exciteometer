using System;
using UnityEngine;

namespace ExciteOMeter
{
    public class HeadsetVelocityAcceleration : EoM_Base_FeatureCalculation
    {
        int length;
        float[] result;

        protected override void SetupStart()
        {
            isTimeBasedFeature = !SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.isSampleBased;
            windowTime = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.windowTime;
            overlappingFraction = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.overlapPercentageTime;

            isSampleBasedFeature = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.isSampleBased;
            sampleBuffer = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.sampleBufferLength;
            overlappingSamples = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.overlapSamplesLength;
            offsetSamplesTimestamp = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.offsetSamplesInTimestamp;
        }

        protected override float[] CalculateFeatureArray(float[] timestamps, float[][] values)
        {
            // `values[0]` contains all the timestamps.
            // `values[1]` contains all the features for a specific timestamp

            length = values.Length;

            Debug.Log("Dim0:" + length + ", Dim1: " + values[0].Length);

            //// Number of features need to coincide with the `Constants.SubsetOfFeaturesTransformDataTypes` for the `FeaturesTransformHeadset`
            //result[0] = values[0][1] * 2;
            //result[1] = values[0][2] * 2;
            //result[2] = values[0][3] * 2;
            //result[3] = values[0][4] * 2;
            //result[4] = values[0][5] * 2;
            //result[5] = values[0][6] * 2;

            result = new float[6];

            result[0] = 1;
            result[1] = 2;
            result[2] = 3;
            result[3] = 4;
            result[4] = 5;
            result[5] = 6;

            return result;
        }
    }
}


