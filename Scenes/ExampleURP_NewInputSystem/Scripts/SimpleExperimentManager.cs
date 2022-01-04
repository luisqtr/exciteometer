using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.Video;

using TMPro;

using ExciteOMeter;

/*
* This script controls a simple experiment that validates the 
* Excite-O-Meter. When the experiment is started, it creates
* a log file with the recordings from the user and starts
* the baseline condition during the time in `baselineTimeSeconds`. Then,
* a set of GameObjects defined in the variable `experimentStages` will
* start appearing during `experimentalStageSeconds`. After each
* experimental stage, a resting state will start during the time
* in `restingTimeSeconds`. Every time a stage starts and stops, a manual
* marker is setup in the Excite-O-Meter to notify when the stages change.
* When the last stage is reached, another resting session is started and
* finally the experiment and the recording stop.
*/

public class SimpleExperimentManager : MonoBehaviour
{
    public enum ExperimentState
    {
        NotRunning,
        Training,
        Baseline,
        RestBetweenStages,
        InExperiment,
    }

    [Header("Setup experiment times")]
    public float baselineTimeSeconds = 30.0f;
    public float restingTimeSeconds = 20.0f;
    public float experimentalStageSeconds = 10.0f;

    [Header("Setup stages")]
    public GameObject trainingGameObject;
    public List<GameObject> experimentStages;
    public bool randomizeStages = true;

    [Header("Setup Materials")]
    public Material restingSkybox;
    public Material experimentSkybox;

    [Header("UI setup")]
    public TextMeshProUGUI remainingStagesText;

    public GameObject buttonForceStopExperiment;

    public GameObject popupMessages;
    public TextMeshProUGUI popupMessageText;
    public GameObject popupForceEndOfExperiment;

    // Keeps track of which experimental stages have been finished in this session
    private GameObject currentExperimentalStage;
    private List<int> remainingPositions = new List<int>(); 
    int playingPositionIdx = 0;

    private bool stageLoadedAndPlaying = false; // Useful for loading assets asynchronously

    public TextMeshProUGUI currentStateText;
    private ExperimentState currentState = ExperimentState.NotRunning;

    public ExperimentState CurrentState
    {
        get => currentState; 
        set
        {
            currentState = value;
            if(currentStateText != null)
                currentStateText.text = CurrentState.ToString();
            
            // Show/Hide buttons
            if(buttonForceStopExperiment != null)
            {
                buttonForceStopExperiment.SetActive(experimentRunning);
            }
        }
    }

    private bool stopExperimentWasForced = false;
    public bool experimentRunning
    {
        get { return CurrentState != ExperimentState.NotRunning && CurrentState != ExperimentState.Training; }
    }

    public static SimpleExperimentManager instance;

