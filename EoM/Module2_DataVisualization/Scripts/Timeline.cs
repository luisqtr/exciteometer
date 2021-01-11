using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// TODO only instantiate more objects if not in pool already > prepare a certain amount of instances on another thread at start
// TODO Zoom out does not go back to 0 as start after custom range


namespace ExciteOMeter.Vizualisation
{
    public class Timeline : MonoBehaviour
    {
        [Header("Timeline Settings")]
        public float oneSecondUnit              = 20f;
        private float oneSecondUtitMinimum;
        public float oneTimeChunk               = 300; // amounts of seconds to show in the scrollview at all times
        public int dividerHeaderFrequency       = 10;   
        public int dividerBackgroundFrequency   = 30;
        public int LabelFrequency               = 10;

        public float startTime                  = 0; 
        public float endTime                    = 300;
        private float timeLineStartHeight       = 0;
        private int entryIDcounter              = 0;

        [Header("Timebar containers")]
        public GameObject holderBackgroundDividers;
        public GameObject holderHeaderDividers;
        public GameObject holderlabels;

        [Header("Timebar prefabs")]
        public GameObject TimeIndicatorPrefab;
        public GameObject TimelineDivider;

        [Header("Timebar values")]
        public TextMeshProUGUI currentTimeStampField;
        public TextMeshProUGUI zoomLevelField;

        [Header("Content holders")]
        public Canvas exciteOcanvas;
        public GameObject leftContent;
        public RectTransform leftScrollableContent;
        public GameObject rightContent;
        public RectTransform rightScrollableContent;
        public RectTransform scrollableTimeBar;

        [Header("ScreenShots")]
        public Transform screenShotPanelHolder;
        public GameObject screenShotPanelPrefab;

        [Header("Graphs Prefabs")]     
        public GameObject markerGraphPrefabLeft;
        public GameObject markerGraphPrefabRight;
        public GameObject lineGrapshPrefabLeft; 
        public GameObject lineGrapshPrefabRight; 

        [Header("ExciteOgraph")]
        private LineGraphData EOMGraph;
        public GameObject EOMLeft;
        public GameObject EOMRight;

        [SerializeField]
        public List<TimeLineEntry> activeTimeLineEntries = new List<TimeLineEntry>();
                
        // Generated objects
        private List<GameObject> headerDividers     = new List<GameObject>();
        private List<GameObject> backgroundDividers = new List<GameObject>();
        private List<GameObject> labels             = new List<GameObject>();

        public static ExciteOMeter.Vizualisation.Timeline instance;

        private void Awake()
        {
            // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            timeLineStartHeight = rightScrollableContent.GetComponent<RectTransform>().rect.height;
        }

        void OnEnable()
        {
            EoM_Events.OnTimelineRedraw         += UpdateTimeline;
            EoM_Events.OnSetTimelineRange       += SetTimelineRange;
            EoM_Events.OnUpdateCurrentTimeValue += UpdateCurrentTime;
        }

        void OnDisable()
        {
            EoM_Events.OnTimelineRedraw         -= UpdateTimeline;
            EoM_Events.OnSetTimelineRange       -= SetTimelineRange;
            EoM_Events.OnUpdateCurrentTimeValue -= UpdateCurrentTime;
        }

        void Start() 
        {
            // Calculate the OneSecondPixel unit on applications resolution in order to show 15mins of data at start
            oneSecondUnit                       = (exciteOcanvas.GetComponent<RectTransform>().rect.width - 250) / oneTimeChunk; 
            oneSecondUtitMinimum                = oneSecondUnit; // remember this start value

            leftScrollableContent.sizeDelta     = new Vector2(oneTimeChunk * oneSecondUnit, timeLineStartHeight);
            rightScrollableContent.sizeDelta    = new Vector2(oneTimeChunk * oneSecondUnit, timeLineStartHeight);

            EOMGraph = EOMRight.GetComponent<LineGraphData>();
            EOMGraph.InitEntry(EOMLeft, EOMRight, EntryTypes.EXCITEOMETER, 0);

            // Draw basic Timeline stuff
            UpdateTimeline();
        }

    // ====================================================================================================
    #region Timeline Generation

        GameObject GenerateAndPlace (GameObject _prefab, Transform _holder)
        {
            GameObject newObj = Instantiate(_prefab, _holder);
            newObj.transform.SetAsFirstSibling();
            return newObj;
        }

