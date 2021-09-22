using System;
using System.Collections.Generic;

namespace QMProjectT
{
    class StageControls
    {
        public BasicConnection _conn;
        public bool JoyStick = false;
        public bool Moving { get; private set; }
        public IDictionary<int, string> Errors { get; private set; }           

        public StageControls(BasicConnection comm)
        {
            _conn = comm;           
            AddErrorBits();
            Setup();                   
        }
        public void Setup()
        {
            //Activate all motors
            _conn.Write("0 s r0xab 1", true, true);
            _conn.Write("1 s r0xab 1", true, true);
            _conn.Write("2 s r0xab 1", true, true);
            _conn.Write("3 s r0xab 1", true, true);
            //set to position loop contorl
            _conn.Write("0 s r0x24 21", true, true);
            _conn.Write("1 s r0x24 21", true, true);
            _conn.Write("2 s r0x24 21", true, true);
            _conn.Write("3 s r0x24 21", true, true);
        }

        public void MotorOn(string axis)
        {
            _conn.Write($"{Axis(axis)} s r0xab 1");
        }
        public void MotorOff(string axis)
        {
            _conn.Write($"{Axis(axis)} s r0xab 0");
        }

        public int Error(string axis)
        {          
            string errorCode = _conn.Write($"{Axis(axis)} g r0xa0");
            
            string[] errorCodeSub = errorCode.Split();
            
            int errorNum = Int32.Parse(errorCodeSub[1]);

            return errorNum;

        }  
        public void JoystickOn()
        {          
            _conn.Write("0 i r0 32769");
            _conn.Write("1 i r0 32769");
            _conn.Write("2 i r0 32769");
            this.JoyStick = true;
        }
        public void JoystickOff()
        {
            _conn.Write("0 i r0 32771");
            _conn.Write("1 i r0 32771");
            _conn.Write("2 i r0 32771");
            this.JoyStick = false;
        }       
        
        public int Wait(string axis)
        {
            int previousPos = Position(axis);
            int count = 0;
            while (true)
            {
                int errorCode = Error(axis);
                int currentPos = Position(axis);               
                int bit27 = 134217728;
                
                //Check if done moving or stuck in same position
                if ((errorCode & bit27) != bit27 || count >= 10)
                {
                    Moving = false;
                    return currentPos;
                }              
                if (currentPos == previousPos)
                {
                    count += 1;
                    previousPos = currentPos;
                }
            }
        }
        public int Home(string axis)
        {
            _conn.Write($"{Axis(axis)} t 2");
            return Wait(axis);
        }

        public int Move(string axis)
        {
            if (this.JoyStick)
            {
                this.JoystickOff();
            }                              
            _conn.Write($"{Axis(axis)} t 1");
            return Wait(axis);
        }
    
        public void Reset()
        {
            _conn.Write($"0 r", false);
            _conn.Write($"1 r", false);
            _conn.Write($"2 r", false);
            _conn.Write($"3 r", false);
        }
        public int MoveRelative(string axis, int pos)
        {
            SetRelative(axis);
            Distance(axis, pos);           
            Move(axis);
            return Wait(axis);
        }      
        public int MoveAbsolute(string axis, int pos)
        {
            SetAbsolute(axis);
            Distance(axis, pos);
            Move(axis);
            return Wait(axis);
        }
        public void SetAbsolute(string axis)
        {                                   
            _conn.Write($"{Axis(axis)} s r0xc8 0");  
        }
        public void SetRelative(string axis)
        {
            _conn.Write($"{Axis(axis)} s r0xc8 256");
        }

        // get fsol status
        public int Fsol(int number)
        {
            int axis = 0;         

            if (number == 1 || number == 2)
            {
                axis = 0;
            }
            else if (number == 3 || number == 4)
            {
                axis = 3;
            }
            else if (number == 5)
            {
                axis = 2;
            }
            
            string res = _conn.Write($"{axis} g r0xab");
            int n;
            try
            {
                n = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                n = -1;
            }
            return n;           
        }

