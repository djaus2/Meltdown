using System;
using Meltdown;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string res = Meltdown_V2.Parse("[[This is Bold]]");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("((This is Italics))");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("{{This is Underline}}");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("(((red|This is red))) (((blue|This is blue)))");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("[({This is bold italics and underline})]");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("AA{{{https://sportronics.com.au}}}BB");
            Console.WriteLine(res);
            res = Meltdown_V2.Parse("AA{{{Click here|https://sportronics.com.au}}}BB");
            Console.WriteLine(res);
            Console.ReadLine();
        }
    }
}
