using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {
	public bool controlPressed;


	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void loadSingle()
	{
		Score.collectables = 0;
		Score.turnsTaken = 0;
		GameStateMachine.currentGameMode = GameStateMachine.GameMode.singlePlayer;
		Application.LoadLevel (1);

	}

	public void loadMulti()
	{
		GameStateMachine.currentGameMode = GameStateMachine.GameMode.multiPlayer;
		Application.LoadLevel ("MultiplayerArena");
	}

	public void controls()
	{
		MenuStates.currentMenuState = MenuStates.MenuGameStates.Controls;
	}

	public void credits()
	{
		MenuStates.currentMenuState = MenuStates.MenuGameStates.Credits;
	}

	public void exitGame()
	{
		Application.Quit ();
	}
}
