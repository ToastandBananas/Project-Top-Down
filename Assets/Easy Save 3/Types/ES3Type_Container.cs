using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("containerName", "containerObjects", "containerItems", "slotCount")]
	public class ES3Type_Container : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Container() : base(typeof(Container))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Container)obj;
			
			writer.WriteProperty("containerName", instance.containerName, ES3Type_string.Instance);
			writer.WriteProperty("containerObjects", instance.containerObjects);
			writer.WriteProperty("containerItems", instance.containerItems);
			writer.WriteProperty("slotCount", instance.slotCount, ES3Type_int.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Container)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "containerName":
						instance.containerName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "containerObjects":
						instance.containerObjects = reader.Read<System.Collections.Generic.List<UnityEngine.GameObject>>();
						break;
					case "containerItems":
						instance.containerItems = reader.Read<System.Collections.Generic.List<ItemData>>();
						break;
					case "slotCount":
						instance.slotCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_ContainerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ContainerArray() : base(typeof(Container[]), ES3Type_Container.Instance)
		{
			Instance = this;
		}
	}
}