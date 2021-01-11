using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    public abstract class EoM_Base_FeatureCalculation : MonoBehaviour
    {
        /*Base class to calculate a cardiac feature resulting from the buffer of incoming
        data. Every time the buffer is full, the feature is calculated based on the incoming data
        and reported in the respective log (if enabled)*/

        [Tooltip("Which is the source of the data from which the features are calculated")]
        public ExciteOMeter.DataType incomingDataType = DataType.NONE;  // What is the source of the data.

        [Tooltip("Which is the type of data sent from this feature calculation")]
        public ExciteOMeter.DataType outputDataType = DataType.NONE;    // What is the source of the data.

        
        [Tooltip("Unique identifier for the log file of this feature")]
        public ExciteOMeter.LogName logIdentifier = LogName.UNDEFINED;

        [Header("DO NOT change here, but in `SettingsVariables.cs`")]
        [SerializeField] protected bool isTimeBasedFeature = true;
        [SerializeField] protected float windowTime = 5f; 
        [SerializeField] private float elapsedWindowTime = 0.0f;        // Counter of the time before calculating feature
        protected float overlappingFraction = .95f;  

        [SerializeField] protected bool isSampleBasedFeature = false;
        [SerializeField] protected int sampleBuffer = 5; 
        [SerializeField] private int elapsedSamples = 0;                // Counter of the time before calculating feature
        protected int overlappingSamples = 4;  
        protected int offsetSamplesTimestamp = 0;

        // Only for sample-based features, timestamp index to which the the feature value should be associated
        private int indexOffsetForTimestamp;

        // Match length of input and output signal
        protected bool matchLengthOfInputSignal = true;
        private bool isFirstCalculatedFeature = true; // Flag to know how many times to repeat the value to match input's length
        private int idxStartRepeating = 0;            // Index to repeat same value over different timestamps.

        // Stores the values that are used to calculate the features.
        private List<float> timestamps = new List<float>();
        private List<float> dataBuffer = new List<float>(); 
        private int elementsToDelete = 1;

        // Result of the feature
        private float featureValue = 0f;
        

        // Count instances that have finished PostProcessing
        private static int numInstances = 0;
        private static int instancesFinishedPostprocessing = 0;
        

        // Start is called before the first frame update
        void Start()
        {
            // PostProcessing flag
            numInstances++;

            // Default values, if this needs to be changed per feature reimplementing the following function in the child class
            isTimeBasedFeature = !SettingsManager.Values.featureSettings.DEFAULT_IS_SAMPLE_BASED;
            windowTime = SettingsManager.Values.featureSettings.DEFAULT_WINDOW_TIME_SECS;
            overlappingFraction = SettingsManager.Values.featureSettings.DEFAULT_OVERLAP_FRACTION;

            isSampleBasedFeature = SettingsManager.Values.featureSettings.DEFAULT_IS_SAMPLE_BASED;
            sampleBuffer = SettingsManager.Values.featureSettings.DEFAULT_SAMPLE_BUFFER;
            overlappingSamples = SettingsManager.Values.featureSettings.DEFAULT_OVERLAP_SAMPLES;

            offsetSamplesTimestamp = SettingsManager.Values.featureSettings.DEFAULT_OFFSET_SAMPLES_TIMESTAMP;

            // Only for sample-based, whether input and output are forced to have same length by resampling data.
            matchLengthOfInputSignal = SettingsManager.Values.featureSettings.matchInputOutputLength;

            // CONDITIONS
            if(isSampleBasedFeature)
            {
                if(overlappingSamples >= sampleBuffer)
                {
                    // Error, the number of samples to delete 
                    ExciteOMeterManager.LogInFile("The samples to delete in buffer feature " + outputDataType.ToString() + " are larger than sampleBufferLength. Check config.json");
                    overlappingSamples = 0;
                }
                else if(offsetSamplesTimestamp > overlappingSamples)
                {
                    ExciteOMeterManager.LogInFile("The offset timestamp of timestamp in feature " + outputDataType.ToString() + " needs to be lower than overlapSamplesLength. Check config.json");
                    offsetSamplesTimestamp = 0;
                }
            }
            // If there are some configurations needed for the specific feature
            SetupStart();
        }

        protected virtual void SetupStart()
        {
            // Reimplement this function if the specific settings for the features are in the
            // `SettingsManager` class. Otherwise, it will define the features specified by
            // default in the SettingsManager
            // This method is called in Start() is called.
        }

        // Most important function, where the feature is calculated based on the data of type `incomingDataType`
        protected abstract float CalculateFeature(float[] timestamps, float[] values);

        // Event receivers when a new data comes
        void OnEnable()
        {
            EoM_Events.OnDataReceived += AddToBuffer;
            EoM_Events.OnLoggingStateChanged += ChangedLogRecordingStatus;
            EoM_Events.OnPostProcessingStarted += PostProcessing;
        }

        void OnDisable()
        {
            EoM_Events.OnDataReceived -= AddToBuffer;
            EoM_Events.OnLoggingStateChanged -= ChangedLogRecordingStatus;
            EoM_Events.OnPostProcessingStarted -= PostProcessing;
        }

        private void AddToBuffer(ExciteOMeter.DataType type, float timestamp, float value)
        {
            if(type == incomingDataType)
            {
                timestamps.Add(timestamp);
                dataBuffer.Add (value);

                // Increase counter of samples
                elapsedSamples = dataBuffer.Count;
            }
        }

        public void ChangedLogRecordingStatus(bool status)
        {
            if(status) // When new logging session starts
            {
                // Log has started
                elapsedWindowTime = 0.0f;   // In case of time-based feature
                elapsedSamples = 0;         // In case of sample-based feature
                isFirstCalculatedFeature = true;    // Restart first feature calculation
                instancesFinishedPostprocessing = 0;    // Controlled of finished postprocessing

                // Restart features calculation from the time the log started.
                timestamps.Clear();
                dataBuffer.Clear();
            }
        }

        void PostProcessing()
        {
            if(isSampleBasedFeature && matchLengthOfInputSignal)
            {
                // CASE: Match length of input signal, recording has finished but input signal is larger than features.
                // This saves in the log the number of lines remaining to match the length of the input.
                idxStartRepeating = isFirstCalculatedFeature? 0 : (overlappingSamples-offsetSamplesTimestamp);
                for (int i = idxStartRepeating; i < timestamps.Count; i++)
                {
                    // Write in files to collect data corresponding to 
                    EoM_Events.Send_OnDataReceived(outputDataType, timestamps[i],  featureValue);
                    LoggerController.instance.WriteLine(logIdentifier, timestamps[i] + "," + featureValue.ToString());
                }
            }
            
            ////////////// POSTPROCESSING
            // Postprocessing control
            instancesFinishedPostprocessing++;
            if(instancesFinishedPostprocessing == numInstances)
            {
                // All feature extraction have finished postprocessing
                ExciteOMeterManager.instance.PostProcessingExciteOMeterLevel();
                // Reset
                instancesFinishedPostprocessing = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(isSampleBasedFeature)
            {
                // SAMPLE-BASED FEATURE
                if (dataBuffer.Count >= sampleBuffer) 
                {	
                    SampleBasedCalculation();
                }
            }
            else
            {
                // TIME-BASED FEATURE
                // Timer control
                elapsedWindowTime += Time.deltaTime;
                // Send data each "windowTime"
                if (elapsedWindowTime >= (windowTime-0.005f)) // 5ms offset in case data is not written because of approximation
                {
                    TimeBasedCalculation();
                }
            }
        }

        void SampleBasedCalculation()
        {
            // Check that data is available
            if (timestamps.Count == 0 && dataBuffer.Count==0)
            {
                // ExciteOMeterManager.DebugLog("No timestamps or data were found to calculate features");
                ExciteOMeterManager.LogInFile("No incoming data " + incomingDataType.ToString() + " was found to calculate feature " + outputDataType.ToString());
                return;
            }
            // Calculate feature
            featureValue = CalculateFeature(timestamps.ToArray(), dataBuffer.ToArray());

            // Calculate offset of timestamp that corresponds to the calculated feature (# displacements to the left in timestamps)
            // Examples: Assume `sampleBuffer=5`
            //           If `offsetSamplesTimestamp=0`, t for calculated feature is [t-4,t-3,...,t]
            //           If `offsetSamplesTimestamp=3`, t for calculated feature is [t-1,t,t+1,t+2,t+3]
            indexOffsetForTimestamp = sampleBuffer-offsetSamplesTimestamp-1;

            // Send events and log in file
            ExciteOMeterManager.DebugLog("A new feature was calculated in " + outputDataType.ToString() + ": " + timestamps[indexOffsetForTimestamp] + ", " + featureValue.ToString());

            // Flag to know if it is the first calculation of the feature.
            // If so, the new feature has to match all the timestamps existing before the first timestamp of the feature.
            if(matchLengthOfInputSignal)
            {
                if(isFirstCalculatedFeature)
                {
                    isFirstCalculatedFeature = false;
                    idxStartRepeating = 0;     // No previous data in array, repeat from beginning of input timestamps.
                }
                else
                {
                    // CASE: Match length and buffer already contains data from previous window
                    // Based on overlap and offset, the position where to start repeating timestamps is the following formula.
                    idxStartRepeating = overlappingSamples-offsetSamplesTimestamp;
                }

                // Fill the previous timestamps of the input signal with the same value of this feature.
                for (int i = idxStartRepeating; i <= indexOffsetForTimestamp; i++)
                {
                    // Write in files to collect data corresponding to 
                    EoM_Events.Send_OnDataReceived(outputDataType, timestamps[i],  featureValue);
                    LoggerController.instance.WriteLine(logIdentifier, timestamps[i] + "," + featureValue.ToString());
                }
            }
            else
            {
                // CASE: DO NOT MATCH LENGTH OF INPUT SIGNAL, BUT USE TIMESTAMP DIFFERENT THAN LAST SAMPLE
                EoM_Events.Send_OnDataReceived(outputDataType, timestamps[indexOffsetForTimestamp],  featureValue);
                LoggerController.instance.WriteLine(logIdentifier, timestamps[indexOffsetForTimestamp] + "," + featureValue.ToString());
            }

            // Rearrange overlap in signal
            elementsToDelete = sampleBuffer - overlappingSamples;

            timestamps.RemoveRange(0,elementsToDelete);
            dataBuffer.RemoveRange (0, elementsToDelete);
        }

        void TimeBasedCalculation()
        {
            // New window time is the original window time minus overlapping window. Set elapsed time in proportional position.
            elapsedWindowTime = windowTime * overlappingFraction;

            // Check that data is available
            if (timestamps.Count == 0 && dataBuffer.Count==0)
            {
                ExciteOMeterManager.LogInFile("No incoming data " + incomingDataType.ToString() + " was found to calculate feature " + outputDataType.ToString());
                return;
            }
            // Calculate feature
            featureValue = CalculateFeature(timestamps.ToArray(), dataBuffer.ToArray());

            // Send events and log in file
            ExciteOMeterManager.DebugLog("A new feature was calculated in " + outputDataType.ToString() + ": " + timestamps[timestamps.Count-1] + ", " + featureValue.ToString());
            EoM_Events.Send_OnDataReceived(outputDataType, timestamps[timestamps.Count-1],  featureValue);
            LoggerController.instance.WriteLine(logIdentifier, timestamps[timestamps.Count-1] + "," + featureValue.ToString());

            // Rearrange overlap in signal
            // Overlap should not be greater than 95%, because it would generate very often feature calculations that might affect performance.
            int elementsToDelete = (int) ( Mathf.Clamp(1.0f - (overlappingFraction), 0f, 0.95f) * dataBuffer.Count);
            timestamps.RemoveRange(0,elementsToDelete);
            dataBuffer.RemoveRange (0, elementsToDelete);
        }
    }
}