        public void AddMarkerGraph (SessionData _sessionData, DataType _dataType) 
        {
            // Instantiate Info and Data prefeb and call their inits with values they need to show
            GameObject markerLeft = GenerateAndPlace(markerGraphPrefabLeft, leftContent.transform);
            GameObject markerRight = GenerateAndPlace(markerGraphPrefabRight, rightContent.transform);

            InstantMarkersData newEntry = markerRight.GetComponent<InstantMarkersData>();
            entryIDcounter++;
            newEntry.InitEntry(markerLeft, markerRight, EntryTypes.INSTANTMARKERS, entryIDcounter);
            newEntry.InitData(_sessionData, _dataType);

            activeTimeLineEntries.Add(newEntry);
            
            UpdateScrollableContentHeight ();
        }

        public void AddLineGraph (SessionData _sessionData, DataType _dataType) 
        {
            // Instantiate Info and Data prefeb and call their inits with values they need to show
            GameObject graphLeft = GenerateAndPlace(lineGrapshPrefabLeft, leftContent.transform);
            GameObject graphRight = GenerateAndPlace(lineGrapshPrefabRight, rightContent.transform);

            // Generate a new entry in the list
            LineGraphData newEntry =  graphRight.GetComponent<LineGraphData>();
            entryIDcounter++;
            newEntry.InitEntry(graphLeft, graphRight, EntryTypes.LINEGRAPH, entryIDcounter);
            
            newEntry.AddSubLineGraph(_sessionData, _dataType);  
            activeTimeLineEntries.Add(newEntry);

            UpdateScrollableContentHeight ();
        }

        public void AddExciteOgraph (SessionData _sessionData) 
        {
            EOMGraph.AddSubLineGraph(_sessionData, DataType.EOM);  
        }

        public void AddScreenShotPanel (SessionData _sessionData)
        {
            if (!_sessionData.sessionVariables.timeseries.ContainsKey(DataType.Screenshots))
                return;

            GameObject newScreenShotPanel = Instantiate(screenShotPanelPrefab, screenShotPanelHolder);
            ScreenShotDisplayer newScreenShotDisplayer = newScreenShotPanel.GetComponent<ScreenShotDisplayer>();
            newScreenShotDisplayer.Init(_sessionData);

            // Flag this datatype as shown
            _sessionData.dataTypes[DataType.Screenshots] = true;
        }

        // When a ROW gets expanded/folded, update the height of the content holder 
        public void UpdateScrollableContentHeight ()
        {
            float contentHeight = 0;
            foreach(TimeLineEntry tr in activeTimeLineEntries)
                contentHeight += tr.GetRowHeight();

            if (contentHeight < timeLineStartHeight) contentHeight = 540;

            leftScrollableContent.sizeDelta = new Vector2(leftScrollableContent.rect.width, contentHeight);
            rightScrollableContent.sizeDelta = new Vector2(rightScrollableContent.rect.width, contentHeight);
        }

    #endregion

    // ====================================================================================================
    #region Timeline Updates

        void UpdateTimeline()
        {
            ClearObjects ();

            // resize the content to the full width on this zoom level
            float newWidth = oneSecondUnit * oneTimeChunk;

            rightScrollableContent.sizeDelta    = new Vector2 (newWidth, rightScrollableContent.sizeDelta.y);
            scrollableTimeBar.sizeDelta         = new Vector2 (newWidth, scrollableTimeBar.sizeDelta.y);

            // BG
            holderBackgroundDividers.GetComponent<HorizontalLayoutGroup>().spacing = ( dividerBackgroundFrequency * oneSecondUnit ) -1;

            for (int b=0;b<((oneTimeChunk / dividerBackgroundFrequency) + 1);b++)
                backgroundDividers.Add( Instantiate(TimelineDivider, holderBackgroundDividers.transform));

            // Seconds
            holderHeaderDividers.GetComponent<HorizontalLayoutGroup>().spacing = (dividerHeaderFrequency * oneSecondUnit) -1;
            
            for (int s=0;s<((oneTimeChunk / dividerHeaderFrequency) + 1);s++)
                headerDividers.Add (Instantiate(TimelineDivider, holderHeaderDividers.transform));

            // Labels
            for (int n=0;n<((oneTimeChunk / LabelFrequency) + 1);n++)
            {
                GameObject label = Instantiate(TimeIndicatorPrefab, holderlabels.transform);
                label.GetComponent<TextMeshProUGUI>().text = TimeLineHelpers.GetTimeFormat( (startTime) + (n * LabelFrequency), false);
                label.GetComponent<RectTransform>().anchoredPosition = new Vector2( (n*LabelFrequency) * oneSecondUnit,0);
                labels.Add(label);
            }

            EoM_Events.Send_RefreshGraphs();
        }

