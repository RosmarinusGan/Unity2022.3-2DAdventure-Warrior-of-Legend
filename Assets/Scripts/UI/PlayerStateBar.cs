using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage, healthDelayImage, powerImage;

    public void healthImageChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }

    public void powerImageChange(float percentage)
    {
        powerImage.fillAmount = percentage;
    }

    private void Update()
    {
        healthDelayImage.fillAmount = healthDelayImage.fillAmount > healthImage.fillAmount
            ? healthDelayImage.fillAmount - Time.deltaTime * 0.5f
            : healthDelayImage.fillAmount = healthImage.fillAmount;
    }
}
