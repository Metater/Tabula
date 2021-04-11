using System;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using MetaRend;
using System.IO;

namespace Tabula
{
    class Program
    {
        static void Main(string[] args)
        {
            string matrixStr = File.ReadAllText(@"C:\Users\Connor\Desktop\Matrix Generator\Matricies\Numbers\1.txt");
            CharacterMatrix characterMatrix = new CharacterMatrix(matrixStr);
            foreach (byte b in characterMatrix.matrix)
            {
                Console.WriteLine(b);
            }
            Console.WriteLine(Directory.GetCurrentDirectory());
            string characterRegistryData = File.ReadAllText(Directory.GetCurrentDirectory() + @"\" + "CharacterRegistry.txt");
            CharacterRegistry characterRegistry = new CharacterRegistry(characterRegistryData);
            foreach (byte b in characterRegistry.GetCharacterMatrix('0').matrix)
            {
                Console.WriteLine(b);
            }
        }
        private static void General()
        {
            Console.WriteLine("Hello World!");
            SerialPortInterface serialPortInterface = new SerialPortInterface();
            serialPortInterface.Setup();
            serialPortInterface.port.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
            {
                SerialPort port = serialPortInterface.port;
                int bytesToRead = port.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                port.Read(buffer, 0, bytesToRead);
                string data = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(data);
            };
            Console.WriteLine("Enter to send!");
            Console.ReadLine();
            Thread.Sleep(7000);
            for (int i = 0; i < 100; i++)
            {
                serialPortInterface.WriteShiftingPacman();
            }
            serialPortInterface.port.Close();
        }
    }
}
