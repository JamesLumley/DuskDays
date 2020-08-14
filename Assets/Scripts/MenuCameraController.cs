using UnityEngine;
using System.Collections;

public class MenuCameraController : MonoBehaviour {
	Vector3 mainMenuPos;
	Vector3 controlsPos;
	Vector3 creditsPos;

	// Use this for initialization
	void Start () {
		mainMenuPos = this.transform.position;
		controlsPos = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + 29);
		creditsPos = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + 120);

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown ("EndTurn"))
		{
			MenuStates.currentMenuState = MenuStates.MenuGameStates.Main;
		}

		if(MenuStates.currentMenuState == MenuStates.MenuGameStates.Main)
		{
			this.transform.position = Vector3.Lerp(this.transform.position, mainMenuPos, 0.1f);
		}
		else if(MenuStates.currentMenuState == MenuStates.MenuGameStates.Controls)
		{
			this.transform.position = Vector3.Lerp (this.transform.position, controlsPos, 0.1f);
		}
		else if(MenuStates.currentMenuState == MenuStates.MenuGameStates.Credits)
		{
			this.transform.position = Vector3.Lerp (this.transform.position, creditsPos, 0.025f);
		}
	}
}
