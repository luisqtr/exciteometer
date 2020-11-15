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

    // Enum used to identify automatic and manual events triggered with EoM
    public enum InstantMarkerType
    {
        NONE,
        AUTOMATIC,
        MANUAL,     // The user can trigger this event to put a customized message.
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
        VariableHeartRate,
        VariableRrInterval,
        VariableRawECG, // Available in Polar H10, but not used
        VariableRawACC, // Available in Polar H10, but not used
        FeatureRMSSD,
        FeatureSDNN,
        EOM,

        GameMainCameraPosition, // Not used, for example for future logs
        GamePlayerEvents,       // Not used
    }

    public static class Constants
    {
        
    }
}
