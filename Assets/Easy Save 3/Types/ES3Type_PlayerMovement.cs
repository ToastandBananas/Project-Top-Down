using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("runSpeed", "dodgeDistance", "isMounted")]
	public class ES3Type_PlayerMovement : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_PlayerMovement() : base(typeof(PlayerMovement))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerMovement)obj;
			
			writer.WriteProperty("runSpeed", instance.runSpeed, ES3Type_float.Instance);
			writer.WriteProperty("dodgeDistance", instance.dodgeDistance, ES3Type_float.Instance);
			writer.WriteProperty("isMounted", instance.isMounted, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerMovement)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "runSpeed":
						instance.runSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "dodgeDistance":
						instance.dodgeDistance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "isMounted":
						instance.isMounted = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_PlayerMovementArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_PlayerMovementArray() : base(typeof(PlayerMovement[]), ES3Type_PlayerMovement.Instance)
		{
			Instance = this;
		}
	}
}