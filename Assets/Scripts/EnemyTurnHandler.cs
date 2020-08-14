using UnityEngine;
using System.Collections;

public class EnemyTurnHandler : MonoBehaviour {

	public int enemyCount;
	public int currentEnemy;
	public enemyTurnState currentProgress;

	public enum enemyTurnState
	{
		playerTurn,
		firstCharacter,
		changingCharacter,
		characterInProgress
	}

	// Use this for initialization
	void Start () {
		currentEnemy = 0;
		currentProgress = enemyTurnState.playerTurn;
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GameStateMachine.currentGameState != GameStateMachine.State.paused)
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn && currentProgress == enemyTurnState.playerTurn)
			{
				enemyCount = EnemyDatabase.enemies.Count;
				currentEnemy = 0;
				currentProgress = enemyTurnState.firstCharacter;
			}

			if(currentProgress == enemyTurnState.characterInProgress)
			{
				if(EnemyDatabase.enemies[currentEnemy].GetComponent<PlayerStats>().currStamina <= 0)
				{
					currentProgress = enemyTurnState.changingCharacter;
					currentEnemy++;
				}
			}
			else if(currentProgress == enemyTurnState.firstCharacter)
			{
				if(EnemyDatabase.enemies[currentEnemy].GetComponent<Enemy_Controller>().active == true)
				{
					EnemyDatabase.enemies[currentEnemy].GetComponent<Enemy_Controller>().selected = true;
					currentProgress = enemyTurnState.characterInProgress;
				}
				else
				{
					currentEnemy++;
					currentProgress = enemyTurnState.changingCharacter;
				}
			}

			if(currentProgress == enemyTurnState.changingCharacter)
			{

				if(currentEnemy == enemyCount)
				{
					GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;
				
					foreach(GameObject enemy in EnemyDatabase.enemies)
					{
						enemy.GetComponent<PlayerStats>().currStamina = enemy.GetComponent<PlayerStats>().maxStamina;
						enemy.GetComponent<PathFinder>().currentState = PathFinder.pathFindingState.notSelected;
						enemy.GetComponent<PathFinder>().visitedNodes.Clear();

					}
					foreach(GameObject player in PlayerDatabase.players)
					{
						player.GetComponent<Player_Controller>().selected = false;
					}
					bool foundActive = false;
					int count = 0;
					while(!foundActive)
					{
						if(PlayerDatabase.players[count].activeSelf == true)
						{
							foundActive = true;
						}
						else
						{
							count++;
						}
					}
						PlayerDatabase.players[count].GetComponent<Player_Controller>().selected = true;
						PlayerDatabase.selectedID = count;
						currentProgress = enemyTurnState.playerTurn;

				}
				else
				{	if(EnemyDatabase.enemies[currentEnemy].GetComponent<Enemy_Controller>().active == true)
					{
						EnemyDatabase.enemies[currentEnemy].GetComponent<Enemy_Controller>().selected = true;
						currentProgress = enemyTurnState.characterInProgress;
					}
					else
					{
						currentEnemy++;
					}
				}

			}

		
		}
	}
}
