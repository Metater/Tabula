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
            SerialPortInterface serialPortInterface = new(3, 57600);

            serialPortInterface.port.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
            {
                Console.Write("Data: " + serialPortInterface.port.ReadExisting());
            };
            serialPortInterface.port.ErrorReceived += (object sender, SerialErrorReceivedEventArgs e) =>
            {
                Console.WriteLine("Error: " + e.EventType.ToString());
            };

            Thread.Sleep(500);

            while (true)
            {
                byte[] data = new byte[1025];
                int offset = 0;
                Write(0xff, data, ref offset);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 64; j++)
                    {
                        Write((byte)(63 - j), data, ref offset);
                    }
                    for (int j = 0; j < 64; j++)
                    {
                        Write((byte)j, data, ref offset);
                    }
                }
                serialPortInterface.Write(data, data.Length);
                Thread.Sleep(512);
                Console.ReadLine();
            }
        }

        private static void Write(byte[] src, byte[] dst, ref int offset)
        {
            for (int i = 0; i < src.Length; i++)
            {
                dst[offset + i] = src[i];
            }
            offset += src.Length;
        }

        private static void Write(byte src, byte[] dst, ref int offset)
        {
            Write(new byte[] {src}, dst, ref offset);
        }
    }
}
