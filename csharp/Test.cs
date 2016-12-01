using System;
using System.IO;
using System.Text;
using System.Collections;

class Test
{
    public static void Main()
    {
        string line;
        float[] mat_data = new float[96];
        float[] mean_vector = new float[96];
        float[,] basis = new float[3,96];
        float[,] centroids = new float[6,3];
        //float[] basis2 = new float[96];
        //float[] basis3 = new float[96];
        string path;
        int j;

        /////////////////////////////////////////////////enter mean vector information
        path = "/home/juan/courses/CS294-98/hw3_gamecontrol/ML/mean3steps_v3.txt";
        System.IO.StreamReader file0 = new System.IO.StreamReader(path);
        j=0;
        while((line = file0.ReadLine()) != null)
         {
            mean_vector[j] = float.Parse(line);
            j=j+1;
        }


        /////////////////////////////////////////////////enter basis vector information
        path = "/home/juan/courses/CS294-98/hw3_gamecontrol/ML/basis3steps_v3.txt";
        System.IO.StreamReader file2 = new System.IO.StreamReader(path);
        j=0;
        while((line = file2.ReadLine()) != null)
         {
             //Console.WriteLine(line);
             string[] data = line.Split(',');
             //Console.WriteLine(data.Length);

             for (int i = 0; i < data.Length-1; i++)
             {
               basis[j,i] = float.Parse(data[i]);
               //Console.WriteLine(data[i]);
             }
             j=j+1;
        }

        /////////////////////////////////////////////////enter clusters information
        path = "/home/juan/courses/CS294-98/hw3_gamecontrol/ML/cluster3steps_v3.txt";
        System.IO.StreamReader file3 = new System.IO.StreamReader(path);
        j=0;
        while((line = file3.ReadLine()) != null)
         {
             //Console.WriteLine(line);
             string[] data = line.Split(',');
             //Console.WriteLine(data.Length);

             for (int i = 0; i < data.Length-1; i++)
             {
               centroids[j,i] = float.Parse(data[i]);
               //Console.WriteLine(data[i]);
             }
             j=j+1;
        }
        /////////////////////////////////////////////////enter all data from mat
        path = "/home/juan/courses/CS294-98/hw3_gamecontrol/matplot/matdraw/2016_11_26_test.txt";
        System.IO.StreamReader file1 = new System.IO.StreamReader(path);
        while((line = file1.ReadLine()) != null)
         {
             //Console.WriteLine(line);
             string[] data = line.Split(',');
             //Console.WriteLine(data.Length);

             for (int i = 1; i < data.Length-1; i++)
             {
               mat_data[i-1] = float.Parse(data[i]);
               //Console.WriteLine(data[i]);
             }
        }

    }

    private void print_data(float[] data) {
     for (int i = 1; i < data.Length-1; i++)
     {
       Console.WriteLine(data[i]);
     }
   	}

}


/*
// Create a temporary file, and put some data into it.
string path = Path.GetTempFileName();
using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.None))
{
    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
    // Add some information to the file.
    fs.Write(info, 0, info.Length);
}
*/
