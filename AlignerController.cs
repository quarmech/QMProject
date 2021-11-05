using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace QMProjectT
{
    public class AlignerController
    {
        public Connection _conn;
        public AlignerController(Connection comm)
        {
            Console.WriteLine("initializing aligner controls...");
            _conn = comm;
        }

        public async Task End()
        {         
            //close port
            _conn.End();
        }

        public async Task Escape()
        {
            string ascii = "ESC";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;          
        }

        public async Task<string> Home()
        {
            string ascii = "ZMD";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            return res;
        }

        public async Task<bool> VacuumStatus()
        {
            string ascii = "RVC";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            Console.WriteLine(res);
            if (res=="\n255\r")
            {
                return true;
            }
            return false;
        }

        public async Task CheckHome()
        {
            while(true)
            {
                string ascii = "ZRS";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
                string res = await command.TSC.Task;
                //int.TryParse(res, out int n);
                Console.Write("n:" + res);
                if (res == "\nD\r")                
                {
                    break;
                }
            }
        }

        public async Task CheckUp()
        {
            while (true)
            {
                string ascii = "ZRS";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
                string res = await command.TSC.Task;
                if (res == null)
                {
                    break;
                }
                //int.TryParse(res, out int n);
                Console.Write("n:" + res);
                if (res == "\nU\r")
                {
                    break;
                }
            }
        }

        public async Task CheckAlign()
        {          
            while (true)
            {
                string ascii = "ASG";
                Command command = new Command(ascii);
                _conn.AddCommand(command);
          
                string res = await command.TSC.Task;
                if (res==null)
                {
                    break;
                }
  
                //int.TryParse(res, out int n);
                Console.Write("n:" + res);
                if (res == "\nC\r")
                {
                    Console.WriteLine("align complete");
                    break;
                }
                if (res == "\nF\r")
                {
                    Console.WriteLine("align failed");
                    break;
                }
            }
        }

        public async Task MoveUp()
        {
            string ascii = "ZMX";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public async Task VacuumOn()
        {
            string ascii = "VAC1";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public async Task VacuumOff()
        {
            string ascii = "VAC0";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public async Task WaitVacuumOn()
        {
            for(int i=0; i<100; i++)
            {
                await Task.Delay(1000);
                bool status = await VacuumStatus();

                if (status)
                {
                    break;
                }
                    
            }
        }

        public async Task ZVacuumUp()
        {
            string ascii = "ZVMX";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public async Task Align()
        {
            string ascii = "APF";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
        }

        public void Command(string s)
        {
            Command command = new Command(s, true, true);
            _conn.AddCommand(command);
          
        }
        public string DelayedRead()
        {
            //wait for aligner to respond. 
            Thread.Sleep(5000);
            return _conn.Read();
        }


        public void Clear()
        {
            _conn.Clear();
        }

        public void Config()
        {        
            _conn.Write("VL", false);
            int count = 0;
                       
            while (true)
            {
                string line = _conn.Read();
                
                
                if (line=="")
                {
                    int row = Console.CursorTop;
                    Console.SetCursorPosition(0, row - 1);
                    Console.WriteLine($"Read {count} lines of config");
                    break;
                }
                count += 1;
                Console.WriteLine(line);    

            }           
            
        }
    }
}
