using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectT
{
    class StageControls
    {
        public BasicConnection _conn;
        public bool JoyStick = false;
        public bool Moving = false;
        
        public StageControls(BasicConnection comm)
        {
            _conn = comm;
            _conn.OpenConnection();
            this.Setup();
            this.HomeX();
            this.HomeY();         
        }
        public void Setup()
        {
            //Activate all motors
            _conn.WriteRead("0 s r0xab 1");
            _conn.WriteRead("1 s r0xab 1");
            _conn.WriteRead("2 s r0xab 1");
            _conn.WriteRead("3 s r0xab 1");
            //set to position loop contorl
            _conn.WriteRead("0 s r0x24 21");
            _conn.WriteRead("1 s r0x24 21");
            _conn.WriteRead("2 s r0x24 21");
            _conn.WriteRead("3 s r0x24 21");
        }

        public void JoystickOn()
        {          
            _conn.WriteRead("0 i r0 32769");
            _conn.WriteRead("1 i r0 32769");
            _conn.WriteRead("2 i r0 32769");
            this.JoyStick = true;
        }
        public void JoystickOff()
        {
            _conn.WriteRead("0 i r0 32771");
            _conn.WriteRead("1 i r0 32771");
            _conn.WriteRead("2 i r0 32771");
            this.JoyStick = false;
        }
        public void HomeX()
        {
            _conn.WriteRead("0 t 2");

            Moving = true;

            while (true)
            {
                string errorCode = _conn.WriteRead("0 g r0xa0");
                string currentPos = _conn.WriteRead("0 g r0x32");
                string[] errorCodeSub = errorCode.Split();
                int bit27 = 134217728;
                int errorNum = Int32.Parse(errorCodeSub[1]);

                if ((errorNum & bit27) == bit27)
                {
                    //Console.WriteLine("moving");
                }
                else
                {
                    Console.WriteLine("done moving");
                    break;
                }
            }
        }
        public void HomeY()
        {
            _conn.WriteRead("1 t 2");

            while (true)
            {
                string errorCode = _conn.WriteRead("1 g r0xa0");
                string currentPos = _conn.WriteRead("1 g r0x32");
                string[] errorCodeSub = errorCode.Split();
                int bit27 = 134217728;
                int errorNum = Int32.Parse(errorCodeSub[1]);

                if ((errorNum & bit27) == bit27)
                {
                    //Console.WriteLine("moving");
                }
                else
                {
                    Console.WriteLine("done moving");
                    break;
                }
            }
        }
        public void MoveX(int n)
        {
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            _conn.WriteRead($"0 s r0xca {n}");
            
            
            _conn.WriteRead("0 t 1");
        }
        public void MoveY(int n)
        {
            if (this.JoyStick)
            {
                this.JoystickOff();
            }

            _conn.WriteRead($"1 s r0xca {n}");


            _conn.WriteRead("1 t 1");
        }
        
        public void Reset()
        {
            _conn.Send($"0 r");
            _conn.Send($"1 r");
            _conn.Send($"2 r");
            _conn.Send($"3 r");
        }
        public void MoveRelative(string axis, int pos)
        {
            string saxis = axis.ToString();
            string spos = pos.ToString();
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            _conn.WriteRead($"{Axis(saxis)} s r0xc8 256");
            _conn.WriteRead($"{Axis(saxis)} s r0xca {spos}");
            _conn.WriteRead($"{Axis(saxis)} t 1");
        }
        public void MoveRelative(string options)
        {
            //format => moveabsolute x 90000

            string axis;
            string n;

            if (this.JoyStick)
            {
                this.JoystickOff();
            }

            try
            {
                axis = options.Split()[0];
                //set to absolute move mode
                _conn.WriteRead($"{Axis(axis)} s r0xc8 256");
                try
                {
                    n = options.Split()[1];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0xca {n}");
                    Console.WriteLine($"moving by distance: {n}");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("no number given");
                }
                //start move
                _conn.WriteRead($"{Axis(axis)} t 1");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }

        }
        public void MoveAbsolute(string axis, int pos)
        {
            string saxis = axis.ToString();
            string spos = pos.ToString();
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            _conn.WriteRead($"{Axis(saxis)} s r0xc8 0");
            _conn.WriteRead($"{Axis(saxis)} s r0xca {spos}");
            _conn.WriteRead($"{Axis(saxis)} t 1");
        }
        public void MoveAbsolute(string options)
        {
            //format => moveabsolute axis 90000
            string axis;
            string n;    
            try
            {
                axis = options.Split()[0];
                //set to absolute move mode
                _conn.WriteRead($"{Axis(axis)} s r0xc8 0");
                try
                {
                    n = options.Split()[1];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0xca {n}");
                    Console.WriteLine($"moving to position {n}");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("no number given");
                }
                //start move
                _conn.WriteRead($"{Axis(axis)} t 1");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
            
        }


        public void SetAbsolute(string axis)
        {                                   
            _conn.WriteRead($"{Axis(axis)} s r0xc8 0");  
        }
        public void SetRelative(string axis)
        {
            _conn.WriteRead($"{Axis(axis)} s r0xc8 256");
        }       

        public void Position(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.WriteRead($"{Axis(axis)} g r0x32");
            }
            else
            {
                //set
                _conn.WriteRead($"{Axis(axis)} s r0x32 {pos}");
            }                          
        }

        public void Velocity(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.WriteRead($"{Axis(axis)} g r0xcb");
            }
            else
            {
                //set
                _conn.WriteRead($"{Axis(axis)} s r0xcb {pos}");
            }
        }

        public void Acceleration(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.WriteRead($"{Axis(axis)} g r0xcc");
            }
            else
            {
                //set
                _conn.WriteRead($"{Axis(axis)} s r0xcc {pos}");
            }
        }

        public void Deceleration(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.WriteRead($"{Axis(axis)} g r0xcd");
            }
            else
            {
                //set
                _conn.WriteRead($"{Axis(axis)} s r0xcd {pos}");
            }
        }

        
   
        
        public string Axis(string a)
        {
            switch (a)
            {
                case "x":
                    return "0";
                case "y":
                    return "1";
                case "z":
                    return "2";
                case "t":
                    return "3";
                default:
                    return "e";
            }
        }


    }

}
