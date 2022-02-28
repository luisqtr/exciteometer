using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using ExciteOMeter;

public class CaptureControllersInputXRI : MonoBehaviour
{

    //[Serializable]
    //public class EventXRI
    //{
    //    public InputActionReference inputActionReference;
    //    public UnityEvent actionFunction;
    //}

    [Header("Event setup")]
    public InputActionReference triggerLeftEvent;
    public InputActionReference triggerRightEvent;

    [Header("Trigger Visualizer")]
    public Slider valueVisualizer;
    private Image fillImage;
    public float MAX_VALUE = 2f;


    [Header("Setup thresholds")]
    public Color colorLow = Color.green;
    public float lowerThreshold = 0.4f;
    public Color colorMid = Color.yellow;
    public float upperThreshold = 0.8f;
    public Color colorHigh = Color.red;

    [Header("Setup ExciteOMeter Log")]
    public ExciteOMeter.LogName logToWrite = ExciteOMeter.LogName.SubjectiveMeasureWithTrigger;
    [Tooltip("After how many seconds should the data be written.")]
    public float sendingPeriod = 1.0f;  // 0 records all frames
    private float elapsedTime = 0.0f;   
    private bool logIsWriting = true;   

    // Get the values of the sensors.
    float leftTriggerValue = 0.0f;
    float rightTriggerValue = 0.0f;
    float totalValue = 0.0f;


    private void Awake()
    {
        // Setup XR callback from button
        if (triggerLeftEvent != null)
        {
            triggerLeftEvent.action.started += ctx => leftTriggerValue = ctx.ReadValue<float>();
            triggerLeftEvent.action.performed += ctx => leftTriggerValue = ctx.ReadValue<float>();
            triggerLeftEvent.action.canceled += ctx => leftTriggerValue = 0.0f;
        }
        if (triggerRightEvent != null)
        {
            triggerRightEvent.action.started += ctx => rightTriggerValue = ctx.ReadValue<float>();
            triggerRightEvent.action.performed += ctx => rightTriggerValue = ctx.ReadValue<float>();
            triggerRightEvent.action.canceled += ctx => rightTriggerValue = 0.0f; //TriggerVisualizer(ctx);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(valueVisualizer != null)
        {
            valueVisualizer.maxValue = MAX_VALUE;
            fillImage = valueVisualizer.fillRect.GetComponent<Image>();
        }
        
        if (logToWrite == LogName.UNDEFINED)
            Debug.LogError("Setup the logToWrite variable correctly, based on LoggerController. Currently logging in general log file of ExciteOMeter");
    }


    void FixedUpdate()
    {
        //OVRInput.FixedUpdate();
        //leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        //rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        totalValue = Mathf.Clamp(leftTriggerValue + rightTriggerValue, 0f, MAX_VALUE);

        if(valueVisualizer != null)
        {
            valueVisualizer.value = totalValue;
            if (totalValue < (lowerThreshold*MAX_VALUE))
                fillImage.color = colorLow;
            else if (totalValue < (upperThreshold*MAX_VALUE))
                fillImage.color = colorMid;
            else
                fillImage.color = colorHigh;
        }

        if(ExciteOMeterManager.currentlyRecordingSession)
        {
            // Timer control
            elapsedTime += Time.deltaTime;

            // Send data each "sendingPeriod"
            if (elapsedTime >= sendingPeriod) 
            {
                // Reset timer for next event
                elapsedTime = 0.0f;

                logIsWriting = LoggerController.instance.WriteLine(logToWrite, 
                                                ExciteOMeterManager.GetTimestampString() + "," +
                                                leftTriggerValue.ToString("F3") + "," +
                                                rightTriggerValue.ToString("F3") + "," +
                                                totalValue.ToString("F3")
                                                );

                if (!logIsWriting)
                    Debug.LogWarning("Error writing movement data. Please setup LoggerController with a file with LogID is" + logToWrite.ToString() );
            }
        }
    }

}
