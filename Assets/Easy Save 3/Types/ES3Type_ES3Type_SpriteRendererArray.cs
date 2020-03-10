using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Instance", "elementType", "type", "isPrimitive", "isValueType", "isCollection", "isDictionary", "isES3TypeUnityObject", "isReflectedType", "isUnsupported")]
	public class ES3Type_ES3Type_SpriteRendererArray : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_ES3Type_SpriteRendererArray() : base(typeof(ES3Types.ES3Type_SpriteRendererArray)){ Instance = this; }

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ES3Types.ES3Type_SpriteRendererArray)obj;
			
			writer.WriteProperty("Instance", Instance);
			writer.WriteProperty("elementType", instance.elementType);
			writer.WriteProperty("type", instance.type);
			writer.WriteProperty("isPrimitive", instance.isPrimitive, ES3Type_bool.Instance);
			writer.WriteProperty("isValueType", instance.isValueType, ES3Type_bool.Instance);
			writer.WriteProperty("isCollection", instance.isCollection, ES3Type_bool.Instance);
			writer.WriteProperty("isDictionary", instance.isDictionary, ES3Type_bool.Instance);
			writer.WriteProperty("isES3TypeUnityObject", instance.isES3TypeUnityObject, ES3Type_bool.Instance);
			writer.WriteProperty("isReflectedType", instance.isReflectedType, ES3Type_bool.Instance);
			writer.WriteProperty("isUnsupported", instance.isUnsupported, ES3Type_bool.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ES3Types.ES3Type_SpriteRendererArray)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Instance":
						Instance = reader.Read<ES3Types.ES3Type>();
						break;
					case "elementType":
						instance.elementType = reader.Read<ES3Types.ES3Type>();
						break;
					case "type":
						instance.type = reader.Read<System.Type>();
						break;
					case "isPrimitive":
						instance.isPrimitive = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isValueType":
						instance.isValueType = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isCollection":
						instance.isCollection = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isDictionary":
						instance.isDictionary = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isES3TypeUnityObject":
						instance.isES3TypeUnityObject = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isReflectedType":
						instance.isReflectedType = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isUnsupported":
						instance.isUnsupported = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ES3Types.ES3Type_SpriteRendererArray();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}

	public class ES3Type_ES3Type_SpriteRendererArrayArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_ES3Type_SpriteRendererArrayArray() : base(typeof(ES3Types.ES3Type_SpriteRendererArray[]), ES3Type_ES3Type_SpriteRendererArray.Instance)
		{
			Instance = this;
		}
	}
}