using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("nourishment", "hydration", "energy", "nourishmentDrainRate", "hydrationDrainRate", "energyDrainRate")]
	public class ES3Type_Needs : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Needs() : base(typeof(Needs))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Needs)obj;
			
			writer.WriteProperty("nourishment", instance.nourishment, ES3Type_int.Instance);
			writer.WriteProperty("hydration", instance.hydration, ES3Type_int.Instance);
			writer.WriteProperty("energy", instance.energy, ES3Type_int.Instance);
			writer.WriteProperty("nourishmentDrainRate", instance.nourishmentDrainRate, ES3Type_float.Instance);
			writer.WriteProperty("hydrationDrainRate", instance.hydrationDrainRate, ES3Type_float.Instance);
			writer.WriteProperty("energyDrainRate", instance.energyDrainRate, ES3Type_float.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Needs)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "nourishment":
						instance.nourishment = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "hydration":
						instance.hydration = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "energy":
						instance.energy = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "nourishmentDrainRate":
						instance.nourishmentDrainRate = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "hydrationDrainRate":
						instance.hydrationDrainRate = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "energyDrainRate":
						instance.energyDrainRate = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_NeedsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_NeedsArray() : base(typeof(Needs[]), ES3Type_Needs.Instance)
		{
			Instance = this;
		}
	}
}