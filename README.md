# Meltdown V2.2.0

<hr/>

**NB: _27th Feb 2022_ https://davidjones.sportronics.com.au is fixed now. Thx**

<hr/>

A simple to code and use Markup language making much use of brackets.

The key point is to bracket the text that is to be formatted. Also, some line
based content is simple to annotate with some characters at the start of the
line.

<b> Now available on Nuget: </b> https://www.nuget.org/packages/Meltdown/  
```Install-Package Meltdown -Version 2.2.0```

[Also, see blog posts on Meltdown](https://davidjones.sportronics.com.au/search.html?query=Meltdown)

## Updates

1.  A second version of the class that formalises the functionality making it  
    easy to change the delimiters used. Also key steps (font color and links)  
    are a separate functions. Uses the schema as below.

2.  Test app has some test strings.

3.  Implemented headings. eg,   
    `[[2]]At start of line` becomes  
    `<h2>At start of Line</h2>`

4.  This page has been refined such that only one schema is presented, as  
    adopted and only one class in the code that implements this.`
    
5.  Simple lists -<space> and -<tab> at start now work. One level only.  
    And Exntended multi level list too. Lines start with ((n)) where n is line level.
    
6.  Table Header row starts with ((T)) Headings are a Csv list  
    Table data rows start with ((t)). Cell data is a Csv list

7.  Added BlazTest a Blazor Server App with which you can compose or paste text that then then gets parsed by Meltdown. 
    Shows the rendered Html as well. Some samples on offer like the console Test app as well.
8.  Meltdown class separated into 2 files with delimeters in the second file.
9.  (V2.0) Color just uses ((..|..)) and links use <<..}..>>
10. Two Markdown Syntaxes included: Links and Headings
11. BlazTest has send email Test page.
12. Functionally the same as previous except added:
     ```!! translates to <br/>```
      and significant rework of coding structure.
13.  Bold-Italics etc combinations: The order of [,( and { don't matter for openning delimiter but closing should mirror it.
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

-   The source content is in the main, Text.
-   **Meltdown** is line based with no state passed between lines

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

-   Formatting tags can be combined with inner text of a Meltdown tag conforming
    to another tag. But 3 character bracketing tags are defined explicitly for
    often some combinations.

-   Desirable: Text can be written in Word and with simple modification be used
    with a copy a paste as Meltdown.
-   Some additional features are indicated by 5 characters at start of the a line:  
    Headings [[n]], Multilevel list ((n)) and Table ((T/t))

**Nb 1:** Bold, italics and underline formats are a pair of [,{ or {. Other
combinations are used for pairs of these format. A delimiter consisting of all 3
is used for a format involving all 3.

**Nb 2:** Markdown syntax has also been implemented also been implemented for
some other entities; currently for HTML links.[](*%3curl%3e*)

| Format                 | Markup                             | Notes                                                          |
|------------------------|------------------------------------|----------------------------------------------------------------|
| Bold                   | ```[[text]]```                     |                                                                |
| Italics                | ```((text))```                     |                                                                |
| UnderLine              | ```{{text}}```                     |                                                                |
| Bold-Italics           | ```[(text)]```                     | *Order doesn't matter but closing delimiter should mirror openning one.* |
| Bold-Underline         | ```[{text}]```                     | *Order doesn't matter but closing delimiter should mirror openning one.* |
| Italics-Underline      | ```({text})```                     | *Order doesn't matter but closing delimiter should mirror openning one.* |
| Bold-Italics-Underline | ```[({text})]```                   | *Order doesn't matter but closing delimiter should mirror openning one.* |
| Font Color             | ```((color name\|text))``` |   Nb: Only a subset of HTML colors accepted. See [code](https://github.com/djaus2/Meltdown/blob/master/Meltdown/Meltdown/Meltdown.cs)                          |
| Links                  | ```<<url>>```                    |                                                                |
| ,,                     | ```<<link text\|url>>```  |       
| Heading                | ```[[n]]``` at start of line             |  where n=1..9 eg ```[[2]]Heading Level 2```                    |
| Bullet List            | ```-space``` or ```-tab``` at start of line    | Only one level of list                                         |
| List Multilevel        | ```((n))``` at start of line             | where n=1..9   _See example at bottom_                         |
| Table                  | ```((T))``` at start of each line        | Table Header row. Headings are Csv list.                       |
|  ,,                    | ```((t))``` at start of each line        | Table Data row. Cells are a Csv list                           |
|  New line  | ```!!``` | Anywhere in text translates to &lt;br/&gt; |
 
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

AA((red|This is red))BB((blue|This is blue))CC

<p>AA<font color="Red">This is red</font>BB<font color="Blue">This is blue</font>CC</p>

AA[({This is bold italics and underline})]BB

<p>AA<b><i><u>This is bold italics and underline</u></i></b>BB</p>

AA<<https://sportronics.com.au>>BB

<p>AA<a href= "https://sportronics.com.au">https://sportronics.com.au</a>BB</p>

AA<<Click here|https://sportronics.com.au>>BB

<p>AA<a href= "https://sportronics.com.au">Click here</a>BB</p>
```

<h1>Heading Level 1</h1>

<p>AA<b>This is Bold</b>BB</p>

<p>AA<i>This is Italics</i>BB</p>

<p>AA<a href= "https://sportronics.com.au">https://sportronics.com.au</a>BB</p>

<p>AA<a href= "https://sportronics.com.au">Click here</a>BB</p>

```
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
```
<ul>
<li>Simple list line one</li>
<li>Simple list line 2 with tab</li>
<li>Simple list line three</li>
<li>Simple list line 4 with tab</li>
</ul>

```
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
((T))Name,Age,Country
((t))Fred,23,Australia
((t))Sue,45,USA
((t))John,21,NZ
   
<table>
<th><td>Name</td><td>Age</td><td>Country</td></th>
<tr><td>Fred</td><td>23</td><td>Australia</td></tr>
<tr><td>Sue</td><td>45</td><td>USA</td></tr>
<tr><td>John</td><td>21</td><td>NZ</td></tr>
</table
```
<table>
<tr><th>Name</th><th>Age</th><th>Country</th></tr>
<tr><td>Fred</td><td>23</td><td>Australia</td></tr>
<tr><td>Sue</td><td>45</td><td>USA</td></tr>
<tr><td>John</td><td>21</td><td>NZ</td></tr>
    </table> 
