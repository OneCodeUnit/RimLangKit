using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace RimLanguageCore.Activities
{
    public static class NamesTranslator
    {
        private static readonly string line4 = "land:ленд,aire:эр,lare:лер,augh:о,aughe:о,wich:идж,chia:шия,chio:шио,eigh:ай,ewer:оуэр,tion:шен,oore:ор,ough:ау,ower:оуэр,ture:чер";
        private static readonly string line3 = "age:идж,ate:эйт,ham:ем,ain:ен,air:эр,ayr:эр,are:эр,lar:лер,cia:шия,ear:ир,eer:ир,eir:ир,ere:ир,ewe:оуэр,iar:айар,ier:ир,igh:ай,mpb:мпб,oar:ор,oor:ор,or:ор,ore:ор,ou:а,our:ор,tch:тч,gue:г,que:к,gua:га,wor:уор,shh:щ";
        private static readonly string line2 = "aa:аа,wa:уо,ai:и,ay:и,ae:а,æ:а,al:ол,ar:эр,au:о,aw:о,ci:си,ce:се,cy:си,ch:ч,ck:к,cz:ц,ea:е,ee:и,ei:и,eo:е,er:ар,eu:ю,ew:ю,ey:и,ge:джо,gi:джи,gy:джи,gg:гг,ia:иа,ie:айе,io:айо,ir:ир,ire:ир,irr:ирр,ll:лл,ng:нг,oa:о,oe:и,œ:и,oi:ой,oy:ой,oo:у,ow:оу,ph:ф,qu:к,sh:ш,ss:сс,th:т,ts:тс,tz:ц,ue:е,ui:и,ur:эр,yr:эр,ya:я,yu:ю,yo:ё,zh:ж";
        private static readonly string line1 = "a:э,b:б,c:к,d:д,e:е,f:ф,g:г,h:х,i:ай,j:дж,k:к,l:л,m:м,n:н,o:о,p:п,q:к,r:р,s:с,t:т,u:у,v:в,w:в,x:кс,y:ай,z:з,ž:ж,č:ч,š:ш,ŝ:щ,è:э,û:ю,â:я,ë:ё,é:е,ï:и,á:э,ö:ё,ç:ц,ó:о,ä:э,ü:у,ã:ы,ô:о,í:и,ò:о";
        private static readonly string[] lines = { line4, line3, line2, line1 };
        private static readonly string lineClear = "se:s,gh:=,lh:l,rh:r,kn:n,mn:n,pn:n,ps:s,pt:t";
        private static readonly string lineStart = "aa:аа,a:эй,ae:и,æ:и,ai:эй,ay:эй,ire:айр,qu:ку,w:=";
        private static readonly string lineEnd = "e:=,gh:ф,h:=,ia:ия,ied:ид,ies:ис";

        public static (bool, string) NamesTranslatorActivity(string currentFile)
        {
            // Заполнение словаря
            Dictionary<string, string> dictionary = new(SetDictionary(lines));
            Dictionary<string, string> dictionaryClear = new(SetDictionary(lineClear));
            Dictionary<string, string> dictionaryStart = new(SetDictionary(lineStart));
            Dictionary<string, string> dictionaryEnd = new(SetDictionary(lineEnd));

            // Открытие файла
            StreamReader sourceText = new(currentFile);
            string textFileRus = currentFile.Replace(".txt", "_NEW.txt");
            StreamWriter translatedText = new(textFileRus);

            // Процесс транслитерации
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            int errors = 0;
            while (true)
            {
                // Чтение строки
                string sourceLine = sourceText.ReadLine().Trim();
                if (sourceLine is null) break;

                foreach (KeyValuePair<string, string> item in dictionaryStart)
                {
                    if (sourceLine.StartsWith(item.Key, comparison))
                    {
                        sourceLine = sourceLine.Replace(item.Key, item.Value);
                        break;
                    }
                }
                foreach (KeyValuePair<string, string> item in dictionaryEnd)
                {
                    if (sourceLine.EndsWith(item.Key, comparison))
                    {
                        sourceLine = sourceLine.Replace(item.Key, item.Value);
                        break;
                    }
                }
                foreach (KeyValuePair<string, string> item in dictionaryClear)
                {
                    if (sourceLine.Contains(item.Key))
                    {
                        sourceLine = sourceLine.Replace(item.Key, item.Value);
                    }
                }

                foreach (KeyValuePair<string, string> item in dictionary)
                {
                    if (sourceLine.Contains(item.Key))
                    {
                        sourceLine = sourceLine.Replace(item.Key, item.Value);
                    }
                }

                for (int i = 0; i < sourceLine.Length; i++)
                {
                    if (sourceLine[i] == '=')
                    {
                        sourceLine = sourceLine.Replace(sourceLine[i].ToString(), string.Empty);
                    }
                }
                if (sourceLine.Length > 1)
                {
                    sourceLine = char.ToUpper(sourceLine[0], CultureInfo.InvariantCulture) + sourceLine[1..];
                }
                else if (sourceLine.Length == 1)
                {
                    sourceLine = sourceLine.ToUpper(CultureInfo.InvariantCulture);
                }
                else
                {
                    sourceLine = "Ошибка";
                    errors++;
                }
                translatedText.WriteLine(sourceLine);
            }

            // Завершение работы
            sourceText.Close();
            translatedText.Close();
            if (errors == 0)
                return (true, string.Empty);
            else
                return (false, errors + " имён записано с ошибкой. Ищите слова \"Ошибка\" в получившихся файлах");
        }

        private static Dictionary<string, string> SetDictionary(string line)
        {
            Dictionary<string, string> dictionary = new();
            foreach (string item in line.Split(","))
            {
                string[] symbols = item.Split(":");
                dictionary.Add(symbols[0], symbols[1]);
            }
            return dictionary;
        }

        private static Dictionary<string, string> SetDictionary(string[] lines)
        {
            Dictionary<string, string> dictionary = new();
            foreach (string line in lines)
            {
                foreach (string item in line.Split(","))
                {
                    string[] symbols = item.Split(":");
                    dictionary.Add(symbols[0], symbols[1]);
                }
            }
            return dictionary;
        }
    }
}