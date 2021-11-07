using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class StageController
    {
        public Connection _conn;
        public bool Moving { get; private set; } = false;
        public IDictionary<int, string> Errors { get; private set; }
        public IDictionary<string, bool> JoyStickDict { get; private set; }

        //public IDictionary<string, bool> MovingDict { get; private set; }

        public StageController(Connection comm)
        {
            Console.WriteLine("initializing stage controls...");
            try
            {
                _conn = comm;
                AddErrorBits();
                //SetupAxis();
                SetupJoystick();              
            } 
            catch 
            {
                Console.WriteLine("Error: Could not connect to Stages");
            }
        }

        public void SetupMotors()
        {
            Console.WriteLine("seting up stage controls...");

            //Activate all motors          
            MotorOn("x");
            MotorOn("y");
            MotorOn("z");
            MotorOn("t");   
            
            //_conn.Write("0 s r0xab 1", true);
            //_conn.Write("1 s r0xab 1", true);
            //_conn.Write("2 s r0xab 1", true);
            //_conn.Write("3 s r0xab 1", true);
            /*
            _conn.Write("0 s r0x24 21", true);
            _conn.Write("1 s r0x24 21", true);
            _conn.Write("2 s r0x24 21", true);
            _conn.Write("3 s r0x24 21", true);
            */
            //set to position loop control          
            _conn.AddCommand(new Command("0 s r0x24 21", true, false));
            _conn.AddCommand(new Command("0 s r0x24 21", true, false));
            _conn.AddCommand(new Command("0 s r0x24 21", true, false));
            _conn.AddCommand(new Command("0 s r0x24 21", true, false));
            
            //default values
            Distance("x", 0);
            Distance("y", 0);
            Distance("z", 0);
            Distance("t", 0);
            Velocity("x", 120000);
            Velocity("y", 120000);
            Velocity("z", 150000);
            Velocity("t", 10000);
            
            Acceleration("x", 200000);
            Acceleration("y", 200000);
            Acceleration("z", 200000);
            Acceleration("t", 200000);
            Deceleration("x", 200000);
            Deceleration("y", 200000);
            Deceleration("z", 200000);
            Deceleration("t", 200000);
            
        }


        
        public void MotorOn(string axis)
        {
            _conn.AddCommand(new Command($"{Axis(axis)} s r0xab 1", true, false));          
        }
        public void MotorOff(string axis)
        {          
            _conn.AddCommand(new Command($"{Axis(axis)} s r0xab 0", true, false));
        }

        public void Stop(string axis)
        {
            string ascii = $"{Axis(axis)} t 0";
            Command command = new Command(ascii);
            _conn.AddCommand(command);         
        }

        public void Stop()
        {          
            Stop("x");
            Stop("y");
            Stop("z");
            Stop("t");
        }
        public void End()
        {
            //turn of joystick
            JoyStickOff();
            //stop all motors
            Stop();
            //close port
            _conn.End();
        }

        public async Task<int> HomeCheck(string axis)
        {
            //create command
            string ascii = $"{Axis(axis)} g r0xc9";
            Command command = new Command(ascii);
            _conn.AddCommand(command);

            //get result
            var res = await command.TSC.Task;

            //convert and return
            string[] splitRes = res.Split();      
            int.TryParse(splitRes[1], out int n);
            return n;
           
        }

        public async Task<bool> IsHomed(string axis)
        {
            int res = await HomeCheck(axis);
            int bit12 = 4096;
            if ((res & bit12) == bit12)
            {
                return true;
            }
            return false;
        }

        public async Task<int> Error(string axis)
        {
            //create command
            string ascii = $"{Axis(axis)} g r0xa0";
            Command command = new Command(ascii);
            _conn.AddCommand(command);

            //get response
            var errorCode = await command.TSC.Task;
            
            //convert and return
            string[] errorCodeSub = errorCode.Split();
            Int32.TryParse(errorCodeSub[1], out int errorNum);      
            return errorNum;           
        }
        public async Task JoyStickFast(string axis)
        {
            //_conn.Write($"{Axis(axis)} i r0 32769");
            string ascii = $"{Axis(axis)} i r0 32769";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            JoyStickDict[axis] = true;
            //if (axis=="z")
            //{
            //start joystick limit check loop
            JoyStickLimitCheck(axis);
            
            //}          
        }

        public async Task JoyStickFast()
        {
            JoyStickFast("x");
            JoyStickFast("y");
            JoyStickFast("z");
            JoyStickFast("t");
        }
        public async Task JoyStickSlow(string axis)
        {
            string ascii = $"{Axis(axis)} i r0 32770";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            JoyStickDict[axis] = true;
            JoyStickLimitCheck(axis);
        }
        public void JoyStickOff(string axis)
        {
            string ascii = $"{Axis(axis)} i r0 32771";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            JoyStickDict[axis] = false;
        }    
        public void JoyStickOff()
        {
             JoyStickOff("x");
             JoyStickOff("y");
             JoyStickOff("z");
             JoyStickOff("t");
        }
     
        private async Task JoyStickLimitCheck(string axis)
        {
            bool limitReached = false;
            int errorCode = await Error(axis);
            int posLimitBit = pow2(9);
            int negLimitBit = pow2(10);

            // if already at limit
            if (((errorCode & negLimitBit) == negLimitBit) ||
                   ((errorCode & posLimitBit) == posLimitBit))
            {
                JoyStickLimitCheck(axis);
            }
            else
            {
                while (JoyStickDict[axis])
                {
                    int errorCodeN = await Error(axis);

                    if (((errorCodeN & negLimitBit) == negLimitBit) ||
                        ((errorCodeN & posLimitBit) == posLimitBit))
                    {
                        if (!limitReached)
                        {
                            Console.WriteLine("limit reached");
                            limitReached = true;
                            JoyStickOff(axis);
                            //wait n seconds turn on joystick
                            await JoyStickDelayedOn(axis);
                        }
                    }
                    else
                    {
                        limitReached = false;
                    }
                    await Task.Delay(50);
                }
            }
        }

        public async Task JoyStickDelayedOn(string axis)
        {
            await Task.Delay(2000);
            await JoyStickFast(axis);
        }

        public async Task<bool> CheckMoveComplete(string axis)
        {
            //Command errorCommand = Error(axis);
            //that returns command object
            //run a loop to wait for commmand.Data to be updated?
            //do logic
            //var code = await errorCommand.TSC.Task;
            /*
            while(errorCommand.Data==null)
            {

            }
            */
            //Console.WriteLine("newCheckMoveComplete: " + code);

            int errorCode;
            int currentPos;
            int? prevPos = null;
            int bit27 = 134217728;
            int repeatCount = 0;
            while(true)
            {           
                errorCode = await Error(axis);
                if (((errorCode & bit27) != bit27))
                {
                    //Moving = false;
                    return false;
                }
                currentPos = await Position(axis);
                if (currentPos == prevPos)
                {
                    repeatCount+=1;
                }
                if (repeatCount>5)
                {
                    //Moving = false;
                    return false;
                }
                prevPos = currentPos;
                await Task.Delay(1000);
            }
        }
 
        public async Task HomeStage(string axis)
        {
            string ascii;
            Command command;
            if (await IsMoving(axis))
            {
                //return "axis in motion";
                //Console.WriteLine("axis in motion");
            }
            if (JoyStickDict[axis])
            {
                JoyStickOff(axis);
            }
            ascii = $"{Axis(axis)} t 2";
            //_conn.Write($"{Axis(axis)} t 2");
            command = new Command(ascii);
            _conn.AddCommand(command);

        }

        public async Task<bool> IsMoving(string axis)
        {
            int bit27 = 134217728;
            int errorCode;
            errorCode = await Error(axis);
            if (((errorCode & bit27) != bit27))
            {
                return false;
            }
            return true;
        }
        
        public async Task MoveAsync(string axis)
        {
            string ascii;
            Command command;
            if (await IsMoving(axis))
            {
                //return "axis in motion";
                Console.WriteLine("axis in motion");
                throw new OperationFailedException("axis in motion");             
            }
            if (JoyStickDict[axis])
            {
                JoyStickOff(axis);
            }
            ascii = $"{Axis(axis)} t 1";
            command = new Command(ascii);
            _conn.AddCommand(command);
        }

        public void Send(string command)
        {
            _conn.AddCommand(new Command(command, true, true));
        }

        public void Reset()
        {
            //note: settigns go to default (vel,accel,decel,etc... )
            //note2: "0 r" does not get response, so read set to false
            
            _conn.AddCommand(new Command("0 r", false));
            _conn.AddCommand(new Command("1 r", true));
            _conn.AddCommand(new Command("2 r", true));
            _conn.AddCommand(new Command("3 r", true));

        }

        public async Task MoveRelativeAsync(string axis, int value)
        {
            SetRelative(axis);
            Distance(axis, value);
            await MoveAsync(axis);     
        }

        public async Task MoveAbsoluteAsync(string axis, int pos)
        {
            //check if homed
            bool homed = await IsHomed(axis);
            if(!homed)
            {
                //await HomeAsync(axis);
                //await CheckMoveComplete(axis);
                Console.WriteLine("axis not homed");
                //throw new Exception("not homed");
                throw new OperationFailedException("axis not homed");
                //return;
            }

            SetAbsolute(axis);
            Distance(axis, pos);
            await MoveAsync(axis);
        }
        
        public void SetAbsolute(string axis)
        {           
            //create command
            string ascii = $"{Axis(axis)} s r0xc8 0";
            Command command = new Command(ascii, true, true);
            _conn.AddCommand(command);          
        }

        public void SetRelative(string axis)
        {   
            //create command
            string ascii = $"{Axis(axis)} s r0xc8 256";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
        }

        // get fsol status
        public async Task<int> Fsol(int number)
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

            string ascii = $"{axis} g r0xab";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            int n;              
            int.TryParse(res.Split()[1], out n);
            Console.WriteLine("Fsol: " + n);
            return n;

            //string res = _conn.Write($"{axis} g r0xab");
            /*
            int n;
            try
            {
                n = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                n = -1;
            }
            catch (FormatException)
            {
                n = -1;
            }
            return n;           
            */
        }

        //Set Fsol
        public async Task<int> Fsol(int number, string status)
        {

            int axis = 0;
            int bit = 0;

            if (number == 1)
            {
                axis = 0;
                bit = (status == "on") ? 3 : 1;
            }
            else if (number == 2)
            {
                axis = 0;
                bit = (status == "on") ? 5 : 1;
            }
            else if (number == 3)
            {
                axis = 3;
                bit = (status == "on") ? 3 : 1;
            }
            else if (number == 4)
            {
                axis = 3;
                bit = (status == "on") ? 5 : 1;
            }
            else if (number == 5)
            {
                axis = 2;
                bit = (status == "on") ? 5 : 1;
            }

            string ascii = $"{axis} s r0xab {bit}";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            //string res = _conn.Write($"{axis} s r0xab {bit}");
            int nres;
            try
            {
                nres = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                nres = -1;
            }
            catch (FormatException)
            {
                nres = -1;
            }
            return nres;
        }

        public void Distance(string axis, int? pos)
        {
            string ascii;
            Command command;
            if (pos is null)
            {
                ascii = $"{Axis(axis)} g r0xca";              
            }
            else
            {
                ascii = $"{Axis(axis)} s r0xca {pos}";
            }
            command = new Command(ascii, true, true);
            _conn.AddCommand(command);                
        }

        public async Task<int> Position(string axis)
        {
            string ascii;
            Command command;
            //create command
            ascii = $"{Axis(axis)} g r0x32";
            command = new Command(ascii);
            _conn.AddCommand(command);

            //get result
            string res = await command.TSC.Task;
            //convert to int
            Int32.TryParse(res.Split()[1], out int n);
            
            return n;
        }

        public async Task<string> Velocity(string axis, int? value)
        {
            //Console.WriteLine("in stagecontorls velocity");
            string res;
            //int nres;
            string ascii;
            Command command;
            if (value is null)
            {
                //get
                //_conn.Write(ascii,false);
                ascii = $"{Axis(axis)} g r0xcb";
                
                
                
                //res = await _conn.ReadBytes2Async();
                //Console.WriteLine(res);
            }
            else
            {
                //set
                //res = _conn.Write($"{Axis(axis)} s r0xcb {pos}");
                ascii = $"{Axis(axis)} s r0xcb {value}";
                
            }
            command = new Command(ascii);
            _conn.AddCommand(command);
            res = await command.TSC.Task;

            //Int32.TryParse(res.Split()[1], out nres);
            //if (value == null)
            //{
            //    Console.WriteLine("velocity: " + nres);
            //}
            return res;
            /*
            try
            {
                
                nres = Int32.Parse(res.Split()[1]);
            }
            catch (IndexOutOfRangeException)
            {
                nres = -1;
            }
            catch (FormatException)
            {
                nres = -1;
            }
            Console.WriteLine("velocity: " + nres);
            return nres;
            */

        }
        public async Task<int> Acceleration(string axis, int? pos)
        {
            string res;
            int nres;
            string ascii;
            Command command;
            
            if (pos is null)
            {
                //get
                //res = _conn.Write($"{Axis(axis)} g r0xcc");
                ascii = $"{Axis(axis)} g r0xcc";
                
            }
            else
            {
                //set
                //res = _conn.Write($"{Axis(axis)} s r0xcc {pos}");
                ascii = $"{Axis(axis)} s r0xcc {pos}";
                
            }
            command = new Command(ascii);
            _conn.AddCommand(command);
            res = await command.TSC.Task;
            Int32.TryParse(res.Split()[1], out nres);
            if (pos == null)
            {
                Console.WriteLine("acceleration: " + nres);
            }

            return nres;
            /*
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
            */
        }
        public async Task<int> Deceleration(string axis, int? pos)
        {
            string res;
            int nres;
            string ascii;
            Command command;
            if (pos is null)
            {
                //get
                //res = _conn.Write($"{Axis(axis)} g r0xcd");
                ascii = $"{Axis(axis)} g r0xcd";
                
            }
            else
            {
                //set
                //res =_conn.Write($"{Axis(axis)} s r0xcd {pos}");
                ascii = $"{Axis(axis)} s r0xcd {pos}";
                
            }
            command = new Command(ascii);
            _conn.AddCommand(command);
            res = await command.TSC.Task;
            Int32.TryParse(res.Split()[1], out nres);
            if (pos == null)
            {
                Console.WriteLine("acceleration: " + nres);
            }        
            return nres;
            /*
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
            */
        }

        public async Task<bool> WaferSensor()
        {
            string ascii = $"2 g r0xa6";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            int input6_1 = 32;

            string[] splitRes = res.Split();
            int.TryParse(splitRes[1], out int n);
            if ((n & input6_1) == 0)
            {
                return true;
            }
            return false;
        }

        public async Task WaitForWafer()
        {
            
            
            for (int i = 0; i < 100; i++)
            {
                
                bool status = await WaferSensor();

                if (status)
                {
                    return;
                }
                await Task.Delay(1000);
            }
            
            throw new OperationFailedException("no  wafer detected");
        }

        public async Task<bool> TBreakOn()
        {
            string ascii = $"0 g r0xa6";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            int input6_1 = 32;
            
            string[] splitRes = res.Split();
            int.TryParse(splitRes[1], out int n);
            if ((n & input6_1) == 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GripperOpen()
        {
            string ascii = $"0 g r0xa6";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            //int input7_2 = 64;
            int input8_3 = 128;
            string[] splitRes = res.Split();                        
            int.TryParse(splitRes[1], out int n);                     
            if ((n & input8_3) == 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GripperClosed()
        {
            string ascii = $"0 g r0xa6";
            Command command = new Command(ascii);
            _conn.AddCommand(command);
            string res = await command.TSC.Task;
            int input7_2 = 64;
            string[] splitRes = res.Split();
            int.TryParse(splitRes[1], out int n);
            
            if ((n & input7_2) == 0)
            {
                return true;
            }
            return false;
        }


        private int pow2(int exp)
        {
            return (int)Math.Pow(2,exp);
        }


        private void SetupJoystick()
        {
            JoyStickDict = new Dictionary<string, bool>();
            JoyStickDict.Add("x", false);
            JoyStickDict.Add("y", false);
            JoyStickDict.Add("z", false);
            JoyStickDict.Add("t", false);
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
