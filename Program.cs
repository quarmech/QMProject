using System;

namespace QMProjectT
{
    class Program
    {
        static BasicConnection stageConnection = new BasicConnection(4, 9600);
        //static BasicConnection alignerConnection = new BasicConnection(5, 19200);
        //static AsyncConnection stageConnection = new AsyncConnection(4, 9600);

        static StageControls stageControls = new StageControls(stageConnection);
        //static AlignerControls alignerContorls = new AlignerControls(alignerConnection);

        static StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

        static ConsoleApp consoleApp = new ConsoleApp(stageControls);
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
                else if (stringComparer.Equals("home", code))
                {
                    consoleApp.Home(splitInput);
                }
                else if (stringComparer.Equals("move", code))
                {
                    consoleApp.Move(splitInput);
                }
                else if (stringComparer.Equals("moveAbs", code))
                {
                    consoleApp.MoveAbsolute(splitInput);
                }
                else if (stringComparer.Equals("moveRel", code))
                {
                    consoleApp.MoveRelative(splitInput);
                }
                else if (stringComparer.Equals("setAbs", code))
                {
                    consoleApp.SetAbsolute(splitInput);
                }
                else if (stringComparer.Equals("setRel", code))
                {
                    consoleApp.SetRelative(splitInput);
                }
                else if (stringComparer.Equals("pos", code))
                {
                    consoleApp.Position(splitInput);
                }
                else if (stringComparer.Equals("dis", code))
                {
                    consoleApp.Distance(splitInput);
                }
                else if (stringComparer.Equals("vel", code))
                {
                    consoleApp.Velocity(splitInput);
                }
                else if (stringComparer.Equals("accel", code))
                {
                    consoleApp.Acceleration(splitInput);
                }
                else if (stringComparer.Equals("decel", code))
                {
                    consoleApp.Deceleration(splitInput);
                }
                else if (stringComparer.Equals("reset", code))
                {
                    consoleApp.Reset();
                }
                else if (stringComparer.Equals("joyon", code))
                {
                    stageControls.JoystickOn();
                }
                else if (stringComparer.Equals("joyoff", code))
                {
                    stageControls.JoystickOff();
                }
                else if (stringComparer.Equals("sequance1", code))
                {
                    Sequance1();
                }
                else if (stringComparer.Equals("sequance2", code))
                {
                    Sequance2();
                }
                else if (stringComparer.Equals("sequance3", code))
                {
                    Sequance3();
                }
                else if (stringComparer.Equals("error", code))
                {
                    consoleApp.Error(splitInput);
                }
                else if (stringComparer.Equals("on", code))
                {
                    consoleApp.MotorOn(splitInput);
                }
                else if (stringComparer.Equals("off", code))
                {
                    consoleApp.MotorOff(splitInput);
                }
                else if (stringComparer.Equals("fsol", code))
                {
                    consoleApp.Fsol(splitInput);
                }
                else
                {
                    stageConnection.Write(input);
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
        static public void Sequance2()
        {
            for (int i = 0; i < 10; i++)
            {
                stageControls.MoveRelative("z", 20000);
                stageControls.MoveRelative("z", -20000);
            }
        }
        static public void Sequance3()
        {
            for (int i = 0; i < 10; i++)
            {
                stageControls.MoveRelative("x", 30000);
                stageControls.MoveRelative("y", 30000);
                stageControls.MoveRelative("x", -30000);
                stageControls.MoveRelative("y", -30000);
            }
        }

    }   
}
