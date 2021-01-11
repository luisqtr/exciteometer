using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExciteOmeterMeter : MonoBehaviour
{
    public enum MeterTypes { SPEEDOMETER, BAR };
    public MeterTypes meterType = MeterTypes.BAR;

    [Header("Refs")]
    public RectTransform pointer;
    
    // Speed O Meter
    float v = 0f;
    Quaternion newRot;
    Coroutine rotater = null;

    // Bar
    public Slider barMask;

    public void SetValue (float _value) 
    {
        switch (meterType)
        {
            case MeterTypes.SPEEDOMETER:
                v = (float)ExciteOMeter.Helpers.RangeMapper(_value, 0, 1, 270, 90, 2);
                newRot = Quaternion.Euler(0f, 0f, v);
                rotater = StartCoroutine(MoveWithinSeconds(pointer,pointer.rotation,newRot,0.3f));
            break;

            case MeterTypes.BAR:
                barMask.value = 1f - _value;
            break;
        }
    }

    // Moves a given object from A to B within given duration
    IEnumerator MoveWithinSeconds(RectTransform obj, Quaternion from, Quaternion to, float duration)
    {
        var timePassed = 0f;
        while(timePassed < duration)
        {
            var factor = timePassed / duration;
            factor = Mathf.SmoothStep(0,1,factor);

            // linear interpolate the position
            obj.rotation = Quaternion.Lerp(from, to, factor);
            timePassed += Mathf.Min(Time.deltaTime, duration - timePassed);
            yield return null;
        }

        // Apply the target position in the end
        obj.rotation = to;
    }


}
