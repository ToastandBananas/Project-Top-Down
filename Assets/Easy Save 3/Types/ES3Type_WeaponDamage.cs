using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("positionOffset", "useGUILayout", "runInEditMode", "enabled", "hideFlags")]
	public class ES3Type_WeaponDamage : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_WeaponDamage() : base(typeof(WeaponDamage))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (WeaponDamage)obj;
			
			writer.WriteProperty("positionOffset", instance.positionOffset, ES3Type_Vector2.Instance);
			writer.WriteProperty("useGUILayout", instance.useGUILayout, ES3Type_bool.Instance);
			writer.WriteProperty("runInEditMode", instance.runInEditMode, ES3Type_bool.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("hideFlags", instance.hideFlags);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (WeaponDamage)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "positionOffset":
						instance.positionOffset = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
						break;
					case "useGUILayout":
						instance.useGUILayout = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "runInEditMode":
						instance.runInEditMode = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "hideFlags":
						instance.hideFlags = reader.Read<UnityEngine.HideFlags>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_WeaponDamageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_WeaponDamageArray() : base(typeof(WeaponDamage[]), ES3Type_WeaponDamage.Instance)
		{
			Instance = this;
		}
	}
}