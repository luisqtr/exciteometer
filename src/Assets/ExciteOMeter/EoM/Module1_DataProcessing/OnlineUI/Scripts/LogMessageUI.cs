using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace ExciteOMeter
{
    public class LogMessageUI : MonoBehaviour
    {
        [Header("Console Popup Text")]
        public bool showConsoleBar = true;
        public GameObject consoleContainer;
        public TextMeshProUGUI consoleOutputText;

        [Header("Panels to show or hide")]
        public CanvasGroup showWhenNotRecording;
        public CanvasGroup showWhenRecording;


        [Header("Indicator of recording status")]
        public Button startStopLogging;
        public Image recordingStatusImage;
        public TextMeshProUGUI recordingStatusButtonText;
        public string isRecordingText = "Stop Session";
        public string isNotRecordingText = "Start Session";
        public GameObject sessionTimeParent;
        public TextMeshProUGUI sessionTimeText;

        public Color connectedColor = new Color(0,1,0);
        public Color disconnectedColor = new Color(1,0,0);

        bool isHideConsoleCoroutineRunning = false;

        bool isLogging = false;

        public static LogMessageUI instance;

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

        // Start is called before the first frame update
        void Start()
        {
            if(instance.consoleOutputText != null)
            {
                instance.consoleContainer.SetActive(false);
            }

            SetRecordingStatus(false);

            WriteConsoleText("Log files stored in:" + SettingsManager.Values.logSettings.mainLogFolder);
        }

        void OnEnable()
        {
            EoM_Events.OnLoggingStateChanged += SetRecordingStatus;
        }


        void OnDisable()
        {
            EoM_Events.OnLoggingStateChanged -= SetRecordingStatus;
        }

        void Update()
        {
            ShowElapsedSessionTime();
        }

        // CONSOLE TEXT

        public void WriteConsoleText(string text)
        {
            if(!showConsoleBar) return;
            if(instance.consoleOutputText != null)
            {
                consoleOutputText.text = text;
                ShowConsoleContainer();
            }
        }

        public void ShowConsoleContainer()
        {
            if(!isHideConsoleCoroutineRunning)
            {
                StartCoroutine(ShowConsoleContainer(5.0f));
            }
        }

        IEnumerator ShowConsoleContainer(float timeout)
        {
            consoleContainer.SetActive(true);
            isHideConsoleCoroutineRunning = true;
            yield return new WaitForSeconds(timeout);

            if(instance.consoleOutputText != null)
            {
                instance.consoleContainer.SetActive(false);
                isHideConsoleCoroutineRunning = false;
            }
        }

        private void ShowElapsedSessionTime()
        {
            if(sessionTimeParent.activeSelf)
            {
                sessionTimeText.text = ExciteOMeterManager.GetTimestamp().ToString("F0");
            }
        }

        // RECORDING

        public void StartStopRecording()
        {
            // Deactivate button to avoid multiple clicks
            //startStopLogging.interactable = false; // BUG: Sometimes does not stop recording and we cannot stop because is not clickable

            // Stop all the logging
            if(isLogging)
            {
                // Logging has started
                ExciteOMeterOnlineUI.instance.ClearSessionMarkers();
            }
            else
            {
                // Logging has stopped
            }

            // Call general 
            ExciteOMeterManager.instance.StartOrStopSessionLog();
        }


        public void SetRecordingStatus(bool status)
        {
            // Update local variable
            isLogging = status;

            // Reenable button of logger
            //startStopLogging.interactable = true;

            // Show buttons with colors
            recordingStatusImage.color = status? connectedColor : disconnectedColor;
            recordingStatusButtonText.text = status? isRecordingText : isNotRecordingText;

            // Show/Hide timer message
            sessionTimeParent.SetActive(status? true : false);

            // Show and hide panels
            showWhenNotRecording.interactable = status? false : true;
            showWhenNotRecording.alpha = status? 0.2f : 1.0f;

            showWhenRecording.interactable = status? true : false;
            showWhenRecording.alpha = status? 1.0f : 0.2f;
        }

    }
}


