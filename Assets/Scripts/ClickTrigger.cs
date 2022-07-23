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
	//we add one more boolean to see if the tic-tac-toe field is already marked. Needs to be public to be accessed by TicTacTocAI script
	public bool isMarked;
	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}

	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEndabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
		//similar to player conditions, we need ai conditions as well
		
	}

	private void SetInputEndabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
		canClick = true;		
		isMarked = false;

	}

	private void OnMouseDown()
	{
		if(canClick){
			_ai.PlayerSelects(_myCoordX, _myCoordY);
			canClick = false;
			isMarked = true;

		}
	}

	public void AIClick()
	{
		_ai.AiSelects(_myCoordX, _myCoordY);
		canClick = false;
		isMarked = true;
	}
}
