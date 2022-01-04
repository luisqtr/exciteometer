using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExciteOMeter.Vizualisation;
using System;
using UnityEngine.InputSystem;

namespace ExciteOMeter
{
	public class EOMdataloader : MonoBehaviour
	{
		private Keyboard kb;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			// StartCoroutine(StartDelayed ());
			kb = Keyboard.current;
		}

		IEnumerator StartDelayed ()
		{
			yield return new WaitForEndOfFrame();
			EoM_Events.Send_TimelineRedraw();
		}

		// Update is called once per frame
		void Update()
		{
			//// OLD INPUT SYSTEM

			if (kb[Key.U].wasPressedThisFrame)
			{
				// Update values and whole timeline
				Debug.Log("Updating Timeline");

				EoM_Events.Send_TimelineRedraw();

			}

			if (kb[Key.C].wasPressedThisFrame)
			{
				// Update values and whole timeline
				Debug.Log("Custom range");

				EoM_Events.Send_SetTimelineRange(10, 90);

			}


			if (kb[Key.J].wasPressedThisFrame)
			{
				// Update values and whole timeline
				float value = 3f;
				Debug.Log("Value:" + value);
				int intvalue = Convert.ToInt32(value);
				Debug.Log("Int Value:" + intvalue);
				MarkerLabel mlabel = (MarkerLabel)intvalue;
				Debug.Log("label:" + mlabel);
				Color labelC = ExciteOMeter.Vizualisation.VisualStyle.InstantMarkersColorTable[mlabel];
				Debug.Log("Color: " + labelC);

				
			}



			if (kb[Key.S].wasPressedThisFrame)
			{
				// Update values and whole timeline
				Debug.Log("Set range");

				EoM_Events.Send_SetTimelineRange(0, 300);

			}

			if (kb[Key.T].wasPressedThisFrame)
			{
				// Update values and whole timeline
				// float newTimestamp = UnityEngine.Random.Range(0f,200f);
				float newTimestamp = 60f;

				Debug.Log("newTimestamp:" + newTimestamp + " = " + TimeLineHelpers.GetTimeFormat(newTimestamp, true));
				EoM_Events.Send_SetTimeMarker(newTimestamp);

			}

			if (kb[Key.D].wasPressedThisFrame)
			{
				OfflineAnalysisManager.instance.GenerateDataTypeListMenu();
			}

			if (kb[Key.G].wasPressedThisFrame)
			{
				int findindex = ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions.FindLastIndex(x => x.isLoaded == true);
				ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraph(ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions[findindex], DataType.HeartRate);
			}

			if (kb[Key.A].wasPressedThisFrame)
			{
				int findindex = ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions.FindLastIndex(x => x.isLoaded == true);
				ExciteOMeter.Vizualisation.Timeline.instance.AddMarkerGraph(ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions[findindex], DataType.AutomaticMarkers);
			}

		}


		public void SetCustomRange()
		{
			Debug.Log("Custom range");

			EoM_Events.Send_SetTimelineRange(UnityEngine.Random.Range(0, 30), UnityEngine.Random.Range(60, 180));
		}
	}

}