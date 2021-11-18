# Meltdown

A simple to code and use Markup language making much use of brackets.

The key point is to bracket the text that is to be formatted. Also, some line
based content is simple to annotate with some characters at the start of the
line.

**This is a work in progress at the moment.**

## Updates

1.  A second version of the class that formalises the functionality making it
    easy to change the delimiters used. Also key steps (font color and links)
    are a separate functions. Uses the schema as below.

2.  Test app has some test strings.

3.  Implemented headings. eg,  
    `[[2]]At start of line` becomes  
    `<h2>At start of Line</h2>`

4.  `This page has been refined such that only one schema is presented, as
    adopted and only one class in the code that implements this.`

## 2Dos

-   Implement lists with preceding characters on line as per Heading Levels.

    -   Can have multiple levels.

-   Tables as Csv lines (perhaps)

## Context

When implementing **SendMail** functionality with C\# code, for example in a
Blazor app, you can send unformatted text or text formatted with HTML tags. HTML
tags are though not simple for a luddite. Alternatively, one could provide a
Markdown parser to generate HTML tags. Meltdown is meant to be a simple to
implement markup as a parser that outputs HTML, that makes much use of brackets.

## The Acid Tests

1.  Can the markup syntax be implemented in, say, C\#, by a 102 level student?

2.  Is the formatting easier than Markdown to use by a non-programmer?

## Specification

The generated content is HTML so

-   The source content is in the main Text.

-   HTML can be seamlessly embedded in a page, and so is unmodified by the
    parser.

-   Certain start and end bracket sequences HTML format the internal text by
    replacing the brackets with the required HTML begin and end tags.

-   The brackets are used in pairs so that a sequence consisting of an open
    bracket, close bracket and text in between is unchanged.

-   Open brackets are combinations of [ ( and {

-   The corresponding close brackets are a mirroring pair of } ) and ]

-   For example: [[Hello there]] will translate to \<b\>Hello there\</b\>

-   Where a parameter is required, or the sequence is in two parts a \| is used
    as a separator.

-   Use of \< .. \> in the syntax is avoided as these are the basis of HTML
    tags.

-   Formatting tags can be combined with inner text of a Meltdown tag conforming
    to another tag. But 3 character bracketing tags are defined explicitly for
    often some combinations.

-   Desirable: Text can be written in Word and with simple modification be used
    with a copy a paste as Meltdown.

**Nb 1:** Bold, italics and underline formats are a pair of [,{ or {. Other
combinations are used for pairs of these format. A delimiter consisting of all 3
is used for a format involving all 3.

**Nb 2:** Markdown syntax has also been implemented also been implemented for
some other entities; currently for HTML links.[](*%3curl%3e*)

| Format                 | Markup                             | Notes                                                          |
|------------------------|------------------------------------|----------------------------------------------------------------|
| Bold                   | [[*\<text\>*]]                     |                                                                |
| Italics                | ((*\<text\>*))                     |                                                                |
| UnderLine              | {{*\<text\>*}}                     |                                                                |
| Bold-Italics           | [(*\<text\>*)]                     | *Order does matter*                                            |
| Bold-Underline         | [{*\<text\>*}]                     | *Order does matter*                                            |
| Italics-Underline      | ({*\<text\>*})                     | *Order does matter*                                            |
| Bold-Italics-Underline | [({*\<text\>*})]                   | *Order does matter*                                            |
| Font Color             | (((*\<color name\>*\|*\<text\>*))) |                                                                |
| Links                  | {{{*\<url\>*}}}                    |                                                                |
| ,,                     | {{{*\<link text\>*\|*\<url\>*}}}   |                                                                |
| Heading                | [[n]]                              | [[n]] is at start of line where n=1..9 eg [[2]]Heading Level 2 |

## Test App Output:

```
[[1]]Heading Level 1
<h1>Heading Level 1</h1>

AA[[This is Bold]]BB

<p>AA<b>This is Bold</b>BB</p>

AA((This is Italics))BB

<p>AA<i>This is Italics</i>BB</p>

AA{{This is Underline}}BB

<p>AA<u>This is Underline</u>BB</p>

AA(((red|This is red)))BB(((blue|This is blue)))CC

<p>AA<font color="Red">This is red</font>BB<font color="Blue">This is blue</font>CC</p>

AA[({This is bold italics and underline})]BB

<p>AA<b><i><u>This is bold italics and underline</u></i></b>BB</p>

AA{{{https://sportronics.com.au}}}BB

<p>AA<a href= "https://sportronics.com.au">https://sportronics.com.au</a>BB</p>

AA{{{Click here|https://sportronics.com.au}}}BB

<p>AA<a href= "https://sportronics.com.au">Click here</a>BB</p>

- Simple list line one
-       Simple list line 2 with tab
- Simple list line three
-       Simple list line 4 with tab

<ul>
<li>Simple list line one</li>
<li>Simple list line 2 with tab</li>
<li>Simple list line three</li>
<li>Simple list line 4 with tab</li>
</ul>


((1)) Extended list level one
((1)) Extended list level 1
((2)) Extended list level two
((3)) Extended list level three
((2)) Extended list level two
((1)) Extended list level one

<ul>
<li> Extended list level one
</li>
<li> Extended list level 1
<ul>
<li> Extended list level two
<ul>
<li> Extended list level three
</ul></li>
<li> Extended list level two</li>
</ul></li>
<li> Extended list level one</li>
</ul>
```
