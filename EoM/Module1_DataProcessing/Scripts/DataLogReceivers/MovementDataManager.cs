using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter{
    public class MovementDataManager : MonoBehaviour
    {
        /// Receives strings from markers and events (like screenshots) and store it in the proper format.

        [Serializable]
        public class MovementRecorder
        {
            public GameObject objectToFollow;
            public ExciteOMeter.LogName logToWrite = LogName.UNDEFINED;
        }

        public List<MovementRecorder> objectsToTrack;


        private bool isLogging = false;
        private float sendingPeriod = 1.0f;

        // Variable to store the transform
        public static MovementDataManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        void OnEnable()
        {
            EoM_Events.OnLoggingStateChanged += SetRecordingStatus;
        }

        void OnDisable()
        {
            EoM_Events.OnLoggingStateChanged -= SetRecordingStatus;
        }

        public void SetRecordingStatus(bool status)
        {
            // Update local variable
            isLogging = status;
            if (isLogging && SettingsManager.Values.logSettings.recordMovementData)
            {
                // Update sending period
                sendingPeriod = (1.0f / (float)SettingsManager.Values.logSettings.recordMovementFrequency);

                foreach (MovementRecorder mov in objectsToTrack)
                {
                    if (mov.logToWrite == LogName.UNDEFINED)
                        Debug.LogWarning("Setup the logToWrite variable correctly, based on LoggerController. Currently logging in general log file of ExciteOMeter");
                    if (mov.objectToFollow == null)
                    {
                        Debug.LogError("The MovementRecorder does not contain a transform to follow. Please deactivate movement if not desired.");
                        return;
                    }
                    // Add a component that sends the rotation

                    if(mov.objectToFollow.GetComponent<MovementCapturer>() == null)
                    {
                        MovementCapturer component = mov.objectToFollow.AddComponent<MovementCapturer>();
                        component.Configure(sendingPeriod, mov.logToWrite);
                    }
                }
            }
            else if (isLogging && !SettingsManager.Values.logSettings.recordMovementData)
            {
                // Not logging anymore, delete the objects
                foreach (MovementRecorder mov in objectsToTrack)
                {
                    // Remove the component that sends the rotation
                    MovementCapturer component = mov.objectToFollow.GetComponent<MovementCapturer>();
                    if (component != null)
#if UNITY_EDITOR
                        DestroyImmediate(component);
#else
                        Destroy(component);
#endif
                }
            }
        }
    }

}