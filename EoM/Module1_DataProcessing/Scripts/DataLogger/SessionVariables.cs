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