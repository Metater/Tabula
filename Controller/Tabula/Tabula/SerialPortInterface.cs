using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO.Ports;
using System.Threading;

namespace Tabula
{
    public class SerialPortInterface
    {
        public SerialPort port;

        public SerialPortInterface()
        {
            int selectedPort = -1;
            bool portValid = false;
            while (!portValid)
            {
                string[] portNames = SerialPort.GetPortNames();
                List<int> portIndexes = new();
                foreach (string port in portNames)
                {
                    string portIndex = port[3..];
                    int portIndexInt = int.Parse(portIndex);
                    portIndexes.Add(portIndexInt);
                }
                portIndexes.Sort();
                string portList = "";
                foreach (int port in portIndexes)
                {
                    if (portIndexes[^1] != port)
                        portList += port.ToString() + ", ";
                    else
                        portList += port.ToString();
                }
                Console.WriteLine($"Enter the index of the port you would like to connect to. ({portList})");
                string selectedPortRaw = Console.ReadLine();
                if (int.TryParse(selectedPortRaw, out selectedPort))
                    if (!portIndexes.Contains(selectedPort))
                        Console.WriteLine("Invalid!");
                    else
                        portValid = true;
            }
            Console.WriteLine("Enter baud rate, valid one are: 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 31250, 38400, 57600, and 115200");
            bool baudRateValid = false;
            int baudRate = 9600;
            int[] validBaudRates = { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 31250, 38400, 57600, 115200 };
            while (!baudRateValid)
            {
                string selectedBaudRateRaw = Console.ReadLine();

                if (int.TryParse(selectedBaudRateRaw, out baudRate))
                    if (validBaudRates.Contains(baudRate))
                        baudRateValid = true;
                if (!baudRateValid)
                {
                    Console.WriteLine("Invalid!");
                    Console.WriteLine("Enter baud rate, valid ones are: 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 31250, 38400, 57600, and 115200");
                }
            }
            port = new("COM" + selectedPort, baudRate, Parity.None, 8, StopBits.One);
            port.Open();
        }

        public void Write(byte data)
        {
            port.Write(new byte[] { data }, 0, 1);
        }

        public void Write(byte[] data, int length)
        {
            port.Write(data, 0, length);
        }
    }
}
