using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;

public class PauseScreen : MonoBehaviour {
	public GameStateMachine.State oldState;
	public GameStateMachine.MPState oldMPState;
	public GameObject pauseScreen;
	public GameObject statScreen;
	public GameObject gameOverScreen;
	public GameObject selector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("Pause"))
		{
			if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
			{
				if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn || GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
				{
					if(GameStateMachine.currentGameState != GameStateMachine.State.paused)
					{
						oldState = GameStateMachine.currentGameState;
						selector.GetComponent<MenuControllerJoystick>().oldState = oldState;
						GameStateMachine.currentGameState = GameStateMachine.State.paused;
					}
				}
				else if(GameStateMachine.currentGameState == GameStateMachine.State.paused)
				{
					GameStateMachine.currentGameState = oldState;
				}
			}
			else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
			{
				if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.player1Turn || GameStateMachine.currentMPGameState == GameStateMachine.MPState.player2Turn)
				{
					if(GameStateMachine.currentMPGameState != GameStateMachine.MPState.paused)
					{
						oldMPState = GameStateMachine.currentMPGameState;
						selector.GetComponent<MenuControllerJoystick>().oldMPState = oldMPState;
						GameStateMachine.currentMPGameState = GameStateMachine.MPState.paused;
					}
				}
				else if(GameStateMachine.currentMPGameState == GameStateMachine.MPState.paused)
				{
					GameStateMachine.currentMPGameState = oldMPState;
				}
			}

		}

		if(GameStateMachine.currentGameState == GameStateMachine.State.paused 
		   || GameStateMachine.currentMPGameState == GameStateMachine.MPState.paused)
		{
			pauseScreen.SetActive(true);
		}
		else
		{
			pauseScreen.SetActive(false);
		}
		if(GameStateMachine.currentGameState == GameStateMachine.State.statScreen || GameStateMachine.currentMPGameState == GameStateMachine.MPState.statScreen)
		{
			statScreen.SetActive(true);
		}
		else
		{
			statScreen.SetActive(false);
		}

		if(GameStateMachine.currentGameState == GameStateMachine.State.gameOver && GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			gameOverScreen.SetActive(true);
		}
		else
		{
			if(gameOverScreen != null)
			{
				gameOverScreen.SetActive(false);
			}
		}

		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer && GameStateMachine.currentGameState == GameStateMachine.State.gameOver)
		{
			if(Input.GetButtonDown("Accept") || Input.GetKeyDown(KeyCode.Return))
			{
				reloadLevel();
			}
		}
	}

	public void returnButton()
	{
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.singlePlayer)
		{
			GameStateMachine.currentGameState = oldState;
		}
		else if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			GameStateMachine.currentMPGameState = oldMPState;
		}
	}

	public void exitButton()
	{
		Time.timeScale = 1;
		GameStateMachine.currentGameState = GameStateMachine.State.playerTurn;
		GameStateMachine.currentMPGameState = GameStateMachine.MPState.player1Turn;
		for(int i = 0; i < PlayerDatabase.players.Count; i++)
		{
			Destroy(PlayerDatabase.players[i]);
		}
		Score.collectables = 0;
		Score.turnsTaken = 0;
		PlayerDatabase.deleteLists ();
		EnemyDatabase.deleteList ();
		NodeDatabase.allNodes.Clear ();
		GameStateMachine.currentGameMode = GameStateMachine.GameMode.menu;
		Application.LoadLevel (0);
	}


	public void reloadLevel()
	{
		Score.currentLevel = Application.loadedLevel;
		foreach(GameObject player in PlayerDatabase.players)
		{
			player.SetActive(true);
			for(int i = 0; i < 5; i++)
			{
				player.GetComponent<Inventory>().inventory[i] = player.GetComponent<Inventory>().oldInventory[i];
			}
			player.GetComponent<PlayerStats>().reloadStats();

		}
		Score.collectables = 0;
		Score.turnsTaken = 0;
		PlayerDatabase.deleteLists();
		EnemyDatabase.deleteList();
		NodeDatabase.allNodes.Clear();
		if(Application.loadedLevel == 1 || Application.loadedLevelName == "Level00")
		{
			Application.LoadLevel("Level00");
		}
		else
		{
			Application.LoadLevel (Score.currentLevel);
		}
	}

	public void nextLevel()
	{
		if(Application.loadedLevelName == "Level00")
		{
			Score.currentLevel = 1;
		}
		else
		{
			Score.currentLevel = Application.loadedLevel;
		}

		Score.currentLevel++;
		foreach(GameObject player in PlayerDatabase.players)
		{
			player.SetActive(true);
		}
		PlayerDatabase.deleteLists();
		EnemyDatabase.deleteList();
		NodeDatabase.allNodes.Clear();
	
		Application.LoadLevel (Score.currentLevel);
	}


	
	public void saveGame()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		PlayerData data = new PlayerData ();
		data.level = Application.loadedLevel;

		foreach(GameObject player in PlayerDatabase.players)
		{
			if(player.GetComponent<PlayerStats>().job == "Swordsman")
			{
				data.swordsmanInventory.Add (player.GetComponent<Inventory>().inventory[0].itemID);
				data.swordsmanInventory.Add (player.GetComponent<Inventory>().inventory[1].itemID);
				data.swordsmanInventory.Add (player.GetComponent<Inventory>().inventory[2].itemID);
				data.swordsmanInventory.Add (player.GetComponent<Inventory>().inventory[3].itemID);
				data.swordsmanInventory.Add (player.GetComponent<Inventory>().inventory[4].itemID);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldAttack);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldMagicAttack);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldHealPower);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldDefense);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldMagicDefense);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldStr);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldDex);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldIntelligence);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldVit);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldMnd);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldSpd);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldMaxHP);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldMaxStamina);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldLevel);
				data.swordsmanStats.Add(player.GetComponent<PlayerStats>().oldExperience);
			}
			else if(player.GetComponent<PlayerStats>().job == "Knight")
			{
				data.knightInventory.Add (player.GetComponent<Inventory>().inventory[0].itemID);
				data.knightInventory.Add (player.GetComponent<Inventory>().inventory[1].itemID);
				data.knightInventory.Add (player.GetComponent<Inventory>().inventory[2].itemID);
				data.knightInventory.Add (player.GetComponent<Inventory>().inventory[3].itemID);
				data.knightInventory.Add (player.GetComponent<Inventory>().inventory[4].itemID);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldAttack);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldMagicAttack);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldHealPower);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldDefense);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldMagicDefense);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldStr);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldDex);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldIntelligence);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldVit);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldMnd);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldSpd);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldMaxHP);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldMaxStamina);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldLevel);
				data.knightStats.Add(player.GetComponent<PlayerStats>().oldExperience);
			}
			else if(player.GetComponent<PlayerStats>().job == "Ranger")
			{
				data.rangerInventory.Add (player.GetComponent<Inventory>().inventory[0].itemID);
				data.rangerInventory.Add (player.GetComponent<Inventory>().inventory[1].itemID);
				data.rangerInventory.Add (player.GetComponent<Inventory>().inventory[2].itemID);
				data.rangerInventory.Add (player.GetComponent<Inventory>().inventory[3].itemID);
				data.rangerInventory.Add (player.GetComponent<Inventory>().inventory[4].itemID);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldAttack);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldMagicAttack);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldHealPower);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldDefense);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldMagicDefense);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldStr);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldDex);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldIntelligence);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldVit);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldMnd);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldSpd);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldMaxHP);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldMaxStamina);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldLevel);
				data.rangerStats.Add(player.GetComponent<PlayerStats>().oldExperience);
			}
			else if(player.GetComponent<PlayerStats>().job == "Mage")
			{
				data.mageInventory.Add (player.GetComponent<Inventory>().inventory[0].itemID);
				data.mageInventory.Add (player.GetComponent<Inventory>().inventory[1].itemID);
				data.mageInventory.Add (player.GetComponent<Inventory>().inventory[2].itemID);
				data.mageInventory.Add (player.GetComponent<Inventory>().inventory[3].itemID);
				data.mageInventory.Add (player.GetComponent<Inventory>().inventory[4].itemID);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldAttack);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldMagicAttack);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldHealPower);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldDefense);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldMagicDefense);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldStr);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldDex);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldIntelligence);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldVit);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldMnd);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldSpd);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldMaxHP);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldMaxStamina);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldLevel);
				data.mageStats.Add(player.GetComponent<PlayerStats>().oldExperience);
			}
			else if(player.GetComponent<PlayerStats>().job == "Healer")
			{
				data.healerInventory.Add (player.GetComponent<Inventory>().inventory[0].itemID);
				data.healerInventory.Add (player.GetComponent<Inventory>().inventory[1].itemID);
				data.healerInventory.Add (player.GetComponent<Inventory>().inventory[2].itemID);
				data.healerInventory.Add (player.GetComponent<Inventory>().inventory[3].itemID);
				data.healerInventory.Add (player.GetComponent<Inventory>().inventory[4].itemID);				
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldAttack);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldMagicAttack);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldHealPower);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldDefense);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldMagicDefense);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldStr);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldDex);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldIntelligence);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldVit);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldMnd);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldSpd);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldMaxHP);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldMaxStamina);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldLevel);
				data.healerStats.Add(player.GetComponent<PlayerStats>().oldExperience);
			}
		}
		bf.Serialize (file, data);
		file.Close ();
	}
	
	public void loadGame()
	{
		if(File.Exists(Application.persistentDataPath+"/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath+"/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();
			foreach(GameObject player in PlayerDatabase.players)
			{
				player.SetActive(true);
			}
			EnemyDatabase.deleteList();
			PlayerDatabase.deleteLists();
			GameStateMachine.currentGameState = GameStateMachine.State.playerTurn; 
			if(data.level == 1 || data.level == 5)
			{
				Application.LoadLevel("Level00");
			}
			else 
			{
				Application.LoadLevel(data.level);
			}
			Score.collectables = 0;
			Score.turnsTaken = 0;
			PlayerDatabase.initSingle();
			PlayerDatabase.selectedID = 0;

			foreach(GameObject player in PlayerDatabase.players)
			{
				if(player.GetComponent<PlayerStats>().job == "Swordsman")
				{
					player.GetComponent<Inventory>().inventory[0] = new Item(data.swordsmanInventory[0]);
					player.GetComponent<Inventory>().inventory[1] = new Item(data.swordsmanInventory[1]);
					player.GetComponent<Inventory>().inventory[2] = new Item(data.swordsmanInventory[2]);
					player.GetComponent<Inventory>().inventory[3] = new Item(data.swordsmanInventory[3]);
					player.GetComponent<Inventory>().inventory[4] = new Item(data.swordsmanInventory[4]);
					player.GetComponent<PlayerStats>().attack = data.swordsmanStats[0];
					player.GetComponent<PlayerStats>().magicAttack = data.swordsmanStats[1];
					player.GetComponent<PlayerStats>().healPower = data.swordsmanStats[2];
					player.GetComponent<PlayerStats>().defense = data.swordsmanStats[3];
					player.GetComponent<PlayerStats>().MagicDefense = data.swordsmanStats[4];
					player.GetComponent<PlayerStats>().str = data.swordsmanStats[5];
					player.GetComponent<PlayerStats>().dex = data.swordsmanStats[6];
					player.GetComponent<PlayerStats>().intelligence = data.swordsmanStats[7];
					player.GetComponent<PlayerStats>().vit = data.swordsmanStats[8];
					player.GetComponent<PlayerStats>().mnd = data.swordsmanStats[9];
					player.GetComponent<PlayerStats>().spd = data.swordsmanStats[10];
					player.GetComponent<PlayerStats>().maxHP = data.swordsmanStats[11];
					player.GetComponent<PlayerStats>().maxStamina = data.swordsmanStats[12];
					player.GetComponent<PlayerStats>().level = data.swordsmanStats[13];
					player.GetComponent<PlayerStats>().experience = data.swordsmanStats[14];

				}
				else if(player.GetComponent<PlayerStats>().job == "Knight")
				{
					player.GetComponent<Inventory>().inventory[0] = new Item(data.knightInventory[0]);
					player.GetComponent<Inventory>().inventory[1] = new Item(data.knightInventory[1]);
					player.GetComponent<Inventory>().inventory[2] = new Item(data.knightInventory[2]);
					player.GetComponent<Inventory>().inventory[3] = new Item(data.knightInventory[3]);
					player.GetComponent<Inventory>().inventory[4] = new Item(data.knightInventory[4]);
					player.GetComponent<PlayerStats>().attack = data.knightStats[0];
					player.GetComponent<PlayerStats>().magicAttack = data.knightStats[1];
					player.GetComponent<PlayerStats>().healPower = data.knightStats[2];
					player.GetComponent<PlayerStats>().defense = data.knightStats[3];
					player.GetComponent<PlayerStats>().MagicDefense = data.knightStats[4];
					player.GetComponent<PlayerStats>().str = data.knightStats[5];
					player.GetComponent<PlayerStats>().dex = data.knightStats[6];
					player.GetComponent<PlayerStats>().intelligence = data.knightStats[7];
					player.GetComponent<PlayerStats>().vit = data.knightStats[8];
					player.GetComponent<PlayerStats>().mnd = data.knightStats[9];
					player.GetComponent<PlayerStats>().spd = data.knightStats[10];
					player.GetComponent<PlayerStats>().maxHP = data.knightStats[11];
					player.GetComponent<PlayerStats>().maxStamina = data.knightStats[12];
					player.GetComponent<PlayerStats>().level = data.knightStats[13];
					player.GetComponent<PlayerStats>().experience = data.knightStats[14];
				}
				else if(player.GetComponent<PlayerStats>().job == "Ranger")
				{
					player.GetComponent<Inventory>().inventory[0] = new Item(data.rangerInventory[0]);
					player.GetComponent<Inventory>().inventory[1] = new Item(data.rangerInventory[1]);
					player.GetComponent<Inventory>().inventory[2] = new Item(data.rangerInventory[2]);
					player.GetComponent<Inventory>().inventory[3] = new Item(data.rangerInventory[3]);
					player.GetComponent<Inventory>().inventory[4] = new Item(data.rangerInventory[4]);
					player.GetComponent<PlayerStats>().attack = data.rangerStats[0];
					player.GetComponent<PlayerStats>().magicAttack = data.rangerStats[1];
					player.GetComponent<PlayerStats>().healPower = data.rangerStats[2];
					player.GetComponent<PlayerStats>().defense = data.rangerStats[3];
					player.GetComponent<PlayerStats>().MagicDefense = data.rangerStats[4];
					player.GetComponent<PlayerStats>().str = data.rangerStats[5];
					player.GetComponent<PlayerStats>().dex = data.rangerStats[6];
					player.GetComponent<PlayerStats>().intelligence = data.rangerStats[7];
					player.GetComponent<PlayerStats>().vit = data.rangerStats[8];
					player.GetComponent<PlayerStats>().mnd = data.rangerStats[9];
					player.GetComponent<PlayerStats>().spd = data.rangerStats[10];
					player.GetComponent<PlayerStats>().maxHP = data.rangerStats[11];
					player.GetComponent<PlayerStats>().maxStamina = data.rangerStats[12];
					player.GetComponent<PlayerStats>().level = data.rangerStats[13];
					player.GetComponent<PlayerStats>().experience = data.rangerStats[14];
				}
				else if(player.GetComponent<PlayerStats>().job == "Mage")
				{
					player.GetComponent<Inventory>().inventory[0] = new Item(data.mageInventory[0]);
					player.GetComponent<Inventory>().inventory[1] = new Item(data.mageInventory[1]);
					player.GetComponent<Inventory>().inventory[2] = new Item(data.mageInventory[2]);
					player.GetComponent<Inventory>().inventory[3] = new Item(data.mageInventory[3]);
					player.GetComponent<Inventory>().inventory[4] = new Item(data.mageInventory[4]);
					player.GetComponent<PlayerStats>().attack = data.mageStats[0];
					player.GetComponent<PlayerStats>().magicAttack = data.mageStats[1];
					player.GetComponent<PlayerStats>().healPower = data.mageStats[2];
					player.GetComponent<PlayerStats>().defense = data.mageStats[3];
					player.GetComponent<PlayerStats>().MagicDefense = data.mageStats[4];
					player.GetComponent<PlayerStats>().str = data.mageStats[5];
					player.GetComponent<PlayerStats>().dex = data.mageStats[6];
					player.GetComponent<PlayerStats>().intelligence = data.mageStats[7];
					player.GetComponent<PlayerStats>().vit = data.mageStats[8];
					player.GetComponent<PlayerStats>().mnd = data.mageStats[9];
					player.GetComponent<PlayerStats>().spd = data.mageStats[10];
					player.GetComponent<PlayerStats>().maxHP = data.mageStats[11];
					player.GetComponent<PlayerStats>().maxStamina = data.mageStats[12];
					player.GetComponent<PlayerStats>().level = data.mageStats[13];
					player.GetComponent<PlayerStats>().experience = data.mageStats[14];
				}
				else if(player.GetComponent<PlayerStats>().job == "Healer")
				{
					player.GetComponent<Inventory>().inventory[0] = new Item(data.healerInventory[0]);
					player.GetComponent<Inventory>().inventory[1] = new Item(data.healerInventory[1]);
					player.GetComponent<Inventory>().inventory[2] = new Item(data.healerInventory[2]);
					player.GetComponent<Inventory>().inventory[3] = new Item(data.healerInventory[3]);
					player.GetComponent<Inventory>().inventory[4] = new Item(data.healerInventory[4]);
					player.GetComponent<PlayerStats>().attack = data.healerStats[0];
					player.GetComponent<PlayerStats>().magicAttack = data.healerStats[1];
					player.GetComponent<PlayerStats>().healPower = data.healerStats[2];
					player.GetComponent<PlayerStats>().defense = data.healerStats[3];
					player.GetComponent<PlayerStats>().MagicDefense = data.healerStats[4];
					player.GetComponent<PlayerStats>().str = data.healerStats[5];
					player.GetComponent<PlayerStats>().dex = data.healerStats[6];
					player.GetComponent<PlayerStats>().intelligence = data.healerStats[7];
					player.GetComponent<PlayerStats>().vit = data.healerStats[8];
					player.GetComponent<PlayerStats>().mnd = data.healerStats[9];
					player.GetComponent<PlayerStats>().spd = data.healerStats[10];
					player.GetComponent<PlayerStats>().maxHP = data.healerStats[11];
					player.GetComponent<PlayerStats>().maxStamina = data.healerStats[12];
					player.GetComponent<PlayerStats>().level = data.healerStats[13];
					player.GetComponent<PlayerStats>().experience = data.healerStats[14];
				}
			}



		}

	}
}

[Serializable]
class PlayerData
{
	public List<int> swordsmanInventory = new List<int> ();
	public List<int> knightInventory = new List<int>();
	public List<int> rangerInventory = new List<int>();
	public List<int> mageInventory = new List<int> ();
	public List<int> healerInventory = new List<int> ();

	public List<int> swordsmanStats = new List<int> ();
	public List<int> knightStats = new List<int>();
	public List<int> rangerStats = new List<int>();
	public List<int> mageStats = new List<int>();
	public List<int> healerStats = new List<int>();

	public int level;
}
