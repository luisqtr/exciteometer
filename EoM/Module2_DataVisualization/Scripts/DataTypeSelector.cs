using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

// TODO Object pooling for all panels and toggles
// TODO After click only this one should disappear, or what is logic anyways?

namespace ExciteOMeter.Vizualisation
{
    public class DataTypeSelector : MonoBehaviour
    {
        [Header("Refs")]
        public Image bar;
        public Image iconButton;
        public TextMeshProUGUI DateTimeField;
        public TextMeshProUGUI SessionIDField;

        [Header("Togglers")]
        public GameObject dataTypeTogglePrefab;
        public Transform dataTypeToggleHolder;

        private SessionData sessionData;
        private Dictionary<Toggle, DataType> generatedToggles = new Dictionary<Toggle, DataType>(); 
        private List<DataType> selectedDataTypes = new List<DataType>();

        public void Init (SessionData _sessionData) 
        {
            sessionData = _sessionData;
            SetupPanel();
        }

        private void SetupPanel()
        {
            // Set button text
            DateTimeField.text   = String.Concat(sessionData.sessionFolder.datetime.ToString("g", DateTimeFormatInfo.InvariantInfo));
            SessionIDField.text  = sessionData.sessionFolder.sessionId;

            // Icon color = session color
            iconButton.color    = sessionData.sessionColor;
            // bar.color           = sessionData.sessionColor;

            // Generate toggles for each available datatype
            foreach (KeyValuePair<DataType,bool> sessionDataType in sessionData.dataTypes)
            {
                if (!sessionDataType.Value) // If false, then show in the option list
                {
                    GameObject newToggleObj = Instantiate(dataTypeTogglePrefab, dataTypeToggleHolder);
                    newToggleObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = sessionDataType.Key.ToString();

                    generatedToggles.Add(newToggleObj.GetComponent<Toggle>(), sessionDataType.Key);
                }
            }
        }

        public void AddSelectedDataTypes ()
        {
            foreach (KeyValuePair<Toggle,DataType> td in generatedToggles)
            {
                if (td.Key.isOn)
                {
                    Debug.Log(td.Value);
                    selectedDataTypes.Add(td.Value);
                }
            } 

            OfflineAnalysisManager.instance.AddSelectedDataTypesToTimeLineAsync (sessionData, selectedDataTypes);
        }
    }
}