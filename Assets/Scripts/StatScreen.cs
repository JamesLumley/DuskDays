using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatScreen : MonoBehaviour {
	public GameObject score;
	public GameObject levels;
	public GameObject collectables;
	public int collectablesNum;
	// Use this for initialization
	void Start () {
		Score.collectables = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{

		score.GetComponent<Text> ().text = Score.turnsTaken.ToString ();
		if (levels != null && PlayerDatabase.players.Count == 5) 
		{
			levels.GetComponent<Text> ().text = PlayerDatabase.players [0].GetComponent<PlayerStats> ().job 
				+ "     " + PlayerDatabase.players [1].GetComponent<PlayerStats> ().job 
				+ "     " + PlayerDatabase.players [2].GetComponent<PlayerStats> ().job 
				+ "     " + PlayerDatabase.players [3].GetComponent<PlayerStats> ().job 
				+ "     " + PlayerDatabase.players [4].GetComponent<PlayerStats> ().job + "\n"
				+ "        " + PlayerDatabase.players [0].GetComponent<PlayerStats> ().level
				+ "             " + PlayerDatabase.players [1].GetComponent<PlayerStats> ().level
				+ "             " + PlayerDatabase.players [2].GetComponent<PlayerStats> ().level
				+ "             " + PlayerDatabase.players [3].GetComponent<PlayerStats> ().level
				+ "             " + PlayerDatabase.players [4].GetComponent<PlayerStats> ().level;
		}
		collectables.GetComponent<Text> ().text = Score.collectables.ToString () + "/" + collectablesNum;
	
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && GameStateMachine.currentGameState == GameStateMachine.State.statScreen)
		{
			if(Input.GetButtonDown("Accept") || Input.GetKeyDown(KeyCode.Return))
			{
				if(Application.loadedLevel != 3)
				{

					if(Application.loadedLevelName == "Level00")
					{
						Score.currentLevel = 1;
					}
					else
					{
						Score.currentLevel = Application.loadedLevel;
					}
					Score.currentLevel++;
					foreach(GameObject player in PlayerDatabase.players)
					{
						player.SetActive(true);
					}
					PlayerDatabase.deleteLists();
					EnemyDatabase.deleteList();
					NodeDatabase.allNodes.Clear();
				
					Application.LoadLevel (Score.currentLevel);
					
				}
				else if(Application.loadedLevel == 3)
				{
					Time.timeScale = 1;
					GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;
					GameStateMachine.currentMPGameState = GameStateMachine.MPState.player1Turn;
					for(int i = 0; i < PlayerDatabase.players.Count; i++)
					{
						Destroy(PlayerDatabase.players[i]);
					}
					PlayerDatabase.deleteLists ();
					EnemyDatabase.deleteList ();
					NodeDatabase.allNodes.Clear ();
					GameStateMachine.currentGameMode = GameStateMachine.GameMode.menu;
					Application.LoadLevel (0);
				}
			}
		}
	}

}
