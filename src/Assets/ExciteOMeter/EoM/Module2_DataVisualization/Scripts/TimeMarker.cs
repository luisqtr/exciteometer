using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExciteOMeter.Vizualisation
{
    public class TimeMarker : MonoBehaviour {

        private bool isDragging;
        private RectTransform rectTransform;
        public RectTransform myLine; // Thin line that should follow in other scrollview on top of graphs

        void Awake() {
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        void OnEnable() {
            EoM_Events.OnRefreshGraphs      += SendCurrentTimeValue; 
            EoM_Events.OnSetTimeMarker      += SetTimeMarker;
            EoM_Events.OnSetTimeMarkerOnX   += SetTimeMarkerOnX;
        }

        void DisEnable() {
            EoM_Events.OnRefreshGraphs      -= SendCurrentTimeValue; 
            EoM_Events.OnSetTimeMarker      -= SetTimeMarker;
            EoM_Events.OnSetTimeMarkerOnX   -= SetTimeMarkerOnX;
        }

        public void Update() {
            if (isDragging) {

                if (Input.mousePosition.x == this.transform.position.x) 
                    return;

                if (Input.mousePosition.x < 251)
                    return; 



                UpdateTransformPosition(Input.mousePosition.x);
                SendCurrentTimeValue ();
            }
        }

        void SendCurrentTimeValue ()
        {
            EoM_Events.Send_UpdateCurrentTimeValue( Timeline.instance.CalculateTimestampOnPosition (rectTransform.anchoredPosition.x));
        }

        void SetTimeMarker(float _timeStamp)
        {
            UpdateRectPosition( Timeline.instance.CalculatePositionOnTimeline(_timeStamp));
            SendCurrentTimeValue();
        }

        void UpdateRectPosition (float _newX)
        {
            rectTransform.anchoredPosition = new Vector2(_newX, rectTransform.rect.y); 
            myLine.anchoredPosition = new Vector2(_newX, myLine.rect.y); 
        }

        void SetTimeMarkerOnX (float _mouseX)
        {
            UpdateTransformPosition( _mouseX );
            SendCurrentTimeValue();
        }

        void UpdateTransformPosition (float _newX)
        {
            this.transform.position = new Vector2(Input.mousePosition.x, this.transform.position.y);
            myLine.transform.position = new Vector2(Input.mousePosition.x, myLine.transform.position.y);
        }

        public void OnPointerDown ()
        {
            isDragging = true;
        }

        public void OnPointerUp ()
        {
            isDragging = false;
        }
    }
}