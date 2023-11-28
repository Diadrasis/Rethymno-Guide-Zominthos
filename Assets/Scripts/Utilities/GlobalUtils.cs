//Diadrasis ©2023
using System;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class GlobalUtils : MonoBehaviour
	{
		public static readonly string langEN = "en";
		public static readonly string langGR = "gr";

		//texture for empty or null strings
		public static readonly string iconMarkerEmpty = "default/no_marker";
        public static readonly string iconMarkerButtonEmpty = "default/no_marker_button";
        public static readonly string textureEmpty = "default/no_image";
        public static readonly string overlayMapDefault = "default/spinalonga_map_v2";

        public static string GetDefaultPeriodIcon(int id)
        {
            switch (id)
            {
                case 1684:
                    return "default/period_icon_ottoman";
                case 1685:
                    return "default/period_icon_venetian";
                case 1686:
                    return "default/period_icon_contemporary";
                default:
                    return iconMarkerEmpty;
            }
        }

        public static readonly string serverFolder = "";//http://139.162.183.176/download/

        public static readonly string updateVersionNumber = "1.0.0";
        public static readonly string jsonUpdatesFilename = "updates_version.json";

        public static readonly string jsonAreasFilename = "guide_areas.json";
        public static readonly string jsonRoutesFilename = "guide_routes.json";
        public static readonly string jsonRouteTypesFilename = "guide_route_types.json";
        public static readonly string jsonPeriodsFilename = "guide_periods.json";
        public static readonly string jsonPoisFilename = "guide_pois.json";
        public static readonly string jsonImagesFilename = "guide_images.json";
        public static readonly string jsonVideosFilename = "guide_videos.json";
        public static readonly string jsonBeaconsFilename = "guide_beacons.json";

        public static readonly string[] jsonExportFiles = new string[]
        {
            jsonAreasFilename, jsonRoutesFilename, jsonRouteTypesFilename, 
            jsonPeriodsFilename, jsonPoisFilename, jsonImagesFilename, 
            jsonVideosFilename
        };

        #region Terms

        //Terms Info
        public static readonly string termSelectArea = "select_area";
		public static readonly string termSelectRouteType = "select_route_type";
		public static readonly string termSelectPeriod = "select_period";
		public static readonly string termSelectPoi = "select_poi";

        //Terms GPS


        //Terms App


        #endregion
                

        /// <summary>
        /// returns the text for current language
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="localizedTexts"></param>
        /// <returns></returns>
        public static string GetText(string lang, cLocalizedText[] localizedTexts)
		{
			string val = string.Empty;
			foreach (cLocalizedText _text in localizedTexts)
			{
				if (_text.key == lang) val = _text.text;
			}
			return val;
		}


        /// <summary>
        /// can be better >> don't check route type id because in next update could be different 
        /// check if route_type_poi_icon is empty or null
        /// </summary>
        /// <param name="route_type_id"></param>
        /// <returns></returns>
        public static bool HasRouteTypePeriods(cRouteType route_type) 
        {
            //if no periods for this route
            return route_type.route_type_poi_icon.IsNull();
        }

        /// <summary>
        /// An object that is equivalent to the version number specified in the input parameter.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        ///https://learn.microsoft.com/en-us/dotnet/api/system.version.parse?view=net-5.0
        public static System.Version GetVersion(string val)
        {
            try
            {
                Version ver = Version.Parse(val);
                //Debug.LogWarningFormat("Converted '{0} to {1}.", val, ver);
                return ver;
            }
            catch (ArgumentNullException)
            {
                Debug.LogWarning("Error: String to be parsed is null.");
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogWarningFormat("Error: Negative value in '{0}'.", val);
            }
            catch (ArgumentException)
            {
                Debug.LogWarningFormat("Error: Bad number of components in '{0}'.", val);
            }
            catch (FormatException)
            {
                Debug.LogWarningFormat("Error: Non-integer value in '{0}'.", val);
            }
            catch (OverflowException)
            {
                Debug.LogWarningFormat("Error: Number out of range in '{0}'.", val);
            }
            return null;
        }

    }

}
