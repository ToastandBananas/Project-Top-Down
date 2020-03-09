using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("currentState", "currentAlliance", "targetPos", "attackTarget", "walkSpeed", "runSpeed", "attackDistance")]
	public class ES3Type_NPCMovement : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_NPCMovement() : base(typeof(NPCMovement))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NPCMovement)obj;
			
			writer.WriteProperty("currentState", instance.currentState);
			writer.WriteProperty("currentAlliance", instance.currentAlliance);
			writer.WritePropertyByRef("targetPos", instance.targetPos);
			writer.WritePropertyByRef("attackTarget", instance.attackTarget);
			writer.WriteProperty("walkSpeed", instance.walkSpeed, ES3Type_float.Instance);
			writer.WriteProperty("runSpeed", instance.runSpeed, ES3Type_float.Instance);
			writer.WriteProperty("attackDistance", instance.attackDistance, ES3Type_float.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NPCMovement)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "currentState":
						instance.currentState = reader.Read<STATE>();
						break;
					case "currentAlliance":
						instance.currentAlliance = reader.Read<ALLIANCE>();
						break;
					case "targetPos":
						instance.targetPos = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "attackTarget":
						instance.attackTarget = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "walkSpeed":
						instance.walkSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "runSpeed":
						instance.runSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "attackDistance":
						instance.attackDistance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_NPCMovementArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_NPCMovementArray() : base(typeof(NPCMovement[]), ES3Type_NPCMovement.Instance)
		{
			Instance = this;
		}
	}
}