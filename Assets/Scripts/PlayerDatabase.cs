using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PlayerDatabase{
	public static List<GameObject> players = new List<GameObject> ();
	public static List<GameObject> bluePlayers = new List<GameObject> ();
	public static List<GameObject> redPlayers = new List<GameObject> ();
	static GameObject[] tempStorage;
	public static int selectedID = 0;
	public static int changedFrame = 0;

	// Use this for initialization
	public static void initSingle()
	{
		tempStorage = new GameObject[GameObject.FindGameObjectsWithTag ("Player").Length];//creates a array of size for the number of players
		tempStorage = GameObject.FindGameObjectsWithTag ("Player"); //adds all the players to the array

		selectedID = 0;//resets selected ID

			//Adds all the players to the list.
			for (int i = 0; i < tempStorage.Length; i++) 
			{
				if(tempStorage[i] != null)
				{
					players.Add(tempStorage[i]);
					players[i].GetComponent<Player_Controller>().listID = i;
				}
			}

		
		foreach (GameObject player in players)
		{
			player.GetComponent<Player_Controller>().selected = false;//defaults all players selected state to false
		}

		players[0].GetComponent<Player_Controller>().selected = true;//defaults the first players selected state to true

	}

	public static void initMulti()
	{
		tempStorage = new GameObject[GameObject.FindGameObjectsWithTag ("BlueTeam").Length];
		tempStorage = GameObject.FindGameObjectsWithTag ("BlueTeam");
		selectedID = 0;
		
		for (int i = 0; i < tempStorage.Length; i++) 
		{
			bluePlayers.Add(tempStorage[i]);
		}

		tempStorage = new GameObject[GameObject.FindGameObjectsWithTag ("RedTeam").Length];
		tempStorage = GameObject.FindGameObjectsWithTag ("RedTeam");
		
		
		for (int i = 0; i < tempStorage.Length; i++) 
		{
			redPlayers.Add(tempStorage[i]);
		}

		bluePlayers[0].GetComponent<Player_Controller>().selected = true;

	}
	

	public static void deleteLists()
	{
		players.Clear ();
		redPlayers.Clear ();
		bluePlayers.Clear ();
	}

	public static GameObject getSelected()
	{
		foreach (GameObject player in players) //search all players
		{
			if(player.GetComponent<Player_Controller>().selected == true)
			{
				return player; //If the player is selected, return it
			}
			else
			{continue;} //else check next
		}
		return null; //will return null if no player is selected
	}

	public static GameObject getPlayer1Selected()
	{
		foreach (GameObject player in bluePlayers)
		{
			if(player.GetComponent<Player_Controller>().selected == true)
			{
				return player;
			}
			else
			{continue;}
		}
		return players [0];

	}
	public static GameObject getPlayer2Selected()
	{
		foreach (GameObject player in redPlayers)
		{
			if(player.GetComponent<Player_Controller>().selected == true)
			{
				return player;
			}
			else
			{continue;}
		}
		return players [0];

	}


}
