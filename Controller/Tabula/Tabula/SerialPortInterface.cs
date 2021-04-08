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

        public SerialPort Setup()
        {
            int selectedPort = -1;
            bool portValid = false;
            while (!portValid)
            {
                string[] portNames = SerialPort.GetPortNames();
                List<int> portIndexes = new List<int>();
                foreach (string port in portNames)
                {
                    string portIndex = port.Substring(3, port.Length - 3);
                    int portIndexInt = int.Parse(portIndex);
                    portIndexes.Add(portIndexInt);
                }
                portIndexes.Sort();
                string portList = "";
                foreach (int port in portIndexes)
                {
                    if (portIndexes[portIndexes.Count - 1] != port)
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
            SerialPort serialPort = new SerialPort("COM" + selectedPort, baudRate, Parity.None, 8, StopBits.One);
            serialPort.Open();
            port = serialPort;
            return serialPort;
        }

        public void Write64(byte[] data)
        {
            port.Write(data, 0, 64);
        }

        public void WritePattern(int patternIndex)
        {
            switch (patternIndex)
            {
                case 0:
                    List<byte> data0 = new List<byte>();
                    for (int i = 0; i < 64; i++)
                    {
                        //data.Add((byte)(i*i));
                        data0.Add((byte)(i));
                        //data.Add((byte)Math.Sqrt(i));
                    }
                    Write64(data0.ToArray());
                    break;
                case 1:
                    List<byte> data1 = new List<byte>();
                    for (int i = 0; i < 64; i++)
                    {
                        data1.Add((byte)(i * 2));
                    }
                    Write64(data1.ToArray());
                    break;
                case 2:
                    List<byte> data2 = new List<byte>();
                    for (int i = 0; i < 64; i++)
                    {
                        data2.Add((byte)(i * 4));
                    }
                    Write64(data2.ToArray());
                    break;
                case 3:
                    List<byte> data3 = new List<byte>();
                    for (int i = 0; i < 64; i++)
                    {
                        data3.Add((byte)(i*i));
                    }
                    Write64(data3.ToArray());
                    break;
                case 4:
                    List<byte> data4 = new List<byte>();
                    for (int i = 0; i < 64; i++)
                    {
                        data4.Add((byte)Math.Sqrt(i));
                    }
                    Write64(data4.ToArray());
                    break;
            }
        }
        public void WriteShiftingLine()
        {
            bool switched = false;
            for (int i = 0; i < 64; )
            {
                List<byte> data = new List<byte>();
                for (int j = 0; j < 64; j++)
                {
                    if (i == j) data.Add(0xff);
                    else if (i == j + 1) data.Add(0xaa);
                    else if (i == j - 1) data.Add(0x55);
                    else data.Add(0x00);
                }
                Write64(data.ToArray());
                Thread.Sleep(5);

                if (i == 63) switched = true;
                if (!switched) i++;
                else i--;
                if (switched && i == 0) break;
            }
        }
        public void WriteShiftingPacman()
        {
            bool switched = false;
            for (int i = 0; i < 64;)
            {
                List<byte> data = new List<byte>();
                for (int j = 0; j < 64; j++)
                {
                    if (i == j) data.Add(0x3c);
                    else if (i == j + 1) data.Add(0x7e);
                    else if (i == j + 2) data.Add(0xff);
                    else if (i == j + 3) data.Add(0xe0);
                    else if (i == j + 4) data.Add(0xff);
                    else if (i == j + 5) data.Add(0xf8);
                    else if (i == j + 6) data.Add(0xe3);
                    else if (i == j + 7) data.Add(0x41);
                    else data.Add(0x00);
                }
                Write64(data.ToArray());
                Thread.Sleep(10);

                if (i == 63) switched = true;
                if (!switched) i++;
                else i--;
                if (switched && i == 0) break;
            }
        }
        public void WriteSawtooth()
        {
            for (int i = 0; i < 64;)
            {
                List<byte> data = new List<byte>();
                for (int j = 0; j < 64; j++)
                {
                    data.Add(GetNum(j + i));
                }
                Write64(data.ToArray());
                Thread.Sleep(60);
            }
        }
        private byte GetNum(int i)
        {
            byte b = 0x01;
            int j = b >> (i % 8);
            return (byte)j;
        }
        public void WriteBounceDot()
        {
            int dotYPos = 1;
            bool bounceUp = true;
            for (int dotXPos = 0; dotXPos < 64; dotXPos++)
            {
                List<byte> data = new List<byte>();
                for (int j = 0; j < 64; j++)
                {
                    if (j == dotXPos)
                        data.Add((byte)dotYPos);
                    else if (j + 1 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos/2));
                        else data.Add((byte)(dotYPos * 2));
                    }
                    else if (j + 2 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 4));
                        else data.Add((byte)(dotYPos * 4));
                    }
                    else if (j + 3 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 8));
                        else data.Add((byte)(dotYPos * 8));
                    }
                    else if (j + 4 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 16));
                        else data.Add((byte)(dotYPos * 16));
                    }
                    else if (j + 5 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 32));
                        else data.Add((byte)(dotYPos * 32));
                    }
                    else if (j + 6 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 64));
                        else data.Add((byte)(dotYPos * 64));
                    }
                    else if (j + 7 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 128));
                        else data.Add((byte)(dotYPos * 128));
                    }
                    else if (j + 8 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 64));
                        else data.Add((byte)(dotYPos * 64));
                    }
                    else if (j + 9 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 32));
                        else data.Add((byte)(dotYPos * 32));
                    }
                    else if (j + 10 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 16));
                        else data.Add((byte)(dotYPos * 16));
                    }
                    else if (j + 11 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 8));
                        else data.Add((byte)(dotYPos * 8));
                    }
                    else if (j + 12 == dotXPos)
                    {
                        if (bounceUp) data.Add((byte)(dotYPos / 4));
                        else data.Add((byte)(dotYPos * 4));
                    }
                    else
                        data.Add(0x00);
                }
                Write64(data.ToArray());
                Thread.Sleep(60);
                if (dotYPos == 128 && bounceUp) bounceUp = false;
                if (dotYPos == 1 && !bounceUp) bounceUp = true;
                if (bounceUp) dotYPos *= 2;
                else dotYPos /= 2;
            }
        }
        public void WriteSnakeDot()
        {
            bool increase = true;
            int x = 0;
            int runs = 0;

            int y = 1;
            while (true)
            {
                List<byte> data = new List<byte>();
                for (int i = 0; i < 64; i++)
                {
                    if (x == i)
                    {
                        data.Add((byte)y);
                    }
                    else
                    {
                        data.Add(0x00);
                    }
                }
                Write64(data.ToArray());
                if (x == 63 && increase)
                {
                    y *= 2;
                    if (y == 256) break;
                    increase = false;
                }
                if (x == 0 && !increase)
                {
                    y *= 2;
                    if (y == 256) break;
                    increase = true;
                }
                if (increase) x++;
                else x--;
                runs++;
                Thread.Sleep(40);
            }
        }
        public void ShiftArrow()
        {
            Thread.Sleep(7000);
            byte[] arrow = new byte[] { 0x81, 0x42, 0x24, 0x18, 0x00, 0x00, 0x18, 0x24, 0x42, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            for (int i = 0; i < 60; i++)
            {
                Write64(arrow);
                arrow = ShiftRight(arrow);
                Thread.Sleep(250);
            }
        }
        private byte[] ShiftRight(byte[] array)
        {
            byte[] newArray = new byte[64];
            for (int i = 0; i < 63; i++)
            {
                newArray[i] = array[i + 1];
            }
            newArray[63] = array[0];
            return newArray;
        }
    }
}
