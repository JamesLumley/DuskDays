using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour {

	public bool selected = false;
	public bool active = false;
	Vector2 targetPos = new Vector2(0,0);
	public int itemID;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(!selected)
		{
			//target = null;
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		else
		{
			if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
			{
				this.GetComponent<Rigidbody>().isKinematic = false;
			}
			else 
			{
				this.GetComponent<Rigidbody>().isKinematic = true;

			}
		}

	}

	void OnGUI()
	{
		targetPos = Camera.main.WorldToScreenPoint (this.transform.position);
		if(GameStateMachine.currentGameState != GameStateMachine.State.paused)
		{
			drawHealth ();
		}
	}

	void drawHealth()
	{
		if(GameStateMachine.currentGameState == GameStateMachine.State.playerTurn)
		{
			if(PlayerDatabase.getSelected () != null)
			{
				if(Vector3.Distance(PlayerDatabase.getSelected().transform.position, this.transform.position) < 9)
				{
					int currHP = this.GetComponent<PlayerStats> ().currHP;
					int maxHP = this.GetComponent<PlayerStats> ().maxHP;
					float hpPercent = ((float)currHP /(float) maxHP) * 15.0f;
					Rect hpRect = new Rect (targetPos.x - (Camera.main.pixelWidth * 0.039f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f , hpPercent * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					Rect backRect = new Rect (targetPos.x - (Camera.main.pixelWidth * 0.039f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, 15 * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					GUI.DrawTexture (backRect, Resources.Load<Texture2D> ("BackBar"));
					GUI.DrawTexture (hpRect, Resources.Load<Texture2D> ("HpBarRed"));
				}
			}
		}
		else if(GameStateMachine.currentGameState == GameStateMachine.State.enemyTurn)
		{
			if(EnemyDatabase.getSelected() != null)
			{
				if(Vector3.Distance(EnemyDatabase.getSelected().transform.position, this.transform.position) < 9)
				{
					int currHP = this.GetComponent<PlayerStats> ().currHP;
					int maxHP = this.GetComponent<PlayerStats> ().maxHP;
					float hpPercent = ((float)currHP /(float) maxHP) * 15.0f;
					Rect hpRect = new Rect (targetPos.x - (Camera.main.pixelWidth * 0.039f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f , hpPercent * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					Rect backRect = new Rect (targetPos.x - (Camera.main.pixelWidth * 0.039f), Camera.main.pixelHeight - targetPos.y - Camera.main.pixelHeight * 0.07f, 15 * 5 * Camera.main.pixelWidth * 0.001f, Camera.main.pixelHeight * 0.012f);
					GUI.DrawTexture (backRect, Resources.Load<Texture2D> ("BackBar"));
					GUI.DrawTexture (hpRect, Resources.Load<Texture2D> ("HpBarRed"));
				}
			}
		}

	}
}
