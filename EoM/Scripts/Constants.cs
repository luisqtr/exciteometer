namespace ExciteOMeter
{
    public enum Devices
    {
        PolarH10,
    }
 
    // Enum to identify the type of incoming data.
    [System.Serializable]
    public enum DataType
    {
        NONE = 0,
        HeartRate,
        RRInterval,
        RawECG,
        RawACC,
        RMSSD,
        SDNN,
        EOM,


        AutomaticMarkers,
        ManualMarkers,
        Screenshots,
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

        TransformHeadset,        //  Collects position + rotation from VR headset
        TransformLeftController, 
        TransformRightController,
        SubjectiveMeasureWithTrigger,   // Stores subjective measurement from controller
    }

    public static class Constants
    {
        
    }
}
