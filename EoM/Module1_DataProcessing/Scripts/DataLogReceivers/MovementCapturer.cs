using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter{
    public class MovementCapturer : MonoBehaviour
    {
        /// Receives strings from markers and events (like screenshots) and store it in the proper format.

        public LogName logToWrite = LogName.UNDEFINED;
        private float sendingPeriod = 1.0f;  // 0 records all frames

        private Transform objectToTrack;

        //[Header("Writing period")]
        //[Tooltip("After how many seconds should the data be written.")]
        
        private float elapsedTime = 0.0f;   
        private bool logIsWriting = true;
        private bool isConfigured = false;

        private float[] transformArray = new float[Constants.TransformDataTypeSuffixes.Length];
        private DataType[] typesTransformArray;

        private string transformArrayText;


        void Start()
        {
            if(objectToTrack == null)
                objectToTrack = gameObject.transform;
        }

        public void Configure(float _sendingPeriod, LogName _logToWrite)
        {
            sendingPeriod = _sendingPeriod;
            logToWrite = _logToWrite;
            typesTransformArray = Constants.SubsetOfTransformDataTypes(logToWrite);
            if (typesTransformArray == null)
            {
                Debug.LogError("Movement can only be stored in LogName.TransformXXXX. Please change the logToWrite from `MovementRecorder`.");
                return;
            }

            // Flag
            isConfigured = true;
        }

        public void FixedUpdate()
        {
            if (isConfigured && ExciteOMeterManager.currentlyRecordingSession)
            {
                // Timer control
                elapsedTime += Time.fixedDeltaTime;

                // Send data each "sendingPeriod"
                if (elapsedTime >= sendingPeriod)
                {
                    // Reset timer for next event
                    elapsedTime = 0.0f;
                    // Define array
                    transformArray[0] = objectToTrack.position.x;
                    transformArray[1] = objectToTrack.position.y;
                    transformArray[2] = objectToTrack.position.z;
                    transformArray[3] = objectToTrack.localRotation.w;  // Quaternion without axis.
                    transformArray[4] = objectToTrack.localRotation.x;  // Q in the axis i
                    transformArray[5] = objectToTrack.localRotation.y;  // Q in the axis j
                    transformArray[6] = objectToTrack.localRotation.z;  // Q in the axis k
                    transformArray[7] = objectToTrack.localEulerAngles.x; // Pitch (up-down)
                    transformArray[8] = objectToTrack.localEulerAngles.y; // Yaw (left-right)
                    transformArray[9] = objectToTrack.localEulerAngles.z; // Roll (towards shoulders)

                    // Create string to save in CSV
                    transformArrayText = "";
                    foreach (float v in transformArray)
                        transformArrayText += "," + ExciteOMeterManager.ConvertFloatToString(v,4);

                    logIsWriting = LoggerController.instance.WriteLine(logToWrite, ExciteOMeterManager.GetTimestampString() + transformArrayText);
                    if (!logIsWriting)
                        Debug.LogWarning("Error writing movement data. Please setup LoggerController with a file with LogID is" + logToWrite.ToString());

                    //Debug.Log("Sending Movement from " + gameObject.name + " > " + transformArrayText);

                    // Send an event with the multidimensional data for the receivers taht can handle multidimensionality
                    EoM_Events.Send_OnDataArrayReceived(DataType.Headset_array, ExciteOMeterManager.GetTimestamp(), transformArray);

                    // Send values individually, because they need to be stored in the .json file as unidimensional data, so that they can be visualized

                    // --------- TODO
                    //// Visualizer is designed to analyze unidimensional data, therefore multidimensional needs to be sent one by one to the system
                    //StartCoroutine(SendDataEventsMovement(ExciteOMeterManager.GetTimestamp()));
                    // BUG: Sending events from the coroutine does not seem to be received...
                    // ---------
                    // If the above works, delete the remaining code!! ----------------------
                    if (transformArray.Length != typesTransformArray.Length)
                        Debug.LogError("The movement arrays are not the same length");

                    for (int i = 0; i < transformArray.Length; i++)
                    {
                        EoM_Events.Send_OnDataReceived(typesTransformArray[i], ExciteOMeterManager.GetTimestamp(), transformArray[i]);
                    }
                    /// --------------------------------------------------------

                }
            }
        }

        IEnumerator SendDataEventsMovement(float timestamp)
        {
            if (transformArray.Length != typesTransformArray.Length)
                Debug.LogError("The movement arrays are not the same length");

            for(int i=0; i<transformArray.Length; i++)
            {
                EoM_Events.Send_OnDataReceived(typesTransformArray[i], timestamp, transformArray[i]);
                yield return null;
            }
        }
    }

}