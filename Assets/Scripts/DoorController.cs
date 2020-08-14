using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorController : MonoBehaviour {
	public GameObject door;
	public GUISkin skin;
	public bool needsKey;
	private bool textActive;
	private int lockedTextTimer;
	private bool lockedTextActive;
	private bool isOpen;
	public List<GameObject> toActivate;
	public List<GameObject> nodesToActivate;
	// Use this for initialization
	void Start () {
		lockedTextTimer = 0;
		door = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerDatabase.getSelected() != null)
		{
			if(PlayerDatabase.getSelected().GetComponent<Player_Controller>().target == null)
			{
				unlockDoor ();
				lockedTextTimer--;
				if(lockedTextTimer == 0)
				{
					lockedTextActive = false;
				}
			}
			else
			{
				textActive = false;
			}
		}
		else
		{
			textActive = false;
		}
	}

	public void unlockDoor()
	{
		//checks if door is closed, player is close and facing the door.
		if(isOpen == false){
			if (Vector3.Distance (this.transform.position, PlayerDatabase.getSelected ().transform.position) < 4) 
			{
				if(Vector3.Angle (PlayerDatabase.getSelected().transform.forward, this.transform.position - PlayerDatabase.getSelected().transform.position) < 90)
				{
					int itemPos = 0;
					textActive = true;
					if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("Interact")) {	
						if(needsKey)
						{
							foreach (Item item in PlayerDatabase.getSelected().GetComponent<Inventory>().inventory)
							{
								//If the door is locked, checks if there is a key in the players inventory
								if(item.itemName == "Door Key")
								{
									PlayerDatabase.getSelected().GetComponent<Inventory>().inventory[itemPos] = new Item();
									this.GetComponent<Animator>().SetBool("Open", true);//plays door animation
									isOpen = true;
									textActive = false;
									if(toActivate.Count > 0)
									{
										foreach (GameObject enemy in toActivate)
										{
											enemy.GetComponent<Enemy_Controller>().active = true;
										}
									}
									if(nodesToActivate.Count > 0)
									{
										foreach (GameObject node in nodesToActivate)
										{
											node.SetActive(true);
										}
									}
									return;
								}
								itemPos++;
							}
							Debug.Log ("Door Locked");
							lockedTextTimer = 100;
							lockedTextActive = true;
						}
						else
						{
							this.GetComponent<Animator>().SetBool("Open", true);//plays door animation
							isOpen = true;
							textActive = false;
							foreach (GameObject enemy in toActivate)
							{
								enemy.GetComponent<Enemy_Controller>().active = true;
							}

							if(nodesToActivate.Count > 0)
							{
								foreach (GameObject node in nodesToActivate)
								{
									node.SetActive(true);
								}
							}
							return;
						}

					}
				}
				else
				{
					textActive = false;
				}

			}
			else
			{
				textActive = false;
			}
	 	}
	}

	public bool getOpen()
	{
		return isOpen;
	}

	void drawText()
	{
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0125); 
		Rect TextRect = new Rect(Camera.main.pixelWidth /2, Camera.main.pixelHeight - Camera.main.pixelHeight * 0.14f,200,50);
		Rect BoxRect = new Rect(Camera.main.pixelWidth/2 - Camera.main.pixelWidth * 0.017f,Camera.main.pixelHeight - Camera.main.pixelHeight * 0.155f,Camera.main.pixelWidth * 0.14f,Camera.main.pixelHeight * 0.07f);
		GUI.Box (BoxRect, "",skin.GetStyle("Slot"));
		GUI.Label (TextRect, "Press E to Open");
	}

	void drawLockedText()
	{
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0225); 
		Rect TextRect = new Rect(Camera.main.pixelWidth /2 - Camera.main.pixelWidth * 0.045f, Camera.main.pixelHeight - Camera.main.pixelHeight * 0.35f,360,50);
		GUI.Label (TextRect, "The door is locked");
	}

	void OnGUI()
	{

		GUI.skin = skin;
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && GameStateMachine.currentGameState != GameStateMachine.State.paused)
		{
			if (textActive) 
			{
				drawText ();
			}
			if (lockedTextActive)
			{
				drawLockedText();
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer && GameStateMachine.currentMPGameState != GameStateMachine.MPState.paused)
		{
			if (textActive) 
			{
				drawText ();
			}
			if (lockedTextActive)
			{
				drawLockedText();
			}
		}
	}
}
