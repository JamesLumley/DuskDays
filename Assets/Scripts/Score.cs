using UnityEngine;
using System.Collections;

public static class Score 
{

	private static int _turnsTaken;
	private static int _collectables;
	private static int _allCollectables;
	private static int _currentLevel;

	public static int turnsTaken 
	{
		get;
		set;
	}

	public static int collectables 
	{
		get;
		set;
	}

	public static int allCollectables 
	{
		get;
		set;
	}

	public static int currentLevel 
	{
		get;
		set;
	}


}