    /// <summary>
    /// Singleton
    /// </summary>
    private void Awake()
    {
        // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void Start()
    {
        if(popupMessages != null)
            popupMessages.SetActive(false);

        if(popupForceEndOfExperiment != null)
            popupForceEndOfExperiment.SetActive(false);

        CurrentState = ExperimentState.NotRunning;
        ConfigureDefaultEnvironment();
        UpdateExperimentalStagesCount();
    }

    void OnEnable()
    {
        EoM_Events.OnLoggingStateChanged += OnLoggingStateChanged;
    }

    public void OnDisable()
    {
        ConfigureDefaultEnvironment();
        CurrentState = ExperimentState.NotRunning;

        EoM_Events.OnLoggingStateChanged -= OnLoggingStateChanged;
    }

    public void StartStopTrainingStage()
    {
        if(trainingGameObject == null)
        {
            Debug.LogWarning("No training game object was set.");
            return;
        }

        // In case training video was set...
        if (CurrentState == ExperimentState.NotRunning)
        {
            CurrentState = ExperimentState.Training;

            // Setup training
            currentExperimentalStage = trainingGameObject;
            OnLoadStageCompleted();
        }
        else if (CurrentState == ExperimentState.Training)
        {
            // Force early stop of player
            ExperimentStageHasEnded();
            //NotifyEndTrainingStage(); // Redundant?
        }
        else
        {
            ShowPopupMessage("Experiment running, not possible to play training video");
        }
    }

    public void StartExperiment()
    {
        if(CurrentState == ExperimentState.NotRunning)
        {
            Debug.Log("New experiment started...");
            ExciteOMeterManager.TriggerMarker("ExperimentStarted", MarkerLabel.CUSTOM_MARKER);

            for (int i = 0; i < experimentStages.Count; i++)
            {
                remainingPositions.Add(i);
            }

            // Setup flags
            stageLoadedAndPlaying = false;
            stopExperimentWasForced = false;

            // Setup environment for default state
            ConfigureDefaultEnvironment();

            // Wait baseline time before starting videos
            CurrentState = ExperimentState.Baseline;

            // Update Video Count
            UpdateExperimentalStagesCount();

            StartCoroutine(WaitingTime(baselineTimeSeconds, LoadAndPlayExperimentalStage));
        }
        else
        {
            Debug.LogWarning("Experiment already runnig");
            ShowPopupMessage("Experiment already running.");
        }   
    }

    private void UpdateExperimentalStagesCount()
    {
        // Show Text
        if(remainingStagesText != null)
            remainingStagesText.text = (experimentStages.Count-remainingPositions.Count).ToString() + "/" + experimentStages.Count.ToString();
    }
    
    private void EndExperiment()
    {
        Debug.Log("End of the experiment");
        ExciteOMeterManager.TriggerMarker("ExperimentEnded", MarkerLabel.CUSTOM_MARKER);
        ConfigureDefaultEnvironment();
        CurrentState = ExperimentState.NotRunning;
        LoggerController.instance.StopLogSession();

        if(!stopExperimentWasForced && popupMessages != null)
        {
            // Show popup with message of successful end of session
            // this is not shown if experimenter forces end of session
            ShowPopupMessage("The experiment has finished successfully. Data can be accessed in the offline analysis.");    
        }
    }

    public void Update()
    {
        // Check if a experimental stage has finished, when each stage has different length as in videos.\
        // Here, all stages are the same and it gets notified in 
    }

    void ConfigureDefaultEnvironment()
    {
        //// In case the skybox changes between running and 
        /// not running (like in videos360), or putting back
        /// the objects in their initial transforms...
        RenderSettings.skybox = restingSkybox;

        if (trainingGameObject != null) trainingGameObject.SetActive(false);
        foreach (GameObject go in experimentStages)
            if(go!= null) go.SetActive(false);
    }

    public GameObject GetExperimentalStage()
    {
        // Take a random index from the remaining videos
        
        // All videos have been watched
        if(remainingPositions.Count == 0)
        {
            return null;
        }

        // Extract a new random unseen video from the list
        if (randomizeStages)
            playingPositionIdx = remainingPositions[UnityEngine.Random.Range(0, remainingPositions.Count)];
        else
            playingPositionIdx = remainingPositions[0]; // Sequential stages

        remainingPositions.Remove(playingPositionIdx);

        // Show text of video count
        // Update Video Count
        UpdateExperimentalStagesCount();

        string _str = "[";
        for (int i = 0; i < remainingPositions.Count; i++)
        {
            _str += " " + remainingPositions[i].ToString() + ",";
        }

        Debug.Log("Random stage " + playingPositionIdx + ": Remaining stages " + remainingPositions.Count + " = " + _str);

        return experimentStages[playingPositionIdx];;
    }


    public void LoadAndPlayExperimentalStage()
    {
        // Get an experimental stage from the ones that are available.
        currentExperimentalStage = GetExperimentalStage();
        
        if (currentExperimentalStage == null)
        {
            // Notify End Session
            EndExperiment();
            return;
        }

        // If any stage is remaining, execute it.
        // Use asyncload if there are large files such as videoclips from resources folder
        ////Addressables.LoadAssetAsync<VideoClip>(videoClip).Completed += OnLoadAssetCompleted;
        OnLoadStageCompleted();
    }

    void OnLoadStageCompleted()
    {
        // Setup flags
        stageLoadedAndPlaying = true;

        // Actions specific for the stage, playing a video, etc.
        ActivateExperimentalStage();

        //Notify Start of Video // If not in training video
        if (CurrentState != ExperimentState.Training)
            NotifyStartExperimentStage();
        else
            ShowPopupMessage("Training stage has started!");
    }

    void ActivateExperimentalStage()
    {
        // The experiment is forced after a fixed amount of time.
        // This might not be needed, for example when playing videos, because
        // each stage has different length.
        StartCoroutine(WaitingTime(experimentalStageSeconds, DeactivateExperimentalStage));

        // Activating experimental objects
        RenderSettings.skybox = experimentSkybox;
        currentExperimentalStage.SetActive(true);
    }

    void DeactivateExperimentalStage()
    {
        if(stageLoadedAndPlaying)
        {
            // Deactivate experimental objects
            currentExperimentalStage.SetActive(false);

            // Setup for the rest of the logic
            ExperimentStageHasEnded();
        }        
    }


    void ExperimentStageHasEnded()
    {
        stageLoadedAndPlaying = false;

        // NOTIFY END OF VIDEO // If not in training video
        if (CurrentState != ExperimentState.Training)
            NotifyEndExperimentStage();
        else
            NotifyEndTrainingStage();
    }

    private void NotifyStartExperimentStage()
    {
        CurrentState = ExperimentState.InExperiment;
        Debug.Log("Experimental stage has started");
        ExciteOMeterManager.TriggerMarker("ExperimentalStageStarted:" + playingPositionIdx.ToString(), MarkerLabel.CUSTOM_MARKER);
    }

    private void NotifyEndExperimentStage()
    {
        Debug.Log("Experimental stage has ended");
        ExciteOMeterManager.TriggerMarker("ExperimentalStageEnded:" + playingPositionIdx.ToString(), MarkerLabel.CUSTOM_MARKER);

        // Go to resting stage if there are upcoming stages
        if(remainingPositions.Count == 0)
        {
            // Skip resting stage
            ConfigureDefaultEnvironment();
            // Check loading remaining stages and end it!
            LoadAndPlayExperimentalStage();
        }
        else
        {
            // Interim resting stage
            RestingStage();
        }
    }

    void NotifyEndTrainingStage()
    {
        trainingGameObject.SetActive(false);

        ConfigureDefaultEnvironment();

        // Go back to beginning of experiment
        CurrentState = ExperimentState.NotRunning;
        ShowPopupMessage("Training stage was stopped...");
    }

    void RestingStage()
    {
        CurrentState = ExperimentState.RestBetweenStages;
        ConfigureDefaultEnvironment();
        StartCoroutine(WaitingTime(restingTimeSeconds, LoadAndPlayExperimentalStage));
    }

    IEnumerator WaitingTime(float time, Action action)
    {
        yield return new WaitForSecondsRealtime(time);

        // Execute action
        action();
        yield return null;
    }

    ////////////////////////
    // UI Button to control experiment
    ////////////////////////

    public void OnLoggingStateChanged(bool isStartingExperiment)
    {
        if(isStartingExperiment)
        {
            // Start Experiment
            StartExperiment();
        }
        else
        {
            // Stopped Experiment
        }
    }

    public void ForceStopExperiment()
    {
        if(experimentRunning && popupForceEndOfExperiment!= null)
        {
            // Show warning
            popupForceEndOfExperiment.SetActive(true);
        }
    }

    public void ForceStopExperimentConfirmed()
    {
        stopExperimentWasForced = true;

        //MarkersUI.instance.CreateCustomMarker("Forced Stop Experiment");
        Debug.Log("Experiment was forced to stop");

        ExperimentStageHasEnded();

        StopAllCoroutines();

        // End this experiment
        EndExperiment();
    }
    
    // UI button to skip watching the whole video (For testing only)
    public void SkipCurrentExperimentStage()
    {
        if(CurrentState == ExperimentState.InExperiment)
            ExperimentStageHasEnded();
        else
            ShowPopupMessage("Not in an experimental stage right now.\n Current stage = " + CurrentState.ToString());
    }

    void ShowPopupMessage(string text)
    {
        popupMessageText.text = text;
        popupMessages.SetActive(true);
    }

}
