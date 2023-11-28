//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno
{

    [CreateAssetMenu(fileName = "New_Message_Item", menuName = "Messages/New Item")]
    [Serializable]
    public class MessageItem : ScriptableObject
    {
        public string key;
    }

}
