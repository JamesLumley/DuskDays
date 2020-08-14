using UnityEngine;
using System.Collections;

public class EnemySelectionMark : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if (GetComponentInParent<Enemy_Controller>().selected == true) 
			{	
				GetComponent<MeshRenderer>().enabled = true;
			}
			else
			{
				GetComponent<MeshRenderer>().enabled = false;
			}
		}
		else if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GetComponentInParent<Player_Controller>().selected == true 
			   || GetComponentInParent<Player_Controller>().MPTarget == true 
			   || GetComponentInParent<Player_Controller>().healTarget == true)
			{
				GetComponent<MeshRenderer>().enabled = true;
			}
			else
			{
				GetComponent<MeshRenderer>().enabled = false;
			}
		}
		
	}
}
