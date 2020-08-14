using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {

	public GameObject targetNode;
	public Vector3 targetPosition;
	public pathFindingState currentState;
	public AnimationStates currentAnimState;
	public GameObject target;
	public Transform shootPoint;
	private Vector3 newPos;
	private Vector3 oldPos;
	public GameObject magicProjectile;
	public List<GameObject> visitedNodes = new List<GameObject>();
	private int layerMask = 1 << 8;


	public enum pathFindingState
	{
		findingFirst,
		findingNext,
		moving,
		movingToTarget,
		atTarget,
		playerTurn,
		attacking,
		notSelected
	}

	
	public enum AnimationStates
	{
		Idle,
		Run,
		Attack,
		Ranged,
		Magic,
		Death
	}
	// Use this for initialization
	void Start () {

		currentState = pathFindingState.notSelected;
		layerMask = ~layerMask;//Bit shift layer mask to mask only players for ray casting.
	}
	
	// Update is called once per frame
	void Update () {
		if(GameStateMachine.currentGameState != GameStateMachine.State.paused)
		{
			if (this.GetComponent<PlayerStats> ().currStamina > 0) 
			{
				oldPos = this.GetComponent<Rigidbody>().position;//For stamina reduction calculation
				if(currentState == pathFindingState.notSelected && GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
				{
					if(this.GetComponent<Enemy_Controller>().selected == true)
					{
						currentState = pathFindingState.findingFirst;//If turn handler selects this enemy, have it move
					}
				}
				else if(currentState == pathFindingState.findingFirst)
				{
					FindTargetPosition();//find what player to attack
					currentState = pathFindingState.moving;//change to moving state
					FindFirstNode ();//find the first path node to move to
				}
				else if(currentState == pathFindingState.findingNext)
				{
					currentState = pathFindingState.moving;//change to move state
					findNextNode();//find the next path node to move to
				}
				else if(currentState == pathFindingState.moving)
				{
					float step = 6 * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, targetNode.transform.position, step); //Moves the enemy towards target
					transform.LookAt(targetNode.transform.position); //enemy will look at target

					if(transform.position == targetNode.transform.position)
					{
						currentState = pathFindingState.findingNext; //if its at the node, find the next one
					}
					if(Vector3.Distance(targetPosition, this.transform.position) < 3 * this.GetComponent<Inventory>().currentEquipped.range)
					{
						currentState = pathFindingState.atTarget;//if its in range of the target, its at its target destination
					}
					currentAnimState = AnimationStates.Run;
				}
				else if(currentState == pathFindingState.movingToTarget)
				{
					float step = 6 * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);//move directly to the target as there are no obstacles left
					transform.LookAt(targetPosition);

					if(Vector3.Distance(targetPosition, this.transform.position) < 3 * this.GetComponent<Inventory>().currentEquipped.range)
					{
						currentState = pathFindingState.atTarget;//if in range, enemy is at target destination
					}
					currentAnimState = AnimationStates.Run;

				}
				else if(currentState == pathFindingState.attacking)
				{
					//Attack and wait until the animation finishes before going back to normal state.
					if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Attack"))
					{
						currentAnimState = AnimationStates.Idle;
					}
					else if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Slingshot_Attack"))
					{
						currentAnimState = AnimationStates.Idle;
					}
					else if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Spellcast"))
					{
						currentAnimState = AnimationStates.Idle;
					}
					if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= 2 && this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Orc_Idle"))
					{
						this.GetComponent<PlayerStats> ().currStamina = 0;
					}

				}
				else if(currentState == pathFindingState.atTarget)
				{

					//Find target enemy, and attack them.
						transform.LookAt(targetPosition);
						findTargetPlayer();
						if(target == null)
						{
							currentState = pathFindingState.movingToTarget;
							return;
						}
					else
					{
						currentAnimState = AnimationStates.Attack;
						if(this.GetComponent<PlayerStats>().job == "Ranger")
						{
							currentAnimState = AnimationStates.Ranged;
							StartCoroutine(rangeAttack());//create projectile
						}
						else if (this.GetComponent<PlayerStats>().job == "Mage")
						{
							currentAnimState = AnimationStates.Magic;
							GameObject projectile = (GameObject)Instantiate(magicProjectile, this.transform.position + new Vector3(0,3,0),new Quaternion(0,0,0,0));
							projectile.GetComponent<MagicProjectile>().target = target;//send projectile to enemy

						}
						AttackPlayer();
						currentState = pathFindingState.attacking;
					}
				}

				newPos = this.GetComponent<Rigidbody>().position;//for calculating stamina reduction
				this.GetComponent<PlayerStats> ().currStamina -= Vector3.Distance (oldPos, newPos);//reduces stamina for amount moved
			}
			else
			{

					currentState = pathFindingState.notSelected;//unselects characters that shouldnt be selected
					this.GetComponent<Enemy_Controller>().selected = false;
				
			}
			if(currentState == pathFindingState.notSelected)
			{
				currentAnimState = AnimationStates.Idle;
				target = null;//resets target to null

			}
			if(this.GetComponent<PlayerStats>().currHP <= 0)
			{
				currentAnimState = AnimationStates.Death;//kills enemy
			}
			if(currentAnimState == AnimationStates.Death)
			{
				if(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Orc_death") && this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
				{
					EnemyDatabase.enemies.Remove(this.gameObject);//after death animation, remove from list in database
					Destroy(this.gameObject);//and destroy the object
				}
			}
			animationCheck ();
		}
	}

	public void FindTargetPosition()
	{
		GameObject targetPlayer = null;
		List<GameObject> inRangeTargets = new List<GameObject> ();

		foreach (GameObject player in PlayerDatabase.players)
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) < this.GetComponent<PlayerStats>().maxStamina && player.activeSelf == true)
			{
				inRangeTargets.Add(player);
			}
		}
		if(inRangeTargets.Count != 0)
		{
			foreach (GameObject player in inRangeTargets)
			{
				if(targetPlayer == null)
				{
					targetPlayer = player;
					if(player.GetComponent<PlayerStats>().currHP - AttackSimulation(player) <= 0)
					{
						targetPosition = targetPlayer.transform.position;
						return;
					}
				}
				else
				{
					if(player.GetComponent<PlayerStats>().currHP - AttackSimulation(player) < targetPlayer.GetComponent<PlayerStats>().currHP - AttackSimulation(targetPlayer))
					{
						targetPlayer = player;
						if(player.GetComponent<PlayerStats>().currHP - AttackSimulation(player) <= 0)
						{
							targetPosition = targetPlayer.transform.position;
							return;
						}
					}
				}
			}
		}
		else
		{
			foreach (GameObject player in PlayerDatabase.players)
			{
				if(player.activeSelf == true)
				{
					if (targetPlayer == null)
					{
						targetPlayer = player;
					}
					else
					{
						if(Vector3.Distance(this.transform.position, player.transform.position) < Vector3.Distance(this.transform.position, targetPlayer.transform.position))
						{
							targetPlayer = player;
						}
					}
				}
			}
		}
		targetPosition = targetPlayer.transform.position;

	}

	public int AttackSimulation(GameObject simTarget)
	{
		int attackModifier;
		int defenseModifier;
		int damageToDo;
		
		if (this.GetComponent<PlayerStats> ().job == "Mage")
		{
			attackModifier = this.GetComponent<PlayerStats>().magicAttack;
			defenseModifier = simTarget.GetComponent<PlayerStats>().MagicDefense;
		}
		else
		{
			attackModifier = this.GetComponent<PlayerStats> ().attack;
			defenseModifier = simTarget.GetComponent<PlayerStats> ().defense;
		}
		
		
		damageToDo = attackModifier - defenseModifier;
		
		
		if (damageToDo < 0)
		{
			damageToDo = 0;
		}
		
		damageToDo = damageToDo + this.GetComponent<Inventory> ().currentEquipped.attackMod;
		
		return damageToDo;

	}

	public void FindFirstNode()
	{
		GameObject nodeCheck = null;

		foreach (GameObject node in NodeDatabase.allNodes)
		{
			if(targetNode == null)
			{
				targetNode = node;
			}
			else
			{
				if(Vector3.Distance(this.transform.position, node.transform.position) < Vector3.Distance(this.transform.position, targetNode.transform.position))
				{
					targetNode = node;
					nodeCheck = node;
				}
			}
		}
		nodeCheck = targetNode;
		if(Vector3.Distance (targetNode.transform.position, targetPosition) > Vector3.Distance (this.transform.position, targetPosition))
		{
			foreach (GameObject node in targetNode.GetComponent<PathNode>().linkedNodes)
			{
				//if(Vector3.Distance (node.transform.position, targetPosition) < Vector3.Distance (this.transform.position, targetPosition))
				if(Vector3.Distance (node.transform.position, targetPosition) < Vector3.Distance (targetNode.transform.position, targetPosition))

				{
					targetNode = node;
				}
			}
			if(targetNode == nodeCheck && !Physics.Raycast(this.transform.position + new Vector3(0,0.7f,0), targetPosition - this.transform.position, Vector3.Distance(this.transform.position + new Vector3(0,1,0) , targetPosition + new Vector3(0,1,0)),layerMask ))
			{
				currentState = pathFindingState.movingToTarget;
			}
			if(Physics.Raycast(this.transform.position + new Vector3(0,0.7f,0), targetPosition - this.transform.position, Vector3.Distance(this.transform.position + new Vector3(0,1,0) , targetPosition + new Vector3(0,1,0)),layerMask ))
			{
				Debug.Log ("Hit");
			}

		}
		visitedNodes.Add(targetNode);


	}

	public void findNextNode()
	{
		GameObject oldNode = targetNode;//Saves the current node

		targetNode = null;
		foreach(GameObject node in oldNode.GetComponent<PathNode>().linkedNodes)//Looks at the nodes linked to the current node
		{
			if(targetNode == null)
			{
				bool visited = false;
				foreach(GameObject nodeCheck in visitedNodes)
				{
					if(node == nodeCheck)
					{
						visited = true;
					}
				}
				if(!visited)
				{
					targetNode = node;//if targetNode is null, and the node being check hasnt been visited, set the target node to it.
				}
			}

			else
			{
				if(Vector3.Distance(targetPosition, node.transform.position) < Vector3.Distance(targetPosition, targetNode.transform.position))//If this node brings it closer than the current one
				{
					bool visited = false;
					foreach(GameObject nodeCheck in visitedNodes)
					{
						if(node == nodeCheck)
						{
							visited = true;//Checks if the node has been visited
						}
					}
					if(!visited)
					{
						targetNode = node;//If it hasnt been visited, and brings it closer to the player, set as target node.
					}
				}
			}


		}
		visitedNodes.Add(targetNode); //Add to the visited list

		//If player is closer than any nodes in the path, and raycasting doesnt detect an obstacle blocking the direct path
		if(Vector3.Distance(targetPosition, this.transform.position) < Vector3.Distance(targetNode.transform.position, targetPosition) 
		   && !Physics.Raycast(this.transform.position + new Vector3(0,0.7f,0), targetPosition - this.transform.position, 
		                    Vector3.Distance(this.transform.position + new Vector3(0,1,0) , targetPosition + new Vector3(0,1,0)),layerMask))
		{
			currentState = pathFindingState.movingToTarget; //Move directly to target
		}

	}

	public IEnumerator rangeAttack()
	{
		yield return new WaitForSeconds(0.3f);
		
		GameObject stone = null;

		stone = Instantiate(Resources.Load("RockEnemy"), shootPoint.transform.position, Random.rotation) as GameObject;
		
		stone.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 20 + new Vector3(0,3,0);
		stone.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 50;
		
		yield return new WaitForSeconds(0.45f);
		
	}

	void AttackPlayer()
	{
		int attackModifier;
		int defenseModifier;
		int damageToDo;

		if (this.GetComponent<PlayerStats> ().job == "Mage")
		{
			attackModifier = this.GetComponent<PlayerStats>().magicAttack;
			defenseModifier = target.GetComponent<PlayerStats>().MagicDefense;
		}
		else
		{
			attackModifier = this.GetComponent<PlayerStats> ().attack;
			defenseModifier = target.GetComponent<PlayerStats> ().defense;
		}


		damageToDo = attackModifier - defenseModifier;


		if (damageToDo < 0)
		{
			damageToDo = 0;
		}

		
		damageToDo = damageToDo + this.GetComponent<Inventory> ().currentEquipped.attackMod;
		damageToDo = damageToDo + this.GetComponent<Inventory> ().currentEquipped.magicAttackMod;
		if(target.GetComponent<PlayerStats>().currHP - damageToDo < 0)
		{
			target.GetComponent<PlayerStats> ().currHP = 0;
			
		}
		else
		{
			target.GetComponent<PlayerStats> ().currHP -= damageToDo;
		}
	}

	void animationCheck()
	{
		
		if(currentAnimState == AnimationStates.Run)
		{
			this.GetComponent<Animator>().SetBool("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", true);
		}
		else if(currentAnimState == AnimationStates.Attack)
		{
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Attack", true);
		}
		else if(currentAnimState == AnimationStates.Ranged)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("rangedAttack", true);
		}
		else if(currentAnimState == AnimationStates.Magic)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool ("spellAttack", true);
		}
		else if(currentAnimState == AnimationStates.Death)
		{
			this.GetComponent<Animator>().SetBool ("Idle", false);
			this.GetComponent<Animator>().SetBool ("Death", true);
		}
		else
		{
			this.GetComponent<Animator>().SetBool("Idle", true);
			this.GetComponent<Animator>().SetBool ("Run", false);
			this.GetComponent<Animator>().SetBool("Attack",false);
			this.GetComponent<Animator>().SetBool ("rangedAttack", false);
			this.GetComponent<Animator>().SetBool ("spellAttack", false);
		}
	}

	void findTargetPlayer()
	{
		List<GameObject> possibleTargets = new List<GameObject> ();
		float distance = 10;
		
		
		foreach (GameObject player in PlayerDatabase.players) 
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) < 3 * GetComponent<Inventory>().currentEquipped.range)
			{
				if(Vector3.Angle (this.transform.forward, player.transform.position - this.transform.position) < 45)
				{
					possibleTargets.Add(player);
					player.GetComponent<Player_Controller>().selected = false;
				}
				else
				{
					player.GetComponent<Player_Controller>().selected = false;
				}
			}
			else
			{
				player.GetComponent<Player_Controller>().selected = false;
			}
		}
		if (possibleTargets.Count == 0) 
		{
			target = null;
		}
		foreach (GameObject player in possibleTargets) 
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) < distance)
			{
				target = player;
			}
		}
		if (target != null) 
		{
			target.GetComponent<Player_Controller> ().selected = true;
		}
	}
}
