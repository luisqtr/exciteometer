using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// TODO When adding a subgraph, inform the valuedisplayer to inject a textfield for displaying data from THIS graph. So multile subgraphs = multiple textfields

namespace ExciteOMeter.Vizualisation
{
    public class LineGraphData : TimeLineEntry
    {
        [Header("Refs")]
        public Transform holderGraphs;
        public GameObject lineGraphItemPrefab;
        public Image bar;
        int subGraphIDcounter = 0;

        [Header("Settings")]
        public List<LineGraphItem> subGraphs = new List<LineGraphItem>();

        void OnEnable() {
            EoM_Events.OnRemoveAllFromSession += RemoveAllFromSessionDataKey;
        }

        void OnDisable() {
            EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSessionDataKey;
        }


        public void AddSubLineGraph (SessionData _sessionData, DataType _dataType)
        {
            if (subGraphs.Count == TimelineSettings.maxSubGraphs)
            {
                Debug.LogWarning("Reached the maximum amount of subgraphs");
                return;
            }

            // Add new subgraph
            subGraphIDcounter++;

            GameObject newGraphObj = Instantiate(lineGraphItemPrefab, holderGraphs);
            LineGraphItem newGraph = newGraphObj.GetComponent<LineGraphItem>();
            subGraphs.Add(newGraph);
            
            // Don't change colors if we are EOM
            if (entryType != EntryTypes.EXCITEOMETER)
            {
                valueDisplayer.AddSubGraphInfo
                ( 
                    _dataType.ToString(), 
                    _sessionData.sessionVariables.timeseries[_dataType].value.Max(), 
                    _sessionData.sessionVariables.timeseries[_dataType].value.Min(),
                    _sessionData.sessionColor,
                    subGraphIDcounter,
                    _sessionData.sessionDataKey
                );   
            } 
            else 
            {
                valueDisplayer.AddSubGraphInfo(_sessionData.sessionFolder.sessionId, 1, 0, _sessionData.sessionColor, subGraphIDcounter, _sessionData.sessionDataKey);
            }

            // Generate visual graph
            newGraph.GetComponent<LineGraphItem>().InitGraph(_sessionData, valueDisplayer, subGraphIDcounter, _dataType);
        }
        
        public void RemoveAllFromSessionDataKey (int _sessionDataKey)
        {
            // Does this bar show graphs from that session?
            for (int i=0;i<subGraphs.Count;i++) 
            {
                if (subGraphs[i].sessionData.sessionDataKey == _sessionDataKey)
                    RemoveSubGraph(subGraphs[i].subGraphID);
            }
        }

        public void RemoveSubGraph (int _subGraphID) 
        {
            Debug.Log("RemoveSubGraph: " + _subGraphID);

            // Find this graph
            int subGraphIndex = subGraphs.FindIndex(x=>x.subGraphID == _subGraphID);

            // If there is only one subgraph, remove entire entry
            if ((subGraphs.Count == 1) && (entryType != EntryTypes.EXCITEOMETER) )
                RemoveEntry();

            // flag as not shown again
            subGraphs[subGraphIndex].sessionData.dataTypes[subGraphs[subGraphIndex].dataType] = false;

            Destroy(subGraphs[subGraphIndex].gameObject);
            subGraphs.RemoveAt(subGraphIndex);


        }
    }
}
