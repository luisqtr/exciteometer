using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ExciteOMeter
{

    /// <summary>
    /// Configures the files that are going to be written in CSV files.
    /// </summary>
    public class LoggerController : MonoBehaviour
    {

        [Tooltip("Name of the folder that will save the Logs, this folder is located in the same path than [YourApplication].exe file.")]
        public string logFolderName = "LogFiles_AppName";


        [Header("Setup log files per session")]
        // Setup from the UnityUI
        public List<DataLogger> loggers = new List<DataLogger>{new DataLogger()}; // Start with a single instance

        private Dictionary<LogName,DataLogger> dictLoggers = new Dictionary<LogName, DataLogger>();  // Dictionary to speed up the search

        // Complete path of main log folder of the app (Streaming Assets)
        private string folderToSaveLogs;
        private string screenshot_folder = "screenshots/";

        private string thisLogPath = "";

        private static bool currentlyLogging = false;
        public static bool isFirstTimestampConfigured = false; // Whether the first timestamp has been configured or not
        public static float firstTimestamp = 0.0f;

        // Screenshots setup
        private bool savingScreenshots = false;
        private bool periodicScreenshots = false;
        private float screenshotPeriodSecs = 20.0f;
        private float elapsedTimePeriodicScreenshots = 0.0f;

        // Records debug log from the main application
        private static DataLogger loggerMainApp;
        
        // Singleton
        public static LoggerController instance;

        // Use this for initialization
        void Awake ()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // SETUP THE MAIN FOLDER OF THE LOGS DEPENDING ON THE PLATFORM (https://docs.unity3d.com/ScriptReference/Application-dataPath.html)
        #if UNITY_EDITOR
            folderToSaveLogs = Application.dataPath + "/../" + logFolderName + "/";      // In the Editor, the logs are at the same level than Assets folder
        #elif UNITY_IOS
            folderToSaveLogs = Application.persistentDataPath + "/" + logFolderName + "/";
        #elif UNITY_WSA
            folderToSaveLogs = Application.persistentDataPath + "/" + logFolderName + "/";
        #elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            // In Release mode, put the logs at the same level than the executable
            folderToSaveLogs = Application.dataPath + "/../" + logFolderName + "/";
        #else
            folderToSaveLogs = Application.dataPath + logFolderName + "/";
        #endif

            // Write and read general settings 
            SettingsManager.Values.logSettings.mainLogFolder = folderToSaveLogs;
            SettingsManager.instance.SaveSettings();

            // Create directory
            if (!File.Exists(folderToSaveLogs))
            {
                Directory.CreateDirectory(folderToSaveLogs);
            }

            // Setup main logger of Excite-O-Meter
            loggerMainApp = new DataLogger(folderToSaveLogs + "/" + SettingsManager.Values.logSettings.mainDebugFilename, "Event");
            loggerMainApp.logID = LogName.UNDEFINED; // This log is internal, therefore does not need to have a public visible ID, and it occupies the UNDEFINED label.
            loggerMainApp.IsLogging = true;
            LoggerController.instance.WriteLine(LogName.UNDEFINED, "New App Execution");
        }

        public void Update()
        {
            // If the logger is running, automatic screenshots options was activated, and on top of that, the automatic screenshot checkbox was also activated.
            if(currentlyLogging && savingScreenshots && periodicScreenshots)
            {
                // Count time for new screenshot
                elapsedTimePeriodicScreenshots += Time.deltaTime;
                if(elapsedTimePeriodicScreenshots > screenshotPeriodSecs)
                {
                    elapsedTimePeriodicScreenshots = 0.0f;
                    SaveScreenshot();
                }
            }
        }

        public bool WriteLine(LogName name, string line)
        {
            if(name == LogName.UNDEFINED) // Write in the main log
                loggerMainApp.WriteLine(line);

            // Access session loggers if it is enabled.
            if(!currentlyLogging)
                return false;

            // When logging is active
            if(dictLoggers.ContainsKey(name))
                return dictLoggers[name].WriteLine(line);
            return false;
        }

        public bool SaveScreenshot()
        {
            if(!savingScreenshots)
                return false;
            
            string screenshot_filepath = ScreenRecorder.instance.CaptureScreenshot();

            if (screenshot_filepath == null)
                return false;
            
            // Screenshot was successful
            EoM_Events.Send_OnStringReceived(DataType.Screenshots, ExciteOMeterManager.GetTimestamp(), screenshot_filepath);

            return true;
        }

        public bool StartOrStopLogSession()
        {
            if (currentlyLogging)
            {
                // STOP LOGGING               
                StopLogSession();
            }
            else
            {
                // START LOGGING
                StartLogSession();
            }
            return currentlyLogging;
        }

        public void StartLogSession()
        {
            if(!currentlyLogging)
            {
                // Allow custom events from the end user
                ExciteOMeterManager.instance.OnStartSessionLog.Invoke();

                currentlyLogging = true;
                ExciteOMeterManager.currentlyRecordingSession = true;

                // START LOGGING
                SetLoggingState(true);
                // Creates directory and files for CSV logging
                SetupNewLog();
                // Setups a JSON file to be saved in the same directory
                SessionVariablesController.instance.SetupNewSession(thisLogPath);

                // SEND EoM Event
                EoM_Events.Send_OnLoggingStateChanged(currentlyLogging);
            }
        }

        public void StopLogSession()
        {
            if(currentlyLogging)
            {
                // NOTE: The final stop is given in `FinalStopLogSession`
                // by ExciteOMeterManager when EoM level is calculated
                // This is a type of pre-stop session
                EoM_Events.Send_OnPostProcessingStarted();
            }
        }

        // JUST CALLED BY ExciteOMeterManager WHEN POST-PROCESSING IS FINISHED.
        public void FinalStopLogSession()
        {
            // Logging is finished
            currentlyLogging = false;
            ExciteOMeterManager.currentlyRecordingSession = false;

            // Notify everyone before closing logs.
            // Specially for feature calculation that matches length of input data before log is closed.
            EoM_Events.Send_OnLoggingStateChanged(currentlyLogging);

            // STOP LOGGING
            SetLoggingState(false);
            CloseLogFiles();
            
            // Create JSON
            SessionVariablesController.instance.StopCurrentSession();

            if(savingScreenshots)
                ScreenRecorder.instance.StopScreenRecorder();

            // Allow custom events from the end user
            ExciteOMeterManager.instance.OnStopSessionLog.Invoke();
        }


        private void SetLoggingState(bool state)
        {
            // Define first timestamp of the log
            if(state == true)
            {
                // Started logging
                firstTimestamp = ExciteOMeterManager.GetTimestamp();
                ExciteOMeterManager.DebugLog("First timestamp at " + firstTimestamp.ToString());
                isFirstTimestampConfigured = true;
            }
            else
            {
                // Stopped logging
                isFirstTimestampConfigured = false;
                firstTimestamp = 0.0f;
                ExciteOMeterManager.DebugLog("First timestamp back to " + firstTimestamp.ToString());
            }

            foreach (DataLogger l in loggers)
            {
                l.IsLogging = currentlyLogging;
            }
        }
       
        // Creates an entire new session for the specific log
        private void SetupNewLog()
        {
            // Clean loggers from previous session. They are cleaned afterwards because after log finishes,
            // logs are needed for postprocessing of ExciteOMeter level
            dictLoggers.Clear();

            // Create new folder based on the time of creation of the session
            DateTime initialTimeLog = DateTime.Now;
            string folder_timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmss"); // If changed the format, change it in the OfflineAnalyzerManager.cs
            thisLogPath = folderToSaveLogs + "/" + folder_timeStamp + "_" + SettingsManager.Values.logSettings.sessionId + "/";
            Directory.CreateDirectory(thisLogPath);

            savingScreenshots = SettingsManager.Values.logSettings.captureScreenshots;
            periodicScreenshots = SettingsManager.Values.logSettings.periodicScreenshots;
            screenshotPeriodSecs = SettingsManager.Values.logSettings.screenshotsPeriodSecs;

            if(savingScreenshots)
            {
                Directory.CreateDirectory(thisLogPath + screenshot_folder);
                ScreenRecorder.instance.SetupScreenRecorder(thisLogPath+screenshot_folder, Camera.main);
            }

            // Setup loggers for this session
            foreach (DataLogger logger in loggers)
            {
                if(logger.logID == LogName.UNDEFINED)
                {
                    Debug.LogError("Error configuring loggers. Please identify a unique logID from the list for the file" + logger.filename);
                }
                else if(dictLoggers.ContainsKey(logger.logID))
                {
                    Debug.LogError("Error configuring logger. Another log has been configured with the logID " + logger.logID + " in " + dictLoggers[logger.logID].filename + " . Please define a unique logId for the filename for " + logger.filename);
                }
                else
                {
                    dictLoggers.Add(logger.logID, logger);
                    logger.InitializeDataLogger(thisLogPath + logger.filename, logger.headers);
                }
            }
        }


        private void CloseLogFiles()
        {
            foreach (DataLogger l in loggers)
            {
                l.Close();
            }
        }

        private void OnDestroy()
        {
            StopLogSession();
            
            // General debug log.
            loggerMainApp.IsLogging = false;
            loggerMainApp.Close();
        }
    }
}