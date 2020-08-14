using UnityEngine;
using System.Collections;

public class TutorialLevel : MonoBehaviour {

	public GameObject door;
	public GameObject unlockedDoor;
	public GameObject chest;
	public GameObject enemy;
	public GameObject enemyMage;
	public GameObject enemyKnight;
	public GameObject boss;

	public enum TutorialStates
	{
		Chest,
		Door,
		Enemy,
		unlockedDoor,
		enemeyTypes,
		Boss,
		End
	}

	public TutorialStates currentTutorialState;
	// Use this for initialization
	void Start () {
		currentTutorialState = TutorialStates.Chest;
	}
	
	// Update is called once per frame
	void Update () {
		stateChange ();
	}

	void OnGUI()
	{
		if(GameStateMachine.currentGameState != GameStateMachine.State.paused 
		   && GameStateMachine.currentGameState != GameStateMachine.State.statScreen 
		   && GameStateMachine.currentGameState != GameStateMachine.State.gameOver)
		{
			switch(currentTutorialState)
			{
			case TutorialStates.Chest:
				drawTutorialText("\nMove a character using WASD or joystick to the chest and open it with E or (X)");
				break;
			case TutorialStates.Door:
				drawTutorialText("\nUse the character now holding the door key to open the gate with E or (X)");
				break;
			case TutorialStates.Enemy:
				drawTutorialText("\nThe enemy now spots you! Kill him before he gets you and heal after if you need to!");
				break;
			case TutorialStates.unlockedDoor:
				drawTutorialText("\nNot all doors need keys, the next one is unlocked. Be careful enemies lurk behind!");
				break;
			case TutorialStates.enemeyTypes:
				drawTutorialText("Mages have high magic defence, and Knights have high physical defence.\nRemember this when choosing your target!");
				break;
			case TutorialStates.Boss:
				drawTutorialText("\nAt the end of this path is a boss, stronger than normal enemies. Be careful!");
				break;
			}
		}
	}

	private void drawTutorialText(string text)
	{
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0225); 
		Rect TextRect = new Rect(Camera.main.pixelWidth * 0.01f, Camera.main.pixelHeight - Camera.main.pixelHeight * 0.3f,3600,1000);
		GUI.Label (TextRect, text);

	}

	private void stateChange()
	{
		if(enemyMage == null || enemyKnight == null)
		{
			currentTutorialState = TutorialStates.Boss;
		}
		else if(unlockedDoor.GetComponent<DoorController>().getOpen())
		{
			currentTutorialState = TutorialStates.enemeyTypes;
		}
		else if(enemy == null)
		{
			currentTutorialState = TutorialStates.unlockedDoor;
		}
		else if(door.GetComponent<DoorController>().getOpen())
		{
			currentTutorialState = TutorialStates.Enemy;
		}
		else if(chest.GetComponent<Chest_Controller>().getOpen())
		{
			currentTutorialState = TutorialStates.Door;
		}
	}
}
