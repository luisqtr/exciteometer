using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using System.Linq;
using System.Threading;

/*


*/

// TODO Now using a slight delay to refresh the graph as the canvas recttransform is apparently still marked as dirty. Divide into +1 canvasgroups?
// TODO Convert GetValueAtTimestamp to use a proven quick search algorithm (ie BinarySearch)

namespace ExciteOMeter.Vizualisation
{
    public class LineGraphItem : MonoBehaviour
    {
        private string                      graphDescription = string.Empty;
        private UILineRenderer              graphRenderer;
        private SessionVariables.TimeSeries data;
        public DataType                     dataType;

        public float                        maxValue;
        public float                        minValue;
        public int                          subGraphID; // To update the correct textfield in the valueDisplayer

        private ValueDisplayer              valueDisplayer;
        public SessionData                  sessionData;

        void Awake() 
        {
            graphRenderer = GetComponent<UILineRenderer>();    
        }

        void OnEnable()
        {
            EoM_Events.OnRefreshGraphs              += RefreshGraphAsync;
            EoM_Events.OnUpdateCurrentTimeValue     += UpdateCurrentTimeValue;
        }

        void OnDisable()
        {
            EoM_Events.OnRefreshGraphs              -= RefreshGraphAsync;
            EoM_Events.OnUpdateCurrentTimeValue     -= UpdateCurrentTimeValue;
        }


        public void InitGraph (SessionData _sessionData, ValueDisplayer _valueDisplayer, int _subGraphID, DataType _dataType)
        {
            sessionData         = _sessionData;
            valueDisplayer      = _valueDisplayer;
            subGraphID          = _subGraphID;
            data                = sessionData.sessionVariables.timeseries[_dataType];
            graphRenderer.color = sessionData.sessionColor;

            dataType            = _dataType;
            minValue            = _dataType == DataType.EOM ? 0f : data.value.Min();
            maxValue            = _dataType == DataType.EOM ? 1f : data.value.Max();

            // Flag this datatype as shown
            sessionData.dataTypes[_dataType] = true;

            RefreshGraphAsync ();
        }

        public void UpdateCurrentTimeValue (float _timeStamp)
        {
            float result =  TimeLineHelpers.GetValueAtTimestamp(_timeStamp, data);
            valueDisplayer.UpdateGraphValues( subGraphID, result);

            if (dataType == DataType.EOM)
                sessionData.currentEOMvalue = result;
        }

        // Updates visual graph with new value
        async void RefreshGraphAsync ()
        {
            await Task.Delay(10);

            List<Vector2> newXY = new List<Vector2>();
            
            for (int v=0; v<data.value.Count; v++)
            {
                if (data.timestamp[v] < Timeline.instance.startTime)
                    continue;

                if (data.timestamp[v] > Timeline.instance.endTime)
                    continue;

                newXY.Add (
                    new Vector2( 
                        (Timeline.instance.CalculatePositionOnTimeline(data.timestamp[v]) / this.GetComponent<RectTransform>().rect.width), 
                        (float)ExciteOMeter.Helpers.RangeMapper(data.value[v], minValue, maxValue, 0f, 1f, 2)
                    )
                );
            }

            // redraw
            graphRenderer.Points = newXY.ToArray();
        }

    }
}