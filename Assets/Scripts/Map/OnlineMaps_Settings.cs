using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno
{
    public class OnlineMaps_Settings : Singleton<OnlineMaps_Settings>
    {

        protected OnlineMaps_Settings() { }

        [HideInInspector]
        public LayerMask layerUI_ToIgnore;

        private void OnEnable()
        {
            layerUI_ToIgnore = LayerMask.NameToLayer("marker");
        }

        public bool IsMarkerUnderGUI(GameObject go)
        {
            return go.layer == layerUI_ToIgnore;
        }

        //OnlineMapsControlBase
        //IsCursorOnUIElement

    }

}
