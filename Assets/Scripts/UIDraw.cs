using UnityEngine;
using System.Collections;

public class UIDraw : MonoBehaviour {

	private GUISkin skin;
	public Texture2D uiTexture;
	private bool draggingItem;
	private Item draggedItem;
	private int prevIndex;

	void Start()
	{
		uiTexture = Resources.Load<Texture2D> ("UIBorder");
		skin = Resources.Load("InvSkin") as GUISkin;

	}
	
	void OnGUI()
	{

		GUI.depth = 2;
		Rect uiRect = new Rect (Screen.width * 0.85f, 0.0f, Screen.width * 0.15f, Screen.height);
		

		if(GameStateMachine.currentGameState != GameStateMachine.State.statScreen 
		   && GameStateMachine.currentGameState != GameStateMachine.State.paused 
		   && GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer
		   && GameStateMachine.currentGameState != GameStateMachine.State.gameOver)
		{
			GUI.DrawTexture (uiRect, uiTexture, ScaleMode.StretchToFill, false, 10.0f);
			StatDrawSingle ();
			drawFaces();
			InventoryDraw ();
			toolTipDraw();

			if(draggingItem)
			{
				GUI.DrawTexture (new Rect(Event.current.mousePosition.x - (Screen.width * 0.028f/2) , Event.current.mousePosition.y - (Screen.width * 0.028f/2),Screen.width * 0.028f, Screen.width * 0.028f),draggedItem.itemIcon);
			}
		}
		else if(GameStateMachine.currentMPGameState != GameStateMachine.MPState.statScreen && GameStateMachine.currentMPGameState != GameStateMachine.MPState.paused && GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			GUI.DrawTexture (uiRect, uiTexture, ScaleMode.StretchToFill, false, 10.0f);
			
			StatDrawMulti ();
			InventoryDraw ();
			toolTipDraw();

		}
	}
	void StatDrawSingle()
	{
		GUI.skin = skin;
		PlayerStats stats = null;
	//	GameObject Player= null;

		if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
		{
		//	Player = PlayerDatabase.getSelected();
			if(PlayerDatabase.getSelected() != null)
			{
				stats = PlayerDatabase.getSelected().GetComponent<PlayerStats>();
			}

		}
		else if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
		{
		//	Player = EnemyDatabase.getSelected();
			if(EnemyDatabase.getSelected() != null)
			{
				stats = EnemyDatabase.getSelected().GetComponent<PlayerStats>();
			}
		}
		if(stats != null)
		{
		
			Rect statRect = new Rect (Screen.width * 0.8625f, Screen.height * 0.43f,  Screen.width * 0.125f, Screen.height * 0.25f);
			GUI.skin.GetStyle("StatDraw").fontSize = (int) (Screen.width * 0.009f);
			GUI.Box(statRect, "Job: " + stats.job + "\nHP: " +stats.currHP + "/"+ stats.maxHP+"     Stamina: " +(int)stats.currStamina + "/"+ stats.maxStamina
			        +"\nAttack: " + stats.attack + "     Mag. Atk: " + stats.magicAttack + "\nDefence: " + stats.defense + "     Mag. Def: " + stats.MagicDefense
			        +"\nSTR: " + stats.str + "   DEX: " + stats.dex + "   VIT: " + stats.vit + "\nINT: " + stats.intelligence + "   MND: " + stats.mnd + "   SPD: " + stats.spd
			        +"\n\nLevel: " + stats.level + "\nExperience: " + stats.experience + "/100",skin.GetStyle("StatDraw"));
		}
        drawSelectedHealth();
        drawSelectedStamina();
		enemiesLeft ();
	}

