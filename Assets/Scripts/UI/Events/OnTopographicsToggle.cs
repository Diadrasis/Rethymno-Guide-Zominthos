using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Diadrasis.Rethymno
{

    public class OnTopographicsToggle : MonoBehaviour
    {
        public UnityEvent OnToggle;
        void Start()
        {
            EventHolder.OnTopographicsToggle += TopographicsToggle;
        }


        void TopographicsToggle()
        {
            OnToggle?.Invoke();
        }
    }
}
