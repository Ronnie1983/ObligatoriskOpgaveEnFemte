using System;
using System.Threading;

namespace ObligatoriskOpgaveEnFemte
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerWorker worker = new ServerWorker();
            worker.Start();
            Console.ReadLine();
        }
    }
}
