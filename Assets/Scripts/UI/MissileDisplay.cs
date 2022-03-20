using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileDisplay : MonoBehaviour
{
    static Text amountText;
    static Image cooldownImage;

    private void Awake()
    {
        amountText =  transform.Find("Missle Number").GetComponent<Text>();
        cooldownImage = transform.Find("Cool Down Image").GetComponent<Image>();
    }

    public static void UpadateText(int amount)
    {
        amountText.text = amount.ToString();
    }

    public static void UpdateCoolDownImage(float fillAmount)
    {
        cooldownImage.fillAmount = fillAmount;
    }
}