        void ClearObjects () 
        {
            foreach(GameObject g in headerDividers)
                Destroy(g);

            foreach(GameObject g in backgroundDividers)
                Destroy(g);

            foreach(GameObject g in labels)
                Destroy(g);

            headerDividers.Clear();
            backgroundDividers.Clear();
            labels.Clear();
        }

        void UpdateCurrentTime (float _timeStamp )
        {
            currentTimeStampField.text = TimeLineHelpers.GetTimeFormat(_timeStamp, true);
        }

    #endregion

    // ====================================================================================================

        public void ClearTimeline ()
        {

        }

        public void MoveUp (int _ID)
        {
            int rowIndex = activeTimeLineEntries.FindIndex(x => x.ID == _ID );
            activeTimeLineEntries[rowIndex].leftBlock.transform.SetSiblingIndex(activeTimeLineEntries[rowIndex].leftBlock.transform.GetSiblingIndex() - 1);
            activeTimeLineEntries[rowIndex].rightBlock.transform.SetSiblingIndex(activeTimeLineEntries[rowIndex].rightBlock.transform.GetSiblingIndex() - 1);
        }


        public void MoveDown (int _ID) 
        {
            int rowIndex = activeTimeLineEntries.FindIndex(x => x.ID == _ID );
            activeTimeLineEntries[rowIndex].leftBlock.transform.SetSiblingIndex(activeTimeLineEntries[rowIndex].leftBlock.transform.GetSiblingIndex() + 1);
            activeTimeLineEntries[rowIndex].rightBlock.transform.SetSiblingIndex(activeTimeLineEntries[rowIndex].rightBlock.transform.GetSiblingIndex() + 1);
        }


        public void RemoveRow (int _ID) 
        {
            int rowIndex = activeTimeLineEntries.FindIndex(x => x.ID == _ID );
            Debug.Log("Timeline RemoveRow ID " + _ID + " at index: " + rowIndex);
            activeTimeLineEntries.RemoveAt(rowIndex);
        }


        // When the user clicked on the TIMEBAR 
        public void OnPointerDown ()
        {
            EoM_Events.Send_SetTimeMarkerOnX(Input.mousePosition.x);
        }

        void SetTimelineRange (float _startTime, float _endTime)
        {
            startTime   = _startTime;
            endTime     = _endTime;

            UpdateTimeline();
        }



    // ====================================================================================================
    #region Helpers

        public float CalculatePositionOnTimeline (float _timestamp)
        {
            return (float)( _timestamp * oneSecondUnit);
        }

        public float CalculateTimestampOnPosition (float _markerLocation) 
        {
            return (float) (_markerLocation / oneSecondUnit) - startTime;
        }

    #endregion

    // ====================================================================================================
    #region Zoom

        void ZoomTimeline (bool _zoomIn) 
        {
            switch(_zoomIn)
            {
                case true:
                    oneSecondUnit += TimelineSettings.zoomIncrementValue;
                break;
                case false:
                    float newOneSecondUnit = oneSecondUnit - TimelineSettings.zoomIncrementValue;
                    if ( newOneSecondUnit < oneSecondUtitMinimum ) 
                        oneSecondUnit = oneSecondUtitMinimum;
                    else oneSecondUnit = newOneSecondUnit;
                break;
            }

            zoomLevelField.text = ((oneSecondUnit/oneSecondUtitMinimum)*100f).ToString("F2") + "%";

            UpdateTimeline();
        }

        public void btnZoomIn ()
        {
            ZoomTimeline(true);
            // EoM_Events.Send_TimelineZoomIn ( true );
        }

        public void btnZoomOut ()
        {
            ZoomTimeline(false);
            // EoM_Events.Send_TimelineZoomIn ( false );
        }

    #endregion

    }
}
