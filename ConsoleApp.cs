using System;

namespace QMProjectT
{
    class ConsoleApp
    {
        public StageControls sc;
        public ConsoleApp(StageControls stageControls)
        {
            sc = stageControls; 
        }

        public void Reset()
        {
            sc.Reset();
            Console.WriteLine("completed reset");
        }
        public void Error(string[] input)
        {
            try
            {
                var axis = input[1];
                int errorNum = sc.Error(axis);

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

        public void Deceleration(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    sc.Deceleration(axis, pos);
                }
                catch
                {
                }
                sc.Deceleration(axis, null);
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
                    sc.Acceleration(axis, pos);
                }
                catch
                {
                }
                sc.Acceleration(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Velocity(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    sc.Velocity(axis, pos);
                }
                catch (IndexOutOfRangeException)
                {
                }
                sc.Velocity(axis, null);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Position(string[] input)
        {
            try
            {
                var axis = input[1];
                sc.Position(axis);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis given");
            }
        }
        public void Distance(string[] input)
        {
            try
            {
                var axis = input[1];
                try
                {
                    int pos = Int32.Parse(input[2]);
                    sc.Distance(axis, pos);
                }
                catch
                {
                }
                sc.Distance(axis, null);
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
        public void Move(string[] input)
        {
            string axis;
            try
            {
                axis = input[1];
                sc.Move(axis);
            }
            catch
            {
                Console.WriteLine("no axis given");
            }
        }

        public void Home(string[] input)
        {
            string axis;
            try
            {
                axis = input[1];
                int res = sc.Home(axis);
                Console.WriteLine($"homeing complete, current position: {res}");
            }
            catch
            {
                Console.WriteLine("no axis given");
            }
        }

        public void MoveAbsolute(string[] input)
        {
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                sc.MoveAbsolute(axis, position);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis or no position given");
            }
        }
        public void MoveRelative(string[] input)
        {
            string axis;
            int position;
            try
            {
                axis = input[1];
                position = Int32.Parse(input[2]);
                int m = sc.MoveRelative(axis, position);
                Console.WriteLine($"moveing done, current position is : {m}");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("no axis or no position given");
            }
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
