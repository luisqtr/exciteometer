using System;
using UnityEngine;
using TMPro;

namespace ExciteOMeter
{
    public class MarkersUI : MonoBehaviour
    {
        /*
        [Header("Custom Marker Message")]
        public TMP_InputField customMarkerMessageIF;
        public Transform markersParent;
        public GameObject instanceMarkerPrefab;
        

        private string defaultText = "No message";

        public static MarkersUI instance;

        /// <summary>
        /// Singleton
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
                Destroy(gameObject);
            }
        }

        // MARKERS

        // General method to create markers
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
        */
    }
}