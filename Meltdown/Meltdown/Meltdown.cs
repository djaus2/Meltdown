using System;
using System.Collections.Generic;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of the the second schema, with some other extras.
    /// </summary>
    public static partial class Meltdown
    {
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

        public static string Parse(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return "";

            string html = "";
            string[] lines = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            int ExtendedListLevel = 0;
            StartsWith swPrevious = StartsWith.nothing;
            foreach (var _line in lines)
            {
                string line = _line; // line get modified hence need copy of the ierator
                string prepend = "";
                int level = 0;

                // Get line type and finish previous state if required
                StartsWith sw = CheckStartsWith(line,ref level);
                if (sw != swPrevious)
                {
                    // Finish table or lists if incomplete.
                    prepend = FinishState(swPrevious, ExtendedListLevel);
                }

                // Process the line
                switch (sw)
                {
                    case StartsWith.blank:
                        line = "<br />";
                        break;
                    case StartsWith.nothing:
                        line =  "<p>" + line + "</p>";
                        break;
                    case StartsWith.heading:
                        char ch = line[2];
                        line = $"<h{ch}>{line.Substring(5)}</h{ch}>";
                        break;
                    case StartsWith.markdownheading:
                        line = line.Substring(level);
                        line = $"<h{level}>{line}</h{level}>";
                        break;
                    case StartsWith.table:
                        if (!(swPrevious == StartsWith.table))
                            prepend += "\n<table>\n";
                        line = DoTable(line);
                        break;
                    case StartsWith.simpleList:
                        line = line.Substring(2);
                        if (!(swPrevious == StartsWith.simpleList))
                            prepend += "\n<ul>\n";
                        line = "<li>" + line + "</li>";
                        break;
                    case StartsWith.xlist:
                        line = DoExtendedList(line, level, ref ExtendedListLevel);
                        break;
                }
                swPrevious = sw;

                // Font color begin-end on same line
                line = GetMeltdownFontColors(line);
                // Nb: Each paragraph requires a separate font color.


                // All links on the line. Can't extend over one line
                line = GetMeltdownLinks(line);

                // Format begin-end on same line
                line = MapMeltdownFormat2Html(line);

                html += "\n" + prepend + line;
            }
            // Finish table or lists if incomplete.
            html += FinishState(swPrevious, ExtendedListLevel);

            return html;
        }

        private static string FinishState(StartsWith sw, int ExtendedListLevel)
        {
            string ret = "";
            switch (sw)
            {
                case StartsWith.blank:
                case StartsWith.nothing:
                case StartsWith.heading:
                    break;
                case StartsWith.table:
                    ret = "\n</table>\n";
                    break;
                case StartsWith.simpleList:
                    ret = "\n</ul>\n";
                    break;
                case StartsWith.xlist:
                    if (ExtendedListLevel > 0)
                    {
                        for (int i = ExtendedListLevel; i != 0; i--)
                            ret += "</ul>\n";
                        ExtendedListLevel = 0;
                        ret = "\n" + ret;
                    }
                    break;
            }
            return ret;
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
            line = line.Replace(linebreak, "<br/>\n");

            //line = line.Replace(strikethru,"Need this <p style="text-decoration: line-through;">")
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


        /// <summary>
        /// Check for Extended list line start ((n)) where n is the list level 0..9.
        /// </summary>
        /// <param name="line">The line being checked</param>
        /// <param name="level">Previously determined list level</param>
        /// <param name="extendedListLevel">Current list depth. 0= not in list</param>
        /// <returns>The processed line</returns>
        private static string DoExtendedList(string line, int level, ref int extendedListLevel)
        {
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
            return line;
        }

        /// <summary>
        /// Table is a Csv list for each line. First is TH
        /// </summary>
        /// <param name="line"></param>
        /// <returns>line with table header or row</returns>
        private static string DoTable(string line)
        {
            char ch = line[2];
            if (ch == 'T')
            {
                line = line.Substring(5);
                line = "<tr><th>" + line.Substring(ListPatternatStartofLine.Length-1).Replace(",", "</th><th>") + "</th></tr>";
            }
            else if (ch == 't')
            {
                line = line.Substring(5);
                line = "<tr><td>" + line.Substring(ListPatternatStartofLine.Length - 1).Replace(",", "</td><td>") + "</td></tr>";
            }
            return line;
        }

        enum StartsWith { blank,nothing,heading,markdownheading,table,simpleList,xlist}

        /// <summary>
        /// Gets line type based upon characters at start.
        /// </summary>
        /// <param name="line">line to be checked</param>
        /// <param name="level">Out Determined heading or extended list level</param>
        /// <returns></returns>
        private static StartsWith CheckStartsWith(string line ,  ref int  level)
        {
            level = 0;
            StartsWith res = StartsWith.nothing;
            if (string.IsNullOrEmpty(line))
                res = StartsWith.blank;
            else if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("((T))*", line))
                res = StartsWith.table;
            else if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("((t))*", line))
                res = StartsWith.table;
            else if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(ListPatternatStartofLine, line))
            {
                char ch = line[2];
                if (char.IsDigit(ch))
                {
                    level = ((int)ch) - 48;
                    res = StartsWith.xlist;
                };
            }
            else if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(HeadingPatternatStartofLine, line))
            {
                char ch = line[2];
                if (char.IsDigit(ch))
                {
                    level = ((int)ch) - 48;
                    res = StartsWith.heading;
                };
            }
            else if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression("#*", line))
            {
                level = 0;
                bool found = true;
                do
                {
                    if (!(line[level] == '#'))
                        found = false;
                    else if (level == (line.Length - 1))
                        found = false;
                    else
                        level++;
                }
                while (found);
                if ((level > 0) && (level < line.Length))
                {
                    if (line[level] == ' ')
                    {
                        res = StartsWith.markdownheading;
                    }
                }
            }
            else
            {
                if ((line.Length > 2)
                            &&
                    ((line.Substring(0, 2) == "- ") || (line.Substring(0, 2) == "-\t")))
                {
                    res = StartsWith.simpleList;
                }
            }
           return res;
        }

    }
}
