//Diadrasis ©2023 - Stathis Georgiou
using System;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class cImage
	{
		public int image_id, poi_id, image_order, area_id, route_id;
		public string image_file;
		public cLocalizedText[] image_title, image_label, image_source;

		public bool IsNull { get { return image_id == 0; } }
	}

}
