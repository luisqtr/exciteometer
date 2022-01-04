namespace ExciteOMeter
{
    public enum Devices
    {
        PolarH10,
        HeadsetXR,
    }

    // Enum to identify the type of incoming data.
    [System.Serializable]
    public enum DataType
    {
        NONE = 0,

        // Heart activity
        HeartRate,
        RRInterval,
        RawECG,
        RawACC,
        // Features
        RMSSD,
        SDNN,

        // Estimated 'Excitement level' >> Excite-O-Meter
        EOM,

        AutomaticMarkers,
        ManualMarkers,
        Screenshots,

        // Suffixes need to match the variable `Constants.TransformSuffixes`
        Headset_array,      // Sends multidimensional data for the receivers that can log multiple values at the time (like `EoM_Base_FeatureCalculation.cs`).
        Headset_posX,
        Headset_posY,
        Headset_posZ,
        Headset_q0,
        Headset_qi,
        Headset_qj,
        Headset_qk,
        Headset_yaw,
        Headset_pitch,
        Headset_roll,

        // Features
        Headset_VelAcc_Array,
        headset_velPosX, headset_accPosX,
        headset_velPosY, headset_accPosY,
        headset_velPosZ, headset_accPosZ,
        // TODO: Features for head rotation in quaternions and euler angles

        // Controllers
        LeftController_posX,
        RightController_posX,
    }

     // Enum used to identify different types of automatic or manual markers
    public enum MarkerLabel
    {
        NONE,
        CUSTOM_MARKER,  // Marker created manually by the user.
        QUICK_MARKER,   // Marker created from the UI
        ABNORMAL_HR,
        ABNORMAL_RMSSD,
        
    }

    // Enum used to identify a log file
    public enum LogName
    {
        UNDEFINED,
        EventsAndMarkers,
        VariableHeartRate,
        VariableRrInterval,
        VariableRawECG, // Available in Polar H10, but not used
        VariableRawACC, // Available in Polar H10, but not used
        FeatureRMSSD,
        FeatureSDNN,
        EOM,

        TransformHeadset,        //  Collects position + quaternion + eulerAngles from VR headset
        FeaturesTransformHeadset, // Velocity and acceleration of position and rotations.

        TransformLeftController, 
        TransformRightController,

        SubjectiveMeasureWithTrigger,   // Stores subjective measurement from controller
    }

    public static class Constants
    {
        // Every time a transform is chosen, it expands automatically to these suffixes.
        // e.g. HeadsetTransform_X, HeadsetTransform_Y, HeadsetTransform_Z, ...

        public static string[] TransformDataTypeSuffixes { get => transformSuffixes; }
        public static string[] transformSuffixes = new string[] {
            "_posX", "_posY", "_posZ", "_q0", "_qi", "_qj", "_qk", "_yaw", "_pitch", "_roll"
        };

        /// <summary>
        /// In movement, the data is multidimensional, but the offline visualizer only allows
        /// plotting unidimensional time series. Therefore, we need to transmit each of the 
        /// dimensions in a single feature channel based on `DataType`. This method makes it
        /// easier to choose which are the subset of `DataType[]` corresponding to each
        /// transform, based on the unique single `LogName`.
        /// </summary>
        /// <param name="logname"></param>
        /// <returns></returns>
        public static DataType[] SubsetOfTransformDataTypes(LogName logname)
        {
            DataType[] dataTypeTransformArray = new DataType[Constants.TransformDataTypeSuffixes.Length];
            switch(logname)
            {
                case LogName.TransformHeadset:
                    dataTypeTransformArray = new DataType[]
                    {
                        DataType.Headset_posX, DataType.Headset_posY, DataType.Headset_posZ,
                        DataType.Headset_q0, DataType.Headset_qi, DataType.Headset_qj, DataType.Headset_qk,
                        DataType.Headset_yaw, DataType.Headset_pitch, DataType.Headset_roll
                    };
                    break;
                case LogName.TransformLeftController:
                    break;
                case LogName.TransformRightController:
                    break;
            }

            return dataTypeTransformArray;

        }

        public static DataType[] SubsetOfFeaturesTransformDataTypes(LogName logname)
        {
            DataType[] dataTypeTransformArray = new DataType[Constants.TransformDataTypeSuffixes.Length];
            switch (logname)
            {
                case LogName.FeaturesTransformHeadset:
                    dataTypeTransformArray = new DataType[]
                    {
                        DataType.headset_velPosX, DataType.headset_accPosX,
                        DataType.headset_velPosY, DataType.headset_accPosY,
                        DataType.headset_velPosZ, DataType.headset_accPosZ,
                    };
                    break;
                // TODO: For controllers
            }

            return dataTypeTransformArray;

        }
    }
}

