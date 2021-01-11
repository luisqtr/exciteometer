using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ExciteOMeter
{
    /// <summary>
    /// Object to control the variables that are recorded from a session.
    /// and handles serialization/deserialization processes.
    /// </summary>
    public class SessionVariablesController : MonoBehaviour
    {
        /// <summary>
        /// Name of the file saved
        /// </summary>
        public string filename = "sessionData.json";
        [Tooltip("Readable JSON, but takes more space from disk")]
        public bool prettyJson = false;

        private string filepath; // Complete path where is located the json file

        private static bool isConfigured = false; // Whether a json file is configured and ready to be saved on disk

        public static SessionVariablesController instance;

        /// <summary>
        /// Instance of the class that contains the structure for the serialization of data.
        /// </summary>
        private static SessionVariables sessionValues;
        public static SessionVariables Values
        {
            get { return sessionValues; }
        }

        /// <summary>
        /// Set instance for settings object and initialize callbacks of UI
        /// </summary>
        private void Awake()
        {
            // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                // Update Settings Manager when loading a new scene
                Destroy(instance.gameObject);
                instance = this;
            }
        }

        void OnEnable()
        {
            // Subscribe to events when this gameobject is active
            EoM_Events.OnDataReceived += ExciteOMeterDataReceived;

            EoM_Events.OnStringReceived += ExciteOMeterMarkerReceived;
        }

        void OnDisable()
        {
            // Unsubscribe from events when this gameobject is inactive
            EoM_Events.OnDataReceived -= ExciteOMeterDataReceived;

            EoM_Events.OnStringReceived -= ExciteOMeterMarkerReceived;

            // Save data in case it was closed without stopping the logger.
            if(isConfigured && filepath != null)
                SaveSettings();
        }

        void Start()
        {
            SettingsManager.Values.logSettings.sessionJsonFilename = filename;
        }

        private void ExciteOMeterDataReceived(ExciteOMeter.DataType type, float timestamp, float value)
        {
            // Only process data when a file is configured
            if(!isConfigured) return;

            Values.timeseries[type].timestamp.Add(timestamp);
            Values.timeseries[type].value.Add(value);
        }

        private void ExciteOMeterMarkerReceived(DataType type, float timestamp, string message, MarkerLabel label)
        {
            // Only process data when a file is configured
            if(!isConfigured) return;

            Values.timeseries[type].timestamp.Add(timestamp);
            Values.timeseries[type].text.Add(message);
        }

        public void WritePostProcessedExciteOMeterIndex(float timestamp, float value)
        {
            if(!isConfigured) return;

            Values.timeseries[DataType.EOM].timestamp.Add(timestamp);
            Values.timeseries[DataType.EOM].value.Add(value);
        }

        /// SETUP JSON

        public void SetupNewSession(string folder)
        {
            filepath = folder + "/" + filename;
            sessionValues = new SessionVariables();
            sessionValues.CreateDictionaryOfTimeSeries();
            isConfigured = true;
        }

        public void StopCurrentSession()
        {
            if(isConfigured)
            {
                // Save file
                SaveSettings();
                // Reset values
                sessionValues = null;
                filepath = null;
                isConfigured = false;
            }
        }

        /// <summary>
        /// Load json from disk
        /// </summary>
        public SessionVariables LoadSessionValues(string filepath)
        {
            if (File.Exists(filepath))
            {
                sessionValues = JsonUtility.FromJson<SessionVariables>(File.ReadAllText(filepath));
                sessionValues.RemoveEmptyTimeSeries();
                return sessionValues;
            }
            else
            {
                // If the file does not exist, create one with default values and read from it.
                Debug.Log("The file does not exist in " + filepath);
                return null;
            }
        }

        /// <summary>
        /// Write a file in the application directory with the settings of the file.
        /// </summary>
        public void SaveSettings()
        {
            string jsonData = JsonUtility.ToJson(sessionValues, prettyJson);
            File.WriteAllText(filepath, jsonData);
        }
    }

}

