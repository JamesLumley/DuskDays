using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour{

	public List<Item> oldInventory = new List<Item>(5);
	public List<Item> inventory = new List<Item>();
	public List<Item> slots = new List<Item> ();
	private int _equippedIndex;
	private int _highlightedItem;
	private Item _currentEquipped;
	private bool _isBeingDragged = false;
	private float itemChangeTimer = 0;
	public bool isFull = false;

	// Use this for initialization
	void Awake()
	{
		if(inventory.Count != 5)
		{
			for (int i = 0; i < 5; i++) 
			{
				slots.Add (new Item());
				inventory.Add (new Item());
			}
		}
		if(Application.loadedLevel <= 2 || Application.loadedLevelName == "MultiplayerArena")
		{
			switch(this.GetComponent<PlayerStats>().job)
			{
			case "Swordsman":
				AddItem(0);
				break;
			case "Knight":
				AddItem(4);
				break;
			case "Mage":
				AddItem (2);
				break;
			case "Healer":
				AddItem(3);
				break;
			case "Ranger":
				AddItem (1);
				break;
			}
		}
		else if(Application.loadedLevel > 2)
		{
			switch(this.GetComponent<PlayerStats>().job)
			{
			case "Swordsman":
				AddItem(5);
				break;
			case "Knight":
				AddItem(9);
				break;
			case "Mage":
				AddItem (7);
				break;
			case "Healer":
				AddItem(8);
				break;
			case "Ranger":
				AddItem (6);
				break;
			}
		}
		AddItem (100);



		equippedIndex = 0;
		currentEquipped = inventory [equippedIndex];
	}

	void OnLevelWasLoaded()
	{
		oldInventory.Clear ();
		for(int i = 0; i < 5; i++)
		{
			oldInventory.Add(inventory[i]);
		}
	}
	
	void Update()
	{
		foreach (Item item in inventory)
		{
			if(item.itemIcon == null)
			{
				isFull = false;
				break;
			}
			else
			{
				isFull = true;
			}
		}
		itemChangeTimer -= Time.deltaTime;

		currentEquipped = inventory [equippedIndex];
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				if(itemChangeTimer <= 0)
				{
					if(Input.GetAxis("ChangeItem") > 0)
					{
						highlightedItem--;
						itemChangeTimer = 0.3f;

						if(highlightedItem < 0)
						{
							highlightedItem = 4;
						}
					}
					else if(Input.GetAxis ("ChangeItem") < 0)
					{
						highlightedItem++;
						itemChangeTimer = 0.3f;
						if(highlightedItem > 4)
						{
							highlightedItem = 0;
						}
					}
				}
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{

				if(itemChangeTimer <= 0)
				{
					if(Input.GetAxis("ChangeItemPlayer1") > 0)
					{

						highlightedItem--;
						itemChangeTimer = 0.3f;
						
						if(highlightedItem < 0)
						{
							highlightedItem = 4;
						}
					}
					else if(Input.GetAxis ("ChangeItemPlayer1") < 0)
					{
						highlightedItem++;
						itemChangeTimer = 0.3f;
						if(highlightedItem > 4)
						{
							highlightedItem = 0;
						}
					}
				}
			}
			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				
				if(itemChangeTimer <= 0)
				{
					if(Input.GetAxis("ChangeItemPlayer2") > 0)
					{

						highlightedItem--;
						itemChangeTimer = 0.3f;
						
						if(highlightedItem < 0)
						{
							highlightedItem = 4;
						}
					}
					else if(Input.GetAxis ("ChangeItemPlayer2") < 0)
					{
						highlightedItem++;
						itemChangeTimer = 0.3f;
						if(highlightedItem > 4)
						{
							highlightedItem = 0;
						}
					}
				}
			}
		}
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
		{
			if(this.tag == "Player")
			{
				if (this.GetComponent<Player_Controller> ().selected == true) 
				{
					if(Input.GetButtonDown("UseItem"))
					{
						if(inventory[highlightedItem].itemType == Item.ItemType.Weapon && inventory[highlightedItem].maxDurability > 0)
						{
							equippedIndex = highlightedItem;
						}
						else if(inventory[highlightedItem].itemType == Item.ItemType.Consumable)
						{
							UseItem(inventory[highlightedItem].itemID);
							inventory[highlightedItem] = new Item();
						}
					}
				}
			}
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				if(this.GetComponent<Player_Controller>().selected == true)
				{
					if(Input.GetButtonDown("UseItemPlayer1"))
					{
						if(inventory[highlightedItem].itemType == Item.ItemType.Weapon && inventory[highlightedItem].maxDurability > 0)
						{
							equippedIndex = highlightedItem;
						}
						else if(inventory[highlightedItem].itemType == Item.ItemType.Consumable)
						{
							UseItem(inventory[highlightedItem].itemID);
							inventory[highlightedItem] = new Item();
						}
					}
				}
			}

			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				if(this.GetComponent<Player_Controller>().selected == true)
				{
					if(Input.GetButtonDown("UseItemPlayer2"))
					{
						if(inventory[highlightedItem].itemType == Item.ItemType.Weapon && inventory[highlightedItem].maxDurability > 0)
						{
							equippedIndex = highlightedItem;
						}
						else if(inventory[highlightedItem].itemType == Item.ItemType.Consumable)
						{
							UseItem(inventory[highlightedItem].itemID);
							inventory[highlightedItem] = new Item();
						}
					}
				}
			}
		}
	
	}

	public Inventory()
	{

	}

	public int highlightedItem 
	{
		get;
		set;
	}

	public bool isBeingDragged
	{
		get{ return _isBeingDragged;}
		set{_isBeingDragged = value;}
	}

	public Item currentEquipped 
	{
		get;
		set;
	}

	public int equippedIndex 
	{
		get;
		set;
	}

    public void AddItem(int id)
	{
		for(int i = 0; i < inventory.Count; i++)
		{
			if(inventory[i].itemName == null)
			{
				inventory[i] = new Item(id);
				break;
			}
		}
	}

	public void UseItem(int id)
	{
		switch(id)
		{
		case 100:
			if(this.GetComponent<PlayerStats>().currHP + 20 >= this.GetComponent<PlayerStats>().maxHP)
			{
				this.GetComponent<PlayerStats>().currHP = this.GetComponent<PlayerStats>().maxHP;
			}
			else
			{
				this.GetComponent<PlayerStats>().currHP += 20;
			}
			break;
		}
	}
}
