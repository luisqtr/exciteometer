using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace ExciteOMeter.Vizualisation
{
    public class TimeMarker : MonoBehaviour {

        private bool isDragging;
        private RectTransform rectTransform;
        public RectTransform myLine; // Thin line that should follow in other scrollview on top of graphs

        private Mouse mouse;
        private float mouseXpos;

        void Awake() {
            rectTransform = gameObject.GetComponent<RectTransform>();

            mouse = Mouse.current;
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

                mouseXpos = mouse.position.x.ReadValue();

                if (mouseXpos == this.transform.position.x) 
                    return;

                if (mouseXpos < 251)
                    return; 



                UpdateTransformPosition(mouseXpos);
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
            mouseXpos = mouse.position.x.ReadValue();
            this.transform.position = new Vector2(mouseXpos, this.transform.position.y);
            myLine.transform.position = new Vector2(mouseXpos, myLine.transform.position.y);
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