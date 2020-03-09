using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("viewAngle", "viewRadius", "showFOV")]
	public class ES3Type_FieldOfView : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_FieldOfView() : base(typeof(FieldOfView))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (FieldOfView)obj;
			
			writer.WriteProperty("viewAngle", instance.viewAngle, ES3Type_float.Instance);
			writer.WriteProperty("viewRadius", instance.viewRadius, ES3Type_float.Instance);
			writer.WriteProperty("showFOV", instance.showFOV, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (FieldOfView)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "viewAngle":
						instance.viewAngle = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "viewRadius":
						instance.viewRadius = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "showFOV":
						instance.showFOV = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_FieldOfViewArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_FieldOfViewArray() : base(typeof(FieldOfView[]), ES3Type_FieldOfView.Instance)
		{
			Instance = this;
		}
	}
}