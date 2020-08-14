using UnityEngine;
using System.Collections;

public class Attack_Control : MonoBehaviour {

	public GUISkin skin;
	public GUIStyle style;
	private bool textActive;

	// Update is called once per frame
	void Update () {
		if (this.GetComponent<Player_Controller> ().target != null) 
		{
			textActive = true;
		}
		else
		{
			textActive = false;
		}
	}

	void OnGUI()
	{	
		GUI.skin = skin;
		
		if (textActive) 
		{
		//	drawText ();
		}
	}


	void drawText()
	{
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0125); 
		Rect TextRect = new Rect(Camera.main.pixelWidth /2, Camera.main.pixelHeight - Camera.main.pixelHeight * 0.14f,200,50);
		Rect BoxRect = new Rect(Camera.main.pixelWidth/2 - Camera.main.pixelWidth * 0.017f,Camera.main.pixelHeight - Camera.main.pixelHeight * 0.155f,Camera.main.pixelWidth * 0.14f,Camera.main.pixelHeight * 0.07f);
		GUI.Box (BoxRect, "",skin.GetStyle("Slot"));
		GUI.Label (TextRect, "Press E to Attack");
	}




}
