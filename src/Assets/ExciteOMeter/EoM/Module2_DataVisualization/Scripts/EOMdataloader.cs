using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExciteOMeter.Vizualisation;
using System;

namespace ExciteOMeter
{
	public class EOMdataloader : MonoBehaviour
	{
		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			// StartCoroutine(StartDelayed ());
		}

		IEnumerator StartDelayed ()
		{
			yield return new WaitForEndOfFrame();
			EoM_Events.Send_TimelineRedraw();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.U))
			{
				// Update values and whole timeline
				Debug.Log("Updating Timeline");

				EoM_Events.Send_TimelineRedraw();

			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				// Update values and whole timeline
				Debug.Log("Custom range");

				EoM_Events.Send_SetTimelineRange(10, 90);

			}


			if (Input.GetKeyDown(KeyCode.J))
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



			if (Input.GetKeyDown(KeyCode.S))
			{
				// Update values and whole timeline
				Debug.Log("Set range");

				EoM_Events.Send_SetTimelineRange(0, 300);

			}

			if (Input.GetKeyDown(KeyCode.T))
			{
				// Update values and whole timeline
				// float newTimestamp = UnityEngine.Random.Range(0f,200f);
				float newTimestamp = 60f;

				Debug.Log("newTimestamp:" + newTimestamp + " = " + TimeLineHelpers.GetTimeFormat(newTimestamp, true));
				EoM_Events.Send_SetTimeMarker(newTimestamp);

			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				OfflineAnalysisManager.instance.GenerateDataTypeListMenu();
			}

			if (Input.GetKeyDown(KeyCode.G))
			{
				int findindex = ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions.FindLastIndex(x => x.isLoaded == true);
				ExciteOMeter.Vizualisation.Timeline.instance.AddLineGraph(ExciteOMeter.Vizualisation.OfflineAnalysisManager.instance.sessions[findindex], DataType.HeartRate);
			}

			if (Input.GetKeyDown(KeyCode.A))
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