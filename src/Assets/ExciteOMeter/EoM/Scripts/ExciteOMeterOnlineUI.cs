using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ExciteOMeter
{
    // Bridges the Online UI with the main Excite-O-Meter Manager
    public class ExciteOMeterOnlineUI : MonoBehaviour
    {
        public bool panelsStartVisible = false;

        [Header("Panels to show or hide")]
        public CanvasGroup headerPanel;
        public CanvasGroup sessionInfoPanel;

        [Header("Warning Signal problem")]
        public GameObject signalProblemWarning;

        [Header("Setup EoM Settings UI")]
        public TMP_InputField sessionNameIF;
        public Toggle periodicScreenshotsToggle;

        [Header("Setup Markers UI")]
        public TMP_InputField customMarkerMessageIF;
        public Transform markersParent;
        public GameObject instanceMarkerPrefab;
        
        private string defaultText = "No message";

        // Singleton
        public static ExciteOMeterOnlineUI instance;

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
            }
        }

        public void Start()
        {
            // Setup ExciteOMeter Managers
            signalProblemWarning.SetActive(false);
            ShowHideExciteOMeterPanels(panelsStartVisible);

            // Setup UI for Settings Manager
            SettingsManager.instance.useSettingsWithoutUI = false;
            SettingsManager.instance.usernameIF = sessionNameIF;
            SettingsManager.instance.periodicScreenshotsToggle = periodicScreenshotsToggle;

            SettingsManager.instance.LoadSettings();
            //
        }
        
        ////// MAIN UI
        public void ToggleExciteOMeterPanels()
        {
            Canvas_Utils.instance.ToggleCanvasGroup(headerPanel);
            Canvas_Utils.instance.ToggleCanvasGroup(sessionInfoPanel);
        }

        private void ShowHideExciteOMeterPanels(bool status)
        {
            Canvas_Utils.instance.SetVisibilityCanvasGroup(headerPanel,status);
            Canvas_Utils.instance.SetVisibilityCanvasGroup(sessionInfoPanel,status);
        }
        
        public void Click_GoToOfflineAnalysis()
        {
            ExciteOMeterManager.instance.StartOfflineAnalysis();
        }        

        ////// GENERAL SETTINGS MANAGER UI
        public void ChangeSessionId(string text)
        {
            SettingsManager.instance.SetSessionId(text);
        }

        public void ChangePeriodicSignals(bool status)
        {
            SettingsManager.instance.SetPeriodicScreenshots(status);
        }

        ////// SIGNALS

        public void ShowDisconnectedSignal()
        {
            signalProblemWarning.SetActive(true);
        }

        ////// MARKERS UI
        public void CreateManualMarker(string defaultText, MarkerLabel markerLabel = MarkerLabel.CUSTOM_MARKER)
        {
            EoM_Events.Send_OnStringReceived(DataType.ManualMarkers, ExciteOMeterManager.GetTimestamp(), defaultText);
            LogMessageUI.instance.WriteConsoleText("New custom marker at " +  ExciteOMeterManager.GetTimestamp().ToString("F2") + " with message " + defaultText);
            GameObject go = Instantiate(instanceMarkerPrefab, markersParent);
            CustomMarkerScriptUI script = go.GetComponent<CustomMarkerScriptUI>();
            script.Setup(ExciteOMeterManager.GetTimestamp(), defaultText, markerLabel);
            // sessionMarkers.Add(script); // Used to keep record of existing session markers
            LoggerController.instance.SaveScreenshot();
        }

        // A custom marker that uses the text from the inputfield
        public void CreateCustomInstantMarkers()
        {
            if(customMarkerMessageIF != null)
            {
                defaultText = customMarkerMessageIF.text;
            }
            CreateManualMarker(defaultText);
        }

        public void ClearSessionMarkers()
        {
            // Deletes gameobjects for new session
            foreach (Transform go in markersParent)
            {
                Destroy(go.gameObject);
            }
        }

        // NOT USE: Examples of automatic markers that should be triggered internally.
        public void SendAutomaticMarker()
        {
            EoM_Events.Send_OnStringReceived(DataType.AutomaticMarkers, ExciteOMeterManager.GetTimestamp(), "HR is abnormal", MarkerLabel.ABNORMAL_HR);
            LoggerController.instance.SaveScreenshot();
        }

        public void SendAutomaticMarker2()
        {
            EoM_Events.Send_OnStringReceived(DataType.AutomaticMarkers, ExciteOMeterManager.GetTimestamp(), "RMSSD > 3*SD", MarkerLabel.ABNORMAL_RMSSD);
            LoggerController.instance.SaveScreenshot();
        }


        /// UNITY INSPECTOR
        void OnValidate()
        {
            // Setup ExciteOMeter Managers
            signalProblemWarning.SetActive(panelsStartVisible);

            // ShowHidePanels
            if(panelsStartVisible)
            {
                headerPanel.interactable = true;
                headerPanel.alpha = 1.0f;
                sessionInfoPanel.interactable = true;
                sessionInfoPanel.alpha = 1.0f;
            }
            else
            {
                headerPanel.interactable = false;
                headerPanel.alpha = 0.0f;
                sessionInfoPanel.interactable = false;
                sessionInfoPanel.alpha = 0.0f;
            }
        }
    }

}