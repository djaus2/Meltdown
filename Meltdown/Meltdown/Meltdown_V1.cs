using System;
using System.Collections.Generic;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of the the second schema, with some other extras.
    /// </summary>
    public static class Meltdown
    {

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
                line = line.Replace("[[", "<b>");
                line = line.Replace("]]", "</b>");
                line = line.Replace("((", "<i>");
                line = line.Replace("))", "</i>");
                line = line.Replace("{{", "<u>");
                line = line.Replace("}}", "</u>");

                foreach (var color in colors)
                {
                    string searchForFontColorStart = "[(" + color + ")]";
                    if (line.ToLower().Contains(searchForFontColorStart.ToLower()))
                        line = line.Replace(searchForFontColorStart, "<font color=\"" + $"{color}" + "\">", true, null);
                }
                string searrhForFontEnd = "[()]";
                if (line.ToLower().Contains(searrhForFontEnd.ToLower()))
                    line = line.Replace(searrhForFontEnd, "</font>", true, null);

                // Allow for [{ txt | URL }] links
                int start;
                int middle;
                int end;
                string linkText;
                string Url;
                while ((System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("*[{*|*}]*", line))
                ||
                 (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("*[{*}]*", line)))
                {
                    start = line.IndexOf("[{");
                    end = line.IndexOf("}]", start);
                    if (end == -1)
                        break;
                    middle = line.IndexOf('|', start);
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


                // If Got a Markdown URL - "Brute" force approach.
                end = 0;
                while (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("*[*](*)*", line))
                {
                    // Got a Markdown URL - "Brute" force approach
                    middle = line.IndexOf("](", end);
                    if (middle == -1)
                        break;
                    end = line.IndexOf(')', middle);
                    if (end == -1)
                        break;
                    start = line.LastIndexOf("[{", middle);
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
        
    }
}
