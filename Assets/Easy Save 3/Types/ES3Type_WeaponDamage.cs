using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("positionOffset")]
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