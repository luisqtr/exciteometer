using System;
using System.Collections.Generic;
using System.Linq;

namespace ExciteOMeter
{
    /// <summary>
    /// Serializable class to save and load the with JSON file manager.
    /// </summary>
    [System.Serializable]
    public class SessionVariables
    {
        [System.Serializable]
        public class TimeSeries{
            public List<float> timestamp;
            public List<float> value;
            public List<string> text;
            public DataType type = DataType.NONE;

            public TimeSeries(DataType _type)
            {
                timestamp = new List<float>();
                value = new List<float>();
                text = new List<string>();
                type = _type;
            }
        }

        [System.Serializable]
        public class InstantMarker{
            public float timestamp;
            public MarkerLabel label = MarkerLabel.NONE;
            public string message;

            public InstantMarker ()
            {
                // Empty construtor to not break the code
            }

            public InstantMarker (float _timeStamp, MarkerLabel _label, string _msg)
            {
                timestamp   = _timeStamp;
                label       = _label;
                message     = _msg;
            }
        }

        // DEFINITION OF PROPERTIES

        // To access the texts of these TimeSeries use the property `value`
        public TimeSeries HR    = new TimeSeries(DataType.HeartRate);
        public TimeSeries RRi   = new TimeSeries(DataType.RRInterval);
        public TimeSeries RMSSD = new TimeSeries(DataType.RMSSD);
        public TimeSeries SDNN  = new TimeSeries(DataType.SDNN);
        public TimeSeries EOM   = new TimeSeries(DataType.EOM);

        // To access the texts of these TimeSeries use the property `text`
        public TimeSeries automaticMarkers = new TimeSeries(DataType.AutomaticMarkers);
        public TimeSeries manualMarkers = new TimeSeries(DataType.ManualMarkers);
        public TimeSeries screenshots = new TimeSeries(DataType.Screenshots);

        // Headset transform
        public TimeSeries headset_posX = new TimeSeries(DataType.Headset_posX);
        public TimeSeries headset_posY = new TimeSeries(DataType.Headset_posY);
        public TimeSeries headset_posZ = new TimeSeries(DataType.Headset_posZ);
        public TimeSeries headset_q0 = new TimeSeries(DataType.Headset_q0);
        public TimeSeries headset_qi = new TimeSeries(DataType.Headset_qi);
        public TimeSeries headset_qj = new TimeSeries(DataType.Headset_qj);
        public TimeSeries headset_qk = new TimeSeries(DataType.Headset_qk);
        public TimeSeries headset_yaw = new TimeSeries(DataType.Headset_yaw);
        public TimeSeries headset_pitch = new TimeSeries(DataType.Headset_pitch);
        public TimeSeries headset_roll = new TimeSeries(DataType.Headset_roll);

        // Headset features
        public TimeSeries headset_velPosX = new TimeSeries(DataType.headset_velPosX);
        public TimeSeries headset_velPosY = new TimeSeries(DataType.headset_velPosY);
        public TimeSeries headset_velPosZ = new TimeSeries(DataType.headset_velPosZ);
        public TimeSeries headset_accPosX = new TimeSeries(DataType.headset_accPosX);
        public TimeSeries headset_accPosY = new TimeSeries(DataType.headset_accPosY);
        public TimeSeries headset_accPosZ = new TimeSeries(DataType.headset_accPosZ);

        public TimeSeries headset_velQuat0 = new TimeSeries(DataType.headset_velQ0);
        public TimeSeries headset_velQuati = new TimeSeries(DataType.headset_velQi);
        public TimeSeries headset_velQuatj = new TimeSeries(DataType.headset_velQj);
        public TimeSeries headset_velQuatk = new TimeSeries(DataType.headset_velQk);
        public TimeSeries headset_accQuat0 = new TimeSeries(DataType.headset_accQ0);
        public TimeSeries headset_accQuati = new TimeSeries(DataType.headset_accQi);
        public TimeSeries headset_accQuatj = new TimeSeries(DataType.headset_accQj);
        public TimeSeries headset_accQuatk = new TimeSeries(DataType.headset_accQk);

        public TimeSeries headset_velEuleri = new TimeSeries(DataType.headset_velPitch);
        public TimeSeries headset_velEulerj = new TimeSeries(DataType.headset_velYaw);
        public TimeSeries headset_velEulerk = new TimeSeries(DataType.headset_velRoll);
        public TimeSeries headset_accEuleri = new TimeSeries(DataType.headset_accPitch);
        public TimeSeries headset_accEulerj = new TimeSeries(DataType.headset_accYaw);
        public TimeSeries headset_accEulerk = new TimeSeries(DataType.headset_accRoll);

