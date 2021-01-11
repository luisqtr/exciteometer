using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*

Base class for a data container in the timeline.

*/


// TODO it should now which item in the list it is 'indexInList' in order to up/down/remove

namespace ExciteOMeter.Vizualisation
{
    [System.Serializable]
    public class TimeLineEntry : MonoBehaviour
    {
        public GameObject leftBlock;
        public GameObject rightBlock;

        private RectTransform leftRectTransform;
        private RectTransform rightRectTransform;

        public ValueDisplayer valueDisplayer;

        public EntryTypes entryType;
        public int ID = 0;

        private bool isToggled = false;
        private float startHeight;

        public void InitEntry (GameObject _leftBlock, GameObject _rightBlock, EntryTypes _entryType, int _ID) 
        {
            entryType           = _entryType;

            leftBlock           = _leftBlock;
            leftRectTransform   = leftBlock.GetComponent<RectTransform>();
            
            valueDisplayer      = leftBlock.GetComponent<ValueDisplayer>();
            leftBlock.GetComponent<ValueDisplayer>().Init(this); // to estatblish a link back to here

            rightBlock          = _rightBlock; 
            rightRectTransform  = rightBlock.GetComponent<RectTransform>();
            
            startHeight         = rightRectTransform.rect.height;

            ID                  = _ID;
            Debug.Log("InitEntry index:" + _ID );
        }

        public float GetRowHeight ()
        {
            return leftRectTransform.rect.height;
        }

    // ====================================================================================================
    #region Interactions

        public void ToggleHeight ()
        {
            isToggled = !isToggled;
            leftRectTransform.sizeDelta     = new Vector2(leftRectTransform.rect.width, isToggled ? TimelineSettings.entryToggledHeight : startHeight);
            rightRectTransform.sizeDelta    = new Vector2(rightRectTransform.rect.width, isToggled ? TimelineSettings.entryToggledHeight : startHeight);

            ExciteOMeter.Vizualisation.Timeline.instance.UpdateScrollableContentHeight();
        }

        public void MoveUp ()
        {
            ExciteOMeter.Vizualisation.Timeline.instance.MoveUp(ID);
        }

        public void MoveDown ()
        {
            ExciteOMeter.Vizualisation.Timeline.instance.MoveUp(ID);
        }

        public void RemoveEntry ()
        {
            Debug.Log("TimelineEntry Remove " + ID);
            
            // Flag as available again before deleting
            if (entryType == EntryTypes.INSTANTMARKERS)
            {
                rightBlock.GetComponent<InstantMarkersData>().sessionData.FlagDataType(rightBlock.GetComponent<InstantMarkersData>().dataType, false);
            }

            if (entryType != EntryTypes.EXCITEOMETER) // These 2 blocks stay on screen always.
            {
                Destroy(leftBlock);
                Destroy(rightBlock);

                ExciteOMeter.Vizualisation.Timeline.instance.RemoveRow(ID);
            }
        }
    
    #endregion

    }
}
