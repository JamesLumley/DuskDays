using UnityEngine;
using System.Collections;

public class Databasecontroller : MonoBehaviour {
	private int currentLevel;
	private bool activePlayers = true;
	bool loaded = false;
	// Use this for initialization
	void Start () {
		//GameStateMachine.currentGameMode = GameStateMachine.GameMode.multiPlayer;
		//GameStateMachine.currentGameMode = GameStateMachine.GameMode.singlePlayer;
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(PlayerDatabase.players.Count == 0)
			{
				PlayerDatabase.initSingle ();
			}
			GameStateMachine.initSingle ();
			EnemyDatabase.init ();
			NodeDatabase.init ();
			loaded = true;
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			GameStateMachine.initMulti();
			PlayerDatabase.initMulti ();
			loaded = true;

		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Checks for level win condition
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && loaded)
		{
			if(EnemyDatabase.enemies.Count == 0)
			{
				GameStateMachine.currentGameState = GameStateMachine.State.statScreen;
			}
			//Checks for game over condition
			foreach(GameObject player in PlayerDatabase.players)
			{
				if(player.activeSelf == true)
				{
					activePlayers = true;
					break;
				}
				else
				{
					activePlayers = false;
				}
			}
			if(activePlayers == false)
			{
				GameStateMachine.currentGameState = GameStateMachine.State.gameOver;
			}
		}

		//Checks for win condition on multiplayer
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer && loaded)
		{
			if(PlayerDatabase.bluePlayers.Count == 0 || PlayerDatabase.redPlayers.Count == 0)
			{
				GameStateMachine.currentMPGameState = GameStateMachine.MPState.statScreen;
			}
		}
	}


	void OnGUI ()
	{
		GUI.depth = 0;

		GameStateMachine.drawCurrentState ();
	}
}
