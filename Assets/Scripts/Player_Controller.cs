using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player_Controller : MonoBehaviour {

	public float speed;
	public float turnSmoothing = 15f;
	public bool selected = false;
	public bool healTarget = false;
	public bool MPTarget = false;
	public bool isDead = false;
	private Vector3 startingPos;
	private Quaternion startingRot;
	private Vector3 oldPos;
	private Vector3 newPos;
	public GameObject target = null;
	public GameObject healTargetObject = null;
	public GUISkin skin;
	Vector2 targetPos = new Vector2(0,0);
	public AnimationStates currentState;
	public Transform shootPoint;
	public int listID;
	public GameObject magicProjectile;


	public enum AnimationStates
	{
		Idle,
		Run,
		Attack,
		Magic,
		Ranged,
		Death
	}

	// Use this for initialization
	void Awake () 
	{

		startingPos = this.transform.position;
		startingRot = this.transform.rotation;
		this.GetComponent<PlayerStats> ().experience = 100;
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			DontDestroyOnLoad (this.gameObject);
		}
	}

	
	void OnLevelWasLoaded(int level)
	{
		this.transform.position = startingPos;
		this.transform.rotation = startingRot;
		this.isDead = false;
		currentState = AnimationStates.Idle;
	}
	// Update is called once per frame
	void Update () {
		if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer) //If Game is single player
		{
			SinglePlayerHandeling(); //Use handeling methods for single player characters
		}
		else if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer) //If game is in multiplayer
		{
			MultiplayerHandeling();//Use handeling methods for multiplayer (Required due to the use of seperate joysticks)
		}

		//Prevents the player from being able to push other units around when moving, but allows for collision when selected
		if(!selected)
		{
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		else
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				this.GetComponent<Rigidbody>().isKinematic = false;
			}
			else
			{
				this.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
	}

	void SinglePlayerHandeling()
	{
		//Death check, used for animation purposes
		if(this.GetComponent<PlayerStats>().currHP <= 0)
		{
			isDead = true;
		}

		if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)//Prevents movement when not the players turn
		{
			//Movement handeling
			if (this.GetComponent<PlayerStats> ().currStamina > 0) {
				oldPos = this.transform.position; //get old position to calculate stamina lost

				if (selected) {
					if (Input.GetKeyDown (KeyCode.E)) {
						this.transform.position += new Vector3 (0.000001f, 0, 0);
						this.transform.position -= new Vector3 (0.000001f, 0, 0);
					}
					
					float h = Input.GetAxis ("Horizontal");
					float v = Input.GetAxis ("Vertical");
					MovementManager (h, v);
					if(h != 0 || v != 0)
					{
						currentState = AnimationStates.Run;
					}
					else
					{
						currentState = AnimationStates.Idle;
					}
				}
				
				
				newPos = this.transform.position;// get new position to calculate stamina lost
				this.GetComponent<PlayerStats> ().currStamina -= Vector3.Distance (oldPos, newPos); //Calculating distance travelled and removing that from stamina, this makes all movement fair
			}

			
			if(Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown ("EndTurn"))
			{
				if(selected)
				{
					Score.turnsTaken++;
					EndTurn();
				}
			}

			if (selected) 
			{
				if (target != null || healTargetObject != null) 
				{
					if(this.GetComponent<PlayerStats>().currStamina > 0)
					{
						if (Input.GetKeyDown (KeyCode.E)  || Input.GetButton("Interact"))
						{
							if(this.GetComponent<PlayerStats>().job == "Healer")
							{
								HealPlayer();
							}
							else
							{
								AttackEnemy();
							}
							currentState = AnimationStates.Attack;
							if(this.GetComponent<PlayerStats>().job == "Ranger")
							{
								currentState = AnimationStates.Ranged;
								StartCoroutine(rangeAttack());

							}
							else if(this.GetComponent<PlayerStats>().job == "Mage" || this.GetComponent<PlayerStats>().job == "Healer")
							{
								if(this.GetComponent<PlayerStats>().job == "Mage")
								{
									GameObject projectile = (GameObject)Instantiate(magicProjectile, this.transform.position + new Vector3(0,3,0),new Quaternion(0,0,0,0));
									projectile.GetComponent<MagicProjectile>().target = target;
								}
								currentState = AnimationStates.Magic;
							}
						}		
					}
				}

				if(this.GetComponent<PlayerStats>().job == "Healer")
				{
					findHealTarget();
				}
				else
				{
					findTargetEnemy ();
				}
				if(this.GetComponent<Inventory>().isBeingDragged == false)
				{
					ChangeSelected();
				}
			} 
			else 
			{
				target = null;
				if(healTargetObject != null)
				{
					healTargetObject.GetComponent<Player_Controller>().healTarget = false;
				}
				healTargetObject = null;
			}
		}

		AnimationCheck ();
	}

	public IEnumerator rangeAttack()
	{
		yield return new WaitForSeconds(0.3f);
		//creates rock, different rock for different modes and players
		GameObject stone = null;
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			stone = Instantiate(Resources.Load("Rock"), shootPoint.transform.position, Random.rotation) as GameObject;
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				 stone = Instantiate(Resources.Load("BlueRock"), shootPoint.transform.position, Random.rotation) as GameObject;
			}
			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				 stone = Instantiate(Resources.Load("RedRock"), shootPoint.transform.position, Random.rotation) as GameObject;
			}
		}
		//gives the rock a forward velocity and random angular velocity
		stone.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 20 + new Vector3(0,3,0); 
		stone.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 50;

		yield return new WaitForSeconds(0.45f);

	}

	void MultiplayerHandeling()
	{
		if(this.GetComponent<PlayerStats>().currHP <= 0)
		{
			isDead = true;
		}

		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			if(selected)
			{
				GameStateMachine.turnChangeTime += Time.deltaTime;
				if (this.GetComponent<PlayerStats> ().currStamina > 0) 
				{
					oldPos = this.transform.position;

					if (Input.GetKeyDown (KeyCode.E)) {
						this.transform.position += new Vector3 (0.000001f, 0, 0);
						this.transform.position -= new Vector3 (0.000001f, 0, 0);
					}
					
					float h = Input.GetAxis ("HorizontalPlayer1");
					float v = Input.GetAxis ("VerticalPlayer1");
					MovementManager (h, v);
					if(h != 0 || v != 0)
					{
						currentState = AnimationStates.Run;
					}
					else
					{
						currentState = AnimationStates.Idle;
					}

					newPos = this.transform.position;
					this.GetComponent<PlayerStats> ().currStamina -= Vector3.Distance (oldPos, newPos);
				}
				if(this.GetComponent<PlayerStats>().job == "Healer")
				{
					findHealTargetMP();
				}
				else
				{
					findTargetPlayer();
				}
				ChangeSelectedMP();

				if (target != null || healTargetObject != null) 
				{
					if(this.GetComponent<PlayerStats>().currStamina > 0)
					{
						if (Input.GetButton("InteractPlayer1"))
						{
							if(this.GetComponent<PlayerStats>().job == "Healer")
							{
								HealPlayer();
							}
							else
							{
								AttackEnemy();
							}
							currentState = AnimationStates.Attack;
							if(this.GetComponent<PlayerStats>().job == "Ranger")
							{
								currentState = AnimationStates.Ranged;
								StartCoroutine(rangeAttack());
								
							}
							else if(this.GetComponent<PlayerStats>().job == "Mage" || this.GetComponent<PlayerStats>().job == "Healer" )
							{
								currentState = AnimationStates.Magic;
								if(this.GetComponent<PlayerStats>().job == "Mage")
								{
									GameObject projectile = (GameObject)Instantiate(magicProjectile, this.transform.position + new Vector3(0,3,0),new Quaternion(0,0,0,0));
									projectile.GetComponent<MagicProjectile>().target = target;
								}
							}
						}		
					}
				}
				if(Input.GetButtonDown ("EndTurnPlayer1"))
				{
					if(GameStateMachine.turnChangeTime > 1.0f)
					{
						EndTurnMulti();
						PlayerDatabase.selectedID = 0;
						GameStateMachine.turnChangeTime = 0;
					}
				}
			}
			else 
			{
				target = null;
				if(healTargetObject != null)
				{
					healTargetObject.GetComponent<Player_Controller>().healTarget = false;
				}
				healTargetObject = null;

			}
		}

		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			if(selected)
			{
				if (this.GetComponent<PlayerStats> ().currStamina > 0) 
				{
					oldPos = this.transform.position;

					if (Input.GetKeyDown (KeyCode.E)) {
						this.transform.position += new Vector3 (0.000001f, 0, 0);
						this.transform.position -= new Vector3 (0.000001f, 0, 0);
					}
					
					float h = Input.GetAxis ("HorizontalPlayer2");
					float v = Input.GetAxis ("VerticalPlayer2");
					MovementManager (h, v);
					if(h != 0 || v != 0)
					{
						currentState = AnimationStates.Run;
					}
					else
					{
						currentState = AnimationStates.Idle;
					}
					
					newPos = this.transform.position;
					this.GetComponent<PlayerStats> ().currStamina -= Vector3.Distance (oldPos, newPos);
				}
				if(this.GetComponent<PlayerStats>().job == "Healer")
				{
					findHealTargetMP();
				}
				else
				{
					findTargetPlayer();
				}
				ChangeSelectedMP();
				
				if (target != null || healTargetObject != null) 
				{
					if(this.GetComponent<PlayerStats>().currStamina > 0)
					{
						if (Input.GetButton("InteractPlayer2"))
						{
							if(this.GetComponent<PlayerStats>().job == "Healer")
							{
								HealPlayer();
							}
							else
							{
								AttackEnemy();
							}
							currentState = AnimationStates.Attack;
							if(this.GetComponent<PlayerStats>().job == "Ranger")
							{
								currentState = AnimationStates.Ranged;
								StartCoroutine(rangeAttack());
								
							}
							else if(this.GetComponent<PlayerStats>().job == "Mage" || this.GetComponent<PlayerStats>().job == "Healer" )
							{
								currentState = AnimationStates.Magic;
								if(this.GetComponent<PlayerStats>().job == "Mage")
								{
									GameObject projectile = (GameObject)Instantiate(magicProjectile, this.transform.position + new Vector3(0,3,0),new Quaternion(0,0,0,0));
									projectile.GetComponent<MagicProjectile>().target = target;
								}
							}
						}		
					}
				}
				if(Input.GetButtonDown ("EndTurnPlayer2"))
				{
					EndTurnMulti();
					PlayerDatabase.selectedID = 0;
				}
			}
			else 
			{
				target = null;
				if(healTargetObject != null)
				{
					healTargetObject.GetComponent<Player_Controller>().healTarget = false;
				}
				healTargetObject = null;

			}
		}
		AnimationCheck ();
		

	}

	void OnGUI()
	{

		GUI.skin = skin;
		targetPos = Camera.main.WorldToScreenPoint (this.transform.position);//This is the position for drawing floating health bars onto the character
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer //Only draws GUI whilst in game.
		   && GameStateMachine.currentGameState != GameStateMachine.State.paused 
		   && GameStateMachine.currentGameState != GameStateMachine.State.statScreen 
		   && GameStateMachine.currentGameState != GameStateMachine.State.gameOver )
		{
			drawHealth();

			if(target != null && this.GetComponent<PlayerStats>().currStamina > 0)
			{
				drawAttackText();
			}
			if(healTargetObject != null && this.GetComponent<PlayerStats>().currStamina > 0)
			{
				drawHealText();
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer 
		        && GameStateMachine.currentMPGameState != GameStateMachine.MPState.paused 
		        && GameStateMachine.currentMPGameState != GameStateMachine.MPState.statScreen)
		{
			drawHealth();
			
			if(target != null && this.GetComponent<PlayerStats>().currStamina > 0)
			{
				drawAttackText();
			}
			if(healTargetObject != null && this.GetComponent<PlayerStats>().currStamina > 0)
			{
				drawHealText();
			}
		}



	}

	void AnimationCheck()
	{
		if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Attack"))
		{
			currentState = AnimationStates.Attack;
		}
		else if(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName ("Orc_Slingshot_Attack"))
		{
			currentState = AnimationStates.Ranged;
		}
		
		if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 
		   && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Attack")) //checks if attack animation has finised
		{
			currentState = AnimationStates.Idle;		
		}
		else if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 
		        && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Slingshot_Attack")) //Checks if slingshot animation has finished
		{
			currentState = AnimationStates.Idle;
		}
		
		if(selected == false)
		{
			currentState = AnimationStates.Idle;	
		}
		if(this.GetComponent<PlayerStats>().currHP <= 0)
		{
			currentState = AnimationStates.Death;
		}
		if(currentState == AnimationStates.Run)
		{
			this.GetComponent<Animator>().SetBool("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", true);
		}
		else if(currentState == AnimationStates.Attack)
		{
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Attack", true);
		}
		else if(currentState == AnimationStates.Ranged)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("rangedAttack", true);
		}
		else if(currentState == AnimationStates.Magic)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("spellAttack", true);
		}
		else if(currentState == AnimationStates.Death)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Death", true);
			if((this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) && this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName ("Orc_death"))
			{
				if(this.tag == "Player")
				{
					//PlayerDatabase.players.Remove(this.gameObject);
				}
				else if (this.tag == "BlueTeam")
				{
					PlayerDatabase.bluePlayers.Remove(this.gameObject);
				}
				else if (this.tag == "RedTeam")
				{
					PlayerDatabase.redPlayers.Remove(this.gameObject);
				}
				this.gameObject.SetActive(false);
			}
		}
		else
		{
			this.GetComponent<Animator>().SetBool("Idle", true);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool("Attack",false);
			this.GetComponent<Animator>().SetBool ("spellAttack", false);
			this.GetComponent<Animator>().SetBool ("rangedAttack", false);
		}
		if(this.GetComponent<PlayerStats>().currStamina <= 0 && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			if(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName ("Orc_Slingshot_Attack") //checks if the attack animations have finished, and no more movement can be made.
			   || this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Attack") 
			   || this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Orc_Spellcast"))
			{
				currentState = AnimationStates.Idle;
			}
		}
		if(this.GetComponent<PlayerStats>().currStamina <= 0 && this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName ("Orc_Run"))//If the player cannot run anymore
		{
			currentState = AnimationStates.Idle;
		}
	}

	void MovementManager(float horizontal, float vertical)
	{
		if (horizontal != 0f || vertical != 0f) 
		{
			Rotation(horizontal,vertical);	
			this.transform.position = this.transform.position + new Vector3(horizontal,0.0f,vertical) * 3 * Time.deltaTime; //Moves the character to new position

		}
	}

	void Rotation(float horizontal, float vertical)
	{
		Vector3 targetDirection = new Vector3 (horizontal, 0f, vertical);
		Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up); //Creates a target rotation from the target direction
		Quaternion newRotation = Quaternion.Lerp (this.transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);//Finds a rotation between the current and desired rotation by a fraction of turn soomthing.
		transform.rotation = newRotation; //changes the rotation to the newly created one

	}

	void ChangeSelected()
	{
		if(PlayerDatabase.changedFrame != Time.frameCount)
		{
			if (Input.GetKeyDown (KeyCode.Space) || Input.GetButtonDown ("SwitchCharacter")) 
			{
				PlayerDatabase.changedFrame = Time.frameCount;
				bool changed = false;
				while(!changed)
				{
					if(PlayerDatabase.players.Count - 1 == PlayerDatabase.selectedID)
					{
						PlayerDatabase.selectedID = 0;
					}
					else
					{
						PlayerDatabase.selectedID++;
					}
					if(target != null)
					{
						target.GetComponent<Enemy_Controller>().selected = false;
					}
					if(PlayerDatabase.players[PlayerDatabase.selectedID].activeSelf == true)
					{
						changed = true;
					}
				}
				this.selected = false;
				PlayerDatabase.players [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;

			}
            else if (Input.GetButtonDown("SwitchCharacterBack"))
            {
				PlayerDatabase.changedFrame = Time.frameCount;
				bool changed = false;
				while(!changed)
				{
	                if (0 == PlayerDatabase.selectedID)
	                {
	                    PlayerDatabase.selectedID = PlayerDatabase.players.Count - 1;
	                }
	                else
	                {
	                    PlayerDatabase.selectedID--;
	                }
					if(target != null)
					{
						target.GetComponent<Enemy_Controller>().selected = false;
					}
					if(PlayerDatabase.players[PlayerDatabase.selectedID].activeSelf == true)
					{
						changed = true;
					}
				}
				this.selected = false;
				PlayerDatabase.players [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;

            }

		}

	}

	void ChangeSelectedMP()
	{
		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			if(PlayerDatabase.changedFrame != Time.frameCount)
			{
				if (Input.GetButtonDown ("SwitchCharacterPlayer1")) 
				{
					PlayerDatabase.changedFrame = Time.frameCount;

					if(PlayerDatabase.bluePlayers.Count - 1 == PlayerDatabase.selectedID)
					{
						PlayerDatabase.selectedID = 0;
					}
					else
					{
						PlayerDatabase.selectedID++;
					}
					if(target != null)
					{
						target.GetComponent<Player_Controller>().MPTarget = false;
					}
					this.selected = false;
					PlayerDatabase.bluePlayers [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;

				}
				else if(Input.GetButtonDown("SwitchCharacterBackPlayer1"))
				{
					PlayerDatabase.changedFrame = Time.frameCount;

					if(0 == PlayerDatabase.selectedID)
					{
						PlayerDatabase.selectedID = PlayerDatabase.bluePlayers.Count - 1;
					}
					else
					{
						PlayerDatabase.selectedID--;
					}
					if(target != null)
					{
						target.GetComponent<Player_Controller>().MPTarget = false;
					}
					this.selected = false;
					PlayerDatabase.bluePlayers [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;

				}
			}
		}
		else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			if(PlayerDatabase.changedFrame != Time.frameCount)
			{
				if (Input.GetButtonDown ("SwitchCharacterPlayer2")) 
				{
					PlayerDatabase.changedFrame = Time.frameCount;

					if(PlayerDatabase.redPlayers.Count - 1 == PlayerDatabase.selectedID)
					{
						PlayerDatabase.selectedID = 0;
					}
					else
					{
						PlayerDatabase.selectedID++;
					}
					if(target != null)
					{
						target.GetComponent<Player_Controller>().MPTarget = false;
					}
					this.selected = false;
					PlayerDatabase.redPlayers [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;



				}
				else if(Input.GetButtonDown("SwitchCharacterBackPlayer2"))
				{
					PlayerDatabase.changedFrame = Time.frameCount;

					if(0 == PlayerDatabase.selectedID)
					{
						PlayerDatabase.selectedID = PlayerDatabase.redPlayers.Count - 1;
					}
					else
					{
						PlayerDatabase.selectedID--;
					}
					if(target != null)
					{
						target.GetComponent<Player_Controller>().MPTarget = false;
					}
					this.selected = false;
					PlayerDatabase.redPlayers [PlayerDatabase.selectedID].GetComponent<Player_Controller> ().selected = true;

				}
			}
		}

	}

	void AttackEnemy()
	{
		if (target.GetComponent<PlayerStats> ().currHP != 0)
		{
			int attackModifier;
			int defenseModifier;
			int damageToDo;

			transform.LookAt (target.transform.position);
			if (this.GetComponent<PlayerStats> ().job == "Mage") {
				attackModifier = this.GetComponent<PlayerStats> ().magicAttack;
				defenseModifier = target.GetComponent<PlayerStats> ().MagicDefense;
			} else {
				attackModifier = this.GetComponent<PlayerStats> ().attack;
				defenseModifier = target.GetComponent<PlayerStats> ().defense;
			}
			damageToDo = attackModifier - defenseModifier;

			if (damageToDo < 0) {
				damageToDo = 0;
			}

			damageToDo = damageToDo + this.GetComponent<Inventory> ().currentEquipped.attackMod;

			if (target.GetComponent<PlayerStats> ().currHP - damageToDo <= 0) 
			{
				target.GetComponent<PlayerStats> ().currHP = 0;
				this.GetComponent<PlayerStats> ().experience += 50;
				if(target.tag == "Enemy")
				{
					if (target.GetComponent<Enemy_Controller> ().itemID > 0) 
					{
						this.GetComponent<Inventory> ().AddItem (target.GetComponent<Enemy_Controller> ().itemID);
					}
				}

			} else {
				target.GetComponent<PlayerStats> ().currHP -= damageToDo;
			}
			this.GetComponent<Inventory>().currentEquipped.currDurability--;
			this.GetComponent<PlayerStats> ().currStamina = 0;
			this.GetComponent<PlayerStats> ().experience += 10;
		}
		
	}

	void HealPlayer()
	{
		int healModifier;
		int healAmount;
		Debug.Log ("Healing");
		healModifier = this.GetComponent<PlayerStats> ().healPower;

		healAmount = healModifier + this.GetComponent<Inventory> ().currentEquipped.healMod;
		if(healTargetObject.GetComponent<PlayerStats>().currHP + healAmount >= healTargetObject.GetComponent<PlayerStats>().maxHP)
		{
			healTargetObject.GetComponent<PlayerStats>().currHP = healTargetObject.GetComponent<PlayerStats>().maxHP;
		}
		else
		{
			healTargetObject.GetComponent<PlayerStats> ().currHP += healAmount;
		}
		this.GetComponent<PlayerStats> ().currStamina = 0;
		this.GetComponent<PlayerStats> ().experience += 30;

	}

	void EndTurn()
	{
		bool enemyDying = false;
		
		foreach(GameObject enemy in EnemyDatabase.enemies)
		{
			if(enemy.GetComponent<PlayerStats>().currHP <= 0)
			{
				enemyDying = true;
			}
		}

		if(!enemyDying)
		{
			foreach (GameObject player in PlayerDatabase.players)
			{
				player.GetComponent<PlayerStats>().currStamina = player.GetComponent<PlayerStats>().maxStamina;
				player.GetComponent<Player_Controller>().target = null;
				player.GetComponent<Player_Controller>().healTargetObject = null;
				player.GetComponent<Player_Controller>().selected = false;
				player.GetComponent<Player_Controller>().healTarget = false;
			}
			foreach (GameObject enemy in EnemyDatabase.enemies)
			{
				enemy.GetComponent<Enemy_Controller>().selected = false;
			}
	        foreach (GameObject enemy in EnemyDatabase.enemies)
	        {
	            if (enemy.GetComponent<Enemy_Controller>().active == true)
	            {
	                GameStateMachine.currentGameState = GameStateMachine.State.enemyTurn;
	                enemy.GetComponent<Enemy_Controller>().selected = true;
	                return;
	            }
	        }
	        GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;

	        foreach (GameObject player in PlayerDatabase.players)
	        {
	            player.GetComponent<Player_Controller>().selected = false;
	        }
			bool active = false;
			int i = 0;
			while(!active)
			{
				if(PlayerDatabase.players[i].activeSelf == true)
				{
					active = true;
					break;
				}
				else
				{
					i++;
				}
			}
	        PlayerDatabase.players[i].GetComponent<Player_Controller>().selected = true;
	        PlayerDatabase.selectedID = i;
		}
	}

	void EndTurnMulti()
	{
		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			foreach (GameObject player in PlayerDatabase.bluePlayers)
			{
				player.GetComponent<PlayerStats>().currStamina = player.GetComponent<PlayerStats>().maxStamina;
				player.GetComponent<Player_Controller>().selected = false;
			}
			foreach (GameObject player in PlayerDatabase.redPlayers)
			{
				player.GetComponent<Player_Controller>().MPTarget = false;
			}
			GameStateMachine.currentMPGameState = GameStateMachine.MPState.player2Turn;
			PlayerDatabase.redPlayers[0].GetComponent<Player_Controller>().selected = true;

		}
		else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			foreach (GameObject player in PlayerDatabase.redPlayers)
			{
				player.GetComponent<PlayerStats>().currStamina = player.GetComponent<PlayerStats>().maxStamina;
				player.GetComponent<Player_Controller>().selected = false;
			}
			foreach (GameObject player in PlayerDatabase.bluePlayers)
			{
				player.GetComponent<Player_Controller>().MPTarget = false;
			}
			GameStateMachine.currentMPGameState = GameStateMachine.MPState.player1Turn;
			PlayerDatabase.bluePlayers[0].GetComponent<Player_Controller>().selected = true;

		}
	}

	void drawHealth()
	{
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(PlayerDatabase.getSelected() != null)
			{
	        if (Vector3.Distance(PlayerDatabase.getSelected().transform.position, this.transform.position) < 9)
		        {
		            int currHP = this.GetComponent<PlayerStats>().currHP;
		            int maxHP = this.GetComponent<PlayerStats>().maxHP;
		            float hpPercent = ((float)currHP / (float)maxHP) * 15.0f;
		            Rect hpRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, hpPercent * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
		            Rect backRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, 15 * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
		            GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
		            GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBar"));
		        }
			}
		}

		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				if(Vector3.Distance (PlayerDatabase.getPlayer1Selected().transform.position, this.transform.position) < 9)
				{
					int currHP = this.GetComponent<PlayerStats>().currHP;
					int maxHP = this.GetComponent<PlayerStats>().maxHP;
					float hpPercent = ((float)currHP / (float)maxHP) * 15.0f;
					Rect hpRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, hpPercent * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					Rect backRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, 15 * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
					if(this.tag == "BlueTeam")
					{
						GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBarBlue"));
					}
					else if(this.tag == "RedTeam")
					{
						GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBarRed"));
					}
				}
			}
			else if(GameStateMachine .currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				if(Vector3.Distance(PlayerDatabase.getPlayer2Selected().transform.position, this.transform.position) < 9)
				{
					int currHP = this.GetComponent<PlayerStats>().currHP;
					int maxHP = this.GetComponent<PlayerStats>().maxHP;
					float hpPercent = ((float)currHP / (float)maxHP) * 15.0f;
					Rect hpRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, hpPercent * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					Rect backRect = new Rect(targetPos.x - (Camera.main.pixelWidth * 0.0395f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, 15 * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
					if(this.tag == "BlueTeam")
					{
						GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBarBlue"));
					}
					else if(this.tag == "RedTeam")
					{
						GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBarRed"));
					}
				}
			}
		}
	}

	void drawAttackText()
	{
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0125); 
		Rect TextRect = new Rect(Camera.main.pixelWidth /2, Camera.main.pixelHeight - Camera.main.pixelHeight * 0.14f,200,50);
		Rect BoxRect = new Rect(Camera.main.pixelWidth/2 - Camera.main.pixelWidth * 0.010f,Camera.main.pixelHeight - Camera.main.pixelHeight * 0.155f,Camera.main.pixelWidth * 0.14f,Camera.main.pixelHeight * 0.07f);
		GUI.Box (BoxRect, "",skin.GetStyle("Slot"));
		GUI.Label (TextRect, "Press E to Attack");
	}

	void drawHealText()
	{
		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.0125); 
		Rect TextRect = new Rect((Camera.main.pixelWidth /2) + (Camera.main.pixelWidth *0.0075f), Camera.main.pixelHeight - Camera.main.pixelHeight * 0.14f,200,50);
		Rect BoxRect = new Rect(Camera.main.pixelWidth/2 - Camera.main.pixelWidth * 0.017f,Camera.main.pixelHeight - Camera.main.pixelHeight * 0.155f,Camera.main.pixelWidth * 0.14f,Camera.main.pixelHeight * 0.07f);
		GUI.Box (BoxRect, "",skin.GetStyle("Slot"));
		GUI.Label (TextRect, "Press E to Heal");
	}

	void findTargetEnemy()
	{
		List<GameObject> possibleTargets = new List<GameObject> ();
		float distance = 10;

		
		foreach (GameObject enemy in EnemyDatabase.enemies) 
		{
			if(Vector3.Distance(this.transform.position, enemy.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)//Finds enemies in range
			{
				if(Vector3.Angle (this.transform.forward, enemy.transform.position - this.transform.position) < 45) //Finds enemies that the player is facing
				{
					possibleTargets.Add(enemy);
					enemy.GetComponent<Enemy_Controller>().selected = false;
				}
				else
				{
					enemy.GetComponent<Enemy_Controller>().selected = false;
				}
			}
			else
			{
				enemy.GetComponent<Enemy_Controller>().selected = false;
			}
		}
		if (possibleTargets.Count == 0) 
		{
			target = null;
		}
		foreach (GameObject enemy in possibleTargets) 
		{
			if(Vector3.Distance(this.transform.position, enemy.transform.position) < distance) //Finds cloest enemies if there is more than one.
			{
				target = enemy;
			}
		}
		if (target != null) 
		{
			target.GetComponent<Enemy_Controller> ().selected = true;//turns on the targets selection marker
		}
	}

	void findHealTarget()
	{
		List<GameObject> possibleTargets = new List<GameObject> ();
		float distance = 10;

		foreach (GameObject player in PlayerDatabase.players) 
		{
			if(player.GetComponent<PlayerStats>().job != "Healer")
			{
				if(Vector3.Distance(this.transform.position, player.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
				{
					if(Vector3.Angle (this.transform.forward, player.transform.position - this.transform.position) < 45)
					{
						possibleTargets.Add(player);
						player.GetComponent<Player_Controller>().healTarget = false;
					}
					else
					{
						player.GetComponent<Player_Controller>().healTarget = false;
					}
				}
				else
				{
					player.GetComponent<Player_Controller>().healTarget = false;
				}
			}
		}

		if (possibleTargets.Count == 0) 
		{
			healTargetObject = null;
		}
		foreach (GameObject player in possibleTargets) 
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) < distance)
			{
				healTargetObject = player;
			}
		}
		if (healTargetObject != null) 
		{
			healTargetObject.GetComponent<Player_Controller> ().healTarget = true;
		}
	}

	void findHealTargetMP()
	{
		List<GameObject> possibleTargets = new List<GameObject> ();
		float distance = 10;
		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			foreach (GameObject player in PlayerDatabase.bluePlayers) 
			{
				if(player.GetComponent<PlayerStats>().job != "Healer")
				{
					if(Vector3.Distance(this.transform.position, player.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
					{
						if(Vector3.Angle (this.transform.forward, player.transform.position - this.transform.position) < 45)
						{
							possibleTargets.Add(player);
							player.GetComponent<Player_Controller>().healTarget = false;
						}
						else
						{
							player.GetComponent<Player_Controller>().healTarget = false;
						}
					}
					else
					{
						player.GetComponent<Player_Controller>().healTarget = false;
					}
				}
			}
			
			if (possibleTargets.Count == 0) 
			{
				healTargetObject = null;
			}
			foreach (GameObject player in possibleTargets) 
			{
				if(Vector3.Distance(this.transform.position, player.transform.position) < distance)
				{
					healTargetObject = player;
				}
			}
			if (healTargetObject != null) 
			{
				healTargetObject.GetComponent<Player_Controller> ().healTarget = true;
			}
		}

		else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			foreach (GameObject player in PlayerDatabase.redPlayers) 
			{
				if(player.GetComponent<PlayerStats>().job != "Healer")
				{
					if(Vector3.Distance(this.transform.position, player.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
					{
						if(Vector3.Angle (this.transform.forward, player.transform.position - this.transform.position) < 45)
						{
							possibleTargets.Add(player);
							player.GetComponent<Player_Controller>().healTarget = false;
						}
						else
						{
							player.GetComponent<Player_Controller>().healTarget = false;
						}
					}
					else
					{
						player.GetComponent<Player_Controller>().healTarget = false;
					}
				}
			}
			
			if (possibleTargets.Count == 0) 
			{
				healTargetObject = null;
			}
			foreach (GameObject player in possibleTargets) 
			{
				if(Vector3.Distance(this.transform.position, player.transform.position) < distance)
				{
					healTargetObject = player;
				}
			}
			if (healTargetObject != null) 
			{
				healTargetObject.GetComponent<Player_Controller> ().healTarget = true;
			}
		}
	}

	void findTargetPlayer()
	{
		List<GameObject> possibleTargets = new List<GameObject> ();
		float distance = 10;
		
		if(this.tag == "BlueTeam")
		{
			foreach (GameObject enemy in PlayerDatabase.redPlayers) 
			{
				if(Vector3.Distance(this.transform.position, enemy.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
				{
					if(Vector3.Angle (this.transform.forward, enemy.transform.position - this.transform.position) < 45)
					{
						possibleTargets.Add(enemy);
						enemy.GetComponent<Player_Controller>().MPTarget = false;
					}
					else
					{
						enemy.GetComponent<Player_Controller>().MPTarget = false;
					}
				}
				else
				{
					enemy.GetComponent<Player_Controller>().MPTarget = false;
				}
			}
			if (possibleTargets.Count == 0) 
			{
				target = null;
			}
			foreach (GameObject enemy in possibleTargets) 
			{
				if(Vector3.Distance(this.transform.position, enemy.transform.position) < distance)
				{
					target = enemy;
				}
			}
			if (target != null) 
			{
				target.GetComponent<Player_Controller> ().MPTarget = true;
			}
		}
		else if(this.tag == "RedTeam")
		{
			foreach (GameObject enemy in PlayerDatabase.bluePlayers) 
			{
				if(Vector3.Distance(this.transform.position, enemy.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
				{
					if(Vector3.Angle (this.transform.forward, enemy.transform.position - this.transform.position) < 45)
					{
						possibleTargets.Add(enemy);
						enemy.GetComponent<Player_Controller>().MPTarget = false;
					}
					else
					{
						enemy.GetComponent<Player_Controller>().MPTarget = false;
					}
				}
				else
				{
					enemy.GetComponent<Player_Controller>().MPTarget = false;
				}
			}
			if (possibleTargets.Count == 0) 
			{
				target = null;
			}
			foreach (GameObject enemy in possibleTargets) 
			{
				if(Vector3.Distance(this.transform.position, enemy.transform.position) < distance)
				{
					target = enemy;
				}
			}
			if (target != null) 
			{
				target.GetComponent<Player_Controller> ().MPTarget = true;
			}
		}
	}
}
