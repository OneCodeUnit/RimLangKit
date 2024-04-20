﻿using System.Text;
using System.Text.RegularExpressions;

namespace RimLangKit
{
    internal sealed class EncodingFix
    {
        internal static bool EncodingFixProcessing(string currentFile)
        {
            string text = File.ReadAllText(currentFile);
            // Замена табов на пробелы
            text = Regex.Replace(text, "\t+", "  ");

            // Замена окончания строки на CRLF
            text = Regex.Replace(text, "(?<!\r)\n", "\r\n");

            //Сохранение c кодировкой utf-8 + BOM
            var data = Encoding.UTF8.GetBytes(text);
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            var encoder = new UTF8Encoding(true);
            text = encoder.GetString(result);

            File.WriteAllText(currentFile, text);

            return true;
        }
    }
}
