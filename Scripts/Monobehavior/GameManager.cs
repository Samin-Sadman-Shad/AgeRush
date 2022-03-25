using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager game;
    public event Action LevelStartEv, LevelPauseEV, LevelEndEV;

    [SerializeField] Transform _player;
    [SerializeField] Transform _levelEnd;

    [SerializeField] Transform _next;
    [SerializeField] Transform _replay;

    public WinState result;

    private void OnEnable()
    {
        game = this;
        _next.gameObject.SetActive(false);
        _replay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isLevelStart)
        {
            StartCoroutine(LevelStartCheck());
        }
        if (isLevelStart)
        {
            if (isStartCORPlaying)
            {
                StopCoroutine(LevelStartCheck());
            }
            LevelEndCheck();
        }

        if (isLevelEnd && !isLevelEndCalled)
        {
            //LevelEndEV();
            //StartCoroutine(LevelEndOperation());
            LevelEndOperation();
        }
    }

    public bool isLevelStart, isLevelPause, isLevelEnd;

    bool isStartCORPlaying, isLevelEndCalled;

    IEnumerator LevelStartCheck()
    {
        if (Input.GetMouseButton(0))
        {
            isStartCORPlaying = true;
            yield return new WaitForSeconds(0.5f);
            LevelStartEv();
            isLevelStart = true;
            isStartCORPlaying = false;
        }
    }

    void LevelEndCheck()
    {
        if (_player.position.z > _levelEnd.position.z)
        {
            isLevelEnd = true;

            if (_player.GetComponentInParent<PlayerController>().currentState > PlayerState.Infant)
            {
                game.result = WinState.win;
            }
            else
            {
                game.result = WinState.lose;
            }

            //LevelEndEV();
        }

        if (_player.GetComponentInParent<PlayerController>().isPlayerDead)
        {
            game.result = WinState.lose;
            isLevelEnd = true;
        }
    }

    void LevelEndOperation()
    {
        //yield return new WaitForSeconds(0.1f);
        LevelEndEV();
        switch (result)
        {
            case WinState.win:
                {
                    _next.gameObject.SetActive(true);
                    break;
                }
            case WinState.lose:
                {
                    _replay.gameObject.SetActive(true);
                    break;
                }
        }

        isLevelEndCalled = true;
    }
}
