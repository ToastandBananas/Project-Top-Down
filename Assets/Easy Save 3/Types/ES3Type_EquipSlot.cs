using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("thisWeaponSlot", "thisEquipmentSlot", "isEmpty", "iconImage", "equipment", "itemData")]
	public class ES3Type_EquipSlot : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_EquipSlot() : base(typeof(EquipSlot))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (EquipSlot)obj;
			
			writer.WriteProperty("thisWeaponSlot", instance.thisWeaponSlot);
			writer.WriteProperty("thisEquipmentSlot", instance.thisEquipmentSlot);
			writer.WriteProperty("isEmpty", instance.isEmpty, ES3Type_bool.Instance);
			writer.WritePropertyByRef("iconImage", instance.iconImage);
			writer.WritePropertyByRef("equipment", instance.equipment);
			writer.WritePropertyByRef("itemData", instance.itemData);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (EquipSlot)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "thisWeaponSlot":
						instance.thisWeaponSlot = reader.Read<WeaponSlot>();
						break;
					case "thisEquipmentSlot":
						instance.thisEquipmentSlot = reader.Read<EquipmentSlot>();
						break;
					case "isEmpty":
						instance.isEmpty = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "iconImage":
						instance.iconImage = reader.Read<UnityEngine.UI.Image>();
						break;
					case "equipment":
						instance.equipment = reader.Read<Equipment>();
						break;
					case "itemData":
						instance.itemData = reader.Read<ItemData>(ES3Type_ItemData.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_EquipSlotArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_EquipSlotArray() : base(typeof(EquipSlot[]), ES3Type_EquipSlot.Instance)
		{
			Instance = this;
		}
	}
}