//Diadrasis ©2023 - Stathis Georgiou
using System;

namespace Diadrasis.Rethymno
{
	[Serializable]
	public class cVideo
	{
		public int video_id, poi_id, video_order;
		public string video_file;
		public cLocalizedText[] video_title, video_label, video_source;
	}

}
