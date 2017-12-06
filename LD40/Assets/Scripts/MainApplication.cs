﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainApplication : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] public UIController _uiController;

    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Raft _raft;
    [SerializeField] private Stream _stream;

    private readonly List<Cat> _cats = new List<Cat>();

    public int CatsCount => _cats.Count;

    public int DrownedCats;
    public int SavedCats;

    protected PlayerCharacter PlayerCharacter { get; set; }

    protected virtual void Awake()
    {
        Application.targetFrameRate = 50;

        PlayerCharacter = Instantiate(_playerCharacter, _position, Quaternion.identity, _raft.transform);
        PlayerCharacter.Init(_raft.RaftStick, _raft.ViewTransform.localScale.x / 2, _raft.ViewTransform .localScale.z / 2, arg =>
            {
                _uiController.ShowInteractionButton(arg);
            },
            () =>
            {
                _uiController.HideInteractionButton();
            },
            (time, arg) =>
            {
                _uiController.StartInteraction(time, arg);
            },
            arg =>
            {
                _cameraController.SetControlStatus(arg);
                _raft.SetControlStatus(arg);
            });

        _cameraController.Init();
        _uiController.Init(PlayerCharacter.PickUpPoint, _target);
        _stream.GenerateStreamZones();
        _stream.GenerateBanks();
        
        StartCoroutine(Co_Wait(() =>
        {
            PlayerCharacter.GrabThePole();
        }));
    }
    
    private IEnumerator Co_Wait(System.Action callback)
    {
        yield return new WaitForSeconds(.2f);

        callback();
    }

    protected virtual void Update()
    {
        var dist = Vector3.Distance(_target.transform.position, PlayerCharacter.transform.position);

        if (dist < 15)
        {
            _uiController.Won();
        }

        //if (_cats.Count <= 0)
        //{
        //    _uiController.Lost();
        //}
    }

    public void PickCat(Cat cat)
    {
        if (!_cats.Contains(cat))
        {
            _cats.Add(cat);
        }
        else
        {
            Debug.LogError("Cats list is already contains " + cat.Name);
        }

        PlayerCharacter.CatPicked(cat);

        _uiController.SetCatsCount(_cats.Count);
    }

    public void LoseCat(Cat cat)
    {
        if (_cats.Contains(cat))
        {
            _cats.Remove(cat);
        }
        else
        {
            Debug.LogError("Cats list does not contain " + cat.Name);
        }

        PlayerCharacter.CatLost(cat);

        _uiController.SetCatsCount(_cats.Count);
    }
}