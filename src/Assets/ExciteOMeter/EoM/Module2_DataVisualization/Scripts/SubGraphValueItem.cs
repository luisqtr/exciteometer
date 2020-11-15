using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ExciteOMeter.Vizualisation
{
    public class SubGraphValueItem : MonoBehaviour
    {
        public Image colorBar;
        public TextMeshProUGUI valueField;
        public Slider meter;
        public Button closeButton;

        public int subGraphID;
        public int sessionDataKey;

        void OnEnable() {
            EoM_Events.OnRemoveAllFromSession += RemoveAllFromSessionDataKey;
        }

        void OnDisable() {
            EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSessionDataKey;
        }

        public void Init (Color _sessionColor, float _max, float _min, int _subGraphID, int _sessionDataKey)
        {
            colorBar.color  = _sessionColor;
            meter.maxValue  = _max;
            meter.minValue  = _min;

            subGraphID      = _subGraphID;
            sessionDataKey  = _sessionDataKey;
        }

        public void SetValue (float _value)
        {
            meter.value = _value;
            valueField.text = _value.ToString("F3");
        }

        public void RemoveAllFromSessionDataKey (int _sessionDataKey)
        {
            if (sessionDataKey == _sessionDataKey)
            {
                Debug.Log(name + "> datakey:" + sessionDataKey + " > subgraph:" + subGraphID + " removed");
                Destroy(this.gameObject);
            }
        }
    }
}