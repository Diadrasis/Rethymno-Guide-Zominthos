//Diadrasis ©2023 - Stathis Georgiou
using System;

namespace Diadrasis.Rethymno
{

    public static class MathExtensions
	{
        public enum SizeUnits { Byte, KB, MB, GB, TB, PB, EB, ZB, YB }

        /// <summary>
        /// byte[] length to file size
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string ToSize(this int value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        }
    }

}

