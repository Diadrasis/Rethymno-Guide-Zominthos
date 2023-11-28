//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diadrasis.Rethymno 
{

	public class DeviceChecker : Singleton<DeviceChecker>
	{
		[ReadOnly]
		public int processorCount, systemMemorySize, graphicsMemorySize;
        [ReadOnly]
		public int maxTextureSize;
		[ReadOnly]
		public bool graphicsMultiThreaded;
		[ReadOnly]
		public bool supportsLocationService, supportsAccelerometer, supportsGyroscope, supportsVibration;

        void Awake()
		{
            systemMemorySize = SystemInfo.systemMemorySize;
            graphicsMemorySize = SystemInfo.graphicsMemorySize;
			graphicsMultiThreaded = SystemInfo.graphicsMultiThreaded;
			processorCount = SystemInfo.processorCount;
			supportsAccelerometer = SystemInfo.supportsAccelerometer;
			supportsVibration = SystemInfo.supportsVibration;
            supportsGyroscope = SystemInfo.supportsGyroscope;
			supportsLocationService = SystemInfo.supportsLocationService;
			maxTextureSize = SystemInfo.maxTextureSize;
        }

        //private void Start()
        //{
			//PrintCustom(nameof(systemMemorySize) + "=" + systemMemorySize);
            //PrintCustom(nameof(graphicsMemorySize) + "=" + graphicsMemorySize);
            //PrintCustom(nameof(graphicsMultiThreaded) + "=" + graphicsMultiThreaded);
            //PrintCustom(nameof(processorCount) + "=" + processorCount);
            //PrintCustom(nameof(maxTextureSize) + "=" + maxTextureSize);
        //}

        public bool HasGyro { get { return supportsGyroscope;} }
        public bool HasVibration { get { return supportsVibration; } }
		public int MaxTextureSize { get { return maxTextureSize; } }

		void PrintCustom(string message)
		{
			Debug.Log(message);
		}
    }

}
