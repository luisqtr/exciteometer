using System.Collections.Generic;
using UnityEngine;

namespace ExciteOMeter
{
    /// <summary>
    /// Wrapper to centralize all the Events from
    /// Excite-O-Meter, without requiring to call the
    /// specific class. 
    /// The class needing to
    /// send an Event, calls EoM_Events.Sendxxxxx(args)
    /// and this function emits the event with the delegate
    /// definition.
    /// Classes needing to subscribe to events, are subscribed
    /// to the EoM_Events.Onxxxxx();
    /// </summary>
    public static class EoM_Events
    {
        // ===================================
        //      EVENTS
        // Triggered when a LSL stream connects or disconnects
        public delegate void ChangedStatusLSL(ExciteOMeter.DataType type);
        public static event ChangedStatusLSL OnStreamConnected;
        public static event ChangedStatusLSL OnStreamDisconnected;
        
        public static void Send_OnStreamConnected(ExciteOMeter.DataType dataType)
        {
            if(OnStreamConnected != null)
                OnStreamConnected(dataType);
        }

        public static void Send_OnStreamDisconnected(ExciteOMeter.DataType dataType)
        {
            if(OnStreamDisconnected != null)
                OnStreamDisconnected(dataType);
        }

        // ===================================
        // Triggered any data is collected, either from sensor or from calculated features
        public delegate void DataFloat(ExciteOMeter.DataType type, float timestamp, float value);
        public static event DataFloat OnDataReceived;
        public static void Send_OnDataReceived(ExciteOMeter.DataType dataType, float timestamp, float value)
        {
            if(OnDataReceived != null)
                OnDataReceived(dataType, timestamp, value);
        }
        

        // ===================================
        // Triggered when a user wants to set a marker at a specific point.
        //  or when ExciteOMeter detects abnormal situations (e.g. HR > 3 times std. deviation.)
        public delegate void DataString(ExciteOMeter.DataType type, float timestamp, string message, MarkerLabel label);
        public static event DataString OnStringReceived;

        public static void Send_OnStringReceived(ExciteOMeter.DataType type, float timestamp, string message, MarkerLabel label = MarkerLabel.CUSTOM_MARKER)
        {
            if(OnStringReceived != null)
                OnStringReceived(type, timestamp, message, label);
        }

        // ===================================
        // Triggered when a Log changes
        public delegate void ChangedLoggingState(bool newState);
        public static event ChangedLoggingState OnLoggingStateChanged;

        public static void Send_OnLoggingStateChanged(bool newState)
        {
            if(OnLoggingStateChanged != null)
                OnLoggingStateChanged(newState);
        }

        // ===================================
        // Triggered when log finished, but before log files are closed.
        // Used to do post processing of signals and still write results on log files.
        // E.g. Match length of input-output during feature calculation,
        //      or calculate EoM features.
        public delegate void PostProcessingStageStarted();
        public static event PostProcessingStageStarted OnPostProcessingStarted;

        public static void Send_OnPostProcessingStarted()
        {
            if(OnPostProcessingStarted != null)
            {
                // This just changes when ExciteOMeter level has been calculated
                // This flag blocks all incoming data from LSL that wants to write on the CSV files
                ExciteOMeterManager.inPostProcessingStage = true;
                OnPostProcessingStarted();
            }
        }

        // ===================================
        // OFFLINE ANALYSIS

        // ===================================
        // Triggered when a set of folders with EOM sessions are loaded
        public delegate void SessionFolders(List<SessionFolder> availableSessions);
        public static event SessionFolders OnAvailableSessionsRequested;

        public static void Send_OnAvailableSessionsRequested(List<SessionFolder> availableSessions)
        {
            if(OnAvailableSessionsRequested != null)
                OnAvailableSessionsRequested(availableSessions);
        }


        // ===================================
        // Triggered when a specific session is loaded
        // public delegate void SessionData(SessionFolder folderData, SessionVariables sessionData);
        // public static event SessionData OnSessionDataLoaded;

