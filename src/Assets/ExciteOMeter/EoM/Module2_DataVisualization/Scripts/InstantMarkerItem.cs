using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// Block that indicates an stored event in the data
namespace ExciteOMeter.Vizualisation
{
	public class InstantMarkerItem : MonoBehaviour
	{
		public Image bar;
        public Image icon;
		public TextMeshProUGUI label;

		public void Init(Color _sessionColor, string _label)
		{
			bar.color = _sessionColor;
            icon.color = _sessionColor;

			label.text = _label;
		}

        public void ShowTooltip() 
        {
            EoM_Events.Send_ShowTooltipInfo(label == null ? "Dummie tooltip random number " + UnityEngine.Random.Range(0f,1000f) : label.text);
        }

        public void HideTooltip() 
        {
            EoM_Events.Send_HideTooltip();
        }
	}
}
