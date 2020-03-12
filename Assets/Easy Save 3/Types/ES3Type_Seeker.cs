using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3Type_Seeker : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_Seeker() : base(typeof(Pathfinding.Seeker))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.Seeker)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.Seeker)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_SeekerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_SeekerArray() : base(typeof(Pathfinding.Seeker[]), ES3Type_Seeker.Instance)
		{
			Instance = this;
		}
	}
}