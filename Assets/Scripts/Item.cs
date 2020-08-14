using UnityEngine;
using System.Collections;
[System.Serializable]
public class Item{
	public string itemName;
	public int itemID;
	public string itemDesc;
	public Texture2D itemIcon;
	public int maxDurability;
	public int currDurability;
	public int attackMod;
	public int magicAttackMod;
	public int healMod;
	public int range;
	public ItemType itemType;

	public enum ItemType
	{
		Weapon,
		Consumable,
		keyItem
	}
	public Item()
	{
		maxDurability = -1;
		itemID = -1;
	}

	public Item(int itemID)
	{
		switch (itemID) 
		{
		case 0:
			this.itemName = "Bronze Sword";
			this.itemID = itemID;
			this.itemDesc = "A weak sword";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 2;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 1;
			this.itemType = ItemType.Weapon;
			break;
		case 1:
			this.itemName = "Bronze Bow";
			this.itemID = itemID;
			this.itemDesc = "A weak bow";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 2;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 3;
			this.itemType = ItemType.Weapon;
			break;
		case 2:
			this.itemName = "Battered Book";
			this.itemID = itemID;
			this.itemDesc = "A barely readable book";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 2;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 3;
			this.itemType = ItemType.Weapon;
			break;
		case 3:
			this.itemName = "Basic Staff";
			this.itemID = itemID;
			this.itemDesc = "A basic healing staff";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 0;
			this.healMod = 2;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 2;
			this.itemType = ItemType.Weapon;
			break;
		case 4:
			this.itemName = "Bronze Axe";
			this.itemID = itemID;
			this.itemDesc = "A weak axe";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 2;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 1;
			this.itemType = ItemType.Weapon;
			break;
		case 5:
			this.itemName = "Iron Sword";
			this.itemID = itemID;
			this.itemDesc = "A normal sword";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 5;
			this.magicAttackMod = 0;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 1;
			this.itemType = ItemType.Weapon;
			break;
		case 6:
			this.itemName = "Iron Bow";
			this.itemID = itemID;
			this.itemDesc = "A normal bow";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 4;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 3;
			this.itemType = ItemType.Weapon;
			break;
		case 7:
			this.itemName = "Spell Book";
			this.itemID = itemID;
			this.itemDesc = "A spell book";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 4;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 3;
			this.itemType = ItemType.Weapon;
			break;
		case 8:
			this.itemName = "Healing Staff";
			this.itemID = itemID;
			this.itemDesc = "A normal healing staff";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 0;
			this.healMod = 2;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 2;
			this.itemType = ItemType.Weapon;
			break;
		case 9:
			this.itemName = "Iron Axe";
			this.itemID = itemID;
			this.itemDesc = "A normal axe";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 4;
			this.magicAttackMod = 0;
			this.healMod = 0;
			this.maxDurability = 10;
			this.currDurability = maxDurability;
			this.range = 1;
			this.itemType = ItemType.Weapon;
			break;
		case 99:
			this.itemName = "Door Key";
			this.itemID = itemID;
			this.itemDesc = "Maybe this\nopens somthing?";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 0;
			this.maxDurability = -1;
			this.currDurability = maxDurability;
			this.range = 0;
			this.itemType = ItemType.keyItem;
			break;
		case 100:
			this.itemName = "Health Potion";
			this.itemID = itemID;
			this.itemDesc = "Restores Health";
			this.itemIcon =  Resources.Load<Texture2D> ("Item Icons/" + itemName);
			this.attackMod = 0;
			this.magicAttackMod = 0;
			this.maxDurability = -1;
			this.currDurability = maxDurability;
			this.range = 0;
			this.itemType = ItemType.Consumable;
			break;
		default:
			break;
		}
	}

		
}
