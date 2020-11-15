using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ExciteOMeter
{
    public class SessionFolderData : MonoBehaviour
    {
        public TMP_Text textButton;

        // Contains the full path of the session folder
        private SessionFolder sessionFolderData;
        
        // Called when a new prefab is created with the available sessions.
        public void SetSessionFolderData(SessionFolder sessionFolder)
        {
            sessionFolderData = sessionFolder;
            SetupButtonUI();
        }

        private void SetupButtonUI()
        {
            // Set button text
            string btnText = String.Concat(sessionFolderData.datetime.ToString("g", DateTimeFormatInfo.InvariantInfo), "\n  ", sessionFolderData.sessionId);
            textButton.text = btnText;
        }

        public void OpenSessionData()
        {
            // Open the folder data from the main offline analysis script
            // ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.OpenSessionFile(sessionFolderData);
        }
    }

}