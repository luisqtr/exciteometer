using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using TMPro;

using UnityEngine.Events;

namespace ExciteOMeter
{
    public class QuickMarkerEoM : MonoBehaviour
    {
        public string markerMessage = "Example quick marker"; 

        public UnityEvent actionWhenClicked;

        private TextMeshProUGUI quickMarkerText;
        private Button quickMarkerButton;

        void Awake()
        {
            SetupMarkerText();
            SetupMarkerButtonCallback();
        }

        void SetupMarkerText()
        {
            quickMarkerText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if(quickMarkerText != null)
                quickMarkerText.text = markerMessage;
        }

        void ExecuteAction()
        {
            if(actionWhenClicked != null)
                actionWhenClicked.Invoke();
        }

        void SetupMarkerButtonCallback()
        {
            quickMarkerButton = gameObject.GetComponent<Button>();
            if(quickMarkerButton != null)
            {
                quickMarkerButton.onClick.RemoveAllListeners();
                // Send an EoM event when clicked the button
                quickMarkerButton.onClick.AddListener(delegate {ExciteOMeterOnlineUI.instance.CreateManualMarker(markerMessage, MarkerLabel.QUICK_MARKER); });
                quickMarkerButton.onClick.AddListener(delegate {ExecuteAction();});
            }
        }

        // When text is changed from the inspector
        public void OnValidate()
        {
            SetupMarkerText();
            SetupMarkerButtonCallback();
        }
    }
}