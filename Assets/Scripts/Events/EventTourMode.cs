//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Diadrasis.Rethymno 
{

	public class EventTourMode : MonoBehaviour
	{
		[Header("Visibility >> onsite = true, offsite = false")]
		public bool useSimpleVisibility;
		[Header("Invert visibility >> onsite = false, offsite = true")]
		public bool invertSimpleVisibility;

        [Space]
        public UnityEvent eventOnSite;
        public UnityEvent eventOffSite;

        private void Awake()
        {
			EventHolder.OnTourChanged += OnTourChanged;
		}

		void OnTourChanged(EnumsHolder.TourMode tourMode)
		{

			CheckSimpleVisibility(tourMode);

			if(tourMode== EnumsHolder.TourMode.OffSite)
            {
				eventOffSite?.Invoke();
            }
            else
            {
				eventOnSite?.Invoke();
            }
		}

		void CheckSimpleVisibility(EnumsHolder.TourMode tourMode)
        {
            if (useSimpleVisibility)
            {
                if (invertSimpleVisibility)
                {
                    switch (tourMode)
                    {
                        case EnumsHolder.TourMode.OffSite:
                        default:
                            gameObject.SetActive(true);
                            break;
                        case EnumsHolder.TourMode.OnSite:
                            gameObject.SetActive(false);
                            break;
                        
                    }
                }
                else
                {
                    switch (tourMode)
                    {
                        case EnumsHolder.TourMode.OffSite:
                        default:
                            gameObject.SetActive(false);
                            break;
                        case EnumsHolder.TourMode.OnSite:
                            gameObject.SetActive(true);
                            break;

                    }
                }
            }
        }
    }

}
