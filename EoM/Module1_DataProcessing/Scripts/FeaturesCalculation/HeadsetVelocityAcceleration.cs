using System;
using UnityEngine;

namespace ExciteOMeter
{
    public class HeadsetVelocityAcceleration : EoM_Base_FeatureCalculation
    {
        int numTimestamps, numFeatures;
        float[] result;

        // Parses float numbers to rotations and quaternions to facilitate calculation
        Vector3 pos_0, pos_1, pos_2;    // Positions in t, t-1, t-2. (t=0 is the last acquired data, available in the last position of `values`
        Quaternion qt_0, qt_1, qt_2;    // Rotations in quaternions for t, t-1, t-2
        Vector3 et_0, et_1, et_2;       // Rotations in euler angles for t, t-1, t-2 

        // Placeholders for calculated features
        Vector3 velPos0, velPos1, accPos0;
        Quaternion velQuat0, velQuat1, accQuat0;
        Vector3 velEuler0, velEuler1, accEuler0;

        // Calculations
        float i, j, k;
        Quaternion qt_1_conjugate;

        protected override void SetupStart()
        {
            isTimeBasedFeature = !SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.isSampleBased;
            windowTime = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.windowTime;
            overlappingFraction = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.overlapPercentageTime;

            isSampleBasedFeature = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.isSampleBased;
            sampleBuffer = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.sampleBufferLength;
            overlappingSamples = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.overlapSamplesLength;
            offsetSamplesTimestamp = SettingsManager.Values.featureSettings.HeadsetVelocityAccelerations.offsetSamplesInTimestamp;

            // Initialize arrays
            
            // Input features
            pos_0 = pos_1 = pos_2 = Vector3.zero;
            et_0 = et_1 = et_2 = Vector3.zero;
            qt_0 = qt_1 = qt_2 = Quaternion.identity;

            // Calculated features
            velPos0 = velPos1 = accPos0 = Vector3.zero;
            velQuat0 = velQuat1 = accQuat0 = Quaternion.identity;
            velEuler0 = velEuler1 = accEuler0 = Vector3.zero;

            // Output result
            result = new float[20];     // Number of features to be calculated from the incoming data.
        }

