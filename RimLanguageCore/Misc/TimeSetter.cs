using System;
using System.Globalization;

namespace RimLanguageCore.Misc
{
    public static class TimeSetter
    {
        /// <summary>
        /// Возвращает текущее время в заданном формате с указанным суффиксом.
        /// </summary>
        /// <param name="format">Формат времени.</param>
        /// <param name="suffix">Строка, добавляемая в конце.</param>
        /// <returns>Сформатированная строка времени.</returns>
        public static string PlaceTime(string format = "HH:mm:ss", string suffix = " - ")
        {
            return DateTime.Now.ToString(format, CultureInfo.InvariantCulture) + suffix;
        }
    }
}