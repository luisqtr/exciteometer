using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace ExciteOMeter.Vizualisation
{
	public class ScreenShotDisplayer : MonoBehaviour
	{
        [Header("Refs")]
        public RawImage screenShotTexture;
        public Image bar;
        public ExciteOmeterMeter myMeter;
        public Texture2D defaultTexture;

        private Dictionary<string,Texture2D> loadedScreenShots = new Dictionary<string, Texture2D>();
        private string currentScreenShotFile = "";
        private SessionData sessionData;

        void OnEnable() 
        {
            EoM_Events.OnUpdateCurrentTimeValue += RefreshPanel;    
            EoM_Events.OnRemoveAllFromSession += RemoveAllFromSessionDataKey;
        }

        void Disable() 
        {
            EoM_Events.OnUpdateCurrentTimeValue -= RefreshPanel;    
            EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSessionDataKey;
        }

        public void Init (SessionData _sessionData)
        {
            sessionData = _sessionData;
            bar.color = sessionData.sessionColor;
            screenShotTexture.texture = defaultTexture;
        }

        public void RemoveAllFromSessionDataKey(int _sessionDataKey)
        {
            if (sessionData.sessionDataKey == _sessionDataKey)
            {
                EoM_Events.OnUpdateCurrentTimeValue -= RefreshPanel;    
                EoM_Events.OnRemoveAllFromSession -= RemoveAllFromSessionDataKey;
                Destroy(this.gameObject);
            }
        }

   // ====================================================================================================

        // Look for the closest image previous to given timestamp and either load it of get it from memery
        void RefreshPanel(float _timeStamp)
        {
            UpdateScreenShot(_timeStamp);
            myMeter.SetValue(sessionData.currentEOMvalue);
        }

   // ====================================================================================================
    #region Screen shots

        void UpdateScreenShot (float _timeStamp)
        {
            string imagepath = string.Empty;

            for ( int i=0; i<sessionData.sessionVariables.timeseries[DataType.Screenshots].timestamp.Count; i++)
            {
                if ( sessionData.sessionVariables.timeseries[DataType.Screenshots].timestamp[i] < _timeStamp) 
                    imagepath = sessionData.sessionVariables.timeseries[DataType.Screenshots].text[i];
                else 
                {
                    if (imagepath != currentScreenShotFile && imagepath != string.Empty)
                    {
                        if (loadedScreenShots.ContainsKey(imagepath))
                        {
                            screenShotTexture.texture = loadedScreenShots[imagepath];
                        }
                        else
                        {
                            loadedScreenShots.Add(imagepath, LoadImage (sessionData.sessionFolder.folderPath + "/screenshots/" + imagepath));
                            screenShotTexture.texture = loadedScreenShots.Values.Last();
                        }
                        
                        currentScreenShotFile = imagepath;
                        break;
                    } 
                }
            }
        }

        public static Texture2D LoadImage(string filePath) 
        {
            Texture2D tex = null;
            byte[] fileData;
        
            if (File.Exists(filePath))     {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

    #endregion

        // void FindClosestScreenShot () {
        //         float target = 100.0f;
        //         int index = int.MaxValue;
        //         float nearest = float.MaxValue;
        //         for(int i = 0; i < objectList.Count; i++)
        //         {
        //         float dist = Math.Abs(objectList[i].value - target);
        //         if(dist < nearest)
        //         {
        //             nearest = dist;
        //             index = i;
        //         }
        //         }

        // }
	}

}