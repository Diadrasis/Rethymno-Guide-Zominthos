using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeGames.BestFit.Utils
{

    public class CommonUtilities : MonoBehaviour
    {
        public static bool IsEditor()
        {
            return Application.isEditor;
        }

        public static readonly string baseclass = "BestResize";

    }

}
