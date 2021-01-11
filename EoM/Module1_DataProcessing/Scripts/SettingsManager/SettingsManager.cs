using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

namespace ExciteOMeter
{
    /// <summary>
    /// Object to control the settings file from a UI inside Unity. This also contains the events that notify the listeners
    /// when the targets or weather need to be updated in the scenario.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        /// <summary>
        /// Name of the file saved
        /// </summary>
        public string filename = "configFile.json";

        
        // To enable run the application from a scene different of MainMenu
        [Tooltip("Allows to use the settings without attaching a UI that manipulates the values.")]
        public bool useSettingsWithoutUI = true;

        private string settingsPath; // Complete path where is located the file

        //////// UI elements
        [Header("DO NOT CHANGE: Setup from `ExciteOMeterOnlineUI.cs`")]
        public TMP_InputField usernameIF;
        public Toggle periodicScreenshotsToggle;

        public static SettingsManager instance;
        /// <summary>
        /// Instance of the class that contains the structure for the serialization of data.
        /// </summary>
        private static SettingsVariables settings;
        public static SettingsVariables Values
        {
            get
            {
                if (settings == null)
                    return new SettingsVariables();
                return settings;
            }
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

            // New Settings object
            settingsPath = Application.streamingAssetsPath + "/" + filename;
            settings = new SettingsVariables();

            // Create directory
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Debug.LogError("StreamingAssets folder did not exist, it was created in the first run. Please stop and play your game again");
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

        }

        /// <summary>
        /// Load settings from file each time the canvas is displayed
        /// </summary>
        private void OnEnable()
        {
            LoadSettings();
        }

        private void OnDisable()
        {
            SaveSettings();
        }

        /// <summary>
        /// Load settings from the hard-drive
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                settings = JsonUtility.FromJson<SettingsVariables>(File.ReadAllText(settingsPath));

                if(!useSettingsWithoutUI)
                    UpdateUI();
            }
            else
            {
                // If the settings file does not exist, create one with default values and read from it.
                SaveSettings(true);
                LoadSettings();
            }
        }

        /// <summary>
        /// Write a file in the application directory with the settings of the file.
        /// </summary>
        public void SaveSettings(bool saveDefaultValues = false)
        {
            if (!saveDefaultValues && !useSettingsWithoutUI)
                // Read current values from UI.
                ReadFromUI();

            string jsonData = JsonUtility.ToJson(settings, true);
            File.WriteAllText(settingsPath, jsonData);
        }

        public void SetSessionId(string text)
        {
            settings.logSettings.sessionId = text;
        }

        public void SetPeriodicScreenshots(bool active)
        {
            settings.logSettings.periodicScreenshots = active;
        }

        /* * * * * * 
        * SETUP UI ELEMENTS
        * * * * * */
        /// <summary>
        /// Update ui elements with settings read from settings file in disk
        /// </summary>
        private void UpdateUI()
        {
            // READ FROM FILES

            // Logger
            usernameIF.text = settings.logSettings.sessionId;
            periodicScreenshotsToggle.isOn = settings.logSettings.periodicScreenshots;
        }

        /// <summary>
        /// Read information from the modified UI and store in local arrays.
        /// </summary>
        private void ReadFromUI()
        {
            // READ FROM UI OBJECTS

            // Data logger
            if(usernameIF != null) settings.logSettings.sessionId = usernameIF.text;
            if (periodicScreenshotsToggle != null) settings.logSettings.periodicScreenshots = periodicScreenshotsToggle.isOn;
        }
    }


    #if UNITY_EDITOR
    // EDITOR
    [CustomEditor(typeof(SettingsManager))]
    public class SettingsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SettingsManager myScript = (SettingsManager)target;
            //if (GUILayout.Button("Update ShortD training settings"))
            //{
            //    myScript.UpdateShortDistanceTraining();
            //}
        }
    }

    #endif

}