	void StatDrawMulti()
	{
		GUI.skin = skin;
		PlayerStats stats = null;
		//	GameObject Player= null;
		
		if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
		{
			//	Player = PlayerDatabase.getSelected();
			stats = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>();
			
			
		}
		else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
		{
			//	Player = EnemyDatabase.getSelected();
			stats = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>();
		}
		
		
		Rect statRect = new Rect (Screen.width * 0.8625f, Screen.height * 0.43f,  Screen.width * 0.125f, Screen.height * 0.25f);
		Rect textRect = new Rect (Screen.width * 0.8650f, Screen.height * 0.435f,  Screen.width * 0.120f, Screen.height * 0.24f);
		GUI.skin.box.fontSize = 100;
		GUI.Box(statRect, "",skin.GetStyle("Slot"));
		GUI.skin.GetStyle("StatDraw").fontSize = (int) (Screen.width * 0.009f);
		GUI.Box(statRect, "Job: " + stats.job + "\nHP: " +stats.currHP + "/"+ stats.maxHP+"     Stamina: " +(int)stats.currStamina + "/"+ stats.maxStamina
		        +"\nAttack: " + stats.attack + "     Mag. Atk: " + stats.magicAttack + "\nDefence: " + stats.defense + "     Mag. Def: " + stats.MagicDefense
		        +"\nSTR: " + stats.str + "   DEX: " + stats.dex + "   VIT: " + stats.vit + "\nINT: " + stats.intelligence + "   MND: " + stats.mnd + "   SPD: " + stats.spd
		        +"\n\nLevel: " + stats.level + "\nExperience: " + stats.experience + "/100",skin.GetStyle("StatDraw"));
		
		drawSelectedHealth();
		drawSelectedStamina();
		playersLeft ();
	}

