//Diadrasis ©2023 - Stathis Georgiou
using StaGeGames.BestFit;
using UnityEngine;
using UnityEngine.UI;

namespace Diadrasis.Rethymno
{

    public class PageSetup : MonoBehaviour
    {
        public BestFitter fitter;
        public RawImage rawImage;

        public void Setup(Texture2D tex)
        {
            rawImage.texture = tex;
            fitter.Init();
        }

    }

}
