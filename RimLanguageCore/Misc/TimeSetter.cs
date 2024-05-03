using System;
using System.Globalization;

namespace RimLanguageCore.Misc
{
    public static class TimeSetter
    {
        public static string PlaceTime()
        {
            DateTime time = DateTime.Now;
            string result = time.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + " - ";
            return result;
        }
    }
}