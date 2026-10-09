using System;
using System.Threading;

class Program
{
    static int batuketa;
    static int biderketa;
    static void batu(int zenb1, int zenb2)
    {
        for (int i = 0; i < 10; i++)
        {
            batuketa = zenb1 + zenb2;
            Console.WriteLine(batuketa);
            Thread.Sleep(300);
        }
    }
    static void bider(int zenb1, int zenb2)
    {
        for (int i = 0; i < 10; i++)
        {
            biderketa = zenb1 * zenb2;
            Console.WriteLine(biderketa);
            Thread.Sleep(1000);
        }
    }

    static void totala()
    {
        int emaitza = batuketa + biderketa;
        Console.WriteLine("Batu eta biderketa batura = " + emaitza);
    }
    public static void Main(string[] args)
    {
        Thread hari1 = new Thread(() => batu(4, 6));
        Thread hari2 = new Thread(() => bider(4, 6));

        hari1.Start();
        hari2.Start();

        hari1.Join();
        hari2.Join();
        Thread hari3 = new Thread(totala);
        hari3.Start();


        hari3.Join();

        Console.WriteLine("Hari guztiak amaitu dira.");
    }
}