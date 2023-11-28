//Diadrasis ©2023
using Diadrasis.Rethymno;
using StaGeGames.BestFit.Extens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno 
{

	public class InfoTextsController : MonoBehaviour
	{
        [Space]
        public EnumsHolder.ApplicationMode appModeNow = EnumsHolder.ApplicationMode.GUIDE;

        public TMPro.TextMeshProUGUI titleText, descShortText, descFullText, testimonyText, bibliographyText;

        public GameObject ShortDescParent, DescParent, SpaceDesc, BibliographyParent;

        private string _title, _shortDesc, _fullDesc, _testimony, _bibliography;

        [Space]
        public Image btnShowBackground, btnHideBackground, btnCloseBackground;

        [Space] public Image narrationBackground, narrationPlayIcon, narrationPauseIcon;

        [Header("Short Desc Background Color")]
        public Image shortDescbackground, titleBackground;
        public Color colShortDescPoisHalf, colShortDescCommonHalf;
        public Color colShortDescPoisFull, colShortDescCommonFull;

        private Color colDefaultHalf, colDeafaultFull;

        private int currentPoiId = -100;
        public bool IsSamePoiSelected(int id)
        {
            if (currentPoiId == id) return true;
            currentPoiId = id;
            return false;
        }

        [Space]
        [ReadOnly]
        public bool IsFromPOI;

        private void Awake()
		{
            EventHolder.OnApplicationModeChanged += OnApplicationModeChanged;

            EventHolder.OnNativePoiAreaClick += OnNativePoiAreaClick;
            EventHolder.OnGuiMarkerClick += OnMarkerClicked;

            EventHolder.OnARInfoTriggered += OnARInfoTriggered;

            EventHolder.OnNativePoiMarkerClick += OnNativePoiMarkerClick;

            EventHolder.OnRouteInfoShow += SetFromRoute;
            EventHolder.OnPeriodInfoShow += SetFromPeriod;
            EventHolder.OnAreaInfoShow += SetFromArea;

            EventHolder.OnPoiInfoClosed += SetPoiSelectedToNull;

            EventHolder.OnInfoShowHalf += OnInfoShowHalf;
            EventHolder.OnInfoShowFull += OnInfoShowFull;

            colDefaultHalf = colShortDescCommonHalf;
            colDeafaultFull = colShortDescCommonFull;
            
        }

        void OnApplicationModeChanged(EnumsHolder.ApplicationMode mode)
        {
            appModeNow = mode;

        }

        void OnInfoShowHalf()
        {
            SetShortDescColorsHalf();
        }

        void OnInfoShowFull()
        {
            SetShortDescColorsFull();
        }

        void OnNativePoiMarkerClick(PoiEntity poiEntity)
        {
            if (poiEntity == null) return;
            //if (IsSamePoiSelected(poiEntity.poi.poi_id)) return;
            SetFromPoi(poiEntity);
        }

        void OnNativePoiAreaClick(AreaEntity areaEntity)
        {
            if (areaEntity == null) return;
            SetFromArea(areaEntity);
        }

        void OnARInfoTriggered(MarkerInstance marker)
        {
            if (appModeNow != EnumsHolder.ApplicationMode.AR) return;
            OnMarkerClicked(marker);
        }

        void OnMarkerClicked(MarkerInstance marker)
        {
            if (marker == null || marker.data == null) return;
            if (marker.data.IsArea)
            {
                SetFromArea(marker.data.areaEntity);
            }
            else
            {
                if (marker.data.poiEntity == null) return;
                //if (IsSamePoiSelected(marker.data.poiEntity.poi.poi_id)) return;
                SetFromPoi(marker.data.poiEntity);
            }
        }

        void SetFromArea(AreaEntity areaEntity)
        {
            ClearTexts();

            if (areaEntity == null || areaEntity.IsNull()) return;

            IsFromPOI = false;

            colShortDescCommonHalf = colDefaultHalf;
            colShortDescCommonFull = colDeafaultFull;

            SetShortDescColorsHalf();

            _title = DataManager.Instance.GetTraslatedText(areaEntity.area.area_title);
            _shortDesc = DataManager.Instance.GetTraslatedText(areaEntity.area.area_short_description);
            _fullDesc = DataManager.Instance.GetTraslatedText(areaEntity.area.area_description);

            if(appModeNow == EnumsHolder.ApplicationMode.AR)
            {
                _fullDesc = _fullDesc.Insert(0, _shortDesc);
                _shortDesc = string.Empty;
            }

            titleText.text = FixText(_title);
            descShortText.text = FixText(_shortDesc);
            descFullText.text = FixText(_fullDesc);

            Recalculate();
        }

        void SetFromRoute(RouteEntity routeEntity)
        {
            ClearTexts();

            if (routeEntity == null || routeEntity.IsNull()) return;

            IsFromPOI = false;
            colShortDescCommonHalf = colDefaultHalf;
            colShortDescCommonFull = colDeafaultFull;

            SetShortDescColorsHalf();

            _title = DataManager.Instance.GetTraslatedText(routeEntity.route.route_title);
            _shortDesc = DataManager.Instance.GetTraslatedText(routeEntity.route.route_short_description);
            _fullDesc = DataManager.Instance.GetTraslatedText(routeEntity.route.route_description);
            _bibliography = routeEntity.route.route_bibliography;

            titleText.text = FixText(_title);
            descShortText.text = FixText(_shortDesc);
            descFullText.text = FixText(_fullDesc);
            bibliographyText.text = FixText(_bibliography);

            Recalculate();
        }

        void SetFromPeriod(PeriodEntity periodEntity)
        {
            ClearTexts();

            if (periodEntity == null || periodEntity.IsNull()) return;

            IsFromPOI = false;
            colShortDescCommonHalf = colDefaultHalf;
            colShortDescCommonFull = colDeafaultFull;

            SetShortDescColorsHalf();

            _title = DataManager.Instance.GetTraslatedText(periodEntity.period.period_title);
            _shortDesc = DataManager.Instance.GetTraslatedText(periodEntity.period.period_short_description);
            //_fullDesc = null for period

            if (appModeNow == EnumsHolder.ApplicationMode.AR)
            {
                _fullDesc = _fullDesc.Insert(0, _shortDesc);
                _shortDesc = string.Empty;
            }

            titleText.text = FixText(_title);
            descShortText.text = FixText(_shortDesc);
            descFullText.text = FixText(_fullDesc);

            Recalculate();
        }

        void SetFromPoi(PoiEntity poiEntity)
        {
            ClearTexts();

            if (poiEntity == null) return;

            IsFromPOI = true;

            Color periodColor = GetPoiColor(poiEntity.poi.period_id, true);

            colShortDescCommonHalf = periodColor;
            colShortDescCommonFull = periodColor;
           
            SetShortDescColorsHalf();

            _title = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_title);
            _shortDesc = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_short_description);
            _fullDesc = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_description);
            _testimony = DataManager.Instance.GetTraslatedText(poiEntity.poi.poi_testimony);
            _bibliography = poiEntity.poi.poi_bibliography;

            if (appModeNow == EnumsHolder.ApplicationMode.AR)
            {
                _fullDesc = _fullDesc.Insert(0, _shortDesc);
                _shortDesc = string.Empty;
            }

            titleText.text = FixText(_title);
            descShortText.text = FixText(_shortDesc);
            descFullText.text = FixText(_fullDesc);
            testimonyText.text = FixText(_testimony);
            bibliographyText.text = FixText(_bibliography);

            Recalculate();
        }

        void ClearTexts() 
        { 
            _title = _shortDesc = _fullDesc = _testimony = string.Empty;
            titleText.text = _title;
            descShortText.text = _shortDesc;
            descFullText.text = _fullDesc;
            testimonyText.text = _testimony;
        }

        void Recalculate()
        {
            titleText.rectTransform.ForceRebuildLayout();

            ShortDescParent.SetActive(!_shortDesc.IsNull());
            if (ShortDescParent.activeSelf)
                descShortText.rectTransform.ForceRebuildLayout();

            DescParent.SetActive(!_fullDesc.IsNull());
            SpaceDesc.SetActive(DescParent.activeSelf);
            //descFullText.transform.parent.gameObject.SetActive(!string.IsNullOrWhiteSpace(_fullDesc));
            if (DescParent.activeSelf)
                descFullText.rectTransform.ForceRebuildLayout();

            BibliographyParent.SetActive(!_bibliography.IsNull());
            if (BibliographyParent.activeSelf)
                bibliographyText.rectTransform.ForceRebuildLayout();

            testimonyText.transform.parent.gameObject.SetActive(!_testimony.IsNull());
            if (testimonyText.transform.parent.gameObject.activeSelf)
                testimonyText.rectTransform.ForceRebuildLayout();
        }

        void SetShortDescColorsHalf()
        {
            narrationPauseIcon.color = colShortDescCommonHalf;
            narrationPlayIcon.color = colShortDescCommonHalf;
            narrationBackground.color = colShortDescCommonHalf;
            titleBackground.color = colShortDescCommonHalf;
            shortDescbackground.color = IsFromPOI ? colShortDescPoisHalf : colShortDescCommonHalf;
            btnCloseBackground.color = colShortDescCommonHalf;
            btnHideBackground.color = colShortDescCommonHalf;
            btnShowBackground.color = colShortDescCommonHalf;
            descShortText.color = IsFromPOI ? Color.black : Color.white;
        }

        void SetShortDescColorsFull()
        {
            narrationPauseIcon.color = colShortDescCommonFull;
            narrationPlayIcon.color = colShortDescCommonFull;
            narrationBackground.color = colShortDescCommonFull;
            titleBackground.color = colShortDescCommonFull;
            shortDescbackground.color = IsFromPOI ? colShortDescPoisFull : colShortDescCommonFull;
            btnCloseBackground.color = colShortDescCommonFull;
            btnHideBackground.color = colShortDescCommonFull;
            btnShowBackground.color = colShortDescCommonFull;
            descShortText.color = IsFromPOI ? Color.black : Color.white;
        }

        //[Space]
        //public Color colOttoman;
        //public Color colVenetian, colContemporary;
            

        Color GetPoiColor(int periodID, bool isHalf)
        {
            PeriodEntity period = DataManager.Instance.GetPeriodEntity(periodID);

            Color myColor = isHalf ? colShortDescCommonHalf : colShortDescCommonFull;

            //if period has color return period_color else return myColor
            return period.period.period_color.HexToColor(myColor);

            //switch (periodID)
            //{
            //    case 1684://Ottoman
            //        //ColorUtility.TryParseHtmlString("#419E99", out myColor);
            //        return colOttoman;// myColor;
            //    case 1685://Venetian
            //        //ColorUtility.TryParseHtmlString("#DDBC9B", out myColor);
            //        return colVenetian;// myColor;
            //    case 1686://Contemporary
            //        //ColorUtility.TryParseHtmlString("#85B2C6", out myColor);
            //        return colContemporary;// myColor;
            //    default:
            //        return myColor;
            //}
        }

        void SetPoiSelectedToNull() { currentPoiId = -100; }

        string FixText(string val)
        {
            if (val.IsNull()) return string.Empty;
            return val.Replace("(CL)", "\n").Replace("(Q)", "\"").Replace("<em>", "<i>").Replace("</em>", "</i>");
        }

    }

}
