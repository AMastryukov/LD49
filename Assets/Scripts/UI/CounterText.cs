using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CounterText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float movementTime = 1f;
    [SerializeField] private Ease movementEase = Ease.OutExpo;

    private TextMeshProUGUI counter;
    private int currentValue = 0;
    private Tween currentTween;

    private void Awake()
    {
        counter = GetComponent<TextMeshProUGUI>();
    }

    public void SetCounter(int value)
    {
        currentTween.Kill();

        counter.text = value.ToString();
        currentValue = value;
    }

    public void UpdateCounter(int value)
    {
        if (counter == null) { return; }

        currentTween.Kill();
        currentTween = DOTween.To(() => currentValue, x => currentValue = x, value, movementTime)
            .SetEase(movementEase)
            .OnUpdate(() =>
            {
                counter.text = currentValue.ToString();
            });
    }
}