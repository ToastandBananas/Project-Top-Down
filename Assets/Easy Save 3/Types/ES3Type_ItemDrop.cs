using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isDropped")]
	public class ES3Type_ItemDrop : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_ItemDrop() : base(typeof(ItemDrop))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ItemDrop)obj;
			
			writer.WriteProperty("isDropped", instance.isDropped, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ItemDrop)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isDropped":
						instance.isDropped = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_ItemDropArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ItemDropArray() : base(typeof(ItemDrop[]), ES3Type_ItemDrop.Instance)
		{
			Instance = this;
		}
	}
}