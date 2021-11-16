using System;
using System.Collections.Generic;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of the the second schema, with some other extras.
    /// </summary>
    public static class Meltdown_V2
    {
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
        private static string ColorPattern = $"{ccolor}*{Reverse(ccolor)}";
        private static string WebPatternwithText = $"*{llink}*{paramSep}*{Reverse(llink)}*";
        private static string WebPatternwithoutText = $"*{llink}*{Reverse(llink)}*";

        static void  SetDelimters(string formatCsv,string  webColorCsv="(((,{{{")
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
            ccolor= delims[0];
            llink = delims[1];
            string strn = delims[2];
            if (strn.Length == 1)
                paramSep = strn[0];;
        }

        // This is a subset of permitted HTML colors
        private static List<string> colors = new List<string>
        {
            "Aquamarine",
            "Black",
            "Blue",
            "Brown",
            "Cyan",
            "DarkBlue",
            "Gray or Grey",
            "Green",
            "LightBlue",
            "Lime",
            "Magenta",
            "Maroon",
            "Olive",
            "Orange",
            "Pink",
            "Purple",
            "Red",
            "Silver",
            "White",
            "Yellow"
        };

        //Ref: https://stackoverflow.com/questions/228038/best-way-to-reverse-a-string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            string ret =  new string(charArray);
            ret = ret.Replace('[', ']').Replace('(', ')').Replace('{', '}');//.Replace('<','>');
            return ret;
        }

        public static string Parse (string txt)
        {
            string html = "";

            if (string.IsNullOrEmpty(txt))
                return "";

            string[] lines = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            bool inList = false;
            foreach (var _line in lines)
            {
                string line = _line;
                if (string.IsNullOrEmpty(line))
                {
                    if (inList)
                    {
                        html += "\n</ul>\n";
                    }
                    else 
                        html += "\n";
                    continue;
                }
                //Bold, Italicand Underline formatting.
                line = line.Replace(bold, "<b>");
                line = line.Replace(Reverse(bold), "</b>");
                line = line.Replace(italics, "<i>");
                line = line.Replace(Reverse(italics), "</i>");
                line = line.Replace(underline, "<u>");
                line = line.Replace(Reverse(underline), "</u>");

                line = line.Replace(bold_italics, "<b><i>");
                line = line.Replace(Reverse(bold_italics), "</i></b>");
                line = line.Replace(bold_underline, "<b><u>");
                line = line.Replace(Reverse(bold_underline), "</u></b>");
                line = line.Replace(italics_underline, "<i><u>");
                line = line.Replace(Reverse(italics_underline), "</u></i>");
                line = line.Replace(bold_italics_underline, "<b><i><u>");
                line = line.Replace(Reverse(bold_italics_underline), "</u></i></b>");

                line = GetMeltdownFontColors(line);

                // All liknks on the line. Can't extend over one line
                line = GetMeltdownLinks(line);

                // All liknks on the line. Can't extend over one line
                line = GetMarkdownLinks(line);

                string newline = "";
                if (line.Length > 2)
                {
                    if ((line.Substring(0, 2) == "- ") || (line.Substring(0, 2) == "-\t"))
                    {
                        line = line.Substring(2);
                        if (!inList)
                            newline = "\n<ul>\n<li>" + line + "</li>";
                        else
                            newline = "\n<li>" + line + "</li>";
                        inList = true;
                    }
                    else
                    {
                        if (inList)
                            newline = "\n</ul>\n<p>" + line + "</p>";
                        else
                            newline = "\n<p>" + line + "</p>";
                        inList = false;
                    }
                }
                else
                {
                    if (inList)
                        newline = "\n</ul>\n<p>" + line + "</p>";
                    else
                        newline = "\n<p>" + line + "</p>";
                    inList = false;
                }
                html += "\n" + newline;
            }
            if (inList)
                html += "\n</ul>\n";
            return html;
        }

        private static string GetMeltdownLinks(string line)
        {
            // Allow for {{{ txt | URL }}} links
            int start;
            int middle;
            int end;
            string linkText;
            string Url;
            while ((System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(WebPatternwithText, line))
            ||
             (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(WebPatternwithoutText, line)))
            {
                start = line.IndexOf(llink);
                if (start == -1)
                    break;
                end = line.IndexOf(Reverse(llink), start);
                if (end == -1)
                    break;
                middle = line.IndexOf(paramSep, start);
                // Allow for Url only
                if (middle != -1)
                    if (middle > end)
                        middle = -1;
                if (middle != -1)
                {
                    linkText = line.Substring(start + 2, middle - start - 2);
                    //2Do: Remove or error for invalid link text characters.
                    Url = line.Substring(middle + 1, end - middle);
                }
                else
                {
                    Url = line.Substring(start + 2, end - start - 2);
                    linkText = Url;
                }
                string link = $"<a href= \"{Url}\">{linkText}</a>";
                string before = "";
                string after = "";
                if (start != 0)
                    before = line.Substring(0, start);
                if (end != line.Length - 1)
                    after = line.Substring(end + 2);
                line = $"{before}{link}{after}";

            }
            return line;
        }

        private static string GetMarkdownLinks(string line)
        {
            int start;
            int middle;
            int end = 0;
            string linkText;
            string Url;
            while (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("*[*](*)*", line))
            {
                // Got a Markdown URL - "Brute" force approach
                // Find the midlle
                middle = line.IndexOf("](", end);
                if (middle == -1)
                    break;
                // From there find the end
                end = line.IndexOf(')', middle);
                if (end == -1)
                    break;
                // From middle find start
                start = line.LastIndexOf("[", middle);
                if (start == -1)
                    break;
                linkText = line.Substring(start + 1, middle - start - 1);
                Url = line.Substring(middle + 2, end - middle - 2);
                string link = $"<a href= \"{Url}\">{linkText}</a>";
                string before = "";
                string after = "";
                if (start != 0)
                    before = line.Substring(0, start);
                if (end != line.Length - 1)
                    after = line.Substring(end + 1);
                line = $"{before}{link}{after}";
            }
            return line;
        }

        private static string GetMeltdownFontColors(string line)
        {
            if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(ColorPattern, line))
            {
                foreach (var color in colors)
                {
                    string searchForFontColorStart = ccolor + color + Reverse(ccolor);
                    if (line.ToLower().Contains(searchForFontColorStart.ToLower()))
                        line = line.Replace(searchForFontColorStart, "<font color=\"" + $"{color}" + "\">", true, null);
                }
            }
            return line;
        }

        
    }
}
