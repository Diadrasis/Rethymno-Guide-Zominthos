//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class MarkerEngineBase : MonoBehaviour
	{
        public MapController mapController;
        public MarkerEngineManager engineManager;

        protected OnlineMaps map;
        protected OnlineMapsTileSetControl control;

        protected OnlineMapsRawImageTouchForwarder forwarder;//???

        protected void Start()
        {
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;
        }

        /// <summary>
        /// filters all pois for wrong data (e.g. far away position)
        /// </summary>
        /// <param name="pois"></param>
        /// <returns>New POIs and Best ZOOM, POSITION for them</returns>
        protected List<PoiEntity> GetFixedPois(List<PoiEntity> pois, Vector2 areaCenter)
        {
            List<PoiEntity> newPois = new List<PoiEntity>();

            bool applyFilter = areaCenter != Vector2.one && engineManager.PoiSettings().dontCreateMarkerIfFarFromArea;
            foreach (PoiEntity data in pois)
            {
                if (applyFilter)
                {
                    // Calculate the distance in km
                    float distance = OnlineMapsUtils.DistanceBetweenPoints(data.poi.geoPosition, areaCenter).magnitude;
                    if (distance > engineManager.PoiSettings().maxKmDistFromArea)
                    {
#if UNITY_EDITOR
                        if (mapController.settings.EditorDebug)
                        {
                            Debug.Log("<color=orange>[ERROR POI FAR AWAY]</color>  dist = " + distance + " km - " + DataManager.Instance.GetTraslatedText(data.poi.poi_title));
                        }
#endif
                        continue;
                    }
                }
                if (!newPois.Contains(data)) newPois.Add(data);
            }

            return newPois;
        }


    }

}

