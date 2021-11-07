using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class UserInputHandler
    {
        private StageController sc;
        private AlignerController ac;

        private  StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

        public UserInputHandler(StageController stageControls, AlignerController alignerControls)
        {
            sc = stageControls;
            ac = alignerControls;     
        }
  
        public async Task<string> UserCommand(string command)
        {
            string res = "";
            var splitInput = command.Split();
            var code = splitInput[0];

            if (stringComparer.Equals("quit", code))
            {
                Quit();
            }
            else if (stringComparer.Equals("home", code))
            {
                Home(splitInput);
            }
            else if (stringComparer.Equals("moveAbs", code))
            {
                MoveAbsolute(splitInput);
            }
            else if (stringComparer.Equals("moveRel", code))
            {
                MoveRelative(splitInput);
            }
            else if (stringComparer.Equals("setAbs", code))
            {
                SetAbsolute(splitInput);
            }
            else if (stringComparer.Equals("setRel", code))
            {
                SetRelative(splitInput);
            }
            else if (stringComparer.Equals("pos", code))
            {
                await Position(splitInput);
            }
            else if (stringComparer.Equals("dis", code))
            {
                Distance(splitInput);
            }
            else if (stringComparer.Equals("vel", code))
            {
                res = await Velocity(splitInput);
            }
            else if (stringComparer.Equals("accel", code))
            {
                Acceleration(splitInput);
            }
            else if (stringComparer.Equals("decel", code))
            {
                Deceleration(splitInput);
            }
            else if (stringComparer.Equals("reset", code))
            {
                Reset();
            }
            else if (stringComparer.Equals("joyfast", code))
            {
                JoyStickFast(splitInput);
            }
            else if (stringComparer.Equals("joyslow", code))
            {
                JoyStickSlow(splitInput);
            }
            else if (stringComparer.Equals("joyoff", code))
            {
                JoyStickOff(splitInput);
            }
            else if (stringComparer.Equals("routine1", code))
            {
                Routines.Routine1(sc, ac);
            }
            else if (stringComparer.Equals("alignwafer300", code))
            {
                await Routines.AlignWafer(sc, ac, 300);
            }
            else if (stringComparer.Equals("alignwafer200", code))
            {
                await Routines.AlignWafer(sc, ac, 200);
            }
            else if (stringComparer.Equals("alignwafer150", code))
            {
                await Routines.AlignWafer(sc, ac, 150);
            }
            else if (stringComparer.Equals("pickupwafer", code))
            {
                await Routines.WaferPickUpPosition(sc, ac);
            }
            else if (stringComparer.Equals("pickupandalign", code))
            {
                await Routines.PickUpWaferAndAlign(sc, ac, 300);
            }
            else if (stringComparer.Equals("error", code))
            {
                Error(splitInput);
            }
            else if (stringComparer.Equals("on", code))
            {
                MotorOn(splitInput);
            }
            else if (stringComparer.Equals("off", code))
            {
                MotorOff(splitInput);
            }
            else if (stringComparer.Equals("fsol", code))
            {
                Fsol(splitInput);
            }
            else if (stringComparer.Equals("read", code))
            {
                sc._conn.ReadBytes2();
            }
            else if (stringComparer.Equals("aread", code))
            {
                res = ac._conn.ReadBytes2();
                Console.WriteLine(res);
            }
            else if (stringComparer.Equals("aclear", code))
            {
                AlignerClear();
            }
            else if (stringComparer.Equals("a", code))
            {
                AlignerCommand(splitInput);
            }
            else if (stringComparer.Equals("joyonall", code))
            {
                sc.JoyStickFast();
            }
            else if (stringComparer.Equals("joyoffall", code))
            {
                sc.JoyStickOff();
            }
            else if (stringComparer.Equals("stop", code))
            {
                Stop();
            }
            else if (stringComparer.Equals("center", code))
            {
                Center(splitInput);
            }
            else if (stringComparer.Equals("poslimit", code))
            {
                PositiveLimit(splitInput);
            }
            else if (stringComparer.Equals("vacuumOn", code))
            {
                ac.VacuumOn();
            }
            else if (stringComparer.Equals("vacuumOff", code))
            {
                ac.VacuumOff();
            }
            else if (stringComparer.Equals("vacuumstatus", code))
            {
                VacuumStatus();
            }
            else if (stringComparer.Equals("gripstatus", code))
            {
                GripStatus();
            }
            else if (stringComparer.Equals("tbreakstatus", code))
            {
                TBreakStatus();
            }
            else if (stringComparer.Equals("waferstatus", code))
            {
                WaferStatus();
            }
            else if (stringComparer.Equals("grip", code))
            {
                sc.Fsol(1, "on");
            }
            else if (stringComparer.Equals("ungrip", code))
            {
                sc.Fsol(2, "on");
            }
            else if (stringComparer.Equals("rotatewafer", code))
            {
                RotateWafer(splitInput);
            }
            else
            {
                sc.Send(command);
            }
            return res;
        }

        

        public async Task TBreakStatus()
        {
            bool tbreak = await sc.TBreakOn();
            //bool close = await sc.GripperClosed();
            if (tbreak)
            {
                Console.WriteLine("break on");
            }
            else
            {
                Console.WriteLine("break off");
            }
        }

        public async Task GripStatus()
        {
            bool open = await sc.GripperOpen();
            //bool close = await sc.GripperClosed();
            if (open)
            {
                Console.WriteLine("gripper opened");
            }
            else
            {
                Console.WriteLine("gripper closed");
            }
        }
        public async Task Center(string[] input)
        {
            string axis;
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {               
                axis = input[1];
                await sc.MoveAbsoluteAsync(axis, Positions.Center[axis]);
            }            
        }

        public async Task PositiveLimit(string[] input)
        {
            string axis;
            if (input.Length < 2)
            {
                Console.WriteLine("no axis given");
            }
            else
            {
                axis = input[1];
                await sc.MoveAbsoluteAsync(axis, Positions.PosLimit[axis]);
            }
        }
        

        public void Quit()
        {                
            Program.End = true;
        }
        public void Stop()
        {
            Routines.Stop = true;
            sc.Stop();
        }

        public void Print(int value)
        {
            Console.WriteLine($"response: {value}");
        }    
        public void Reset()
        {
            sc.Reset();
            Console.WriteLine("completed reset");
        }

        public async Task RotateWafer(string[] input)
        {
            try
            {
                var value = input[1];
                //Int32.TryParse(Split()[1], out int n);
                await ac.RotateWafer(value);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no value given");
            }

        }
        public async Task Error(string[] input)
        {
            try
            {
                var axis = input[1];
                //sc.Error(axis);
                
                int errorNum = await sc.Error(axis);

                Console.WriteLine($"error code: {errorNum}");

                foreach (int key in sc.Errors.Keys)
                {
                    if ((errorNum & key) == key)
                    {
                        Console.WriteLine($"{Math.Log10(key) / Math.Log10(2)} : {sc.Errors[key]}");
                    }
                }            
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        public void JoyStickFast(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.JoyStickFast(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        
        public void JoyStickSlow(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.JoyStickSlow(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void JoyStickOff(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.JoyStickOff(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        public void Deceleration(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    sc.Deceleration(axis, pos); //set
                }
                catch (IndexOutOfRangeException)
                {
                    sc.Deceleration(axis, null); //get
                    //Print(res);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Acceleration(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    //int res = sc.Acceleration(axis, pos); //set
                    sc.Acceleration(axis, pos);
                }
                catch (IndexOutOfRangeException)
                {
                    //int res = sc.Acceleration(axis, null); //get
                    //Print(res);
                    sc.Acceleration(axis, null);
                }             
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public async Task<string> Velocity2(string[] input)
        {
            string res = "";
            if (input.Length==1)
            {
                res = "no axis given";
            }
            else if(input.Length==2)
            {
                var axis = input[1];
                res = await sc.Velocity(axis, null);
                if (input.Length==3)
                {
                    int value;
                    int.TryParse(input[2], out value);
                    await sc.Velocity(axis, value);
                }
            }

            Console.WriteLine("Velocity: " + res);
            return res;
        }
        public async Task<string> Velocity(string[] input)
        {
            //Console.WriteLine("in consoleAPP velocity");
            string res = "";

            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    //int n = await sc.Velocity(axis, pos);  //set
                    res = await sc.Velocity(axis, pos);
                    //res = n.ToString();  
                }
                catch (IndexOutOfRangeException)
                {
                    //int n = await sc.Velocity(axis, null); //get
                    res = await sc.Velocity(axis, null);
                    //res = n.ToString();
                }               
            }
            catch (IndexOutOfRangeException)
            {
                res = "no axis given";
                
            }
            Console.WriteLine(res);
            return res;
        }
        public async Task Position(string[] input)
        {
            string res = "";
            try
            {
                var axis = input[1];
                //int n = await sc.Position(axis); // get
                //sc.Position(axis);
                sc.Position(axis); // get
                //res = n.ToString();              
            }
            catch (IndexOutOfRangeException)
            {
                res = "no axis given";              
            }
            //Console.WriteLine("Position: " + res);
            
        }
        public void Distance(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    //int res = sc.Distance(axis, pos); //set
                    sc.Distance(axis, pos);

                }
                catch
                {
                    //int res = sc.Distance(axis, null); //get
                    sc.Distance(axis, null);
                    //Print(res);
                }                                          
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }           
        }
        public void SetAbsolute(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.SetAbsolute(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void SetRelative(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.SetRelative(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        
        public void Home(string[] input)
        {           
            string axis;
            if (input.Length<2)
            {
                Console.WriteLine("no axis given");               
            }
            else
            {
                axis = input[1];
                sc.HomeStage(axis);
            }           
        }      

        public async Task<string> MoveAbsolute2(string[] input)
        {
            string res = "";
            string axis;
            int position;
            if (input.Length<3)
            {
                res = "no axis or no position given";
            }
            else
            {
                axis = input[1];
                position = Int32.Parse(input[2]);              
                sc.MoveAbsoluteAsync(axis, position);
                res = "moving started";
            }
            Console.WriteLine(res);
            return res;
        }
        public async Task<string> MoveAbsolute(string[] input)
        {
            string res = "";
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                //res = await sc.MoveAbsoluteAsync(axis, position);
                sc.MoveAbsoluteAsync(axis, position);
            }
            catch (IndexOutOfRangeException)
            {
                res = "no axis or no position given";                
            }
            Console.WriteLine(res);
            return res;
        }

        
        public async Task<string> MoveRelative(string[] input)
        {
            string res = "";
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                //res = await sc.MoveRelativeAsync(axis, position);
                sc.MoveRelativeAsync(axis, position);
                //Console.WriteLine($"moving done, current position is : {m}");
            }
            catch (IndexOutOfRangeException)
            {
                res = "no axis or no position given";              
            }
            Console.WriteLine(res);
            return res;
        }
        public void MotorOn(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.MotorOn(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void MotorOff(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.MotorOff(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }

        public void Fsol(string[] input)
        {
            //format: fsol <number> <command>
            try
            {
                int number = Int32.Parse(input[1]);
                try
                {
                    var command = input[2];
                    sc.Fsol(number, command);
                }
                catch (IndexOutOfRangeException)
                {
                    sc.Fsol(number);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no number given");
            }
        }

        /*
         * Aligner
         * 
         */

        public async Task VacuumStatus()
        {
            bool status = await ac.VacuumStatus();
            //bool close = await sc.GripperClosed();
            if (status)
            {
                Console.WriteLine("vacuum is on");
            }
            else
            {
                Console.WriteLine("vacuum is off");
            }
        }

        public async Task WaferStatus()
        {
            bool status = await sc.WaferSensor();
            //bool close = await sc.GripperClosed();
            if (status)
            {
                Console.WriteLine("wafer is sensed");
            }
            else
            {
                Console.WriteLine("no wafer");
            }
        }

        public void AlignerClear()
        {
            ac.Clear();
        }

        public void AlignerCommand(string[] input)
        {
            //string res = ac.Command(String.Join(" ",input.Skip(1)));
            ac.Command(String.Join(" ", input.Skip(1)));
            //Console.WriteLine(res);
        }
        public void AlignerConfig()
        {
            ac.Config();          
        }

        public int Axis(string a)
        {
            switch (a)
            {
                case "x":
                    return 0;
                case "y":
                    return 1;
                case "z":
                    return 2;
                case "t":
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
