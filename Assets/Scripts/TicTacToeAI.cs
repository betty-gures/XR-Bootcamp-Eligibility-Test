using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public enum TicTacToeState { none, cross, circle }

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

    int _aiLevel;

    TicTacToeState[,] boardState;

    [SerializeField]
    private bool _isPlayerTurn;

    [SerializeField]
    private int _gridSize = 3;

    [SerializeField]
    private TicTacToeState playerState = TicTacToeState.cross;
    TicTacToeState aiState = TicTacToeState.circle;

    [SerializeField]
    private GameObject _xPrefab;

    [SerializeField]
    private GameObject _oPrefab;

    public UnityEvent onGameStarted;
    public Action onAITurnStart;
    public Action onAITurnEnd;

    //Call This event with the player number to denote the winner
    public WinnerEvent onPlayerWin;

    ClickTrigger[,] _triggers;
    GridDisplayUI gridDisplayUI;

    private void Awake()
    {
        if (onPlayerWin == null)
        {
            onPlayerWin = new WinnerEvent();
        }
        gridDisplayUI = FindObjectOfType<GridDisplayUI>();
    }

    //Called in UIButton OnClick Events in the Starting Panel
    public void StartAI(int AILevel)
    {
        _aiLevel = AILevel;
        StartGame();
    }

    public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
    {
        _triggers[myCoordX, myCoordY] = clickTrigger;
    }

    private void StartGame()
    {
        _triggers = new ClickTrigger[3, 3];
        boardState = new TicTacToeState[3, 3];
        onGameStarted.Invoke();
    }

    public void PlayerSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, playerState);
    }

    public void AiSelects(int coordX, int coordY)
    {
        SetVisual(coordX, coordY, aiState);
    }

    private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
    {
        Instantiate(
            targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
            _triggers[coordX, coordY].transform.position,
            Quaternion.identity
        );
        UpdateBoardState(coordX, coordY, targetState);
    }

    IEnumerator StartAITurn()
    {
        onAITurnStart?.Invoke();
        yield return new WaitForSeconds(0.5f);
        _isPlayerTurn = false;
        ClickTrigger clickTrigger = CalculateBestMove();
        if (clickTrigger)
        {
            clickTrigger.AIClick();
        }
        else // This only works because the player always goes first
        {
            onPlayerWin.Invoke(-1);
        }
        yield return new WaitForSeconds(0.5f);
        onAITurnEnd?.Invoke();
    }

    private void stopGameAIWon()
    {
        Time.timeScale = 0f;
    }
    private void UpdateBoardState(int coordX, int coordY, TicTacToeState state)
    {
        boardState[coordX, coordY] = state;
        gridDisplayUI.UpdateGrid(coordX, coordY, state);
        if (CheckIfPlayerWon(state))
        {
            if (state == TicTacToeState.cross)
            {
                onPlayerWin.Invoke(0);
                Invoke("stopGameAIWon", 0.2f);

            }
            else if (state == TicTacToeState.circle)
            {
                onPlayerWin.Invoke(1);
                Invoke("stopGameAIWon",0.2f);
            }
            return;
        }
        if (_isPlayerTurn)
        {
            StartCoroutine(StartAITurn());
        }
        else
        {
            _isPlayerTurn = true;
        }
    }

    private ClickTrigger CalculateBestMove()
    {
        ClickTrigger diaganolPlay = CheckForDiaganolWin();
        ClickTrigger horizontalPlay = CheckForHorizontalWin();
        ClickTrigger verticalPlay = CheckForVerticalWin();

        if (diaganolPlay != null)
        {
            return diaganolPlay;
        }

        if (horizontalPlay != null)
        {
            return horizontalPlay;
        }

        if (verticalPlay != null)
        {
            return verticalPlay;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardState[i, j] == TicTacToeState.none)
                {
                    return _triggers[i, j];
                }

            }
        }
        //Couldnt find anywhere to play
        return null;


    }
    private bool CheckIfPlayerWon(TicTacToeState state)
    {
        int[] mySquaresHorizontally = new int[3]; // How many squares are filled with enemy icons per grid.
        int[] mySquaresVertically = new int[3]; // How many squares are filled with enemy icons per grid.
        int mydiaganolSquaresRight = 0;
        int mydiaganolSquaresLeft = 0;
        //Check each grid horizontally and gather information
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardState[i, j] == state)
                {
                    mySquaresHorizontally[i]++;
                }
                if (boardState[j, i] == state)
                {
                    mySquaresVertically[i]++;
                }

                
            }
        }
        if ((boardState[0, 2] == state) && (boardState[1, 1] == state) && (boardState[2, 0] == state))
            mydiaganolSquaresLeft = 3;
        if ((boardState[0, 0] == state) && (boardState[1, 1] == state) && (boardState[2, 2] == state))
            mydiaganolSquaresRight = 3;


        for (int i = 0; i < 3; i++)
        {
            if (mySquaresHorizontally[i] == 3 || mySquaresVertically[i] == 3 || mydiaganolSquaresLeft == 3 || mydiaganolSquaresRight == 3)
            {
                return true;
            }
        }

        return false;
    }
    private ClickTrigger CheckForHorizontalWin()
    {
        int[] enemySquaresNumber = new int[3]; // How many squares are filled with enemy icons per grid.

        //Check each grid horizontally and gather information
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardState[i, j] == playerState)
                {
                    enemySquaresNumber[i]++;
                }
            }
        }

        //Check to make sure no rows have a win condition HORIZONTALLY attached and if they do play a block there
        for (int i = 0; i < 3; i++)
        {
            if (enemySquaresNumber[i] == 2)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[i, j] == TicTacToeState.none)
                    {
                        return _triggers[i, j];
                    }
                }
            }
        }
        //If nothing is found that we need to address return null
        return null;
    }

    private ClickTrigger CheckForVerticalWin()
    {
        int[] enemySquaresNumber = new int[3]; // How many squares are filled with enemy icons per grid.

        //Check each grid horizontally and gather information
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardState[j, i] == playerState)
                {
                    enemySquaresNumber[i]++;
                }
            }
        }

        //Check to make sure no rows have a win condition Vertically attached and if they do play a block there
        for (int i = 0; i < 3; i++)
        {
            if (enemySquaresNumber[i] == 2)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[j, i] == TicTacToeState.none)
                    {
                        return _triggers[j, i];
                    }
                }
            }
        }
        //If nothing is found that we need to address return null
        return null;
    }

    private ClickTrigger CheckForDiaganolWin()
    {
        int enemyDiaganolSquaresRight = 0;
        int enemyDiaganolSquaresLeft = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (boardState[j, i] == playerState || boardState[i, j] == playerState)
                {
                    if(i == 1 && j == 1)
                    {
                        enemyDiaganolSquaresRight++;
                        enemyDiaganolSquaresLeft++;
                    }
                    
                    if ((i == 0 && j == 0) ||
                    (j == 2 && i == 2))
                    {
                        enemyDiaganolSquaresRight++;
                    }
                    
                    if ((i == 0 && j == 2) ||
                        (j == 2 && i == 0))
                    {
                        enemyDiaganolSquaresLeft++;
                    }
                }
            }
        }

        if (enemyDiaganolSquaresLeft == 2)
        {
            if (boardState[0,2] == TicTacToeState.none)
            {
                return _triggers[0, 2];
            }

            if (boardState[1, 1] == TicTacToeState.none)
            {
                return _triggers[1, 1];
            }
            if (boardState[2, 0] == TicTacToeState.none)
            {
                return _triggers[2, 0];
            }
        }

        if (enemyDiaganolSquaresRight == 2)
        {
            if (boardState[0, 0] == TicTacToeState.none)
            {
                return _triggers[0, 0];
            }

            if (boardState[1, 1] == TicTacToeState.none)
            {
                return _triggers[1, 1];
            }
            if (boardState[2, 2] == TicTacToeState.none)
            {
                return _triggers[2, 2];
            }
        }
        return null;
    }
}
