using System;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using MetaRend;
using System.IO;
using System.Diagnostics;

namespace Tabula
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPortInterface serialPortInterface = new();

            serialPortInterface.port.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
            {
                Console.Write("Data: " + serialPortInterface.port.ReadExisting());
            };
            serialPortInterface.port.ErrorReceived += (object sender, SerialErrorReceivedEventArgs e) =>
            {
                Console.WriteLine("Error: " + e.EventType.ToString());
            };

            Thread.Sleep(500);

            serialPortInterface.Write(0xff);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    serialPortInterface.Write(0x00);
                }
                for (int j = 0; j < 64; j++)
                {
                    serialPortInterface.Write(0xff);
                }
            }
            Console.ReadLine();
        }
    }
}
