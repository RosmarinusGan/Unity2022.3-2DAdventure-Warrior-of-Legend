using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;
    public FadeDeleSO fadeDele;

    private void OnEnable()
    {
        fadeDele.OnEventDele += OnFadeActive;
    }

    private void OnDisable()
    {
        fadeDele.OnEventDele -= OnFadeActive;
    }
    
    private void OnFadeActive(Color target, float duration) => fadeImage.DOBlendableColor(target, duration);
}