    void drawSelectedHealth()
    {
        int maxHP = 0;
        int currHP = 0;
        if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
        {
            if (GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
            {
                currHP = PlayerDatabase.getSelected().GetComponent<PlayerStats>().currHP;
                maxHP = PlayerDatabase.getSelected().GetComponent<PlayerStats>().maxHP;
            }
            else if (GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
            {
                if (EnemyDatabase.getSelected() != null)
                {
                    currHP = EnemyDatabase.getSelected().GetComponent<PlayerStats>().currHP;
                    maxHP = EnemyDatabase.getSelected().GetComponent<PlayerStats>().maxHP;
                }
            }
        }
        else if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
        {
            if (GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
            {
                currHP = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>().currHP;
                maxHP = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>().maxHP;
            }
            else if (GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
            {
                currHP = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>().currHP;
                maxHP = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>().maxHP;
            }
        }

        if (EnemyDatabase.getSelected() != null || GameStateMachine.currentGameState != GameStateMachine.State.enemyTurn || GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
        {
            float hpPercent = ((float)currHP / (float)maxHP) * 15.0f;
            Rect hpRect = new Rect(((float)(Screen.width - Screen.width * 0.13f)), Screen.height - Screen.height * 0.305f, hpPercent * 7.3f * Screen.width * 0.001f, Screen.height * 0.024f);
            Rect backRect = new Rect(((float)(Screen.width - Screen.width * 0.13f)), Screen.height - Screen.height * 0.305f, 15 * 7.3f * Screen.width * 0.001f, Screen.height * 0.024f);
            GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
            GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBar"));
        }
    }

    void drawSelectedStamina()
    {
        float currStamina = 0;
        float maxStamina = 0;
        if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
        {
            if (GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
            {
                currStamina = PlayerDatabase.getSelected().GetComponent<PlayerStats>().currStamina;
                maxStamina = PlayerDatabase.getSelected().GetComponent<PlayerStats>().maxStamina;
            }
            else if (GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
            {
                if (EnemyDatabase.getSelected() != null)
                {
                    currStamina = EnemyDatabase.getSelected().GetComponent<PlayerStats>().currStamina;
                    maxStamina = EnemyDatabase.getSelected().GetComponent<PlayerStats>().maxStamina;
                }
            }
        }
        else if (GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
        {
            if (GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
            {
                currStamina = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>().currStamina;
                maxStamina = PlayerDatabase.getPlayer1Selected().GetComponent<PlayerStats>().maxStamina;
            }
            else if (GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
            {
                currStamina = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>().currStamina;
                maxStamina = PlayerDatabase.getPlayer2Selected().GetComponent<PlayerStats>().maxStamina;
            }
        }
        if (EnemyDatabase.getSelected() != null || GameStateMachine.currentGameState != GameStateMachine.State.enemyTurn || GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
        {
            float staminaPercent = ((float)currStamina / (float)maxStamina) * 15.0f;
            Rect staminaRect = new Rect(((float)(Screen.width - Screen.width * 0.13f)), Screen.height - Screen.height * 0.275f, staminaPercent * 7.3f * Screen.width * 0.001f, Screen.height * 0.024f);
            Rect backRect = new Rect(((float)(Screen.width - Screen.width * 0.13f)), Screen.height - Screen.height * 0.275f, 15 * 7.3f * Screen.width * 0.001f, Screen.height * 0.024f);
            GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
            GUI.DrawTexture(staminaRect, Resources.Load<Texture2D>("StaminaBar"));
        }
    }




	void InventoryDraw()
	{
		Inventory inventory = null;

		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				inventory = PlayerDatabase.getSelected().GetComponent<Inventory>();
			}
			else if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
			{
				if(EnemyDatabase.getSelected() != null)
				{
					inventory = EnemyDatabase.getSelected().GetComponent<Inventory>();
				}
			}
		}
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				inventory = PlayerDatabase.getPlayer1Selected().GetComponent<Inventory>();
			}
			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				inventory = PlayerDatabase.getPlayer2Selected().GetComponent<Inventory>();
			}
		}

		if(inventory != null)
		{
			int a = 0;
			for (int i = 0; i < 5; i++) 
			{
				if(inventory.inventory[i].currDurability == 0)
				{
					inventory.inventory[i] = new Item();
				}
				//rect (x pos, y pos, x size, y size)
				
				Rect slotRect = new Rect (Screen.width * 0.8625f, i * Screen.height * 0.080f + Screen.height * 0.022f, Screen.width * 0.125f, Screen.height * 0.078f);
				Rect highLightRect = new Rect (Screen.width * 0.8615f, i * Screen.height * 0.080f + Screen.height * 0.0205f, Screen.width * 0.127f, Screen.height * 0.082f); 
				Rect iconRect = new Rect (Screen.width * 0.868f, i * Screen.height * 0.080f + Screen.height * 0.033f, Screen.width * 0.028f, Screen.width * 0.028f);
				Rect duraRect = new Rect (Screen.width * 0.92f, i * Screen.height * 0.080f + Screen.height * 0.065f, Screen.width * 0.125f, Screen.height * 0.078f);
				Rect TextRect = new Rect (Screen.width * 0.9f, i * Screen.height * 0.080f + Screen.height * 0.038f, 160, 50);
				Rect equippedRect = new Rect(Screen.width * 0.9f, i * Screen.height * 0.080f + Screen.height * 0.060f, Screen.width * 0.125f, Screen.height * 0.078f);
				GUI.skin.label.normal.textColor = Color.black;
				GUI.skin.label.fontSize = (int)(Screen.width * 0.013); 

				if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer || GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
				{
					if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn 
					   || GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn 
					   || GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
					{
						if(i == inventory.highlightedItem)
						{
							GUI.DrawTexture(highLightRect, Resources.Load<Texture2D>("SelectionBack"));
						}
					}
				}
				GUI.Box (slotRect, "", skin.GetStyle ("Slot"));
				GUI.Label (TextRect, inventory.inventory[i].itemName);

				inventory.slots [a] = inventory.inventory [a];
				if (inventory.slots [i].itemIcon != null) 
				{
					GUI.DrawTexture (iconRect, inventory.inventory [i].itemIcon);
					if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
					{
						if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
						{
							if(slotRect.Contains(Event.current.mousePosition))
							{
								inventory.highlightedItem = i;
								if(Event.current.button == 0 && Event.current.type == EventType.mouseDrag && !draggingItem)
								{
									draggingItem = true;
									inventory.isBeingDragged = true;
									prevIndex = i;
									draggedItem = inventory.slots[i];
									inventory.inventory[i] = new Item();
								}
								if(Event.current.type == EventType.mouseUp && draggingItem)
								{
									if(prevIndex == inventory.equippedIndex)
									{
										inventory.equippedIndex = i;	
									}
									inventory.inventory[prevIndex] = inventory.inventory[i];
									inventory.inventory [i] = draggedItem;
									inventory.isBeingDragged = false;
									draggingItem = false;
									draggedItem = null;
								}
								if(Event.current.button == 1 && Event.current.type == EventType.mouseUp)
								{
									if(inventory.inventory[i].itemType == Item.ItemType.Weapon)
									{
										inventory.equippedIndex = i;
									}
									else if(inventory.inventory[i].itemType == Item.ItemType.Consumable)
									{
										inventory.UseItem(inventory.inventory[i].itemID);
										inventory.inventory[i] = new Item();
									}
								}
							}
							else if(Event.current.type == EventType.mouseUp && draggingItem && i == 4)
							{
								inventory.inventory[prevIndex] = draggedItem;
								inventory.isBeingDragged = false;
								draggingItem = false;
								draggedItem = null;
							}
						}
					}
				}
				else
				{
					if(slotRect.Contains(Event.current.mousePosition))
					{
						if(Event.current.type == EventType.mouseUp && draggingItem)
						{
							if(prevIndex == inventory.equippedIndex)
							{
								inventory.equippedIndex = i;	
							}
							inventory.inventory [i] = draggedItem;
							inventory.isBeingDragged = false;
							draggingItem = false;
							draggedItem = null;
						}
					}
					else if(Event.current.type == EventType.mouseUp && draggingItem && i == 4)
					{

						inventory.inventory[prevIndex] = draggedItem;
						inventory.isBeingDragged = false;
						draggingItem = false;
						draggedItem = null;
					}
				}
				GUI.skin.label.normal.textColor = Color.white;
				GUI.skin.label.fontSize = (int)(Screen.width * 0.009); 
						
				if (inventory.inventory [i].maxDurability != -1) 
				{
					GUI.Label (duraRect, inventory.inventory [i].currDurability.ToString () + "/" + inventory.inventory [i].maxDurability.ToString ());

					if(inventory.inventory[i] == inventory.currentEquipped)
					{
						GUI.skin.label.fontSize = (int)(Screen.width * 0.012); 
						GUI.Label(equippedRect, "E");
					}
				}
						
				a++;

			}
		}
	}

	void toolTipDraw()
	{
		Inventory inventory = null;
		
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				inventory = PlayerDatabase.getSelected().GetComponent<Inventory>();
			}
			else if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
			{
				if(EnemyDatabase.getSelected() != null)
				{
					inventory = EnemyDatabase.getSelected().GetComponent<Inventory>();
				}
			}
		}
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn)
			{
				inventory = PlayerDatabase.getPlayer1Selected().GetComponent<Inventory>();
			}
			else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
			{
				inventory = PlayerDatabase.getPlayer2Selected().GetComponent<Inventory>();
			}
		}

		for (int i = 0; i < 5; i++) 
		{
			Rect slotRect = new Rect (Screen.width * 0.8625f, i * Screen.height * 0.080f + Screen.height * 0.022f, Screen.width * 0.125f, Screen.height * 0.078f);

			if(slotRect.Contains(Event.current.mousePosition) && inventory.inventory[i].itemDesc != null)
			{
				GUI.depth = 1;
				GUI.skin.box.fontSize = (int)(Screen.width * 0.012); //sets font size to be based off resolution
				Item item = inventory.slots[i];
				if(item.currDurability > 0)//checks to see if it is a weapon or not
				{
					//Draws a box based off mouse position
					GUI.Box (new Rect(Event.current.mousePosition.x - Screen.width * 0.15f, Event.current.mousePosition.y, 
					        Screen.width * 0.15f, Screen.height* 0.133f),
				         	item.itemDesc +"\n Attack + " +item.attackMod + "\n Magic + "+ item.magicAttackMod + "\n Heal + " +item.healMod);
				}
				else
				{
					GUI.Box (new Rect(Event.current.mousePosition.x - Screen.width * 0.15f, Event.current.mousePosition.y, Screen.width * 0.15f, Screen.height* 0.133f),
					         item.itemDesc);
				}
			}
		}

	}

	void drawFaces()
	{
		int maxHP = 0;
		int currHP = 0;
		float maxStamina = 0;
		float currStamina = 0;
		int count = 0;
		foreach (GameObject player in PlayerDatabase.players)
		{
			currHP = player.GetComponent<PlayerStats>().currHP;
			maxHP = player.GetComponent<PlayerStats>().maxHP;
			currStamina = player.GetComponent<PlayerStats>().currStamina;
			maxStamina = player.GetComponent<PlayerStats>().maxStamina;

			float hpPercent = ((float)currHP / (float)maxHP) * 15.0f;
			float staminaPercent = ((float)currStamina / (float)maxStamina) * 15.0f;
			Rect hpRect = new Rect(((Screen.width - Screen.width * 0.93f + (count * Screen.width * 0.15f))), Screen.height - Screen.height * 0.96f, hpPercent * 4.0f * Screen.width * 0.001f, Screen.height * 0.024f);
			Rect backRect = new Rect(((Screen.width - Screen.width * 0.93f + (count * Screen.width * 0.15f))), Screen.height - Screen.height * 0.96f, 15 * 4.0f * Screen.width * 0.001f, Screen.height * 0.024f);
			Rect staminaRect= new Rect(((Screen.width - Screen.width * 0.93f + (count * Screen.width * 0.15f))), Screen.height - Screen.height * 0.93f, staminaPercent * 4.0f * Screen.width * 0.001f, Screen.height * 0.024f);
			Rect staminaBackRect = new Rect(((Screen.width - Screen.width * 0.93f + (count * Screen.width * 0.15f))), Screen.height - Screen.height * 0.93f, 15 * 4.0f * Screen.width * 0.001f, Screen.height * 0.024f);
		
			GUI.DrawTexture(backRect, Resources.Load<Texture2D>("BackBar"));
			GUI.DrawTexture(hpRect, Resources.Load<Texture2D>("HpBar"));
			GUI.DrawTexture(staminaBackRect, Resources.Load<Texture2D>("BackBar"));
			GUI.DrawTexture(staminaRect, Resources.Load<Texture2D>("StaminaBar"));


			Rect imageRect = new Rect (count * Screen.width * 0.15f + Screen.width * 0.01f, Screen.height * 0.015f, Screen.width * 0.05f, Screen.height * 0.1f);

			if(player.GetComponent<Player_Controller>().selected == true && GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
			{
				Rect selectedRect = new Rect (count * Screen.width * 0.15f + Screen.width * 0.01f - Screen.width * 0.0025f, Screen.height * 0.015f - Screen.height * 0.00275f, Screen.width * 0.055f, Screen.height * 0.108f);
				GUI.DrawTexture (selectedRect, Resources.Load<Texture2D>("SelectionBack"));
			}
			GUI.DrawTexture (imageRect, Resources.Load<Texture2D> (player.GetComponent<PlayerStats>().job));

			count++;
		}
	}

	void enemiesLeft()
	{
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.020);
		Rect textRect = new Rect (Screen.width * 0.8625f, Screen.height * 0.765f, Screen.width * 0.125f, Screen.height * 0.08f);
		Rect numberRect = new Rect (Screen.width * 0.9125f, Screen.height * 0.83f, Screen.width * 0.2f, Screen.height * 0.1f);
		GUI.DrawTexture (textRect, Resources.Load<Texture2D> ("enemiesLeft"));
		GUI.skin.label.fontSize = (int)(Screen.width * 0.030);
		GUI.Label (numberRect, EnemyDatabase.enemies.Count.ToString());
	}

	void playersLeft()
	{	
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.020);
		Rect textRect = new Rect (Screen.width * 0.8625f, Screen.height * 0.765f, Screen.width * 0.125f, Screen.height * 0.18f);
		Rect numberRect = new Rect (Screen.width * 0.9125f, Screen.height * 0.83f, Screen.width * 0.2f, Screen.height * 0.1f);
		GUI.Label (textRect, " Players Left\n  Blue      Red\n    " + PlayerDatabase.bluePlayers.Count.ToString() +"          " + PlayerDatabase.redPlayers.Count.ToString());
		GUI.skin.label.fontSize = (int)(Screen.width * 0.030);

	}
}
