using System;
using System.IO.Ports;

namespace QMProjectT
{
    public class BasicConnection : SerialPort
    {
        static private string ComPortName = "";
        public BasicConnection(int ComPortNum, int Baud) : base()
        {
            base.BaudRate = Baud;
            base.Parity = Parity.None;
            base.DataBits = 8;
            base.StopBits = StopBits.One;

            base.ReadTimeout = 1000;
            base.WriteTimeout = 1000;

            ComPortName = "COM" + ComPortNum;
            Console.WriteLine($"intializing at {ComPortName}");

            try
            {
                base.PortName = ComPortName;
                base.Open();
                Console.WriteLine("connection opened");
                this.Clear();               
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: Port {0} is in use", ComPortName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Uart exception: " + ex);
            }
        }

        public void Clear()
        {
            /* 
             * read until nothing to read
             */           
            while (true)
            {
                try
                {
                    Read(true);
                }
                catch (TimeoutException)
                {
                    break;
                }
            }
            Console.WriteLine("output cleared");
        }
            

        public string Write(string code, bool read = true, bool silent=false)
        {
            try
            {
                base.WriteLine(code);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Uart exception: " + ex);
            }
            catch (TimeoutException)
            {
                string error = "Failed to read response";
                Console.WriteLine(error);         
            }

            if (!read)
            {
                return "read is off";
            }
            return Read(silent);

        }

        public string Read(bool silent = false)
        {
            try
            {
                string response = base.ReadTo("\r");

                if (!silent)
                {
                    Console.WriteLine($"response: {response}");
                }
                return response;
            }
            catch (TimeoutException ex)
            {
                if (!silent)
                {
                    string error = "Failed to read response";
                    Console.WriteLine(error);
                }

                throw ex;
                //return "ex";
            }
        }

        public void End()
        {
            base.Close();
        }

        public string WriteRead(string code, bool silent=false)
        {
            this.Write(code);
            string response = this.Read(silent);
            return response;
        }

        
    }
}
