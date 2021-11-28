using System;
using System.Collections.Generic;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of the the second schema, with some other extras.
    /// </summary>
    public static partial class Meltdown
    {
        /// <summary>
        /// Default delimeters.
        /// Note only supply the left hand delimeter. Right hand is generated from it.
        /// </summary>
        private static string bold = "[[";
        private static string italics = "((";
        private static string underline = "{{";

        // Combinations use a juxtaposition of the second characters of the 3 above
        private static string bold_italics = $"{bold[1]}{italics[1]}";
        private static string bold_underline = $"{bold[1]}{underline[1]}";
        private static string italics_underline = $"{italics[1]}{underline[1]}";
        private static string bold_italics_underline = $"{bold[1]}{italics[1]}{underline[1]}";

        private static string ccolor = "(((";
        private static string llink = "{{{";
        private static char paramSep = '|';

        // Next 3 are used for searches, formed from previous 3
        // Searches use System.IO.Enumeration.FileSystemName.MatchesSimpleExpression()
        private static string ColorPattern = $"*{ccolor}*{paramSep}*{Reverse(ccolor)}*";
        private static string WebPatternwithText = $"*{llink}*{paramSep}*{Reverse(llink)}*";
        private static string WebPatternwithoutText = $"*{llink}*{Reverse(llink)}*";
        private static string HeadingPatternatStartofLine = $"[[?]]*";
        private static string ListPatternatStartofLine = $"((?))*";

        /// <summary>
        /// Change the default delimeters by sending 1 or 2 Csv lists
        /// </summary>
        /// <param name="formatCsv">CSV list of bold,italic,underline strings</param>
        /// <param name="webColorCsv">Csv list of font color and link deleimeters plus arg delimeter</param>
        static void SetDelimters(string formatCsv, string webColorCsv = "(((,{{{,|")
        {
            if (string.IsNullOrEmpty(formatCsv))
                return;
            string[] delims = formatCsv.Split(',');
            if (delims.Length < 3)
                return;
            bold = delims[0];
            italics = delims[1];
            underline = delims[2];
            if (string.IsNullOrEmpty(webColorCsv))
                return;
            delims = webColorCsv.Split(',');
            if (delims.Length < 3)
                return;
            ccolor = delims[0];
            llink = delims[1];
            string strn = delims[2];
            if (strn.Length == 1)
                paramSep = strn[0];
        }

        //Ref: https://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            string ret = new string(charArray);
            ret = ret.Replace('[', ']').Replace('(', ')').Replace('{', '}');//.Replace('<','>');
            return ret;
        }
    }
}
