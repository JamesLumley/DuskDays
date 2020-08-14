using UnityEngine;
using System.Collections;

public class Chest_Controller : MonoBehaviour {
	private bool opened = false;
	public GameObject Chest;
	public GUISkin skin;
	public GUIStyle style;
	public bool textActive;
	//public int collisionCount;
	public int insideItemID;
	// Use this for initialization
	void Start () {

	}

	
	// Update is called once per frame
	void Update () 
	{
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
		{
			singlePlayerHandle ();
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			multiplayerHandle();
		}
	}

	void singlePlayerHandle()
	{
		if(PlayerDatabase.getSelected() != null)//Checks if player is selected
		{
			if(PlayerDatabase.getSelected().GetComponent<Player_Controller>().target == null)//Checks if player isnt targeting enemy already
			{
				if(opened == false) //checks if the chest is open
				{
					if (Vector3.Distance (this.transform.position, PlayerDatabase.getSelected ().transform.position) < 2.5) //checks if the player is close enough
					{
						if(Vector3.Angle (PlayerDatabase.getSelected().transform.forward, this.transform.position - PlayerDatabase.getSelected().transform.position) < 45) //checks if the player is facing the chest
						{
							textActive = true;//sets text to active
							if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("Interact")) 
							{
								if(PlayerDatabase.getSelected().GetComponent<Inventory>().isFull == false)//checks if inventory is full already
								{
									string job = PlayerDatabase.getSelected().GetComponent<PlayerStats>().job;
									//Adds item based on ID and player job
									if(insideItemID == 99)
									{
										//do nothing
									}
									else if(insideItemID == 100)
									{
										//do nothing
									}
									else if(job == "Swordsman")
									{
										insideItemID = insideItemID;
									}
									else if(job == "Ranger")
									{
										insideItemID = insideItemID + 1;
									}
									else if(job == "Mage")
									{
										insideItemID = insideItemID + 2;
									}
									else if(job == "Healer")
									{
										insideItemID = insideItemID + 3;
									}
									else if(job == "Knight")
									{
										insideItemID = insideItemID + 4;
									}
									
									PlayerDatabase.getSelected().GetComponent<Inventory>().AddItem(insideItemID);//Create item and adds to inventory.
									Chest.GetComponent<Animation>().Play ();//plays chest animation
									textActive = false;//gets rid of open text
									opened = true;//prevents it from being opened in future
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

	void multiplayerHandle()
	{
		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			if(PlayerDatabase.getPlayer1Selected().GetComponent<Player_Controller>().target == null)
			{
				if(opened == false)
				{
					if (Vector3.Distance (this.transform.position, PlayerDatabase.getPlayer1Selected ().transform.position) < 2.5) 
					{
						if(Vector3.Angle (PlayerDatabase.getPlayer1Selected().transform.forward, this.transform.position - PlayerDatabase.getPlayer1Selected().transform.position) < 45)
						{
							textActive = true;
							if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("InteractPlayer1")) 
							{
								if(PlayerDatabase.getPlayer1Selected().GetComponent<Inventory>().isFull == false)
								{
									string job = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>().job;
									if(insideItemID == 99)
									{
										//do nothing
									}
									else if(insideItemID == 100)
									{
										//do nothing
									}
									else if(job == "Swordsman")
									{
										insideItemID = insideItemID;
									}
									else if(job == "Ranger")
									{
										insideItemID = insideItemID + 1;
									}
									else if(job == "Mage")
									{
										insideItemID = insideItemID + 2;
									}
									else if(job == "Healer")
									{
										insideItemID = insideItemID + 3;
									}
									else if(job == "Knight")
									{
										insideItemID = insideItemID + 4;
									}
									
									PlayerDatabase.getPlayer1Selected().GetComponent<Inventory>().AddItem(insideItemID);
									Chest.GetComponent<Animation>().Play ();
									textActive = false;
									opened = true;
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
			else
			{
				textActive = false;
			}
		}
		else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			if(PlayerDatabase.getPlayer2Selected().GetComponent<Player_Controller>().target == null)
			{
				if(opened == false)
				{
					if (Vector3.Distance (this.transform.position, PlayerDatabase.getPlayer2Selected ().transform.position) < 2.5) 
					{
						if(Vector3.Angle (PlayerDatabase.getPlayer2Selected().transform.forward, this.transform.position - PlayerDatabase.getPlayer2Selected().transform.position) < 45)
						{
							textActive = true;
							if (Input.GetKeyDown (KeyCode.E) || Input.GetButtonDown("InteractPlayer2")) 
							{
								if(PlayerDatabase.getPlayer2Selected().GetComponent<Inventory>().isFull == false)
								{
									string job = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>().job;
									if(insideItemID == 99)
									{
										//do nothing
									}
									else if(insideItemID == 100)
									{
										//do nothing
									}
									else if(job == "Swordsman")
									{
										insideItemID = insideItemID;
									}
									else if(job == "Ranger")
									{
										insideItemID = insideItemID + 1;
									}
									else if(job == "Mage")
									{
										insideItemID = insideItemID + 2;
									}
									else if(job == "Healer")
									{
										insideItemID = insideItemID + 3;
									}
									else if(job == "Knight")
									{
										insideItemID = insideItemID + 4;
									}
									
									PlayerDatabase.getPlayer2Selected().GetComponent<Inventory>().AddItem(insideItemID);
									Chest.GetComponent<Animation>().Play ();
									textActive = false;
									opened = true;
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
			else
			{
				textActive = false;
			}
		}
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

	void OnGUI()
	{
		GUI.skin = skin;
		if (textActive) 
		{
			drawText ();		
		}

	}

	public bool getOpen()
	{
		return opened;
	}
}
