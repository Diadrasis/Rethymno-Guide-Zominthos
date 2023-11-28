//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class FullScreenToggler : MonoBehaviour
	{

		void Update()
		{
#if UNITY_STANDALONE_WIN
            if (Input.GetKey(KeyCode.F11))
            {
                //Screen.fullScreenMode = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;\
                ToggleFullScreen();
            }
#endif
        }

        public static void ToggleFullScreen()
        {
           // Screen.fullScreenMode = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

            if(Screen.fullScreen)
            {
                Screen.SetResolution(1680, 945, false);
            }
            else
            {
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            }
        }

    }

}
