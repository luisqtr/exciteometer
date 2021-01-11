using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    public class LSL_Inlet_ACC : InletIntSamples
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
        protected override void Process(int[] newSample, double timestamp)
        {
            //Assuming that a sample contains at least 3 values for x,y,z
            // Debug.Log($"Receiving from stream {StreamName}, first sample {newSample[0]}");

            if(newSample[0] > 1.0e7)  // Check values with bad parsing
            {
                ExciteOMeterManager.DebugLog("Error parsing value ACC: " + BitConverter.ToString(BitConverter.GetBytes(newSample[0])) + ", " + newSample[0].ToString("F2"));
            }
            else
            {
                //EoM_Events.Send_OnDataReceived(VariableType, new float[3]{(float) newSample[0], (float) newSample[1], (float) newSample[2]});
                // TODO: The event for ACC should be a different delegate receiving float[] instead of single float.
                EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), (float) newSample[0]);
                EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), (float) newSample[1]);
                EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), (float) newSample[2]);

                LoggerController.instance.WriteLine(LogName.VariableRawACC, timestamp.ToString("F2") + "," + newSample[0].ToString("F0") + "," + newSample[1].ToString("F0") + "," + newSample[2].ToString("F0"));
            }
        }
    }
}