        // public static void Send_OnSessionLoaded(SessionFolder folderInfo, SessionVariables sessionInfo)
        // {
        //     if(OnSessionDataLoaded != null)
        //         OnSessionDataLoaded(folderInfo, sessionInfo);
        // }




        // ===================================
        // OFFLINE ANALYSIS GUI 

        // ===================================
        // Update requested to refresh the timeline (and therefor all data in it)
        public delegate void RefreshGraphs();
        public static event RefreshGraphs OnRefreshGraphs;

        public static void Send_RefreshGraphs()
        {
            if(OnRefreshGraphs != null)
                OnRefreshGraphs();
        }

        // Zoom timeline request
        public delegate void TimelineZoom(bool zoomIn);
        public static event TimelineZoom OnTimelineZoom;

        public static void Send_TimelineZoomIn(bool zoomIn)
        {
            if(OnTimelineZoom != null)
                OnTimelineZoom(zoomIn);
        }

        // Set custom start and endtime 
        public delegate void SetTimelineRange(float startTime, float endTime);
        public static event SetTimelineRange OnSetTimelineRange;

        public static void Send_SetTimelineRange(float startTime, float endTime)
        {
            if(OnSetTimelineRange != null)
                OnSetTimelineRange(startTime, endTime);
        }

        // Force updating / redrawing the timeline
        public delegate void TimelineRedraw();
        public static event TimelineRedraw OnTimelineRedraw;

        public static void Send_TimelineRedraw()
        {
            if(OnTimelineRedraw != null)
                OnTimelineRedraw();
        }

        // Marker position sends out his time location
        public delegate void UpdateCurrentTimeValue(float value);
        public static event UpdateCurrentTimeValue OnUpdateCurrentTimeValue;

        public static void Send_UpdateCurrentTimeValue(float value)
        {
            if(OnUpdateCurrentTimeValue != null)
                OnUpdateCurrentTimeValue(value);
        }

        // Marker position set on TIME
        public delegate void SetTimeMarker(float value);
        public static event SetTimeMarker OnSetTimeMarker;

        public static void Send_SetTimeMarker(float value)
        {
            if(OnSetTimeMarker != null)
                OnSetTimeMarker(value);
        }

        // Marker position set on transform.position value X
        public delegate void SetTimeMarkerOnX(float value);
        public static event SetTimeMarkerOnX OnSetTimeMarkerOnX;

        public static void Send_SetTimeMarkerOnX (float value)
        {
            if(OnSetTimeMarkerOnX != null)
                OnSetTimeMarkerOnX(value);
        }

        // Triggered when a specific session is loaded
        public delegate void SessionDataLoaded(ExciteOMeter.Vizualisation.SessionData loadedSession);
        public static event SessionDataLoaded OnSessionDataLoaded;

        public static void Send_SessionDataLoaded(ExciteOMeter.Vizualisation.SessionData loadedSession)
        {
            if(OnSessionDataLoaded != null)
                OnSessionDataLoaded(loadedSession);
        }

        // Request to show tooltip
        public delegate void ShowTooltipInfo(string info);
        public static event ShowTooltipInfo OnShowTooltipInfo;

        public static void Send_ShowTooltipInfo (string info)
        {
            if(OnShowTooltipInfo != null)
                OnShowTooltipInfo(info);
        }

        // Request to show tooltip
        public delegate void HideTooltip();
        public static event HideTooltip OnHideTooltip;

        public static void Send_HideTooltip ()
        {
            if(OnHideTooltip != null)
                OnHideTooltip();
        }

        // Request to show tooltip
        public delegate void RemoveAllFromSession(int sessionDataKey);
        public static event RemoveAllFromSession OnRemoveAllFromSession;

        public static void Send_RemoveAllFromSession (int sessionDataKey)
        {
            if(OnRemoveAllFromSession != null)
                OnRemoveAllFromSession(sessionDataKey);
        }


    }
}