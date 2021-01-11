using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    public class LSL_Inlet_ECG : InletIntSamples
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
        protected  void Process2(int numSamples, int[,] newSamples, double[] timeStamps)
        //protected override void Process(int numSamples, int[,] newSamples, double[] timeStamps)
        {
            ExciteOMeterManager.DebugLog("ECG NumSamples: " + numSamples);

            // pull as long samples are available
            for (int i = 0; i < numSamples; i++)
            {
                if(newSamples[0,i] > 1.0e7)  // Check values with bad parsing
                {
                    ExciteOMeterManager.DebugLog("Error parsing value: " + BitConverter.ToString(BitConverter.GetBytes(newSamples[0,i])) + ", " + newSamples[0,i].ToString("F2"));
                    continue;
                }
                else
                {
                    EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), newSamples[0,i]);
                    LoggerController.instance.WriteLine(LogName.VariableRawECG, newSamples[0,i].ToString("F0") + "," + newSamples[0,i].ToString("F0"));
                }
            }

            // Debug.Log($"Receiving from stream {StreamName}, first sample {newSample[0]}");

            //TODO: The event only sends float[], all samples need to be parsed to float
            
            // EoM_Events.Send_OnDataReceived(VariableType, new float[1]{(float) newSample[0]});            
            //LoggerController.instance.WriteLine(LogName.VariableRawECG, timestamp.ToString("F0") + "," + newSample[0].ToString("F0"));
        }

        // In case it is inheriting from InletIntSample instead of InletIntChunk
        // protected void Process2(int[] newSample, double timestamp)
        protected override void Process(int[] newSample, double timestamp)
        {
            // Debug.Log($"Receiving from stream {StreamName}, first sample {newSample[0]}");

            //TODO: The event only sends float[], all samples need to be parsed to float
            
            if(newSample[0] > 1.0e7)  // Check values with bad parsing
            {
                Debug.Log("Error parsing value ECG: " + BitConverter.ToString(BitConverter.GetBytes(newSample[0])) + ", " + newSample[0].ToString("F2"));
            }
            else
            {
                EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), newSample[0]);            
                LoggerController.instance.WriteLine(LogName.VariableRawECG, timestamp.ToString("F0") + "," + newSample[0].ToString("F0"));
            }
        }
    }
}
