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


        }

        public void OpenConnection()
        {
            try
            {
                base.PortName = ComPortName;
                base.Open();
                this.Clear();
                Console.WriteLine("opened");
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
            while(true)
            {
                try
                {
                    this.Recv();
                }
                catch (TimeoutException)
                {
                    break;
                }
            }
            Console.WriteLine("clear completed");
        }
            

        public void Send(string code)
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
        }

        public string Recv()
        {
            try
            {
                string response = base.ReadTo("\r");
                Console.WriteLine($"response: {response}");
                return response;
            }
            catch (TimeoutException ex)
            {
                string error = "Failed to read response";
                Console.WriteLine(error);
                throw ex;
            }
        }

        public void End()
        {
            base.Close();
        }

        public string WriteRead(string code)
        {
            this.Send(code);
            string response = this.Recv();
            return response;
        }
    }
}
