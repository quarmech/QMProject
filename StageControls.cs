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
        
        public StageControls(BasicConnection comm)
        {
            _conn = comm;
            _conn.OpenConnection();
            this.Setup();
            this.XHome();
            this.YHome();         
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
        public void XHome()
        {
            _conn.WriteRead("0 t 2");

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
        public void YHome()
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
        public void MoveX(string code)
        {
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            try
            {
                var pos = code.Split()[1];
                _conn.WriteRead($"0 s r0xca {pos}");
            }
            catch (IndexOutOfRangeException)
            {                
            }
            _conn.WriteRead("0 t 1");
        }
        public void MoveY(string code)
        {
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            try
            {
                var pos = code.Split()[1];
                _conn.WriteRead($"1 s r0xca {pos}");
            }
            catch (IndexOutOfRangeException)
            {
            }
            _conn.WriteRead("1 t 1");
        }     
        public void Reset()
        {
            _conn.Send($"0 r");
            _conn.Send($"1 r");
            _conn.Send($"2 r");
            _conn.Send($"3 r");
        }

        public void MoveRelative(string code)
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
                axis = code.Split()[1];
                //set to absolute move mode
                _conn.WriteRead($"{Axis(axis)} s r0xc8 256");
                try
                {
                    n = code.Split()[2];
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
        public void MoveAbsolute(char axis, int pos)
        {
            string axiss = axis.ToString();
            string poss = pos.ToString();
            if (this.JoyStick)
            {
                this.JoystickOff();
            }
            _conn.WriteRead($"{Axis(axiss)} s r0xc8 0");
            _conn.WriteRead($"{Axis(axiss)} s r0xca {poss}");
            _conn.WriteRead($"{Axis(axiss)} t 1");
        }
        public void MoveAbsolute(string code)
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
                axis = code.Split()[1];
                //set to absolute move mode
                _conn.WriteRead($"{Axis(axis)} s r0xc8 0");
                try
                {
                    n = code.Split()[2];
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


        public void SetAbsolute(string code)
        {
            string axis;           
            try
            {
                axis = code.Split()[1];                                      
                _conn.WriteRead($"{Axis(axis)} s r0xc8 0");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void SetRelative(string code)
        {
            string axis;
            try
            {
                axis = code.Split()[1];
                _conn.WriteRead($"{Axis(axis)} s r0xc8 256");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        public void Position(string code)
        {
            string axis;
            string n;
            try
            {
                axis = code.Split()[1];
                try
                {
                    n = code.Split()[2];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0x32 {n}");
                }
                catch (IndexOutOfRangeException)
                {

                }
                // get            
                _conn.WriteRead($"{Axis(axis)} g r0x32");

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
    

        public void Velocity(string code)
        {
            string axis;
            string n;
            try
            {
                axis = code.Split()[1];
                try
                {
                    n = code.Split()[2];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0xcb {n}");
                }
                catch (IndexOutOfRangeException)
                {

                }
                // get
                _conn.WriteRead($"{Axis(axis)} g r0xcb");
                
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Acceleration(string code)
        {
            string axis;
            string n;
            try
            {
                axis = code.Split()[1];
                try
                {
                    n = code.Split()[2];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0xcc {n}");
                }
                catch (IndexOutOfRangeException)
                {

                }
                // get            
                _conn.WriteRead($"{Axis(axis)} g r0xcc");

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Deceleration(string code)
        {
            string axis;
            string n;
            try
            {
                axis = code.Split()[1];
                try
                {
                    n = code.Split()[2];
                    //set
                    _conn.WriteRead($"{Axis(axis)} s r0xcd {n}");
                }
                catch (IndexOutOfRangeException)
                {

                }
                // get            
                _conn.WriteRead($"{Axis(axis)} g r0xcd");

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
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
