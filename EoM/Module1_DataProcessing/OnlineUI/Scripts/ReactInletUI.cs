using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ExciteOMeter
{
    public class ReactInletUI : MonoBehaviour
    {
        [Header("Incoming data type")]
        public DataType dataType = DataType.NONE;

        [Header("UI setup")]
        public TextMeshProUGUI labelText;
        public Image connectionStatusImage;
        public TextMeshProUGUI valueText;

        public Color connectedColor = new Color(0,1,0);
        public Color disconnectedColor = new Color(1,0,0);

        [Header("Line")]
        public OnlineLineGraph onlineLine;
        public Color lineColor = new Color(1,0,0);
        public Color notRecordingColor = new Color(0.95f,0.95f,0.95f);
        
        private bool currentlyConnected = false;

        private void Start()
        {
            // Setup UI children
            if(labelText == null) labelText = transform.GetComponentInChildren<TextMeshProUGUI>();
            if(connectionStatusImage ==null) connectionStatusImage = transform.GetComponentInChildren<Image>();
            if(labelText != null) labelText.text = dataType.ToString();

            // Setup connection indication
            currentlyConnected = false;
            SetConnectedStatus(currentlyConnected);
        }

        void OnEnable()
        {
            EoM_Events.OnStreamConnected += StreamConnection;
            EoM_Events.OnStreamDisconnected += StreamDisconnection;
            EoM_Events.OnDataReceived += DataReceived;
            EoM_Events.OnLoggingStateChanged += SetRecordingStatus;
        }

        void OnDisable()
        {
            EoM_Events.OnStreamConnected -= StreamConnection;
            EoM_Events.OnStreamDisconnected -= StreamDisconnection;
            EoM_Events.OnDataReceived -= DataReceived;
            EoM_Events.OnLoggingStateChanged -= SetRecordingStatus;
        }
        
        void OnValidate()
        {
            onlineLine.UpdateLineColor(lineColor);
        }

        private void StreamConnection(DataType type)
        {
            // If type of data from new LSL connection is equal to this flag's type
            if (type == dataType)
            {
                currentlyConnected = true;
                SetConnectedStatus(currentlyConnected);
            }
        }

        private void StreamDisconnection(DataType type)
        {
            // If type of data from new LSL connection is equal to this flag's type
            if (type == dataType)
            {
                currentlyConnected = false;
                SetConnectedStatus(currentlyConnected);
            }

            ExciteOMeterOnlineUI.instance.ShowDisconnectedSignal();
        }

        // Sets the color of the image as connected or disconnected
        private void SetConnectedStatus(bool status)
        {
            connectionStatusImage.color = status? connectedColor : disconnectedColor;
        }

        
        private void DataReceived(DataType type, float timestamp, float value)
        {
            if(type == dataType && valueText != null)
            {
                valueText.text = value.ToString("F0");
                if(onlineLine != null)
                    onlineLine.PlotNewSample(value);
            }
        }

        public void SetRecordingStatus(bool status)
        {
            if(status)
            {
                // Log started
                onlineLine.RestartPlot();
                onlineLine.UpdateLineColor(lineColor);
            }
            else
            {
                // Not recording, show the lines in different color
                onlineLine.UpdateLineColor(notRecordingColor);
            }
        }
    }
}
