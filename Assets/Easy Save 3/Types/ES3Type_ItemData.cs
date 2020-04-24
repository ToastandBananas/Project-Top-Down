using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("hasBeenRandomized", "item", "equipment", "consumable", "inventoryIcon", "gameSprite", "itemName", "value", "currentStackSize", "maxDurability", "durability", "damage", "defense", "freshness", "uses", "ammoTypePrefab", "currentAmmoCount")]
	public class ES3Type_ItemData : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_ItemData() : base(typeof(ItemData))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ItemData)obj;
			
			writer.WriteProperty("hasBeenRandomized", instance.hasBeenRandomized, ES3Type_bool.Instance);
			writer.WritePropertyByRef("item", instance.item);
			writer.WritePropertyByRef("equipment", instance.equipment);
			writer.WritePropertyByRef("consumable", instance.consumable);
			writer.WritePropertyByRef("inventoryIcon", instance.inventoryIcon);
			writer.WritePropertyByRef("gameSprite", instance.gameSprite);
			writer.WriteProperty("itemName", instance.itemName, ES3Type_string.Instance);
			writer.WriteProperty("value", instance.value, ES3Type_int.Instance);
			writer.WriteProperty("currentStackSize", instance.currentStackSize, ES3Type_int.Instance);
			writer.WriteProperty("maxDurability", instance.maxDurability, ES3Type_int.Instance);
			writer.WriteProperty("durability", instance.durability, ES3Type_float.Instance);
			writer.WriteProperty("damage", instance.damage, ES3Type_int.Instance);
			writer.WriteProperty("defense", instance.defense, ES3Type_int.Instance);
			writer.WriteProperty("freshness", instance.freshness, ES3Type_int.Instance);
			writer.WriteProperty("uses", instance.uses, ES3Type_int.Instance);
			writer.WritePropertyByRef("ammoTypePrefab", instance.ammoTypePrefab);
			writer.WriteProperty("currentAmmoCount", instance.currentAmmoCount, ES3Type_int.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ItemData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "hasBeenRandomized":
						instance.hasBeenRandomized = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "item":
						instance.item = reader.Read<Item>();
						break;
					case "equipment":
						instance.equipment = reader.Read<Equipment>();
						break;
					case "consumable":
						instance.consumable = reader.Read<Consumable>();
						break;
					case "inventoryIcon":
						instance.inventoryIcon = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "gameSprite":
						instance.gameSprite = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "itemName":
						instance.itemName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "value":
						instance.value = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "currentStackSize":
						instance.currentStackSize = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "maxDurability":
						instance.maxDurability = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "durability":
						instance.durability = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "damage":
						instance.damage = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "defense":
						instance.defense = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "freshness":
						instance.freshness = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "uses":
						instance.uses = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ammoTypePrefab":
						instance.ammoTypePrefab = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "currentAmmoCount":
						instance.currentAmmoCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_ItemDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ItemDataArray() : base(typeof(ItemData[]), ES3Type_ItemData.Instance)
		{
			Instance = this;
		}
	}
}