//Diadrasis ©2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class EnumsHolder : MonoBehaviour
	{
		public enum FontSizeState { NORMAL, MEDIUM, LARGE }
		public enum DragDirection { Left, Right, Up, Down }
		public enum Platform { PC, MOBILE }
        public enum Language { GREEK, ENGLISH, FRENCH, GERMAN, RUSSIAN }
		//Initializing, UpdateCheck, Updating, Loading, SplashScreen,
		public enum AppState { None, AreasView, RoutesView, PeriodsView, PoisView, PoiSelected, AR, MR }
		public enum RouteType { None, InfoMonuments, VisitMuseum, FollowNarrator, StoriesOfDecayAndConservation, AncientGrafiti }
		public enum InfoPanelState { Closed, HalfOpen, FullOpen }
        public enum GeneralPanelState { Close, Open }
		public enum MapLayerType { Custom, Satellite }
        public enum GpsStatus { OFF, ON }
		public enum LocationMode { NULL, NEAR_AREA, NEAR_POI, FAR }
		public enum TourMode { OffSite, OnSite }
        public enum TriggerMode { GPS, BEACON }
        public enum MapsProvider { google, custom}
		public enum IconType { NULL, ROUTE, PERIOD, POI, POI_ACTIVE, POI_VISITED}

		/// <summary>
		/// MoveFirst => move and when stop then zoom
		/// ZoomFirst => zoom and when stop then move
		/// MoveAndZoom => move and zoom at the same time
		/// </summary>
		public enum SmoothType { MoveFirst, ZoomFirst, MoveAndZoom }

		public enum TermsId { }

		public enum FileType { UNKNOWN, IMAGE, AUDIO, VIDEO, JSON }

		public enum ApplicationMode { GUIDE, AR }
			 
	}

}
