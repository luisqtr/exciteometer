using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExciteOMeter;

public class EoM_DataReceiver : MonoBehaviour
{
    private bool isCurrentlyRecording = false;

    void OnEnable()
    {
        // Subscribe to events when this gameobject is active
        EoM_Events.OnLoggingStateChanged += ExciteOMeterLoggingStateChanged;
        EoM_Events.OnDataReceived += ExciteOMeterDataReceived;
        EoM_Events.OnStringReceived += ExciteOMeterMarkerReceived;
    }

    void OnDisable()
    {
        // Unsubscribe from events when this gameobject is inactive
        EoM_Events.OnLoggingStateChanged -= ExciteOMeterLoggingStateChanged;
        EoM_Events.OnDataReceived -= ExciteOMeterDataReceived;
        EoM_Events.OnStringReceived -= ExciteOMeterMarkerReceived;
    }


    private void ExciteOMeterDataReceived(DataType type, float timestamp, float value)
    {
        ///// You can uncomment the line below to receive data only when 
        ///// the Excite-O-Meter is recording data.
        //if (!isCurrentlyRecording) return;

        switch (type)
        {
            case DataType.NONE:
                break;
            case DataType.HeartRate:
                Debug.Log($"Received HR with timestamp {timestamp}, value {value}");
                break;
            case DataType.RRInterval:
                Debug.Log($"Received RR-interval with timestamp {timestamp}, value {value}");
                break;
            case DataType.RawECG:
                // Currently not available
                break;
            case DataType.RawACC:
                // Currently not available
                break;
            case DataType.RMSSD:
                Debug.Log($"Received calculated feature RMSSD with timestamp {timestamp}, value {value}");
                break;
            case DataType.SDNN:
                Debug.Log($"Received calculated feature SDNN with timestamp {timestamp}, value {value}");
                break;
            case DataType.EOM:
                break;
            case DataType.AutomaticMarkers:
                // These events are captured by `EoM_Events.OnStringReceived`
                break;
            case DataType.ManualMarkers:
                // These events are captured by `EoM_Events.OnStringReceived`
                break;
            case DataType.Screenshots:
                Debug.Log("A new screenshot was generated in the recorded session");
                break;
            default:
                break;
        }
    }


    private void ExciteOMeterMarkerReceived(DataType type, float timestamp, string message, MarkerLabel label)
    {
        Debug.Log($"A new event marker was generated with type {type}, timestamp {timestamp.ToString("F0")} and message {message}");
    }

    private void ExciteOMeterLoggingStateChanged(bool newState)
    {
        isCurrentlyRecording = newState;
        Debug.Log("The logging state of a new session has changed to" + newState);    
    }

}
