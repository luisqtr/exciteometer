using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// TODO Upgrade the way of deleting the dataTypeSelector window, not destroy but pooling

namespace ExciteOMeter.Vizualisation
{        
    
    // Contains all session information, used or partly/not used as graph, links to folder info, the actual loaded data etc.
    [Serializable]
    public class SessionData 
    {
        public bool isLoaded = false; // SessionVariables are loaded from disk into the class
        public bool onScreen = false; // currently shown
        public Color sessionColor = Color.white;
        public float currentEOMvalue = 0;
        public int sessionDataKey;
        public SessionListItem  sessionListItem;
        public SessionFolder    sessionFolder;
        public SessionVariables sessionVariables = null;

        public Dictionary<DataType, Boolean> dataTypes = new Dictionary<DataType, bool>();

        public SessionData (Color _sessionColor, int _sessionDataKey) 
        {
            sessionColor = _sessionColor;
            sessionDataKey = _sessionDataKey;
        }
        public void FlagDataType (DataType _dataType, bool _onOff)
        {
            dataTypes[_dataType] = _onOff;
        }

        public void ShowInMenu(bool _onOff)
        {
            onScreen = !_onOff;
            sessionListItem.gameObject.SetActive(_onOff);
        }
    }


    public class OfflineAnalysisManager : MonoBehaviour
    {
        [Header("Used when this scene is started in Editor.")]
        public GameObject EomManagerPrefab;

        private string mainLogFolder, sessionFilename;

        public List<SessionData> sessions = new List<SessionData>();
        public List<Color32> sessionColors = new List<Color32>();

        public Transform sessionListHolder;
        public GameObject sessionListItemPrefab;
        public Transform dataTypeSelectorListHolder;
        public GameObject dataTypeSelectorListPrefab;

        public ExciteOmeterMeter excitementMeter;

        Coroutine ExciteOmeterRoutine;

        public static OfflineAnalysisManager instance;

    // ====================================================================================================
    #region Starters

        private void Awake()
        {
            // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            // FOR DEBUG: When this scene is launched from the Editor and not called from another Scene.
            // Create an SettingsManager, in case it does not come from the previous session as DontDestroyOnLoad()
            if(ExciteOMeterManager.instance == null && EomManagerPrefab != null)
            {
                Instantiate(EomManagerPrefab);
                // Debug.LogError("This scene should be called from another scene containing the ExciteOMeterManager script, or add the prefab on this script to be instantiated in runtime.");
            }
        }

        void OnEnable()
        {
            EoM_Events.OnSessionDataLoaded += SessionLoaded;
            EoM_Events.OnRemoveAllFromSession += RemoveAllFromSession;
        }

        void OnDisable()
        {
            EoM_Events.OnSessionDataLoaded -= SessionLoaded;
            EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSession;
        }

        void Start()
        {
            GetSessionDataFromDisk();
            GenerateSessionListMenu();
        }

    #endregion

    // ====================================================================================================
    #region Sessions Disk & Menu

