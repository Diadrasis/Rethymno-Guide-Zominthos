//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno
{

    [CreateAssetMenu(fileName = "Help_Database", menuName = "Help/New Help database")]
    [Serializable]
    public class HelpDatabase : ScriptableObject
    {
        public Transform prefabPage;
        [Space]
        public List<Texture2D> pages_textures = new List<Texture2D>();

    }

}
