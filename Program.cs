using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;          // Serial stuff in here.
using System.Threading;
using System.IO;

namespace Serial
{
    class Program
    {
        static SerialPort port;
        static void Main(string[] args)
        {
            int baud;
            string name;

            if (args.Length < 2)
            {
                Console.WriteLine("Welcome, usage");
                Console.WriteLine();
                Console.WriteLine("SerialComm.exe [port] [baud] ([filename-to-import] [targer-address])*");
                Console.WriteLine();
                Console.WriteLine("* optional");
                return;    
            }

            // Get Port
            if (!SerialPort.GetPortNames().ToList().Exists(x => x.ToLower() == args[0].ToLower()))
            {
                Console.WriteLine("Invalid port, available ports:");
                if (SerialPort.GetPortNames().Count() >= 0)
                {
                    foreach (string p in SerialPort.GetPortNames())
                    {
                        Console.WriteLine(p);
                    }
                }
                else
                {
                    Console.WriteLine("No Ports available, press any key to exit.");
                    Console.ReadLine();
                }
                return;
            }
            else
            {
                name = args[0].ToLower();
            }

            // Get baud
            try
            {
                baud =  int.Parse(args[1]);
            }
            catch
            {
                Console.WriteLine("Baud: Invalid integer.");
                return;
            }

            //Get File
            if (args.Length >= 3)
            {
                Console.WriteLine();
                Console.WriteLine("Open Serial...");
                BeginSerial(baud, name);
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
                Console.WriteLine("Serial Started.");
                using (Stream source = File.OpenRead(args[2]))
                {
                    int pos = 0;
                    if (args.Length == 4)
                    {
                        if (!int.TryParse(args[3], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out pos)) {
                            Console.WriteLine("Error converting: " + args[4] + " to HEX");
                            return;
                        }
                    }
                    
                    byte[] buffer = new byte[16];
                    int bytesRead;
                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //Console.WriteLine("write " + pos.ToString("X") + " " + string.Join(' ', buffer.Select(x => x.ToString("X")).Take(bytesRead).ToArray()));
                        port.WriteLine("write " + pos.ToString("X") + " " + string.Join(' ', buffer.Select(x => x.ToString("X")).Take(bytesRead).ToArray()));
                        pos = pos + buffer.Length;
                        Thread.Sleep(500);
                     
                    }

                    while (true)
                    {
                        port.WriteLine(Console.ReadLine());
                    }
                }
            }


            //No file
            if (args.Length < 3) 
            {
                Console.WriteLine();
                Console.WriteLine("Open Serial...");
                BeginSerial(baud, name);
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                try 
                {
                    port.Open();
                }
                catch (Exception ex) {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
                Console.WriteLine("Serial Started.");
                Console.WriteLine(" ");
                Console.WriteLine("Ctrl+C to exit program");
                Console.WriteLine("Send:");

                while (true)
                {
                    port.WriteLine(Console.ReadLine());
                }
            }
        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(port.ReadLine());
        }

        static void BeginSerial(int baud, string name)
        {
            port = new SerialPort(name, baud);
        }
    }
}