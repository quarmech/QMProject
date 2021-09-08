using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace QMProjectT
{
    class Program
    {
        static StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        
        static BasicConnection stageConnection = new BasicConnection(4, 9600);
        //static BasicConnection alignerConnection = new BasicConnection(5, 19200);

        static StageControls stageControls = new StageControls(stageConnection);
        //static AlignerControls alignerContorls = new AlignerControls(alignerConnection);
        static void Main()
        {
            //main program loop
            while (true)
            {
                Console.Write($"Command:");
                string input = Console.ReadLine();
                var splitInput = input.Split();
                var code = splitInput[0];                              
                
                if (stringComparer.Equals("quit", code))
                {
                    stageConnection.End();
                    break;
                }
                else if (stringComparer.Equals("homex", code))
                {
                    stageControls.HomeX();
                }
                else if (stringComparer.Equals("homey", code))
                {
                    stageControls.HomeY();
                }
                else if (stringComparer.Equals("movex", code))
                {
                    MoveX(splitInput);
                }
                else if (stringComparer.Equals("movey", code))
                {
                    MoveY(splitInput);
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
                    Position(splitInput);
                }
                else if (stringComparer.Equals("vel", code))
                {
                    Velocity(splitInput);
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
                    stageControls.Reset();
                }
                else if (stringComparer.Equals("joystickon", code))
                {
                    stageControls.JoystickOn();
                }
                else if (stringComparer.Equals("joystickoff", code))
                {
                    stageControls.JoystickOff();
                }
                else if (stringComparer.Equals("sequance1", code))
                {
                    Sequance1();
                }
                else
                {
                    stageConnection.WriteRead(input);
                }

            }
        }
        static public void Sequance1()
        {
            stageControls.MoveAbsolute("x", 120000);
            stageControls.MoveAbsolute("x", 20000);
            stageControls.MoveAbsolute("y", 50000);
            stageControls.MoveAbsolute("x", 100000);

        }
        static public void Deceleration(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    stageControls.Deceleration(axis, pos);
                }
                catch
                {
                }
                stageControls.Deceleration(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void Acceleration(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    stageControls.Acceleration(axis, pos);
                }
                catch
                {
                }
                stageControls.Acceleration(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void Velocity(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    stageControls.Velocity(axis, pos);
                }
                catch
                {
                }
                stageControls.Velocity(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void Position(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {                   
                    int pos = Int32.Parse(input[2]);
                    stageControls.Position(axis, pos);
                }
                catch
                {                   
                }
                stageControls.Position(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void SetAbsolute(string[] input)
        {           
            try
            {
                var axis = input[1];
                stageControls.SetAbsolute(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void SetRelative(string[] input)
        {
            try
            {
                var axis = input[1];
                stageControls.SetRelative(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        static public void MoveX(string[] input)
        {
            try
            {
                int value = Int32.Parse(input[1]);
                stageControls.MoveX(value);
            }
            catch
            {
                Console.WriteLine("no position/distance given");
            }
        }
        static public void MoveY(string[] input)
        {
            try
            {
                int value = Int32.Parse(input[1]);
                stageControls.MoveY(value);
            }
            catch
            {
                Console.WriteLine("no position/distance given");
            }
        }
        static public void MoveAbsolute(string[] input)
        {
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                stageControls.MoveAbsolute(axis, position);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis or no position given");
            }           
        }
        static public void MoveRelative(string[] input)
        {
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                stageControls.MoveRelative(axis, position);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis or no position given");
            }
        }
    }
}

