using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter{
    public class MarkersReceiver : MonoBehaviour
    {
        /// Receives strings from markers and events (like screenshots) and store it in the proper format.

        void OnEnable()
        {
            EoM_Events.OnStringReceived += ProcessStringLog;
        }

        void OnDisable()
        {
            EoM_Events.OnStringReceived -= ProcessStringLog;
        }

        void ProcessStringLog(ExciteOMeter.DataType type, float timestamp, string message, MarkerLabel label)
        {
            bool written = LoggerController.instance.WriteLine(LogName.EventsAndMarkers,
                                                ExciteOMeterManager.ConvertFloatToString(timestamp)+ "," +
                                                type.ToString() + "," +
                                                message + "," +
                                                label.ToString() );

            if (!written)
                Debug.LogWarning("The Logger Controller has not been setup to store strings. Please setup a file with LogID EventsAndMarkers.");
        }
    }

}