        //Set Fsol
        public int Fsol(int number, string command)
        {
            int axis = 0;
            int bit = 0;
            
            int curBit = Fsol(number);
            
            int set(int n) => SetBit(curBit, n);
            int clr(int n) => ClearBit(curBit, n);

            if (number == 1)
            {
                axis = 0;
                bit = (command == "on") ? set(2) : clr(2);
            }
            else if (number == 2)
            {
                axis = 0;
                bit = (command == "on") ? set(4) : clr(4);
            }
            else if (number == 3)
            {
                axis = 3;
                bit = (command == "on") ? set(2) : clr(2);
            }
            else if (number == 4)
            {
                axis = 3;
                bit = (command == "on") ? set(4) : clr(4);
            }
            else if (number == 5)
            {
                axis = 2;
                bit = (command == "on") ? set(4) : clr(4);
            }


            string res = _conn.Write($"{axis} s r0xab {bit}");
            int nres;
            try
            {
                nres = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                nres = -1;
            }
            return nres;
        }

        private int SetBit(int value, int bit)
        {
            return (value | bit);
        }
        private int ClearBit(int value, int bit)
        {
            return (value & ~bit);
        }
        public void Distance(string axis, int? pos)
        { 
            if (pos is null)
            {
                //get
                _conn.Write($"{Axis(axis)} g r0xca");
            }
            else
            {
                //set
                _conn.Write($"{Axis(axis)} s r0xca {pos}");
            }
        }
        public int Position(string axis)
        {                   
            string res = _conn.Write($"{Axis(axis)} g r0x32");
            int nres;
            try
            {
                nres = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                nres = -1;
            }
            return nres;
        }
        public int Velocity(string axis, int? pos)
        {
            string res;
            if (pos is null)
            {
                //get
                res = _conn.Write($"{Axis(axis)} g r0xcb");
            }
            else
            {
                //set
                res = _conn.Write($"{Axis(axis)} s r0xcb {pos}");
            }
            int nres;
            try
            {
                nres = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                nres = -1;
            }
            return nres;
        }
        public void Acceleration(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.Write($"{Axis(axis)} g r0xcc");
            }
            else
            {
                //set
                _conn.Write($"{Axis(axis)} s r0xcc {pos}");
            }
        }
        public void Deceleration(string axis, int? pos)
        {
            if (pos is null)
            {
                //get
                _conn.Write($"{Axis(axis)} g r0xcd");
            }
            else
            {
                //set
                _conn.Write($"{Axis(axis)} s r0xcd {pos}");
            }
        } 
        private int pow2(int exp)
        {
            return (int)Math.Pow(2,exp);
        }
        private void AddErrorBits()
        {           
            Errors = new Dictionary<int, string>();
            Errors.Add(pow2(0), "Short circuit detected.");
            Errors.Add(pow2(1), "Drive over temperature.");
            Errors.Add(pow2(2), "Over voltage");
            Errors.Add(pow2(3), "Under voltage");
            Errors.Add(pow2(4), "Motor temperature sensor active");
            Errors.Add(pow2(5), "Feedback error");
            Errors.Add(pow2(6), "Motor phasing error.");
            Errors.Add(pow2(7), "Current output limited.");
            Errors.Add(pow2(8), "Voltage output limited");
            Errors.Add(pow2(9), "Positive limit switch active");
            Errors.Add(pow2(10), "Negative limit switch active");
            Errors.Add(pow2(11), "Enable input not active");
            Errors.Add(pow2(12), "Drive is disabled by software");
            Errors.Add(pow2(13), "Trying to stop motor");
            Errors.Add(pow2(14), "Motor brake activated");
            Errors.Add(pow2(15), "PWM outputs disabled.");
            Errors.Add(pow2(16), "Positive software limit condition");
            Errors.Add(pow2(17), "Negative software limit condition");
            Errors.Add(pow2(18), "Tracking error.");
            Errors.Add(pow2(19), "Tracking warning.");
            Errors.Add(pow2(20), "Drive has been reset");
            Errors.Add(pow2(21), "Position has wrapped");
            Errors.Add(pow2(22), "Drive fault");
            Errors.Add(pow2(23), "Velocity limit has been reached");
            Errors.Add(pow2(24), "Acceleration limit has been reached.");
            Errors.Add(pow2(25), "Position outside of tracking window.");
            Errors.Add(pow2(26), "Home switch is active");
            Errors.Add(pow2(27), "Set if trajectory is running or motor has not yet settled into position");
            Errors.Add(pow2(28), "Velocity window");
            Errors.Add(pow2(29), "Phase not yet initialized");
            Errors.Add(pow2(30), "Command fault");
            Errors.Add(pow2(31), "Not defined.");          
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
