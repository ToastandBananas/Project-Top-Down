using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isVerticalDoorway", "isOpen", "playerInRange", "NPCInRange", "NPCGameObjectsInRange")]
	public class ES3Type_Door : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Door() : base(typeof(Door))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Door)obj;
			
			writer.WriteProperty("isVerticalDoorway", instance.isVerticalDoorway, ES3Type_bool.Instance);
			writer.WriteProperty("isOpen", instance.isOpen, ES3Type_bool.Instance);
			writer.WriteProperty("playerInRange", instance.playerInRange, ES3Type_bool.Instance);
			writer.WriteProperty("NPCInRange", instance.NPCInRange, ES3Type_bool.Instance);
			writer.WritePrivateField("NPCGameObjectsInRange", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Door)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isVerticalDoorway":
						instance.isVerticalDoorway = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isOpen":
						instance.isOpen = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "playerInRange":
						instance.playerInRange = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "NPCInRange":
						instance.NPCInRange = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "NPCGameObjectsInRange":
					reader.SetPrivateField("NPCGameObjectsInRange", reader.Read<System.Collections.Generic.List<UnityEngine.GameObject>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_DoorArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_DoorArray() : base(typeof(Door[]), ES3Type_Door.Instance)
		{
			Instance = this;
		}
	}
}