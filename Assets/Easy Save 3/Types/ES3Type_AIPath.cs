using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("maxAcceleration", "canMove", "maxSpeed")]
	public class ES3Type_AIPath : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_AIPath() : base(typeof(Pathfinding.AIPath))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.AIPath)obj;
			
			writer.WriteProperty("maxAcceleration", instance.maxAcceleration, ES3Type_float.Instance);
			writer.WriteProperty("canMove", instance.canMove, ES3Type_bool.Instance);
			writer.WriteProperty("maxSpeed", instance.maxSpeed, ES3Type_float.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.AIPath)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "maxAcceleration":
						instance.maxAcceleration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "canMove":
						instance.canMove = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "maxSpeed":
						instance.maxSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_AIPathArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_AIPathArray() : base(typeof(Pathfinding.AIPath[]), ES3Type_AIPath.Instance)
		{
			Instance = this;
		}
	}
}