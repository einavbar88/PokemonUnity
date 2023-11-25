using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return image.DOFade(1f, duration).WaitForCompletion();
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return image.DOFade(0f, duration).WaitForCompletion();
    }
}

 