using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonActivator : MonoBehaviour {

	public List<GameObject> buttons = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(MenuStates.currentMenuState == MenuStates.MenuGameStates.Controls || MenuStates.currentMenuState == MenuStates.MenuGameStates.Credits)
		{
			foreach (GameObject button in buttons)
			{
				button.gameObject.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject button in buttons)
			{
				button.gameObject.SetActive(true);
			}
		}
	}
}