        protected override float[] CalculateFeatureArray(float[] timestamps, float[][] values)
        {
            // `values[:]` contains all the timestamps.
            // `values[0][:]` contains all the features for the timestamp[0]

            // There are 10 features incoming from Movement
            //transformArray[0] = objectToTrack.position.x;
            //transformArray[1] = objectToTrack.position.y;
            //transformArray[2] = objectToTrack.position.z;
            //transformArray[3] = objectToTrack.localRotation.w;  // Real dimension of Q
            //transformArray[4] = objectToTrack.localRotation.x;  // Q in the axis i
            //transformArray[5] = objectToTrack.localRotation.y;  // Q in the axis j
            //transformArray[6] = objectToTrack.localRotation.z;  // Q in the axis k
            //transformArray[7] = objectToTrack.localEulerAngles.y; // Yaw (left-right)
            //transformArray[8] = objectToTrack.localEulerAngles.x; // Yaw (up-down)
            //transformArray[9] = objectToTrack.localEulerAngles.z; // Yaw (towards shoulders)

            numTimestamps = values.Length;  // Number of timestamped values, the specific time is received in `timestamps`
            numFeatures = values[0].Length; // Number of features for each timestamped value
            if (numTimestamps < 3 || timestamps.Length != numTimestamps || numFeatures != 10)
            {
                Debug.LogWarning("Calculation of movement features does not have enough number of timestamps or input dimensions to calculate features. " +
                    "numTimestamps:" + numTimestamps + ", numFeatures: " + numFeatures);
                return null;
            }

            // Parse incoming data to Transforms
            pos_0.x = values[numTimestamps - 1][0];     pos_0.y = values[numTimestamps - 1][1];     pos_0.z = values[numTimestamps - 1][2]; // t=0 is the last value (numTimestamps-1)
            pos_1.x = values[numTimestamps - 2][0];     pos_1.y = values[numTimestamps - 2][1];     pos_1.z = values[numTimestamps - 2][2];
            pos_2.x = values[numTimestamps - 3][0];     pos_2.y = values[numTimestamps - 3][1];     pos_2.z = values[numTimestamps - 3][2];

            qt_0.w = values[numTimestamps - 1][3];      qt_0.x = values[numTimestamps - 1][4];      qt_0.y = values[numTimestamps - 1][5];      qt_0.z = values[numTimestamps - 1][6];
            qt_1.w = values[numTimestamps - 2][3];      qt_1.x = values[numTimestamps - 2][4];      qt_1.y = values[numTimestamps - 2][5];      qt_1.z = values[numTimestamps - 2][6];
            qt_2.w = values[numTimestamps - 3][3];      qt_2.x = values[numTimestamps - 3][4];      qt_2.y = values[numTimestamps - 3][5];      qt_2.z = values[numTimestamps - 3][6];

            et_0.x = values[numTimestamps - 1][7];      et_0.y = values[numTimestamps - 1][8];      et_0.z = values[numTimestamps - 1][9];
            et_1.x = values[numTimestamps - 2][7];      et_1.y = values[numTimestamps - 2][8];      et_1.z = values[numTimestamps - 2][9];
            et_2.x = values[numTimestamps - 3][7];      et_2.y = values[numTimestamps - 3][8];      et_2.z = values[numTimestamps - 3][9];


            // Differential features
            velPos0 = pos_0 - pos_1;
            velPos1 = pos_1 - pos_2;
            accPos0 = velPos0 - velPos1;

            velEuler0 = CalculateDistanceEulerAngles(et_0, et_1);
            velEuler1 = CalculateDistanceEulerAngles(et_1, et_2);
            accEuler0 = CalculateDistanceEulerAngles(velEuler0, velEuler1);

            velQuat0 = CalculateDistanceQuaternion(qt_0, qt_1);
            velQuat1 = CalculateDistanceQuaternion(qt_1, qt_2);
            accQuat0 = CalculateDistanceQuaternion(velQuat0, velQuat1);

            ////Output should be equivalent to the number of calculated features (so far 20)
            ///// NOTE::: If there are errors in this section. Check that number of features in 
            // `result` are the same than the number of elements in
            // `Constants.SubsetOfFeaturesTransformDataTypes` for the `FeaturesTransformHeadset`

            //DataType.headset_velPosX, DataType.headset_velPosY, DataType.headset_velPosZ,
            //DataType.headset_accPosX, DataType.headset_accPosY, DataType.headset_accPosZ,
            //DataType.headset_velQ0, DataType.headset_velQi, DataType.headset_velQj, DataType.headset_velQk,
            //DataType.headset_accQ0, DataType.headset_accQi, DataType.headset_accQj, DataType.headset_accQk,
            //DataType.headset_velEi, DataType.headset_velEj, DataType.headset_velEk,
            //DataType.headset_accEi, DataType.headset_accEj, DataType.headset_accEk,

            // Map calculated features to array, in order.
            result[0] = velPos0.x;      result[1] = velPos0.y;      result[2] = velPos0.z;
            result[3] = accPos0.x;      result[4] = accPos0.y;      result[5] = accPos0.z;
            result[6] = velQuat0.w;     result[7] = velQuat0.x;     result[8] = velQuat0.y;     result[9] = velQuat0.z;
            result[10]= accQuat0.w;     result[11]= accQuat0.x;     result[12]= accQuat0.y;     result[13]= accQuat0.z;
            result[14]= velEuler0.x;    result[15]= velEuler0.y;    result[16]= velEuler0.z;
            result[17]= accEuler0.x;    result[18]= accEuler0.y;    result[19]= accEuler0.z;

            return result;
        }

        /// <summary>
        /// Calculates the distance between two Euler angles
        /// </summary>
        /// <param name="angle_t">Rotation at time t</param>
        /// <param name="angle_t_1">Rotation at time t-1</param>
        /// <returns>Vector3 indicating velocity between two Euler angles</returns>
        private Vector3 CalculateDistanceEulerAngles(Vector3 angle_t, Vector3 angle_t_1)
        {
            // Equation 17 from Hyie 2009: https://doi.org/10.1007/s10851-009-0161-2
            Vector3 dist = Vector3.zero;

            i = Mathf.Abs(angle_t.x - angle_t_1.x);
            j = Mathf.Abs(angle_t.y - angle_t_1.y);
            k = Mathf.Abs(angle_t.z - angle_t_1.z);

            dist.x = Mathf.Min(i, 360 - i);
            dist.y = Mathf.Min(j, 360 - j);
            dist.z = Mathf.Min(k, 360 - k);

            //if(angle_t != angle_t_1)
            //    Debug.Log("FEAT: " + angle_t + angle_t_1 + "->" + "i=" + i + ", j=" + j + ",k=" + k + " --> dist" + dist);

            return dist;
        }

        /// <summary>
        /// Calculates the quaternion representing the velocity between
        /// subsequent quaternions
        /// </summary>
        /// <param name="qt">Quaternion at t</param>
        /// <param name="qt_1">Quaternion at t-1</param>
        /// <returns>Quaternion representing rotation velocity between both</returns>
        private Quaternion CalculateDistanceQuaternion(Quaternion qt, Quaternion qt_1)
        {
            // Equation 16 from Switonski 2019: https://doi.org/10.1007/s11045-018-0611-3
            
            // Calculate complex conjugate by inverting non-real dimensions
            qt_1_conjugate = new Quaternion(-qt_1.x, -qt_1.y, -qt_1.z, qt_1.w); // Quaternion.Inverse() does NOT do the same than this!

            // Apply hamilton product, the order needs to be the same than this
            return qt*qt_1_conjugate;
        }
    }
}


