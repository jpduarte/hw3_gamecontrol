// cmdline1.cs
// arguments: A B C
/*using System;

public class CommandLine
{
   public static void Main(string[] args)
   {
       // The Length property is used to obtain the length of the array.
       // Notice that Length is a read-only property:
       Console.WriteLine("Number of command line parameters = {0}",
          args.Length);
       for(int i = 0; i < args.Length; i++)
       {
           Console.WriteLine("Arg[{0}] = [{1}]", i, args[i]);
       }
   }
}*/

// cmdline2.cs
// arguments: John Paul Mary
using System;

public class CommandLine2
{
   public static void Main(string[] args)
   {
       Console.WriteLine("Number of command line parameters = {0}",
          args.Length);
       foreach(string s in args)
       {
          Console.WriteLine(s);
       }
   }
}
