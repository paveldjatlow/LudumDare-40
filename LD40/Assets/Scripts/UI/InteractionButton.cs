﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private Image _filler;

    private Transform _point;

    private Coroutine _interaction;

    public void Init(Transform point)
    {
        _point = point;
        _filler.fillAmount = 0;
    }

    private void Update()
    {
        if (_point == null) return;

        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _point.position);
    }

    public void StartInteraction(Action callback)
    {
        if (!_view.activeSelf)
        {
            Debug.LogError("Are you nuts?");
            return;
        }

        if (_interaction != null)
        {
            StopCoroutine(_interaction);
            _interaction = null;
        }

        _interaction = StartCoroutine(Co_Wait(1, callback));
    }

    private IEnumerator Co_Wait(float time, Action callback)
    {
        var t = 0f;

        while (t < time)
        {
            _filler.fillAmount = t;

            t += Time.deltaTime;

            yield return null;
        }

        callback();
    }

    public void Show()
    {
        _view.SetActive(true);
    }

    public void Hide()
    {
        if (_interaction != null)
        {
            StopCoroutine(_interaction);
            _interaction = null;
        }

        _filler.fillAmount = 0;

        _view.SetActive(false);
    }
}