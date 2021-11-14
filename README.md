# Meltdown
A simple to code and use Markup language making much use of brackets.

The key point is to bracket the text that is to be formatted.

## Context

When implementing SendMail functionality with C\# code, for example in a Blazor
app, you can send unformatted text or text formatted with HTML tags. HTML tags
are though not simple for a luddite. Alternatively, one could provide a Markdown
parser to generate HTML tags. Meltdown is meant to be a simple to implement
markup as a parser that outputs HTML, that makes much use of brackets.

The Acid Tests

1.  Can the markdown syntax be implemented in, say, C\#, by a 102 level student?
2.  Is the formatting easier than Markdown to use by a non-programmer?

The generated content is HTML so
-   The source content is in the main Text.
-   HTML can be seamlessly embedded in a page, and so is unmodified by the
    parser.
-   Certain start and end bracket sequences HTML format the internal text by
    replacing the brackets with the required HTML begin and end tags.
-   The brackets are used in pairs so that a sequence consisting of an open
    bracket, close bracket and text in between is unchanged.
-   Open brackets are a pair of [ ( and {
-   The corresponding close brackets are a mirroring pair of } ) and ]
-   For example: [[Hello there]] will translate to \<b\>Hello there\</b\>
-   Where a parameter is required, or the sequence is in two parts a \| is used
    as a separator.
-   Use of \< .. \> in the syntax is avoided as these are the basis of HTML
    tags.
-   Formatting tags can be combined with inner text of a Meltdown tag conforming
    to another tag. But 3 and 4 bracketing tags are defined explicitly for often
    some combinations.
-   Desirable: Text can be written in Word and with simple modification be
    presented was copy a paste as Meltdown.

Format | Markup | Notes
-------|--------|-------
Bold | [[*\<text\>*]]
Italics | [(*\<text\>*)]
Underline | [{*\<text\>*}]
Bold-Italics | [[(*\<text\>*)]] | *The inner [( .. )] can be reversed.*
Bold-Italics-Underline | [[({*\<text\>*})]] | *The inner [({ …})] can be in any order.*
Italics-Underline | [({*\<text\>*})] | *The inner ({ ..}) can be reversed.*
Font Color | {{*\<color name\>*\|*\<text\>*}}
Links | {{*\<url\>*}}
,, | {{*\<link text\>*\|*\<url\>*}}
Heading | ((*\<heading level 1,2or3 etc\>*\|*\<text\>*))

**Nb 1:** Bold, italics and underline formats all start with [ , bold takes precedence for combinations and italics takes precedence over underline. 
For example, Bold-Italics starts with the open square bracket, then the bold second character, followed by the Italics second character [-[-(    
*If these specifications are adopted then the notes in the third column are defunct.*

**Nb 2:** Markdown syntax has also been implemented: \[*\<link text\>*\](*\<url\>*)

*An alternative specification under consideration is:*

Format | Markup
-|-
Bold | [[*\<text\>*]]
Italics | ((*\<text\>*))
UnderLine | {{*\<text\>*}}
