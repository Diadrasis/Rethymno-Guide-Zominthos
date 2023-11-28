//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Diadrasis.Rethymno 
{

	public class UI_AppStateEvent : MonoBehaviour
	{
        public EnumsHolder.AppState appState = EnumsHolder.AppState.None;

        [Space]
        public UnityEvent OnAreasView;
        public UnityEvent OnRoutesView, OnPeriodsView, OnPoisView, OnPoiSelected;

        public void Init()
        {
            EventHolder.OnStateChanged += OnStateChanged;
        }

        void OnStateChanged(EnumsHolder.AppState _appState)
        {
            appState = _appState;

            switch (appState)
            {
                case EnumsHolder.AppState.None:
                    break;
                case EnumsHolder.AppState.AreasView:
                    OnAreasView?.Invoke();
                    break;
                case EnumsHolder.AppState.RoutesView:
                    OnRoutesView?.Invoke();
                    break;
                case EnumsHolder.AppState.PeriodsView:
                    OnPeriodsView?.Invoke();
                    break;
                case EnumsHolder.AppState.PoisView:
                    OnPoisView?.Invoke();
                    break;
                case EnumsHolder.AppState.PoiSelected:
                    OnPoiSelected?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }

}
