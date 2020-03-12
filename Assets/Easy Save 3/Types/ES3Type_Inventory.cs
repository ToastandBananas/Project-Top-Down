using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("pocketsSlotCount", "bagSlotCount", "horseBagSlotCount", "pocketItems", "bagItems", "horseBagItems")]
	public class ES3Type_Inventory : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Inventory() : base(typeof(Inventory))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Inventory)obj;
			
			writer.WriteProperty("pocketsSlotCount", instance.pocketsSlotCount, ES3Type_int.Instance);
			writer.WriteProperty("bagSlotCount", instance.bagSlotCount, ES3Type_int.Instance);
			writer.WriteProperty("horseBagSlotCount", instance.horseBagSlotCount, ES3Type_int.Instance);
			writer.WriteProperty("pocketItems", instance.pocketItems);
			writer.WriteProperty("bagItems", instance.bagItems);
			writer.WriteProperty("horseBagItems", instance.horseBagItems);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Inventory)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "pocketsSlotCount":
						instance.pocketsSlotCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "bagSlotCount":
						instance.bagSlotCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "horseBagSlotCount":
						instance.horseBagSlotCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "pocketItems":
						instance.pocketItems = reader.Read<System.Collections.Generic.List<ItemData>>();
						break;
					case "bagItems":
						instance.bagItems = reader.Read<System.Collections.Generic.List<ItemData>>();
						break;
					case "horseBagItems":
						instance.horseBagItems = reader.Read<System.Collections.Generic.List<ItemData>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_InventoryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_InventoryArray() : base(typeof(Inventory[]), ES3Type_Inventory.Instance)
		{
			Instance = this;
		}
	}
}