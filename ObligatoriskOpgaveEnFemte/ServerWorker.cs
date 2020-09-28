using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpgaveEtLib.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ObligatoriskOpgaveEnFemte
{
    public class ServerWorker
    {
        private string _jsonString;
        private static int _number = 0;
        private static List<Cykel> _bikes = new List<Cykel>()
        {
            new Cykel(CreateId(), "Red", 50, 5),
            new Cykel(CreateId(), "Blå", 1500, 3),
            new Cykel(CreateId(), "Sort", 2500, 15),
            new Cykel(CreateId(), "Hvid", 2000, 5),
            new Cykel(CreateId(), "Sort", 8000, 24)
        };

        private static int CreateId()
        {
            _number = _number + 1;

            return _number;
        }
        public ServerWorker()
        {
            
        }

        public void Start()
        {
            //opret server
            TcpListener server = new TcpListener(IPAddress.Loopback, 4646);
            server.Start();

            while (true)
            {
                //venter på et opkald fra en klient
                TcpClient socket = server.AcceptTcpClient();

                Task.Run(() =>
                {
                    TcpClient tempSocket = socket;
                    DoClient(tempSocket);

                });
            }
        }

        public void DoClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            sw.WriteLine($"Du kan taste følgende:");
            sw.WriteLine($"HentAlle, Hent 1..., Gem efterfulgt af json data");

            while (true)
            {
                //læs input fra klient
                string line = sr.ReadLine();
                Console.WriteLine($"Input fra klient: {line}");
                string[] elements = line.Split(" ");
                if (elements[0]=="HentAlle")
                {
                    foreach (Cykel c in _bikes)
                    {
                        _jsonString = JsonSerializer.Serialize(c);
                        sw.WriteLine(_jsonString);
                        sw.Flush();
                    }
                }
                else if (elements[0]=="Hent")
                {
                    int integerTest = 0;
                    try
                    {
                        bool testNumber = int.TryParse(elements[1], out integerTest);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ingen med dette nummer, prøv igen");
                        //throw;
                    }
                    Cykel cykelNumber = _bikes.Find(c => c.Id == integerTest);
                    _jsonString = JsonSerializer.Serialize(cykelNumber);
                    sw.WriteLine(_jsonString);
                    sw.Flush();
                } else if (elements[0]=="Gem")
                {
                    try
                    {
                        //Cykel newBike = JsonSerializer.Deserialize<Cykel>(elements[1]);
                        Cykel newBike1 = JsonConvert.DeserializeObject<Cykel>(elements[1]);
                        _bikes.Add(newBike1);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //throw;
                    }
                    
                }
                else
                {
                    sw.WriteLine($"Forkert indtastning, prøv igen");
                    sw.Flush();
                }
            }
        }
    }
}
