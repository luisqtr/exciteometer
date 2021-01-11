using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    public class LSL_Inlet_RRi : InletFloatSamples
    {
        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime
            
            // registerAndLookUpStream();
        }

        /// <summary>
            /// Override this method to implement whatever should happen with the samples...
            /// IMPORTANT: Avoid heavy processing logic within this method, update a state and use
            /// coroutines for more complexe processing tasks to distribute processing time over
            /// several frames
            /// </summary>
            /// <param name="newSample"></param>
            /// <param name="timeStamp"></param>
        protected override void Process(float[] newSample, double timeStamp)
        {
            //TODO: Use the timestamp from the sensor, which is in nanoseconds.
            EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), newSample[0]);

            LoggerController.instance.WriteLine(LogName.VariableRrInterval, ExciteOMeterManager.GetTimestamp().ToString("F6") + "," + newSample[0].ToString("F3"));
        }
    }
}

