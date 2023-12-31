//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace Diadrasis.Rethymno 
{

	public class RequestPermissionScript : MonoBehaviour
	{

        private void Awake()
        {
#if !UNITY_ANDROID
            this.enabled = false;
#endif
        }

#if UNITY_ANDROID

        // "android.permission.VIBRATE" 
        // "android.permission.INTERNET" 
        //"android.permission.READ_MEDIA_AUDIO"
        //"android.permission.READ_MEDIA_IMAGES" 
        //"android.permission.READ_MEDIA_VIDEO"

        //"android.permission.ACCESS_BACKGROUND_LOCATION" //https://developer.android.com/reference/android/Manifest.permission#ACCESS_BACKGROUND_LOCATION

        internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
        }

        internal void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
        }

        internal void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }

        void Start()
        {
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // The user authorized use of the microphone.
            }
            else
            {
                bool useCallbacks = false;
                if (!useCallbacks)
                {
                    // We do not have permission to use the FineLocation.
                    // Ask for permission or proceed without the functionality enabled.
                    Permission.RequestUserPermission(Permission.FineLocation);
                }
                else
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                    callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                    callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                    Permission.RequestUserPermission(Permission.FineLocation, callbacks);
                }
            }
        }



#endif

    }

}

