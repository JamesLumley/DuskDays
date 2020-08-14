using UnityEngine;
using System.Collections;

public class SelectionMarker : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponentInParent<Player_Controller>().selected == true || GetComponentInParent<Player_Controller>().MPTarget == true || GetComponentInParent<Player_Controller>().healTarget == true) 
		{	
			GetComponent<MeshRenderer>().enabled = true;
		}
		else
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
	
	}
}
