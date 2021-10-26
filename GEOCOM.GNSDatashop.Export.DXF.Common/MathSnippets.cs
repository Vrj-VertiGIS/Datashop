using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public static class MathSnippets
    {
        /// <summary>
        ///  Bring angular value (in degrees) to the interval [0..360]
        /// </summary>
        /// <param name="degValue"></param>
        public static double NormalizeDeg(double degValue)
        {
            while (0 > degValue)
                degValue += 360;
            while (360 < degValue)
                degValue -= 360;
            return degValue;
        }

        /// <summary>
        /// Normalize radian value to be within a full circle inverval [0..2*Pi]
        /// </summary>
        /// <param name="radValue"></param>
        /// <returns></returns>
        public static double NormalizeRad(double radValue)
        {
            while (0 > radValue)
                radValue += 2 * Math.PI;
            while (2 * Math.PI < radValue)
                radValue -= 2 * Math.PI;
            return radValue;
        }

        //////////////////////////////////////////////////////////////////////////
        // Loest ein Gleichungssystem mit 2 Unbekannnten.
        // a00, a01, a10, a11: Koeffizientenmatrix
        // c00, c10: Konstantenmatrix
        // c00, c10: Loesungsmatrix
        // True = Loesbar
        public static bool Equation(double a00, double a01, double a10, double a11, double c00, double c10, out double x00, out double x10)
        {
            var rDet = a00 * a11 - a01 * a10;

            var solvable = Math.Abs(rDet) > 1E-10;
            if (solvable)
            {
                x00 = (c00 * a11 - a01 * c10) / rDet;
                x10 = (c10 * a00 - a10 * c00) / rDet;
            }
            else
            {
                x00 = double.NaN;
                x10 = double.NaN;
            }
            return solvable;
        }

    }
}
