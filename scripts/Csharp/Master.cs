using System;

namespace scripts.Csharp
{
    class Master
    {
        static void Main(string[] args)
        {
            Random r = new Random();
            Console.WriteLine(r.NextDouble());
        }
    }
}