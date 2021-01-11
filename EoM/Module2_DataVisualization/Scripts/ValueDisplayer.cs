using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*


*/
// TODO meter can only work when there is only ONE graph

namespace ExciteOMeter.Vizualisation
{
    public class ValueDisplayer : MonoBehaviour
    {
        [Header("Graphics")]
        public Image colorBar;

        [Header("Markers")]
        public TextMeshProUGUI markersAmountField;
        public TextMeshProUGUI markersTypeField;
        public Button markersCloseButton;

        [Header("Prefabs")]        
        public GameObject subGraphValuePrefab;
        public GameObject subGraphMinMaxlabelPrefab;
        public GameObject subGraphSessionInfoPrefab;

        [Header("Holders")]
        public Transform subGraphValuesHolder;
        public Transform subGraphMinMaxlabelHolder;
        public Transform subGraphSessionInfoHolder;

        // Remember 
        public List<SubGraphValueItem> subGraphValues       = new List<SubGraphValueItem>();
        public List<GameObject> subGraphMinMaxlabels        = new List<GameObject>();
        public List<TextMeshProUGUI> subGraphSessionInfos   = new List<TextMeshProUGUI>();

        // Parent holder of the row
        public TimeLineEntry timeLineEntry;
        public EntryTypes entryType;

    // ====================================================================================================

        public void Init (TimeLineEntry _timeLineEntry)
        {
            timeLineEntry = _timeLineEntry;
            entryType = timeLineEntry.entryType;
        }
        
        public void SetBarColor ( Color _sessionColor) 
        {
            if (entryType != EntryTypes.EXCITEOMETER)
                colorBar.color = _sessionColor;
        }     

        public void SetMarkersInfo (string _markerType, string _amount)
        {
            markersTypeField.text = _markerType;
            markersAmountField.text = _amount;
        }

    // ====================================================================================================
    #region Sub Graphs

        public void AddSubGraphInfo (string _label, float _max, float _min, Color _sessionColor, int _subGraphID, int _sessionDataKey)
        {
            AddSubGraphValue(_sessionColor, _max, _min, _subGraphID, _sessionDataKey);

            if (entryType == EntryTypes.EXCITEOMETER)
                AddsubGraphSessionInfo(_sessionColor, _label);

            if (entryType == EntryTypes.LINEGRAPH)
            {
                AddSubGraphMinMaxLabel( _label,  _max,  _min);
            }
        }
        
        // All graphs get a value displayer
        void AddSubGraphValue (Color _sessionColor, float _max, float _min, int _subGraphID, int _sessionDataKey)
        {
            GameObject newSubGraphValue = Instantiate(subGraphValuePrefab, subGraphValuesHolder);
            subGraphValues.Add(newSubGraphValue.GetComponent<SubGraphValueItem>());
            newSubGraphValue.GetComponent<SubGraphValueItem>().Init(_sessionColor, _max, _min, _subGraphID, _sessionDataKey);

            newSubGraphValue.GetComponent<SubGraphValueItem>().closeButton.onClick.AddListener(delegate { Remove(_subGraphID, _sessionDataKey); });
        }

        // Per subgraph we have Min, Max, Label
        void AddSubGraphMinMaxLabel (string _label, float _max, float _min)
        {
            GameObject newSubGraphMinMaxlabel = Instantiate(subGraphMinMaxlabelPrefab, subGraphMinMaxlabelHolder);
            
            TextMeshProUGUI maxField = newSubGraphMinMaxlabel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            maxField.text   = _max.ToString(); // Max

            TextMeshProUGUI labelField = newSubGraphMinMaxlabel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            labelField.text = _label; // Label

            TextMeshProUGUI minField = newSubGraphMinMaxlabel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            minField.text   = _min.ToString(); // Min
            
            subGraphMinMaxlabels.Add(newSubGraphMinMaxlabel);
        }

        // ExciteOmeter has session info to display
        void AddsubGraphSessionInfo (Color _sessionColor, string _label)
        {
            GameObject newSubGraphSessionInfo = Instantiate(subGraphSessionInfoPrefab, subGraphSessionInfoHolder);
            TextMeshProUGUI sessionInfoField = newSubGraphSessionInfo.GetComponent<TextMeshProUGUI>();
            sessionInfoField.text = _label;
            subGraphSessionInfos.Add(sessionInfoField);
        }

    #endregion

    // ====================================================================================================
    #region Markers




    #endregion

    // ====================================================================================================
    #region Update values

        public void UpdateGraphValues (int _subGraphID, float _value) 
        {
            int subGraphIndex = subGraphValues.FindIndex( x => x.subGraphID == _subGraphID);
            subGraphValues[subGraphIndex].SetValue(_value);
        }

    #endregion

    // ====================================================================================================
    #region Interactions

        public void MoveUp ()
        {
            timeLineEntry.MoveUp();
        }

        public void MoveDown ()
        {
            timeLineEntry.MoveDown();
        }

        public void Remove (int _subGraphID, int _sessionDataKey)
        {
            Debug.Log("ValueDisplayer Remove " + entryType);
            
            switch (entryType) 
            {
                case EntryTypes.INSTANTMARKERS:
                    timeLineEntry.RemoveEntry();
                break;
                case EntryTypes.EXCITEOMETER:
                    EoM_Events.Send_RemoveAllFromSession(_sessionDataKey);
                break;
                case EntryTypes.LINEGRAPH:
                    timeLineEntry.rightBlock.GetComponent<LineGraphData>().RemoveSubGraph(_subGraphID);
                break;
            }

        }

        public void ToggleHeight ()
        {
            timeLineEntry.ToggleHeight();
        }

    #endregion
    

    }

}
