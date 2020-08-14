using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {


	public int attack;
	public int magicAttack;
	public int healPower;
	public int defense;
	public int MagicDefense;
	public int str;
	public int dex;
	public int intelligence;
	public int vit;
	public int mnd;
	public int spd;
	public int maxHP;
	public int currHP;
	public int maxStamina;
	public float currStamina;
	public int level = 1;
	public int experience;
	public string job;

	public int oldAttack;
	public int oldMagicAttack;
	public int oldHealPower;
	public int oldDefense;
	public int oldMagicDefense;
	public int oldStr;
	public int oldDex;
	public int oldIntelligence;
	public int oldVit;
	public int oldMnd;
	public int oldSpd;
	public int oldMaxHP;
	public int oldMaxStamina;
	public int oldLevel;
	public int oldExperience;

	private bool start = true;

	// Use this for initialization
	void Start () {
		initStats (job);
	}
	
	// Update is called once per frame
	void Update () {
		if(experience >= 100)
		{
			levelUp();
		}
	}

	void OnLevelWasLoaded()
	{
		oldAttack = attack;
		oldMagicAttack = magicAttack;
		oldHealPower = healPower;
		oldDefense = defense;
		oldMagicDefense = MagicDefense;
		oldStr = str;
		oldDex = dex;
		oldIntelligence = intelligence;
		oldVit = vit;
		oldMnd = mnd;
		oldSpd = spd;
		oldMaxHP = maxHP;
		oldMaxStamina = maxStamina;
		oldLevel = level;
		oldExperience = experience;

		currStamina = maxStamina;
		currHP = maxHP;
	}

	public void reloadStats()
	{
		attack = oldAttack;
		magicAttack = oldMagicAttack;
		healPower = oldHealPower;
		defense = oldDefense;
		MagicDefense = oldMagicDefense;
		str = oldStr;
		dex = oldDex;
		intelligence = oldIntelligence;
		vit = oldVit;
		mnd = oldMnd;
		spd = oldSpd;
		maxHP = oldMaxHP;
		maxStamina = oldMaxStamina;
		level = oldLevel;
		experience = oldExperience;
	}

	void initStats(string job)
	{
		if (job == "Swordsman") 
		{
			str = 10;
			dex = 6;
			intelligence = 2;
			vit = 5;
			mnd = 2;
			spd = 6;
			maxHP = 15;
			currHP = maxHP;
			maxStamina = 15;
			currStamina = maxStamina;
			attack = (int)(str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Knight")
		{
			str = 6;
			dex = 5;
			intelligence = 3;
			vit = 9;
			mnd = 3;
			spd = 3;
			maxHP = 25;
			currHP = maxHP;
			maxStamina = 10;
			currStamina = maxStamina;
			attack = (int)(str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Ranger")
		{
			str = 7;
			dex = 10;
			intelligence = 4;
			vit = 4;
			mnd = 4;
			spd = 8;
			maxHP = 15;
			currHP = maxHP;
			maxStamina = 20;
			currStamina = maxStamina;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Mage")
		{
			str = 3;
			dex = 4;
			intelligence = 10;
			vit = 3;
			mnd = 6;
			spd = 6;
			maxHP = 15;
			currHP = maxHP;
			maxStamina = 15;
			currStamina = maxStamina;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Healer")
		{
			str = 0;
			dex = 0;
			intelligence = 5;
			vit = 2;
			mnd = 10;
			spd = 5;
			maxHP = 10;
			currHP = maxHP;
			maxStamina = 15;
			currStamina = maxStamina;
			attack = 0;
			magicAttack = 0;
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}
		if(GameStateMachine.currentGameMode == GameStateMachine.GameMode.multiPlayer)
		{
			currHP += 50;
			maxHP = currHP;
		}
	}

	public void levelUp()
	{
		if (job == "Swordsman") 
		{
			str +=3;
			dex += 2;
			intelligence += 2;
			vit += 2;
			mnd += 1;
			spd += 2;
			maxHP += 5;
			currHP = maxHP;
			maxStamina += 5;
			attack = (int)(str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Knight")
		{
			str += 2;
			dex += 2;
			intelligence += 1;
			vit += 3;
			mnd += 1;
			spd += 1;
			maxHP += 10;
			currHP = maxHP;
			maxStamina += 4;
			attack = (int)(str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Ranger")
		{
			str += 2;
			dex += 3;
			intelligence += 1;
			vit += 1;
			mnd += 1;
			spd += 3;
			maxHP += 4;
			currHP = maxHP;
			maxStamina += 7;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Mage")
		{
			str += 1;
			dex += 1;
			intelligence += 3;
			vit += 1;
			mnd += 2;
			spd += 2;
			maxHP += 4;
			currHP = maxHP;
			maxStamina += 4;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower = 0;
		}
		else if(job == "Healer")
		{
			str += 0;
			dex += 0;
			intelligence += 2;
			vit += 1;
			mnd += 3;
			spd += 2;
			maxHP += 3;
			currHP = maxHP;
			maxStamina += 4;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}
		Random.seed = (int)Time.realtimeSinceStartup;
		int Lucky = Random.Range (0, 100);

		if(Lucky == 7 || Lucky == 77)
		{
			str += 1;
			dex += 1;
			intelligence += 1;
			vit += 1;
			mnd += 1;
			spd += 1;
			maxHP += 1;
			currHP = maxHP;
			maxStamina += 1;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}
		else if(Lucky < 50)
		{
			str += 1;
			dex += 0;
			intelligence += 1;
			vit += 0;
			mnd += 0;
			spd += 0;
			maxHP += 0;
			currHP = maxHP;
			maxStamina += 1;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}
		else if(Lucky > 50)
		{
			str += 0;
			dex += 0;
			intelligence += 0;
			vit += 1;
			mnd += 1;
			spd += 1;
			maxHP += 1;
			currHP = maxHP;
			maxStamina += 0;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}
		else if(Lucky == 50)
		{
			str += 2;
			dex += 0;
			intelligence += 2;
			vit += 0;
			mnd += 2;
			spd += 0;
			maxHP += 0;
			currHP = maxHP;
			maxStamina += 0;
			attack = (int) (str * 1.5);
			magicAttack = (int)(intelligence * 1.5);
			defense = (int)(vit * 1.2);
			MagicDefense = (int)(intelligence * 0.5) + mnd;
			healPower =(int) (mnd * 1.5);
		}

		level++;
		experience = experience - 100;

		if(start == true)
		{
			currStamina = maxStamina;
			start = false;
			oldAttack = attack;
			oldMagicAttack = magicAttack;
			oldHealPower = healPower;
			oldDefense = defense;
			oldMagicDefense = MagicDefense;
			oldStr = str;
			oldDex = dex;
			oldIntelligence = intelligence;
			oldVit = vit;
			oldMnd = mnd;
			oldSpd = spd;
			oldMaxHP = maxHP;
			oldMaxStamina = maxStamina;
			oldLevel = level;
			oldExperience = experience;
		}
		if(this.tag == "Enemy")
		{
			currStamina = maxStamina;
		}
	

	}
}
