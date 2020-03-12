using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("target", "enabled")]
	public class ES3Type_AIDestinationSetter : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_AIDestinationSetter() : base(typeof(Pathfinding.AIDestinationSetter))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.AIDestinationSetter)obj;
			
			writer.WritePropertyByRef("target", instance.target);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.AIDestinationSetter)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "target":
						instance.target = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_AIDestinationSetterArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_AIDestinationSetterArray() : base(typeof(Pathfinding.AIDestinationSetter[]), ES3Type_AIDestinationSetter.Instance)
		{
			Instance = this;
		}
	}
}