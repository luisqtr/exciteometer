using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 


*/
namespace ExciteOMeter
{
    public static class Helpers
    {
        public static double RangeMapper(double sourceNumber, double fromA, double fromB, double toA, double toB, int decimalPrecision ) 
        {
            double deltaA = fromB - fromA;
            double deltaB = toB - toA;
            double scale  = deltaB / deltaA;
            double negA   = -1 * fromA;
            double offset = (negA * scale) + toA;
            double finalNumber = (sourceNumber * scale) + offset;
            int calcScale = (int) System.Math.Pow(10, decimalPrecision);
            return (double) System.Math.Round(finalNumber * calcScale) / calcScale;
        }

    }

}