        // The main dict used in the viz
        public Dictionary<DataType, TimeSeries> timeseries = new Dictionary<DataType, TimeSeries>();
        
        public void CreateDictionaryOfTimeSeries()
        {
            timeseries = new Dictionary<DataType, TimeSeries>();   
            // To access the texts of these TimeSeries use the property `value` because are float time series
            timeseries.Add(DataType.HeartRate,      HR);
            timeseries.Add(DataType.RRInterval,     RRi);
            timeseries.Add(DataType.RMSSD,          RMSSD);
            timeseries.Add(DataType.SDNN,           SDNN);
            timeseries.Add(DataType.EOM,            EOM);

            // To access the texts of these TimeSeries use the property `text` because are strings
            timeseries.Add(DataType.AutomaticMarkers,   automaticMarkers);
            timeseries.Add(DataType.ManualMarkers,      manualMarkers);
            timeseries.Add(DataType.Screenshots,        screenshots);

            // To access the texts of these TimeSeries use the property `value` because are float time series
            // Multidimensional time series for head movement
            timeseries.Add(DataType.Headset_posX, headset_posX);
            timeseries.Add(DataType.Headset_posY, headset_posY);
            timeseries.Add(DataType.Headset_posZ, headset_posZ);
            timeseries.Add(DataType.Headset_q0, headset_q0);
            timeseries.Add(DataType.Headset_qi, headset_qi);
            timeseries.Add(DataType.Headset_qj, headset_qj);
            timeseries.Add(DataType.Headset_qk, headset_qk);
            timeseries.Add(DataType.Headset_yaw, headset_yaw);
            timeseries.Add(DataType.Headset_pitch, headset_pitch);
            timeseries.Add(DataType.Headset_roll, headset_roll);

            // Features head movement
            timeseries.Add(DataType.headset_velPosX, headset_velPosX);
            timeseries.Add(DataType.headset_velPosY, headset_velPosY);
            timeseries.Add(DataType.headset_velPosZ, headset_velPosZ);
            timeseries.Add(DataType.headset_accPosX, headset_accPosX);
            timeseries.Add(DataType.headset_accPosY, headset_accPosY);
            timeseries.Add(DataType.headset_accPosZ, headset_accPosZ);

            timeseries.Add(DataType.headset_velQ0, headset_velQuat0);
            timeseries.Add(DataType.headset_velQi, headset_velQuati);
            timeseries.Add(DataType.headset_velQj, headset_velQuatj);
            timeseries.Add(DataType.headset_velQk, headset_velQuatk);
            timeseries.Add(DataType.headset_accQ0, headset_accQuat0);
            timeseries.Add(DataType.headset_accQi, headset_accQuati);
            timeseries.Add(DataType.headset_accQj, headset_accQuatj);
            timeseries.Add(DataType.headset_accQk, headset_accQuatk);

            timeseries.Add(DataType.headset_velPitch, headset_velEuleri);
            timeseries.Add(DataType.headset_velYaw, headset_velEulerj);
            timeseries.Add(DataType.headset_velRoll, headset_velEulerk);
            timeseries.Add(DataType.headset_accPitch, headset_accEuleri);
            timeseries.Add(DataType.headset_accYaw, headset_accEulerj);
            timeseries.Add(DataType.headset_accRoll, headset_accEulerk);


            //foreach (DataType headset_datatype in Constants.SubsetOfTransformDataTypes(LogName.TransformHeadset))
            //{
            //    timeseries.Add(headset_datatype, new TimeSeries(headset_datatype));
            //}

            // TODO: The same than above for controllers
        }

        public void RemoveEmptyTimeSeries()
        {
            // Creates the dictionary with the timeseries and deletes the entries without data.            
            CreateDictionaryOfTimeSeries();

            DataType[] keys = new DataType[timeseries.Count];
            timeseries.Keys.CopyTo(keys,0);
            foreach(DataType dt in keys)
            {
                if(timeseries[dt].timestamp.Count == 0)
                {
                    // It exists, but it is empty.
                    timeseries.Remove(dt);
                }
            }
        }
    }

    /// <summary>
    /// Class used to serialize the list of available sessions on a specific folder.
    /// </summary>
    [System.Serializable]
    public struct SessionFolder
    {
        public string folderPath; // Which is the full path to open the specific session
        public DateTime datetime; // Useful to sort sessions from newest to oldest
        public string sessionId; // Custom names that users 
        public string sessionFilepath; // Where the json file is located
    }
}