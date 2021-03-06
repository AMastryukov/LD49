using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private List<Transform> menuItems;
    [SerializeField] private List<Transform> advisors;

    private List<Vector3> _originalMenuPositions;
    private List<Vector3> _originalAdvisorPositions;
    private float _distance = 1500f;
    private float _delay = 0.1f;

    private void Awake()
    {
        _originalMenuPositions = new List<Vector3>();
        _originalAdvisorPositions = new List<Vector3>();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < advisors.Count; i++)
        {
            advisors[i].transform.DOKill();
        }

        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].transform.DOKill();
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(SlideCoroutine());
    }

    private IEnumerator SlideCoroutine()
    {
        #region Reset Positions
        foreach (var transform in menuItems)
        {
            _originalMenuPositions.Add(transform.position);
            transform.position += Vector3.left * _distance;
        }

        foreach (var transform in advisors)
        {
            _originalAdvisorPositions.Add(transform.position);
            transform.position += Vector3.right * _distance;
        }
        #endregion

        for (int i = 0; i < advisors.Count; i++)
        {
            advisors[i].transform.DOMove(_originalAdvisorPositions[i], 2f).SetEase(Ease.OutExpo);
            yield return new WaitForSeconds(_delay);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].transform.DOMove(_originalMenuPositions[i], 2f).SetEase(Ease.OutExpo);
            yield return new WaitForSeconds(_delay);
        }
    }
}
