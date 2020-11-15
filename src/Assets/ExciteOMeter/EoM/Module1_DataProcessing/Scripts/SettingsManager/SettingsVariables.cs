using System;
using System.Collections.Generic;
/// <summary>
/// Serializable class to save and load the settings with JSON file manager.
/// </summary>
public class SettingsVariables
{
    // DEFINITION OF LOG PROPERTIES
    [System.Serializable]
    public class LogSettings
    {
        // Name of the user
        public string sessionId = "SessionID";

        public string mainLogFolder = "";

        public string mainDebugFilename = "logDebug.txt";

        public string sessionJsonFilename = "session.json";

        public bool captureScreenshots = true;
        public bool periodicScreenshots = true;
        public float screenshotsPeriodSecs = 15.0f;
    }

    [System.Serializable]
    public class FeatureSettings
    {
        public bool DEFAULT_IS_SAMPLE_BASED = false;
        public float DEFAULT_WINDOW_TIME_SECS = 20; // 20 Seconds of signal buffer
        public float DEFAULT_OVERLAP_FRACTION = 0.5f; // Updated every 10 seconds
        public int DEFAULT_SAMPLE_BUFFER = 5;
        public int DEFAULT_OVERLAP_SAMPLES = 1;
        public int DEFAULT_OFFSET_SAMPLES_TIMESTAMP = 0;
        public bool matchInputOutputLength = true;

        [System.Serializable]
        public class RealtimeFeature
        {
            // Whether the feature is calculated based on elapsed time or number of collected samples
            public bool isSampleBased;
            // Total data collection time before calculating a feature (in seconds).
            public float windowTime;    
            // Once the buffer is full, how much overlap do you want until
            // next feature calculation. E.g. 0 will delete the whole buffer and wait
            // until `windowTimeRMSSD` seconds, but 0.5 will delete 50% of the buffer and calculate
            // a new feature in half the time of `windowTimeRMSSD`.
            public float overlapPercentageTime;
            // In case the feature is sample-based. How many samples to store before calculating the feature.
            public int sampleBufferLength;
            // How many samples to dismiss when a feature is calculated. I.e. the overlap for the next feature.
            // Each time a new feature is calculated, this value will be replicated (sampleBufferLength-overlamSamplesLength)
            // with the goal of matching the number of samples of the incoming data and the feature.
            public int overlapSamplesLength;
            // When a new feature is calculated, it can belong to a previous timestamp that is `offsetTimeSamples` behind.
            // Corresponds to the number of displacements to the left in timestamps array
            // E.g. A feature with `sampleBufferLength=5` and `offsetSamplesInTimestamp=0` corresponds to a feature that is 
            //      calculated from [t-4,t-3,t-2,t-1,t], but if `offsetSamplesInTimestamp=3` corresponds to a feature
            //      calculated from [t-2,t,t+1,t+2,t+3]. And the logfile with contain the timestamp with the offset.
            public int offsetSamplesInTimestamp;

            // Structure to save multiple features directly.
            public RealtimeFeature(bool _isSampleBased,
                                float _windowTime, float _overlapPercentageTime,
                                int _sampleBufferLength, int _overlapSamplesLength, int _offsetSamplesInTimestamp=0)
            {
                // Time-based or sample-based feature
                isSampleBased = _isSampleBased;
                // In case it is time-based
                windowTime = _windowTime;
                overlapPercentageTime = _overlapPercentageTime;
                // In case it is sample-based
                sampleBufferLength = _sampleBufferLength;
                overlapSamplesLength = _overlapSamplesLength;
                offsetSamplesInTimestamp = _offsetSamplesInTimestamp;
            }
        }

        public RealtimeFeature RMSSD =  new RealtimeFeature(true,  10f, 0.0f, 10, 9, 8);
        public RealtimeFeature SDNN = new RealtimeFeature(false, 10f, 0.0f, 5, 1);
        
    }

    [System.Serializable]
    public class OfflineAnalysisSettings
    {
        // Identify when the application executes offline mode.
        public bool isInOfflineMode = false;

        // This variable stores the name of the scene from which 
        // the offline analysis was called, so we can return to the
        // same scene when exiting the Offline Analysis
        public string callerSceneName = ""; 
    }

    ////////////////////
    // Instances
    ////////////////////
    
    public LogSettings logSettings = new LogSettings();

    public FeatureSettings featureSettings = new FeatureSettings();

    public OfflineAnalysisSettings offlineAnalysisSettings = new OfflineAnalysisSettings();

}