        void GetSessionDataFromDisk ()
        {
            mainLogFolder   = SettingsManager.Values.logSettings.mainLogFolder;
            sessionFilename = SettingsManager.Values.logSettings.sessionJsonFilename;
            // Folders with available sessions
            string[] dirs = Directory.GetDirectories(mainLogFolder);

            // Temp variables for string processing
            int rootLength = mainLogFolder.Length;
            string dirName = ""; // Variable to store the dirname without full path.
            string[] dirNameParts; // Separate between date and sessionId
            string[] sessionFiles;

            // Copy predefined sessioncolor options 
            sessionColors.AddRange(VisualStyle.SessionsColorTable);
            int sessionCounter = -1;

            foreach(string dir in dirs)
            {
                // Extract folder name without whole path
                dirName = dir.Substring(rootLength);

                // Check if the folder has a valid file with a sessionFilename
                sessionFiles = Directory.GetFiles(dir, sessionFilename, SearchOption.TopDirectoryOnly);

                if(sessionFiles.Length == 1) // There should be only one session file per folder
                {
                    // Process the folder
                    dirNameParts = dirName.Split('_');

                    // Every session data gets a unique key
                    sessionCounter++;

                    // Make a session and define a color for it
                    int sessionColorIndex   = UnityEngine.Random.Range(0,sessionColors.Count);
                    SessionData newSession  = new SessionData(sessionColors[sessionColorIndex], sessionCounter);
                    sessionColors.RemoveAt(sessionColorIndex);
                    if (sessionColors.Count == 0) sessionColors.AddRange(VisualStyle.SessionsColorTable);

                    // Add folder info
                    SessionFolder newSessionFolder      = new SessionFolder();
                    newSessionFolder.sessionFilepath    = sessionFiles[0];      // Full path to json file
                    newSessionFolder.folderPath         = dir;                  // Full path to session folder
                    newSessionFolder.datetime           = DateTime.ParseExact(dirNameParts[0], "yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    newSessionFolder.sessionId          = dirName.Substring(dirNameParts[0].Length + 1);
                    newSession.sessionFolder            = newSessionFolder;

                    // Add to list of sessions
                    sessions.Add(newSession);
                }
                else
                {
                    // Move to the next folder and avoid processing this folder
                    Debug.Log("Skipping folder " + dirName + " because session filename " + sessionFilename + " was not found.");
                }
            }
        }

        // Generate visual menu with all session info
        void GenerateSessionListMenu()
        {
            if (sessions.Count == 0)
                return;

            foreach ( SessionData s in sessions) 
            {
                // Generate button for this found session
                GameObject sessionListItemObj = Instantiate(sessionListItemPrefab, sessionListHolder);
                SessionListItem sessionListItem = sessionListItemObj.GetComponent<SessionListItem>();
                // Link
                sessionListItem.Init(s);
                s.sessionListItem = sessionListItem;
            }
        }

        public void HideSessionListMenu ()
        {
            sessionListHolder.parent.transform.parent.transform.parent.gameObject.SetActive(false); // Yeah yeah, ugly, I know
        }

        // Start loading data in the given sessiondata container
        public void OpenSessionFile(SessionData _sessionData)
        {
            HideSessionListMenu ();

            string filePath = _sessionData.sessionFolder.sessionFilepath;

            if (_sessionData.isLoaded)
            {
                SessionLoaded(_sessionData);
            } 
            else
            {
                try
                {
                    _sessionData.sessionVariables = SessionVariablesController.instance.LoadSessionValues(filePath);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Exception while opening session from path " + filePath + " > " + e.Message);
                    return;
                }
                
                EoM_Events.Send_SessionDataLoaded(_sessionData);
            }
        }

        // Handle data 
        void SessionLoaded(SessionData _loadedSessionData)
        {
            Debug.Log("Loaded:" + _loadedSessionData.sessionFolder.sessionFilepath);
            
            if (!_loadedSessionData.isLoaded)
            {
                _loadedSessionData.isLoaded = true;

                // Get the keys of the available time series.
                foreach(DataType dt in _loadedSessionData.sessionVariables.timeseries.Keys)
                {
                    Debug.Log("Found datatype:" + dt);
                    _loadedSessionData.dataTypes.Add(dt, false);
                }
            }

            _loadedSessionData.ShowInMenu(false);

            // Now we have some data laoded, start the meter
            if (ExciteOmeterRoutine == null)
                ExciteOmeterRoutine = StartCoroutine("ExcitementRoutine");

            //! Default we always show the ExciteOmeter graph and a screenshotpanel
            Timeline.instance.AddExciteOgraph(_loadedSessionData);
            Timeline.instance.AddScreenShotPanel(_loadedSessionData);
        }

        // Show back in the menu after being removed from screen
        void RemoveAllFromSession (int _sessionDataKey)
        {
            sessions.Find(x=>x.sessionDataKey == _sessionDataKey).ShowInMenu(true);
        }

        IEnumerator ExcitementRoutine ()
        {
            float excitementLevel = 0;
            
            while (true) 
            {
                if (sessions.FindIndex(x=>x.isLoaded) != -1)
                {
                    float newExcitementLevel = 0;
                    int divideby = 0;

                    for (int s=0;s<sessions.Count;s++)
                    {
                        if (!sessions[s].isLoaded)
                            continue;

                        divideby++;
                        newExcitementLevel += sessions[s].currentEOMvalue;
                    }

                    newExcitementLevel = newExcitementLevel / divideby;

                    if (newExcitementLevel != excitementLevel)
                        excitementMeter.SetValue(newExcitementLevel);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

    #endregion

    // ====================================================================================================
    #region Session DataTypes

        public void GenerateDataTypeListMenu () 
        {
            foreach (SessionData s in sessions)
            {
                if (!s.isLoaded) // is it loaded in mem
                    continue;
                
                if (!s.onScreen) // is it available at the moment
                    continue;

                if (!s.dataTypes.ContainsValue(false)) // are there still datatypes not shows
                    continue;

                GameObject dataTypeSelectorItem = Instantiate(dataTypeSelectorListPrefab, dataTypeSelectorListHolder);
                dataTypeSelectorItem.GetComponent<DataTypeSelector>().Init(s);
            }
        }

        public async void AddSelectedDataTypesToTimeLineAsync (SessionData _sessionData, List<DataType> _dataTypes) 
        {
            Debug.Log("______________Adding:");
            Debug.Log("From: " + _sessionData.sessionFolder.sessionId);
            
            foreach (DataType dt in _dataTypes)
            {
                switch(TimeLineHelpers.GetEntryTypeForDataType(dt))
                {
                    case EntryTypes.INSTANTMARKERS:
                        Debug.Log(dt);
                        Timeline.instance.AddMarkerGraph(_sessionData, dt);
                    break;
                    case EntryTypes.LINEGRAPH:
                        Debug.Log(dt);
                        Timeline.instance.AddLineGraph(_sessionData, dt);
                    break;
                }

                // Build in a break
                await Task.Delay(10);
            }

            HideDataTypeListMenu ();
        }

        public void HideDataTypeListMenu () 
        {
            foreach (Transform child in dataTypeSelectorListHolder.transform) {
                GameObject.Destroy(child.gameObject);
            }

            dataTypeSelectorListHolder.transform.parent.transform.parent.transform.parent.gameObject.SetActive(false);
        }

    #endregion

// ====================================================================================================
    #region Button handlers

        public void BackToMainScene()
        {
            if (ExciteOmeterRoutine != null) 
                StopCoroutine("ExcitementRoutine");

            ExciteOMeterManager.instance.ReturnFromOfflineAnalysis();
        }

        public void ExitApp()
        {
            ExciteOMeterManager.instance.ExitApplication();
        }

    #endregion
    }
}