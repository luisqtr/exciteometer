using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace ExciteOMeter.Vizualisation
{
    public class SessionListItem : MonoBehaviour
    {
        public Image iconButton;
        public TextMeshProUGUI DateTimeField;
        public TextMeshProUGUI SessionIDField;

        private SessionData sessionData;

        public void Init (SessionData _sessionData) 
        {
            sessionData = _sessionData;
            SetupButtonUI();
        }

        private void SetupButtonUI()
        {
            // Set button text
            DateTimeField.text   = String.Concat(sessionData.sessionFolder.datetime.ToString("g", DateTimeFormatInfo.InvariantInfo));
            SessionIDField.text  = sessionData.sessionFolder.sessionId;

            // Icon color = session color
            iconButton.color = sessionData.sessionColor;
        }

        public void LoadSessionData()
        {
            Debug.Log("LoadSessionData");
            // Open the folder data from the main offline analysis script
            OfflineAnalysisManager.instance.OpenSessionFile(sessionData);
        }

        // public void SetLoaded (bool _onOff)
        // {
        //     // Disble button and show that we are alrady in mem
        //     gameObject.SetActive(_onOff);
        // }d
    }
}