using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MultiplayerWinScreen : MonoBehaviour {
	public GameObject winner;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Checks who won and writes text accordingly
		if(PlayerDatabase.bluePlayers.Count == 0)
		{
			winner.GetComponent<Text>().text = "Red Player Wins";
		}
		else if(PlayerDatabase.redPlayers.Count == 0)
		{
			winner.GetComponent<Text>().text = "Blue Player Wins";
		}

		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer && GameStateMachine.currentMPGameState == GameStateMachine.MPState.statScreen)
		{
			if(Input.GetButtonDown("Accept"))
			{
				GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;
				GameStateMachine.currentMPGameState = GameStateMachine.MPState.player1Turn;
				PlayerDatabase.deleteLists ();
				EnemyDatabase.deleteList ();
				GameStateMachine.currentGameMode = GameStateMachine.GameMode.menu;
				Application.LoadLevel (0);
			}
		}
	}
}
