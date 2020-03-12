using System;
using System.Text;
using System.Globalization;

namespace IntegrationTools.DataConversion
{
    public class StringHelper
    {
        public String UTF8ByteArrayToString(Byte[] characters)
        {
            return (new UTF8Encoding()).GetString(characters);
        }

        public Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            return (new UTF8Encoding()).GetBytes(pXmlString);
        }

        public string RemoveUTF8(string bstr)
        {
            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes((bstr).TrimStart().TrimEnd())));
        }

        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
