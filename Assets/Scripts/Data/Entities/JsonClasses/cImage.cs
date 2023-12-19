//Diadrasis ©2023 - Stathis Georgiou
using System;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class cImage
	{
		public int image_id, poi_id, ref_id, image_order, area_id, route_id;
		public string image_file;
		public cLocalizedText[] image_title, image_label, image_source;

        public void FixRefID() { if (ref_id == 0) ref_id = poi_id; }

        public bool IsNull { get { return image_id == 0; } }
	}

}
