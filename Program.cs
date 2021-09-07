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
                string code = Console.ReadLine();
          
                if (stringComparer.Equals("quit", code))
                {
                    stageConnection.Close();
                    break;
                }
                else if (stringComparer.Equals("xhome", code))
                {
                    stageControls.XHome();
                }
                else if (stringComparer.Equals("yhome", code))
                {
                    stageControls.YHome();
                }
                else if (stringComparer.Equals("movex", code))
                {                 
                    stageControls.MoveX(code);
                }
                else if (stringComparer.Equals("movey", code))
                {
                    stageControls.MoveY(code);
                }
                else if (stringComparer.Equals("moveAbsolute", code.Split()[0]))
                {                   
                    stageControls.MoveAbsolute(code);                  
                }
                else if (stringComparer.Equals("moveRelative", code.Split()[0]))
                {
                    stageControls.MoveRelative(code);
                }                             
                else if (stringComparer.Equals("setAbsolute", code.Split()[0]))
                {
                    stageControls.SetAbsolute(code);
                }
                else if (stringComparer.Equals("setRelative", code.Split()[0]))
                {
                    stageControls.SetRelative(code);
                }
                else if (stringComparer.Equals("pos", code.Split()[0]))
                {
                    stageControls.Position(code);
                }
                else if (stringComparer.Equals("vel", code.Split()[0]))
                {
                    stageControls.Velocity(code);
                }
                else if (stringComparer.Equals("accel", code.Split()[0]))
                {
                    stageControls.Acceleration(code);
                }
                else if (stringComparer.Equals("decel", code.Split()[0]))
                {
                    stageControls.Deceleration(code);
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
                    stageConnection.WriteRead(code);
                }

            }
        }
        static public void Sequance1()
        {
            stageControls.MoveAbsolute('x', 12000);
            stageControls.MoveAbsolute('x', 120000);
            stageControls.MoveAbsolute('y', 50000);
            stageControls.MoveAbsolute('x', -100000);

        }
    }
}
