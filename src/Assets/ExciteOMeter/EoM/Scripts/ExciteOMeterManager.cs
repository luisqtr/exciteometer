using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ExciteOMeter
{
    public class ExciteOMeterManager : MonoBehaviour
    {
        private readonly string SCENE_NAME_OFFLINE_ANALYSIS = "ExciteOMeter_OfflineAnalysis";

        public static bool showLog = false;

        public static bool currentlyRecordingSession = false;

        [Tooltip("Whether you want the Excite-O-Meter log messages to be shown in Console in play mode.")]
        public bool showLogInConsole = false;

        public UnityEvent OnStartSessionLog;
        public UnityEvent OnStopSessionLog;


        private bool waitingPostprocessingToGoToOffline = false;
        public static bool inPostProcessingStage = false;

        public static ExciteOMeterManager instance;

        /// <summary>
        /// Set instance for settings object and initialize callbacks of UI
        /// </summary>
        private void Awake()
        {
            // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
            if (instance == null)
            {
                instance = this;
                // This object is destroyed only when returninig from EoM offline analysis
                DontDestroyOnLoad(instance.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                // Uncomment to update Singleton when loading a new scene?
                //Destroy(instance.gameObject);
                //instance = this;
            }
        }

        void Update()
        {
            // Show EoM log in editor
            showLog = showLogInConsole;
        }
        
        // Wrapper of debug log for EoM functions
        public static void DebugLog(string text)
        {
            if (showLog)
                Debug.Log(text);
        }

        public static void LogError(string text)
        {
            LoggerController.instance.WriteLine(LogName.UNDEFINED, "ExciteOMeter >> Log Error >> " + text);
            Debug.LogError("ExciteOMeter >> Log Error >> "+text);
        }

        public static void LogInFile(string text)
        {
            LoggerController.instance.WriteLine(LogName.UNDEFINED, "ExciteOMeter >> " + text);
        }

        public void PostProcessingExciteOMeterLevel()
        {
            // All data needs to be written first on the json file before used for calculation
            // this functions delays the execution
            
            // Calculate ExciteOMeter Level
            ExciteOMeterCalculation.Calculate(SessionVariablesController.Values);
            
            // Definitive stop of logging after post-processing of data
            inPostProcessingStage = false;
            LoggerController.instance.FinalStopLogSession();

            // In case user wants to go to offline analysis but didn't stop the recording of the session from the button
            if(waitingPostprocessingToGoToOffline)
            {
                waitingPostprocessingToGoToOffline = false;
                GoToOfflineAnalysis();
            }
        }

        public void StartOrStopSessionLog()
        {
            LoggerController.instance.StartOrStopLogSession();
        }

        public void StartSessionLog()
        {
            LoggerController.instance.StartLogSession();
        }

        public void StopSessionLog()
        {
            LoggerController.instance.StopLogSession();
        }

        public void StartOfflineAnalysis()
        {
            if(currentlyRecordingSession)
            {
                // In case logging is running. Stop it.
                waitingPostprocessingToGoToOffline = true;
                LoggerController.instance.StopLogSession();
            }
            else
            {
                // Change scene immediately
                GoToOfflineAnalysis();
            }
        }

        private void GoToOfflineAnalysis()
        {
            // Get the name of the scene that needs to be activated again once offline analysis is over.
            SettingsManager.Values.offlineAnalysisSettings.callerSceneName = SceneManager.GetActiveScene().name;

            try
            {
                SettingsManager.Values.offlineAnalysisSettings.isInOfflineMode = true;
                SceneManager.LoadScene(SCENE_NAME_OFFLINE_ANALYSIS);
            }
            catch (System.Exception)
            {
                SettingsManager.Values.offlineAnalysisSettings.isInOfflineMode = false;
                LogMessageUI.instance.WriteConsoleText("Error loading scene for offline Analysis" + SCENE_NAME_OFFLINE_ANALYSIS + ". Make sure it is included in the build settings");
            }

            // Delete UI object. This is created again from the caller scene.
            if(ExciteOMeterOnlineUI.instance != null)
            {
                Destroy(ExciteOMeterOnlineUI.instance.gameObject);
            }
            
            // Open the offline analysis scene
            Debug.Log("Caller scene: " + SettingsManager.Values.offlineAnalysisSettings.callerSceneName + " isInOfflineAnalysis=" + SettingsManager.Values.offlineAnalysisSettings.isInOfflineMode);
        }

        public void ReturnFromOfflineAnalysis()
        {
            try
            {
                SettingsManager.Values.offlineAnalysisSettings.isInOfflineMode = false;
                SceneManager.LoadScene(SettingsManager.Values.offlineAnalysisSettings.callerSceneName);

                // When the mainScene is loaded again from the offline analyzer. Restart all the ExciteOMeterManager to link elements to other objects that were destroyed before
                Destroy(this.gameObject);
                if(ExciteOMeterOnlineUI.instance != null)
                {
                    Destroy(ExciteOMeterOnlineUI.instance.gameObject);
                }
            }
            catch (System.Exception)
            {
                SettingsManager.Values.offlineAnalysisSettings.isInOfflineMode = true;
                Debug.LogError("Error returning from offline mode. Error loading caller scene."+SettingsManager.Values.offlineAnalysisSettings.callerSceneName);
            } 
        }

        public void ExitApplication()
        {
            Debug.Log("Quitting scene...");
            Application.Quit();
        }

        public static float GetTimestamp()
        {
            if(LoggerController.isFirstTimestampConfigured)
            {
                return Time.fixedTime - LoggerController.firstTimestamp;
            }
            return Time.fixedTime;
        }
    }

}