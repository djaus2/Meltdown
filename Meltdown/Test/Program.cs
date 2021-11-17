using System;
using Meltdown;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            string msg = "[[1]]Heading Level 1";
            Console.Write(msg);
            string res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA[[This is Bold]]BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA((This is Italics))BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA{{This is Underline}}BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA(((red|This is red)))BB(((blue|This is blue)))CC";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA[({This is bold italics and underline})]BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA{{{https://sportronics.com.au}}}BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA{{{Click here|https://sportronics.com.au}}}BB";
            Console.Write(msg);
            res = Meltdown_V2.Parse(msg);
            Console.WriteLine(res);

            Console.ReadLine();
        }
    }
}
