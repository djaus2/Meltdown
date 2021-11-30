using System;
using Meltdown;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg;
            string res;

            
            Console.WriteLine();
            msg = "[[1]]Heading Level 1";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA[[This is Bold]]BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA((This is Italics))BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA{{This is Underline}}BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA((red|This is red))BB((blue|This is blue))CC";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA[({This is bold italics and underline})]BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA<<https://sportronics.com.au>>BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "AA<<Click here|https://sportronics.com.au>>BB";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            
            Console.WriteLine();
            msg = "- Simple list line one\n-\tSimple list line 2 with tab";
            msg += "\n- Simple list line three\n-\tSimple list line 4 with tab";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);


            Console.WriteLine();
            msg = "((1)) Extended list level one\n((1)) Extended list level 1";
            msg += "\n((3)) Extended list level two\n((3)) Extended list level three";
            msg += "\n((2)) Extended list level two\n((1)) Extended list level one";
            Console.Write(msg);
             res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);

            Console.WriteLine();
            msg = "((T))Name,Age,Country";
            msg += "\n((t))Fred,23,Australia";
            msg += "\n((t))Sue,45,USA";
            msg += "\n((t))John,21,NZ";
            Console.Write(msg);
            res = Meltdown.Meltdown.Parse(msg);
            Console.WriteLine(res);


            Console.ReadLine();
        }
    }
}
