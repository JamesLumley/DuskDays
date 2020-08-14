using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuControllerJoystick : MonoBehaviour {
	private int index = 0;
	public List<GameObject> buttons = new List<GameObject>();
	private float menuChangeTimer = 0;
	public GameStateMachine.State oldState;
	public GameStateMachine.MPState oldMPState;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Accept") || Input.GetKeyDown(KeyCode.Return))
		{
			buttonPress();
		}

		menuChangeTimer -= Time.deltaTime;

		if(menuChangeTimer <= 0)
		{
			if(Input.GetAxis("Vertical") > 0)
			{
				index--;
				menuChangeTimer = 0.3f;
			}
			else if(Input.GetAxis("Vertical") < 0)
			{
				index++;
				menuChangeTimer = 0.3f;

			}
		}

		if(index > buttons.Count - 1)
		{
			index = 0;
		}
		if(index < 0)
		{
			index = buttons.Count - 1;
		}

		this.transform.position = new Vector3(buttons[index].transform.position.x, buttons[index].transform.position.y, this.transform.position.z);
	}

	void buttonPress()
	{
		PointerEventData pointer = new PointerEventData(EventSystem.current);

		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.menu)
		{
			switch(index)
			{
				case 0:
					ExecuteEvents.Execute(buttons[0].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 1:
					ExecuteEvents.Execute(buttons[1].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 2:
					ExecuteEvents.Execute(buttons[2].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 3:
					ExecuteEvents.Execute(buttons[3].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 4:
					ExecuteEvents.Execute(buttons[4].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{

			switch(index)
			{
				case 0:
				ExecuteEvents.Execute(buttons[0].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 1:
				ExecuteEvents.Execute(buttons[1].gameObject, pointer, ExecuteEvents.submitHandler);
					break;
				case 2:
					GameStateMachine.currentGameState = oldState;
					break;
				case 3:
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
					break;
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			switch(index)
			{
			case 0:
				GameStateMachine.currentMPGameState = oldMPState;
				break;
			case 1:
				Time.timeScale = 1;
				GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;
				GameStateMachine.currentMPGameState = GameStateMachine.MPState.player1Turn;
				PlayerDatabase.deleteLists ();
				EnemyDatabase.deleteList ();
				NodeDatabase.allNodes.Clear ();
				GameStateMachine.currentGameMode = GameStateMachine.GameMode.menu;
				Application.LoadLevel (0);
				break;
			}
		}
	}
}
