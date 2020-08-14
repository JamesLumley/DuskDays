using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private GameObject currentSelected;
	private Vector3 cameraOffset;

	void Start()
	{
		GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 0.85f, 1.0f);
		cameraOffset = new Vector3 (0, 10, -21);
	}

	void Update () {
		//Single Player
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				currentSelected = PlayerDatabase.getSelected ();
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 0.85f, 1.0f); //Camera rectangle needs to be smaller to show UI

			}
			else if (GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
			{
				currentSelected = EnemyDatabase.getSelected();
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 0.85f, 1.0f); //Camera rectangle needs to be smaller to show UI

			}
			else if (GameStateMachine.currentGameState == GameStateMachine.State.paused 
			         || GameStateMachine.currentGameState == GameStateMachine.State.statScreen
			         || GameStateMachine.currentGameState == GameStateMachine.State.gameOver)
			{
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f); //UI is not drawn in these states so camera can be full screen
			}
			if(currentSelected != null)
			{
				this.transform.position = Vector3.Lerp(this.transform.position, currentSelected.transform.position + cameraOffset, 0.15f); //For smooth camera movement
			}
		}
		//Multi Player
		else if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				currentSelected = PlayerDatabase.getPlayer1Selected();
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 0.85f, 1.0f);//Camera rectangle needs to be smaller to show UI
			}
			else if (GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				currentSelected = PlayerDatabase.getPlayer2Selected();
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 0.85f, 1.0f);//Camera rectangle needs to be smaller to show UI
			}
			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.paused || GameStateMachine.currentMPGameState == GameStateMachine.MPState.statScreen)
			{
				GetComponent<Camera>().rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f); //UI is not drawn in these states so camera can be full screen
			}
			this.transform.position = currentSelected.transform.position + cameraOffset;

		}

	}
}
