using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using TMPro;

namespace ExciteOMeter
{
    public class CustomMarkerScriptUI : MonoBehaviour
    {
        public TextMeshProUGUI timestampText;

        public TextMeshProUGUI messageText;

        [Header("Image color of label")]
        public Image markerImageColor;
        public Color colorDefault = new Color(0,0,1);
        public Color colorQuickMarker = new Color(1,0,0);
        public Color colorCustomMarker = new Color(0,1,0);

        public float timestamp;

        public void Setup(float timestamp, string text, MarkerLabel markerLabel)
        {
            this.timestamp = timestamp;
            timestampText.text = timestamp.ToString("F2");
            messageText.text = text;

            if(markerImageColor != null)
            {
                Color markerColor = colorDefault;
                // Show the color of the marker based on label
                switch(markerLabel)
                {
                    case MarkerLabel.CUSTOM_MARKER:
                        markerColor = colorCustomMarker;
                        break;
                    case MarkerLabel.QUICK_MARKER:
                        markerColor = colorQuickMarker;
                        break;
                }
                markerImageColor.color = markerColor;
            }
            
        }
    }
}