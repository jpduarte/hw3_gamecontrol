using System;
using System.IO;
using System.Text;
using System.Collections;

class Test
{
    public static void Main()
    {
        // Create a temporary file, and put some data into it.
        string path = "/home/juan/courses/CS294-98/hw3_gamecontrol/matplot/matdraw/2016_11_26_test.txt";//Path.GetTempFileName();
        // Open the stream and read it back.
        System.IO.StreamReader file = new System.IO.StreamReader(path);
        string line;
        while((line = file.ReadLine()) != null)
         {
         Console.WriteLine(line);
         string[] data = line.Split(',');
         Console.WriteLine(data.Length);
         float[] mat_data = new float[96];
         for (int i = 1; i < data.Length-1; i++)
         {
           mat_data[i-1] = float.Parse(data[i]);
           //Console.WriteLine(data[i]);
         }
        }
    }
}
