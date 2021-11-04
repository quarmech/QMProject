using System;
using System.Threading;
using System.Threading.Tasks;


namespace QMProjectT
{
    public class Program
    {
        static Connection stageConnection= new Connection(4, 9600, 2000);
        static Connection alignerConnection = new Connection(6, 19200, 6000);

        static StageController stageControls = new StageController(stageConnection);
        static AlignerController alignerContorls = new AlignerController(alignerConnection);

        static UserInputHandler consoleApp = new UserInputHandler(stageControls, alignerContorls);

        public static bool End = false;
        
        public static async Task Main()
        {
            
            stageConnection.OpenPort();
            
            stageControls.SetupMotors();
            alignerConnection.OpenPort();
            //alignerConnection.ReadingLoop();

            //ping stages
            

            //main program loop
            while (!End)
            {
                Console.Write($"Command:");
                string input = Console.ReadLine();

                 consoleApp.UserCommand(input);              
            }

            await EndProcedure();


        }

        public static async Task EndProcedure()
        {
            //end procedure of aligner
            //end procdedure of stage
            //close aligner port
            //close stage port
            await stageControls.End();
            await alignerContorls.End();

        }
              

    }   
}
