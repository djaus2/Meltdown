using System;
using System.Collections.Generic;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of the the second schema, with some other extras.
    /// </summary>
    public static class Meltdown
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
                paramSep = strn[0]; ;
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
            string ret = new string(charArray);
            ret = ret.Replace('[', ']').Replace('(', ')').Replace('{', '}');//.Replace('<','>');
            return ret;
        }

        public static string Parse(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return "";

            string html = "";
            string[] lines = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            bool inList = false;
            int ExntendedListLevel = 0;
            bool inTable = false;
            foreach (var _line in lines)
            {
                bool isHeading = false;
                string line = _line; // line get modified hence need copy of the ierator
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

                line = Check4HeadinsatStartof(line, ref isHeading, ref inList);


                // List can be created two ways:
                // (i) Start line with -<space> or -<tab>
                // This only can create first level list
                // (ii) Start line with ((n)) where n is the list level 0..9
                // Can creat multilevel lists.
                // Nb: List modes are mutually exclusive.
                if (!isHeading)
                {
                    if ((!inList) && (ExntendedListLevel == 0))
                        line = Check4TableatStartof(line, ref html, ref inTable);
                    if (!inTable)
                    {
                        if (!inList)
                            line = Check4ListatStartof(line, ref ExntendedListLevel);
                        if (ExntendedListLevel == 0)
                            line = DoBullet(line, ref inList);
                    }
                }

                // Font color begin-end on same line
                line = GetMeltdownFontColors(line);
                // Nb: Each paragraph requires a separate font color.


                // All links on the line. Can't extend over one line
                line = GetMeltdownLinks(line);

                // All liknks on the line. Can't extend over one line
                line = GetMarkdownLinks(line);



                // Format begin-end on same line
                line = MapMeltdownFormat2Html(line);

                html += "\n" + line;
            }
            if (inList)
                html += "\n</ul>\n";
            else if (ExntendedListLevel > 0)
            {
                string post = "";
                for (int i = ExntendedListLevel; i != 0; i--)
                    post += "</ul>\n";
                html += post;
                ExntendedListLevel = 0;
            }
            else if (inTable)
                html += "\n</table\n";
            return html;
        }

        private static string DoBullet(string line, ref bool inList)
        {
            if ((line.Length > 2)
                    &&
            ((line.Substring(0, 2) == "- ") || (line.Substring(0, 2) == "-\t")))
            {
                line = line.Substring(2);
                if (!inList)
                    line = "\n<ul>\n<li>" + line + "</li>";
                else
                    line = "<li>" + line + "</li>";
                inList = true;
            }
            else
            {
                if (inList)
                    line = "</ul>\n" + line + "";
                else
                    line = "\n<p>" + line + "</p>";
                inList = false;
            }

            return line;
        }

        private static string MapMeltdownFormat2Html(string line)
        {
            line = line.Replace(bold_italics_underline, "<b><i><u>");
            line = line.Replace(Reverse(bold_italics_underline), "</u></i></b>");

            line = line.Replace(bold_italics, "<b><i>");
            line = line.Replace(Reverse(bold_italics), "</i></b>");
            line = line.Replace(bold_underline, "<b><u>");
            line = line.Replace(Reverse(bold_underline), "</u></b>");
            line = line.Replace(italics_underline, "<i><u>");
            line = line.Replace(Reverse(italics_underline), "</u></i>");


            //Bold, Italicand Underline formatting.
            line = line.Replace(bold, "<b>");
            line = line.Replace(Reverse(bold), "</b>");
            line = line.Replace(italics, "<i>");
            line = line.Replace(Reverse(italics), "</i>");
            line = line.Replace(underline, "<u>");
            line = line.Replace(Reverse(underline), "</u>");



            //line = line.Replace(strikethru,,,,)
            //line = line.Replace(Reverse(strikethru), ...);
            return line;
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
                    linkText = line.Substring(start + llink.Length, middle - start - llink.Length);
                    //2Do: Remove or error for invalid link text characters.
                    Url = line.Substring(middle + 1, end - middle - 1);
                }
                else
                {
                    Url = line.Substring(start + llink.Length, end - start - llink.Length);
                    linkText = Url;
                }
                string link = $"<a href= \"{Url}\">{linkText}</a>";
                string before = "";
                string after = "";
                if (start != 0)
                    before = line.Substring(0, start);
                if (end != line.Length - 1)
                    after = line.Substring(end + llink.Length);
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
            while (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(ColorPattern, line))
            {
                foreach (var color in colors)
                {
                    string searchForFontColorStart = ccolor + color.ToLower() + paramSep;
                    bool searchagain = true;
                    while (searchagain)
                    {
                        // serachagain: Having found the color there might be more instances on same line.
                        int start = line.IndexOf(searchForFontColorStart);
                        if (start != -1)
                        {
                            int mid = start + searchForFontColorStart.Length;
                            int end = line.IndexOf(Reverse(ccolor), mid);
                            if (end != -1)
                            {
                                string atEnd = "";
                                if ((end + ccolor.Length) < (line.Length - 1))
                                    atEnd = line.Substring(end + ccolor.Length);
                                if (start != 0)
                                {
                                    line = line.Substring(0, start) + "<font color=\"" + $"{color}" + "\">"
                                        + line.Substring(mid, end - mid)
                                        + "</font>"
                                        + atEnd;
                                }
                                else
                                {
                                    line = "<font color=\"" + $"{color}" + "\">"
                                        + line.Substring(mid, end - mid)
                                        + "</font>"
                                        + atEnd;
                                }
                            }
                            else
                                searchagain = false;

                        }
                        else
                            searchagain = false;
                    }
                    // If no more color patterns then stop.
                    if (!(System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(ColorPattern, line)))
                        break;
                }
            }
            return line;
        }

        private static string Check4HeadinsatStartof(string line, ref bool isHeading, ref bool inList)
        {
            if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(HeadingPatternatStartofLine, line))
            {
                char ch = line[2];
                if (char.IsDigit(ch))
                {
                    if (inList)
                    {
                        line = $"\n<ul>\n<h{ch}>{line.Substring(5)}</h{ch}>";
                    }
                    else
                        line = $"<h{ch}>{line.Substring(5)}</h{ch}>";
                    isHeading = true;
                    inList = false;
                }
            }
            return line;
        }


        /// <summary>
        /// Check for Extended list line start ((n)) where n is the list level 0..9.
        /// </summary>
        /// <param name="line">The line being checked</param>
        /// <param name="extendedListLevel">Current list depth. 0= not in list</param>
        /// <returns>The processed line</returns>
        private static string Check4ListatStartof(string line, ref int extendedListLevel)
        {
            if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(ListPatternatStartofLine, line))
            {
                char ch = line[2];
                if (char.IsDigit(ch))
                {
                    int level = ((int)ch) - 48;
                    if (level == extendedListLevel)
                        line = $"</li>\n<li>{line.Substring(5)}";
                    else
                    {
                        int diff = level - extendedListLevel;
                        if (diff > 0)
                        {
                            string pre = "";
                            for (int i = 0; i < diff; i++)
                                pre += "<ul>\n";
                            line = $"{pre}<li>{line.Substring(5)}";
                        }
                        else
                        {
                            string pre = "";
                            for (int i = 0; i < -diff; i++)
                                pre += "</ul></li>\n";
                            line = $"{pre}<li>{line.Substring(5)}</li>";
                        }
                    }
                    extendedListLevel = level;
                }
                else
                {
                    // If the line is'nt a list line then end the list if in extended list
                    if (extendedListLevel != 0)
                    {
                        string pre = "";
                        for (int i = 0; i < extendedListLevel; i++)
                            pre += "</ul>\n</li>\n";
                        line = pre + line;
                        extendedListLevel = 0;
                    }
                }
            }
            else
            {
                // If the line is'nt a list line then end the list if in extended list
                if (extendedListLevel != 0)
                {
                    string pre = "";
                    for (int i = extendedListLevel; i != 0; i--)
                        pre += "</ul>\n";
                    line = pre + line;
                    extendedListLevel = 0;
                }
            }
            return line;
        }

        /// <summary>
        /// Table is a Csv list for each line. First is TH
        /// </summary>
        /// <param name="line"></param>
        /// <param name="html"></param>
        /// <param name="inTable"></param>
        /// <returns></returns>
        private static string Check4TableatStartof(string line, ref string html, ref bool inTable)
        {
            if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("((T))*", line))
            {
                char ch = line[2];
                if (ch == 'T')
                {
                    if (!inTable)
                    {
                        inTable = true;
                        html += "\n<table>";
                        line = "<tr><th>" + line.Substring(ListPatternatStartofLine.Length-1).Replace(",", "</th><th>") + "</th></tr>";
                    }
                    else
                    {
                        line = "<tr><td>" + line.Substring(ListPatternatStartofLine.Length-1).Replace(",", "</td><td>") + "</td></tr>";
                    }

                }
            }
            else if (inTable == true)
            {
                html += "\n</table>\n";
                inTable = false;
            }
            return line;
        }
    }
}
