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
            //General();
            MainFunction();
        }

        private static void MainFunction()
        {
            SerialPortInterface serialPortInterface = new SerialPortInterface();
            serialPortInterface.Setup();
            string charactersPath = GetPathGeneration(Directory.GetCurrentDirectory(), 6) + @"\characters";
            CharacterScroller characterScroller = new CharacterScroller();
            characterScroller.Configure(charactersPath);
            characterScroller.Updated += (byte[] obj) =>
            {
                byte[] array = new byte[64];
                Buffer.BlockCopy(obj, 0, array, 0, obj.Length);
                /*
                for (int i = 0; i < 64; i++)
                {
                    if (i < obj.Length - 1)
                        array[i] = obj[i];
                    else
                        array[i] = 0x00;
                }
                */
                Array.Reverse(array);
                serialPortInterface.Write64(array);
            };
            characterScroller.SetText("HELLO WORLD HELLO WORLD HELLO WORLD HELLO WORLD");
            for (int i = 0; i < 12212; i++)
            {
                characterScroller.Update();
                NOP(0.02);
                if (Console.KeyAvailable)
                    break;
            }
        }

        private static void NOP(double durationSeconds)
        {
            var durationTicks = Math.Round(durationSeconds * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks)
            {

            }
        }

        private static string GetPathGeneration(string child, int gens)
        {
            string bufferedPath = "";
            for (int i = 0; i < gens; i++)
            {
                if (i == 0)
                    bufferedPath = Directory.GetParent(child).FullName;
                else
                    bufferedPath = Directory.GetParent(bufferedPath).FullName;
            }
            return bufferedPath;
        }
        private static void Test()
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
