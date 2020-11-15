using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace ExciteOMeter.Vizualisation
{
    public class ToolTip : MonoBehaviour
    {
        public TextMeshProUGUI messageField;
        public RectTransform bgRect;
        public RectTransform myCanvasRect;

        public RawImage screenShotTexture;

        RectTransform myRect;
        Vector2 paddingSize = new Vector2(30,20);
        bool isActive = false;

        Coroutine HideTimer;

        void Awake() 
        {
            myRect = GetComponent<RectTransform>();
            HideToolTip();
        }

        void OnEnable() 
        {
            EoM_Events.OnShowTooltipInfo    += ShowToolTipInfo;
            EoM_Events.OnHideTooltip        += StartTimer;
        }

        void OnDisable() 
        {
            EoM_Events.OnShowTooltipInfo    -= ShowToolTipInfo;
            EoM_Events.OnHideTooltip        -= StartTimer;
        }

        void ShowToolTipInfo (string _info)
        {
            if (HideTimer != null)
                StopCoroutine("Timer");

            isActive = true;

            messageField.gameObject.SetActive(true);
            messageField.SetText(_info);
            messageField.ForceMeshUpdate();

            Vector2 textSize = messageField.GetRenderedValues(false);
            bgRect.sizeDelta = textSize + paddingSize;

            screenShotTexture.gameObject.SetActive(false);
            bgRect.gameObject.SetActive(true);
        }

        // public void ShowToolTipImage (string pathToImage)
        // {
        //     isActive = true;

        //     screenShotTexture.texture = LoadPNG(pathToImage);

        //     Vector2 imgSize = new Vector2(320,240);
        //     bgRect.sizeDelta = imgSize + paddingSize;
        //     screenShotTexture.GetComponent<RectTransform>().sizeDelta = imgSize;

        //     messageField.gameObject.SetActive(false);
        //     bgRect.gameObject.SetActive(true);
        //     screenShotTexture.gameObject.SetActive(true);
        // }

        // public static Texture2D LoadPNG(string filePath) 
        // {
        //     Texture2D tex = null;
        //     byte[] fileData;
        
        //     if (File.Exists(filePath))     {
        //         fileData = File.ReadAllBytes(filePath);
        //         tex = new Texture2D(2, 2);
        //         tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        //     }
        //     return tex;
        // }

        public void StartTimer ()
        {
            if (HideTimer != null)
                StopCoroutine("Timer");

            HideTimer = StartCoroutine("Timer");
        }

        IEnumerator Timer () 
        {
            yield return new WaitForSeconds(0.1f);
            HideToolTip();
        }

        public void HideToolTip ()
        {
            bgRect.gameObject.SetActive(false);
            messageField.gameObject.SetActive(false);
            screenShotTexture.gameObject.SetActive(false);
            isActive = false;
        }

        void Update() 
        {
            if (!isActive)
                return;

            Vector2 newPos = Input.mousePosition;
            
            if ((newPos.x + bgRect.rect.width) > myCanvasRect.rect.width) 
                newPos.x = myCanvasRect.rect.width - bgRect.rect.width;

            if ((newPos.y + + bgRect.rect.height) > myCanvasRect.rect.height) 
                newPos.y = myCanvasRect.rect.height - bgRect.rect.height;

            myRect.anchoredPosition = newPos;
        }

    }

}
