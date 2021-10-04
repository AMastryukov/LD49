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

    public TextMeshProUGUI Counter;

    private int currentValue = 0;
    private Tween currentTween;

    private void Awake()
    {
        Counter = GetComponent<TextMeshProUGUI>();
    }

    public void SetCounter(int value, string prefix = "")
    {
        currentTween.Kill();

        Counter.text = prefix + value.ToString();
        currentValue = value;
    }

    public void UpdateCounter(int value, string prefix = "")
    {
        if (Counter == null) { return; }

        currentTween.Kill();
        currentTween = DOTween.To(() => currentValue, x => currentValue = x, value, movementTime)
            .SetEase(movementEase)
            .OnUpdate(() =>
            {
                Counter.text = prefix + currentValue.ToString();
            });
    }
}