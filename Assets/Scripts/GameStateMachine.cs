using UnityEngine;
using System.Collections;

public static class GameStateMachine {

	public static State currentGameState;
	public static GameMode currentGameMode;
	public static MPState currentMPGameState;
	private static Texture2D enemyTurnTexture;
	private static Texture2D playerTurnTexture;
	private static Texture2D blueTurnTexture;
	private static Texture2D redTurnTexture;
	public static float turnChangeTime;

	public enum State
	{
		playerTurn,
		enemyTurn,
		statScreen,
		paused,
		gameOver
	}

	public enum MPState
	{
		player1Turn,
		player2Turn,
		statScreen,
		paused,
		gameOver
	}

	public enum GameMode
	{
		menu,
		singlePlayer,
		multiPlayer,
	}

	public static void initMenu()
	{
		currentGameMode = GameMode.menu;
	}

	public static void initSingle()
	{
		currentGameState = State.playerTurn;
		currentGameMode = GameMode.singlePlayer;
		enemyTurnTexture = Resources.Load<Texture2D> ("enemyTurn");
		playerTurnTexture = Resources.Load<Texture2D> ("yourTurn");
	}

	public static void initMulti()
	{
		currentMPGameState = MPState.player1Turn;
		currentGameMode = GameMode.multiPlayer;
		turnChangeTime = 0.0f;
		blueTurnTexture = Resources.Load<Texture2D> ("blueTurn");
		redTurnTexture = Resources.Load<Texture2D> ("redTurn");
	}
	
	public static void drawCurrentState()
	{
		Rect slotRect = new Rect (Screen.width * 0.8625f, Screen.height * 0.945f, Screen.width * 0.125f, Screen.height * 0.05f);
		if(currentGameMode == GameMode.singlePlayer)
		{
			if(currentGameState == State.playerTurn)
			{
				GUI.DrawTexture (slotRect, playerTurnTexture);
			}

			else if(currentGameState == State.enemyTurn)
			{
				GUI.DrawTexture (slotRect, enemyTurnTexture);
			}
		}
		else if(currentGameMode == GameMode.multiPlayer)
		{
			if(currentMPGameState == MPState.player1Turn)
			{
				GUI.DrawTexture (slotRect, blueTurnTexture);
			}
			else if(currentMPGameState == MPState.player2Turn)
			{
				GUI.DrawTexture (slotRect, redTurnTexture);
			}
		}


		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.fontSize = (int)(Screen.width * 0.013); 


	}
}
