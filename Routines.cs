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
        static public async Task PickUpWaferAndAlign(StageController sc, AlignerController ac, int size)
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
                //await sc.CheckMoveComplete("x");
                await sc.MoveAbsoluteAsync("y", Positions.Center["y"]);
                await sc.CheckMoveComplete("y");

                //await Task.WhenAll(sc.MoveAbsoluteAsync("x", Positions.PosLimit["x"]), sc.MoveAbsoluteAsync("y", Positions.Center["y"]) );
                await ac.VacuumOff();
                //if vacuum still on return;
                bool vacuum = await ac.VacuumStatus();
                if (vacuum)
                {
                    throw new OperationFailedException("vacuum did not shut off");
                    //Console.WriteLine("vacuum did not shut off");
                    //Stop = true;
                    //return;
                }

                //ungrip
                await sc.Fsol(2, "on");

                await ac.MoveUp();
                await ac.WaitForChuckUp();
                Console.WriteLine("ready for wafer");
            }
            catch (OperationFailedException ex)
            {
                Stop = true;
                Console.WriteLine("operation failed");
            }
        }

        static public async Task AlignWafer(StageController sc, AlignerController ac, int size)
        {
            //await Task.WhenAll(sc.MoveAbsoluteAsync("x", Positions.PosLimit["x"]), sc.MoveAbsoluteAsync("y", Positions.Center["y"]) );
            //await Task.Delay(3000);
            try
            {
                await sc.WaitForWafer();

                //Go to align position
                await sc.MoveAbsoluteAsync("x", Positions.XAlignLocation);
                await sc.MoveAbsoluteAsync("y", Positions.YAlignLocations[size]);
                //await sc.CheckMoveComplete("y");
                //await Task.WhenAll(sc.MoveAbsoluteAsync("x", Positions.XAlignLocation), sc.MoveAbsoluteAsync("y", Positions.YAlignLocation[size]));

                //ungrip
                await sc.Fsol(2, "on");
                //TODO: check if delay enought
                await Task.Delay(100);
                //check gripper status via presence sensor
                bool isClosed = await sc.GripperClosed();
                if (isClosed)
                {
                    Console.WriteLine("gripper did not open");
                    Stop = true;
                    return;
                }

                //turn off vacuum and check status
                await ac.VacuumOff();
                bool vacuum = await ac.VacuumStatus();
                if (vacuum)
                {
                    Console.WriteLine("vacuum did not shut off");
                    Stop = true;
                    return;
                }

                //move wafer down
                var a = await ac.Home();
                //wait for chuck to be down
                await ac.CheckHome();
                //center wafer (grip)
                await sc.Fsol(1, "on");
                await Task.Delay(500);
                //TODO: check grip status
                bool open = await sc.GripperOpen();
                if (open)
                {
                    Console.WriteLine("gripper did not close");
                    Stop = true;
                    return;
                }
                //delay
                //await Task.Delay(2000);

                //vacuum On and check status
                await ac.VacuumOn();
                await Task.Delay(500);

                //Move up with vacuum
                await ac.ZVacuumUp();

                //tODO: wait for up
                await ac.WaitForChuckUp();
                //check z status.
                //check vacuum
                await ac.WaitVacuumOn();
             
                //align wafer
                await ac.Align();
                //TODO: check alignment success
                //wait for align to complete
                await Task.Delay(2000);
                await ac.CheckAlign();

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
                await sc.HomeAsync("y");
                await sc.CheckMoveComplete("y");
                await sc.MoveAbsoluteAsync("y", 200000);
                await sc.CheckMoveComplete("y");
            }
            Stop = false;
        }
    }
}
