using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("currentWeapons", "currentEquipment")]
	public class ES3Type_EquipmentManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_EquipmentManager() : base(typeof(EquipmentManager))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (EquipmentManager)obj;
			
			writer.WriteProperty("currentWeapons", instance.currentWeapons);
			writer.WriteProperty("currentEquipment", instance.currentEquipment);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (EquipmentManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "currentWeapons":
						instance.currentWeapons = reader.Read<ItemData[]>();
						break;
					case "currentEquipment":
						instance.currentEquipment = reader.Read<ItemData[]>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_EquipmentManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_EquipmentManagerArray() : base(typeof(EquipmentManager[]), ES3Type_EquipmentManager.Instance)
		{
			Instance = this;
		}
	}
}