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
    private PillarDisplay _pillarDisplay;
    private GameManager gameManager;

    private void Awake()
    {
        Counter = GetComponent<TextMeshProUGUI>();
        _pillarDisplay = FindObjectOfType<PillarDisplay>();
        gameManager = FindObjectOfType<GameManager>();
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
    public void UpdateCounter(int value, bool affectColor, string prefix = "")
    {
        if(affectColor)
        {
            if (Counter == null) { return; }

            currentTween.Kill();
            currentTween = DOTween.To(() => currentValue, x => currentValue = x, value, movementTime)
                .SetEase(movementEase)
                .OnUpdate(() =>
                {
                    Counter.text = prefix + currentValue.ToString();
                    if (int.Parse(Counter.text) <= ((int)(gameManager.MaximumPillar * gameManager.criticalThreshold)) || int.Parse(Counter.text) >= ((int)(gameManager.MaximumPillar * (1 - gameManager.criticalThreshold))))
                    {
                        Counter.color = _pillarDisplay.GetColorByName("Negative");

                    }
                });
        }
        else
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
}