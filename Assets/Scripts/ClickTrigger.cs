using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
    TicTacToeAI _ai;

    [SerializeField]
    private int _myCoordX = 0;
    [SerializeField]
    private int _myCoordY = 0;

    [SerializeField]
    private bool canClick;
    public bool IsClicked;

    private void Awake()
    {
        _ai = FindObjectOfType<TicTacToeAI>();
    }

    private void Start()
    {

        _ai.onGameStarted.AddListener(AddReference);
        _ai.onGameStarted.AddListener(() => SetInputEnabled(true));
        _ai.onPlayerWin.AddListener((win) => SetInputEnabled(false));
        _ai.onAITurnStart += () => SetInputEnabled(false);
        _ai.onAITurnEnd += () => SetInputEnabled(true);
    }

    private void SetInputEnabled(bool val)
    {
        canClick = val;
    }

    private void AddReference()
    {
        _ai.RegisterTransform(_myCoordX, _myCoordY, this);
        IsClicked = false;
        canClick = true;
    }

    private void OnMouseDown()
    {
        if ((IsClicked==false)&&(Time.timeScale==1)&&(canClick))
        {
            _ai.PlayerSelects(_myCoordX, _myCoordY);
            IsClicked = true;
            canClick = false;
        }
    }

    public void AIClick()
    {
        _ai.AiSelects(_myCoordX, _myCoordY);
        IsClicked = true;
        canClick = false;
    }
}
