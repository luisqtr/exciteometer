using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// Handles the visual graph bar
        
// TODO some pooling might be smart
// TODO Tooltip general
        
namespace ExciteOMeter.Vizualisation
{
    public class InstantMarkersData : TimeLineEntry
    {
        [Header("Refs")]
        public GameObject   instantMarkerItemPrefab;
        public GameObject   holderInstantMarkers;
        public Image        bar;

        public SessionData  sessionData;
        public DataType     dataType; // the requested data type 

        private List<GameObject> generatedInstantMarkerItems = new List<GameObject>();
        
    // ====================================================================================================

        void OnEnable()
        {
            EoM_Events.OnRefreshGraphs += RefreshGraph;
            EoM_Events.OnRemoveAllFromSession += RemoveAllFromSession;
        }

        void OnDisable()
        {
            EoM_Events.OnRefreshGraphs -= RefreshGraph;
            EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSession;
        }

    // ====================================================================================================
    
        public void InitData(SessionData _sessionData, DataType _dataType) 
        {
            sessionData = _sessionData;
            dataType    = _dataType;

            valueDisplayer.SetBarColor(sessionData.sessionColor);
            valueDisplayer.SetMarkersInfo( TimeLineHelpers.SplitCamelCase(_dataType.ToString()), sessionData.sessionVariables.timeseries[_dataType].timestamp.Count.ToString());
            
            // Tell the button on the displayer to remove entire entry when clicked
            valueDisplayer.markersCloseButton.onClick.AddListener(delegate { RemoveBridge(); });

            // Flag this datatype as shown
            sessionData.dataTypes[_dataType] = true;
            
            RefreshGraph();
        }

        void RemoveBridge ()
        {
            //! Needs a bridge to enter the inherited methods
            RemoveEntry ();
        }

        void RemoveAllFromSession (int _sessionDataKey)
        {
            if (sessionData.sessionDataKey == _sessionDataKey)
            {
                EoM_Events.OnRefreshGraphs -= RefreshGraph;
                EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSession;
                RemoveEntry();
            }
        }

        public void RefreshGraph () 
        {
            ClearGraph ();

            for (int t=0;t<sessionData.sessionVariables.timeseries[dataType].timestamp.Count;t++) 
            {
                if (sessionData.sessionVariables.timeseries[dataType].timestamp[t] < Timeline.instance.startTime)
                    continue;

                if (sessionData.sessionVariables.timeseries[dataType].timestamp[t] > Timeline.instance.endTime)
                    continue;

                GameObject instantMarker = Instantiate(instantMarkerItemPrefab, holderInstantMarkers.transform);
                instantMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(Timeline.instance.CalculatePositionOnTimeline(sessionData.sessionVariables.timeseries[dataType].timestamp[t]),0f);

                instantMarker.GetComponent<InstantMarkerItem>().Init(GetmarkerColor(t), sessionData.sessionVariables.timeseries[dataType].text[t]);

                generatedInstantMarkerItems.Add(instantMarker);
            }
        }

        Color GetmarkerColor (int markerIndex)
        {
            switch(dataType)
            {
                case DataType.AutomaticMarkers:
                    if (sessionData.sessionVariables.timeseries[dataType].value.Count > 0)
                        return ExciteOMeter.Vizualisation.VisualStyle.InstantMarkersColorTable[(MarkerLabel)Convert.ToInt32(sessionData.sessionVariables.timeseries[dataType].value[markerIndex])];
                    else return sessionData.sessionColor;
                // break; // Generates warning

                case DataType.ManualMarkers:
                    return sessionData.sessionColor;
                // break; // Generates warning

                default:
                    return sessionData.sessionColor;    
            }
        }

        void ClearGraph () 
        {
            if (generatedInstantMarkerItems.Count > 0)
            {
                foreach(GameObject g in generatedInstantMarkerItems)
                    Destroy(g);

                generatedInstantMarkerItems.Clear();
            }
        }

    }

}