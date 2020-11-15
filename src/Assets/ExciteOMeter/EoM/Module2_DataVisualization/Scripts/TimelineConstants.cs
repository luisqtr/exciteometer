using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ExciteOMeter.Vizualisation
{
    
    public enum EntryTypes { DEFAULT, INSTANTMARKERS, LINEGRAPH, EXCITEOMETER };
    
    
    public static class TimelineSettings
    {
        public static int zoomIncrementValue = 1;
        public static int entryToggledHeight = 300;
        public static int maxSubGraphs = 4;
    }


    public static class VisualStyle 
    {
        public static Dictionary<ExciteOMeter.MarkerLabel, Color> InstantMarkersColorTable = new Dictionary<MarkerLabel, Color>() 
        {
            {ExciteOMeter.MarkerLabel.NONE,              new Color32(184,184,184,150)},
            {ExciteOMeter.MarkerLabel.CUSTOM_MARKER,     new Color32(70,168,174,150)},
            {ExciteOMeter.MarkerLabel.ABNORMAL_HR,       new Color32(246,83,107,150)},
            {ExciteOMeter.MarkerLabel.ABNORMAL_RMSSD,    new Color32(180,33,46,150)}
        };

        public static Dictionary<EntryTypes, Color> TimeLineEntryColorTable = new Dictionary<EntryTypes, Color>() 
        {
            {EntryTypes.DEFAULT,                        new Color32(0x38,0x38,0x38,0xFF)}, //#383838
            {EntryTypes.INSTANTMARKERS,                 new Color32(32,38,36,255)},
            {EntryTypes.LINEGRAPH,                      new Color32(33,35,38,255)},
            {EntryTypes.EXCITEOMETER,                   new Color32(255,255,255,255)}
        };

        public static List<Color32> SessionsColorTable = new List<Color32>()
        {
            { new Color32 (0x02,0x75,0xd8,0xff ) }, // #0275d8
            { new Color32 (0x5c,0xb8,0x5c,0xff ) }, // #5cb85c
            { new Color32 (0x5b,0xc0,0xde,0xff ) }, // #5bc0de
            { new Color32 (0xf0,0xad,0x4e,0xff ) }, // #f0ad4e
            { new Color32 (0xaf,0x00,0xb9,0xff ) }, // #AF00B9
            { new Color32 (0xaf,0x3f,0x10,0xff ) }, // #AF3F10
        };
    }

    public static class TimeLineHelpers
    {
        // Global methods to help the timeline

        // Lookup what to generate for what type of datatype
        private static Dictionary<List<DataType>, ExciteOMeter.Vizualisation.EntryTypes> dataTypeLookup = new Dictionary<List<DataType>, Vizualisation.EntryTypes>() 
        {
            { new List<DataType>() { DataType.AutomaticMarkers, DataType.ManualMarkers }, EntryTypes.INSTANTMARKERS },
            { new List<DataType>() { DataType.EOM,DataType.HeartRate, DataType.RawACC, DataType.RawECG, DataType.RMSSD,DataType.RMSSD, DataType.RRInterval, DataType.SDNN }, EntryTypes.LINEGRAPH },
        };

        public static EntryTypes GetEntryTypeForDataType (DataType _dataType)
        {
            foreach( KeyValuePair<List<DataType>,EntryTypes> td in dataTypeLookup)
            {
                if (td.Key.Contains(_dataType)) 
                {
                    return td.Value;
                }
            }

            Debug.LogWarning("Got:" + _dataType + " and cant find a matching entryType so using " + EntryTypes.DEFAULT.ToString());
            return EntryTypes.DEFAULT;
        }

        public static System.DateTime ConvertToDateTime (string _dateTimeString) 
        {
            return DateTime.ParseExact(_dateTimeString,"yyyy-MM-dd|HH:mm:ss.fff", null);
        }

        public static string GetTimeFormat (float _time, bool _msecs)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(_time);
            //string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            return _msecs ? string.Format("{0:D2}:{1:D2}:{2:D2}", t.Minutes, t.Seconds, t.Milliseconds) : string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds) ;
        }

        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static float GetValueAtTimestamp (float _timeStamp, SessionVariables.TimeSeries _data)
        {
            float foundValue = 0f;

            for (int v=0;v<_data.value.Count;v++) 
            {
                if (_data.timestamp[v] > _timeStamp)
                    break;

                foundValue = _data.value[v];
            }

            return foundValue;
        }


    }

}