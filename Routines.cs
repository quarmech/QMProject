using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class Routines
    {    
        static public bool Stop { get; set; } = false;

        static Routines(){}
        static public async Task PickUpWaferAndAlignCycle(StageController sc, AlignerController ac, int size)
        {
            int count = 0;
            Stop = false;
            while(!Stop)
            {
                await WaferPickUpPosition(sc, ac);
                await AlignWafer(sc, ac, size);
                count += 1;
                Console.WriteLine("count:"+count);
            }          
        }

        static public async Task WaferPickUpPosition(StageController sc, AlignerController ac)
        {
            try
            {
                //pick up wafer
                await sc.MoveAbsoluteAsync("x", Positions.XAlignLocation);               
                await sc.MoveAbsoluteAsync("y", Positions.Center["y"]);
                await sc.CheckMoveComplete("x");
                await sc.CheckMoveComplete("y");

                ac.VacuumOff();
                await ac.WaitVacuumOff();   

                //ungrip
                await sc.Fsol(2, "on");

                ac.MoveUp();
                await ac.WaitForUp();

                Console.WriteLine("ready for wafer");
            }
            catch (OperationFailedException)
            {
                Stop = true;
                Console.WriteLine("routine failed");
            }
        }

        static public async Task AlignWafer(StageController sc, AlignerController ac, int size)
        {
            try
            {
                await sc.WaitForWafer();

                //Go to align position
                await sc.MoveAbsoluteAsync("x", Positions.XAlignLocation);
                await sc.MoveAbsoluteAsync("y", Positions.YAlignLocations[size]);
                
                //ungrip
                await sc.Fsol(2, "on");
                await sc.WaitForUngrip();

                //turn off vacuum
                ac.VacuumOff();
                await ac.WaitVacuumOff();         

                //move wafer down
                var a = await ac.MoveDown();              
                await ac.WaitForDown();

                //center wafer (grip)
                await sc.Fsol(1, "on");              
                await sc.WaitForGrip();          

                //vacuum On
                await ac.VacuumOn();
                await ac.WaitVacuumOn();

                //Move up with vacuum
                await ac.ZVacuumUp();
                await ac.WaitForUp();

                //make sure in correct position
                await sc.CheckMoveComplete("x");
                await sc.CheckMoveComplete("y");

                //align wafer
                await ac.Align();
                //TODO: check alignment success
                //wait for align to complete
                //await Task.Delay(2000);
                await ac.WaitForAlign();

                Console.WriteLine("wafer aligned");
            }
            catch (OperationFailedException)
            {
                Stop = true;
                Console.WriteLine("operation failed");
            }
        }
        
        static public async Task Routine1(StageController sc, AlignerController ac)
        {
            while (!Stop)
            {
                await sc.HomeStage("y");
                await sc.CheckMoveComplete("y");
                await sc.MoveAbsoluteAsync("y", 200000);
                await sc.CheckMoveComplete("y");
            }
            Stop = false;
        }
    }
}
