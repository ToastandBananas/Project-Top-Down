using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isEmpty", "slotCoordinate", "item", "itemData", "parentSlot", "childrenSlots", "slotParent", "stackSizeText")]
	public class ES3Type_InventorySlot : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_InventorySlot() : base(typeof(InventorySlot))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (InventorySlot)obj;
			
			writer.WriteProperty("isEmpty", instance.isEmpty, ES3Type_bool.Instance);
			writer.WriteProperty("slotCoordinate", instance.slotCoordinate, ES3Type_Vector2.Instance);
			writer.WritePropertyByRef("item", instance.item);
			writer.WritePropertyByRef("itemData", instance.itemData);
			writer.WritePropertyByRef("parentSlot", instance.parentSlot);
			writer.WriteProperty("childrenSlots", instance.childrenSlots, ES3Type_InventorySlotArray.Instance);
			writer.WritePropertyByRef("slotParent", instance.slotParent);
			writer.WritePropertyByRef("stackSizeText", instance.stackSizeText);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (InventorySlot)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isEmpty":
						instance.isEmpty = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "slotCoordinate":
						instance.slotCoordinate = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "item":
						instance.item = reader.Read<Item>();
						break;
					case "itemData":
						instance.itemData = reader.Read<ItemData>(ES3Type_ItemData.Instance);
						break;
					case "parentSlot":
						instance.parentSlot = reader.Read<InventorySlot>(ES3Type_InventorySlot.Instance);
						break;
					case "childrenSlots":
						instance.childrenSlots = reader.Read<InventorySlot[]>(ES3Type_InventorySlotArray.Instance);
						break;
					case "slotParent":
						instance.slotParent = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "stackSizeText":
						instance.stackSizeText = reader.Read<UnityEngine.UI.Text>(ES3Type_Text.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_InventorySlotArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_InventorySlotArray() : base(typeof(InventorySlot[]), ES3Type_InventorySlot.Instance)
		{
			Instance = this;
		}
	}
}