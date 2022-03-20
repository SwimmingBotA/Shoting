using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar_HUD : StatsBar
{
    [SerializeField] protected Text percentText;

    protected virtual void SetPercentText()
    {
        percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
        //percentText.text = (targetFillAmount * 100f).ToString("P0");
    }

    public override void Initialized(float currentValue, float maxValue)
    {
        base.Initialized(currentValue, maxValue);
        SetPercentText();
    }

    protected override IEnumerator BufferedFillCoroutine(Image image)
    {
        SetPercentText();
        return base.BufferedFillCoroutine(image);
    }
}
