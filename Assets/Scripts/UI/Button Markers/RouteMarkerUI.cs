//Diadrasis ©2023

namespace Diadrasis.Rethymno 
{

	public class RouteMarkerUI : ButtonMarkerBase
	{
		public void Init(RouteEntity routeEntity)
        {
			btn.onClick.RemoveAllListeners();
			SetText(routeEntity.GetRouteTitle());
			SetTexture(routeEntity.GetRouteIcon(), false);
			btn.onClick.AddListener(() => EventHolder.OnRouteSelected?.Invoke(routeEntity));
		}

	}

}
