using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("level", "maxHealth", "health", "maxStamina", "stamina", "maxMana", "mana", "maxEncumbrance", "encumbrance", "defense", "isPlayer", "isDead")]
	public class ES3Type_BasicStats : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_BasicStats() : base(typeof(BasicStats))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (BasicStats)obj;
			
			writer.WriteProperty("level", instance.level, ES3Type_float.Instance);
			writer.WriteProperty("maxHealth", instance.maxHealth, ES3Type_float.Instance);
			writer.WriteProperty("health", instance.health, ES3Type_float.Instance);
			writer.WriteProperty("maxStamina", instance.maxStamina, ES3Type_float.Instance);
			writer.WriteProperty("stamina", instance.stamina, ES3Type_float.Instance);
			writer.WriteProperty("maxMana", instance.maxMana, ES3Type_float.Instance);
			writer.WriteProperty("mana", instance.mana, ES3Type_float.Instance);
			writer.WriteProperty("maxEncumbrance", instance.maxEncumbrance, ES3Type_float.Instance);
			writer.WriteProperty("encumbrance", instance.encumbrance, ES3Type_float.Instance);
			writer.WriteProperty("defense", instance.defense, ES3Type_float.Instance);
			writer.WriteProperty("isPlayer", instance.isPlayer, ES3Type_bool.Instance);
			writer.WriteProperty("isDead", instance.isDead, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (BasicStats)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "level":
						instance.level = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "maxHealth":
						instance.maxHealth = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "health":
						instance.health = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "maxStamina":
						instance.maxStamina = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "stamina":
						instance.stamina = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "maxMana":
						instance.maxMana = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "mana":
						instance.mana = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "maxEncumbrance":
						instance.maxEncumbrance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "encumbrance":
						instance.encumbrance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "defense":
						instance.defense = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "isPlayer":
						instance.isPlayer = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isDead":
						instance.isDead = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_BasicStatsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_BasicStatsArray() : base(typeof(BasicStats[]), ES3Type_BasicStats.Instance)
		{
			Instance = this;
		}
	}
}