using System.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


namespace ExciteOMeter
{
    public class OfflineAnalysisUI : MonoBehaviour
    {
        /*
        public Transform parentSessions;
        public GameObject sessionFolderPrefab;

        [Header("Setup info when a new session is loaded")]
        public TMPro.TextMeshProUGUI dateTimeString;
        public TMPro.TextMeshProUGUI sessionIdString;

        public SessionVariables.TimeSeries EOMdata = new SessionVariables.TimeSeries("EOM");

        void OnEnable()
        {
            EoM_Events.OnAvailableSessionsRequested += SetAvailableSessions;
            // EoM_Events.OnSessionDataLoaded += SessionLoaded;
        }


        void OnDisable()
        {
            EoM_Events.OnAvailableSessionsRequested -= SetAvailableSessions;
            // EoM_Events.OnSessionDataLoaded -= SessionLoaded;
        }

        void SetAvailableSessions(List<SessionFolder> availableSessions)
        {
            // Load the folder of sessions and instantiate the respective buttons.
            for (int i = 0; i < availableSessions.Count; i++)
            {
                GameObject go = Instantiate(sessionFolderPrefab, parentSessions);
                SessionFolderData script = go.GetComponent<SessionFolderData>();

                // Set the path that it will process when clicked.
                script.SetSessionFolderData(availableSessions[i]);
            }
        }

        void SessionLoaded(SessionFolder folderInfo, SessionVariables sessionInfo)
        {
            // CleanDashboardForNewSession();

            // General info about the session that is extracted from the folder name.
            dateTimeString.text = folderInfo.datetime.ToString("F", DateTimeFormatInfo.InvariantInfo);
            sessionIdString.text = folderInfo.sessionId;

            // When a session is loaded. The `sessionInfo` contains all the info recorded.
            // Refer to the class `SessionVariables` to check the available data.
            if(sessionInfo != null)
            {
                    // HR signal
                    if(sessionInfo.HR != null)
                    {
                        //! HR GRAPH
                        ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraphs(new List<SessionVariables.TimeSeries> { sessionInfo.HR });


                    }

                    //! Test with multiple subgraphs within one linegraph
                    if ( (sessionInfo.HR != null) && (sessionInfo.RRi != null))
                    {
                        //  ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraphs(new List<SessionVariables.TimeSeries> { sessionInfo.HR, sessionInfo.RRi });
                    }

                    // RRi signal
                    if(sessionInfo.RRi != null)
                    {

                        ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraphs(new List<SessionVariables.TimeSeries> { sessionInfo.RRi });

                    }

                    // RMSSD feature
                    if(sessionInfo.RMSSD != null)
                    {

                        ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraphs(new List<SessionVariables.TimeSeries> { sessionInfo.RMSSD });

                    }

                    // SDNN feature
                    if(sessionInfo.SDNN != null)
                    {

                        ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraphs(new List<SessionVariables.TimeSeries> { sessionInfo.SDNN });

                    }                    
                    if (sessionInfo.automaticMarkers != null )
                    {
                        //! There are markers, generate a graph for it
                        ExciteOMeter.Vizualisation.Timeline.instance.AddMarkerGraph(InstantMarkerType.AUTOMATIC.ToString(), sessionInfo.automaticMarkers);
                    }

                    if (sessionInfo.manualMarkers != null )
                    {
                        //! There are markers, generate a graph for it
                        ExciteOMeter.Vizualisation.Timeline.instance.AddMarkerGraph(InstantMarkerType.MANUAL.ToString(), sessionInfo.manualMarkers);
                    }

                EoM_Events.Send_TimelineRedraw();
                // Invoke("MaybeItNeedsADelay", 1f);
            }
            else
            {
                Debug.Log("Session information could not be loaded.");
            }
        }

       
    */
    }

}