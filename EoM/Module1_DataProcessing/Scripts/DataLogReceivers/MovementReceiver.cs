using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter{
    public class MovementReceiver : MonoBehaviour
    {
        /// Receives strings from markers and events (like screenshots) and store it in the proper format.

        public Transform objectToTrack;
        public LogName logToWrite = LogName.UNDEFINED;

        [Header("Writing period")]
        [Tooltip("After how many seconds should the data be written.")]
        public float sendingPeriod = 1.0f;  // 0 records all frames
        private float elapsedTime = 0.0f;   
        private bool logIsWriting = true;   


        void Start()
        {
            if(objectToTrack == null)
                objectToTrack = gameObject.transform;

            if (logToWrite == LogName.UNDEFINED)
                Debug.LogError("Setup the logToWrite variable correctly, based on LoggerController. Currently logging in general log file of ExciteOMeter");
        }

        void FixedUpdate()
        {
            if(ExciteOMeterManager.currentlyRecordingSession)
            {
                // Timer control
                elapsedTime += Time.deltaTime;

                // Send data each "sendingPeriod"
                if (elapsedTime >= sendingPeriod) 
                {
                    // Reset timer for next event
                    elapsedTime = 0.0f;

                    logIsWriting = LoggerController.instance.WriteLine(logToWrite, 
                                                    ExciteOMeterManager.GetTimestamp().ToString("F5") + "," +
                                                    objectToTrack.position.x.ToString("F5") + "," +
                                                    objectToTrack.position.y.ToString("F5") + "," +
                                                    objectToTrack.position.z.ToString("F5") + "," +
                                                    objectToTrack.rotation.w.ToString("F5") + "," +
                                                    objectToTrack.rotation.x.ToString("F5") + "," +
                                                    objectToTrack.rotation.y.ToString("F5") + "," +
                                                    objectToTrack.rotation.z.ToString("F5")
                                                    );

                    if (!logIsWriting)
                        Debug.LogWarning("Error writing movement data. Please setup LoggerController with a file with LogID is" + logToWrite.ToString() );
                }
            }
        }
    }

}