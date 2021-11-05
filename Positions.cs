using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class Positions
    {
        public static IDictionary<string, int> Center { get; private set; }
        public static IDictionary<string, int> PosLimit { get; private set; }

        public static int XAlignLocation = 317955;

        public static IDictionary<int, int> YAlignLocations = new Dictionary<int, int>()
        {
                { 150, 2460 },
                { 200, 27552 },
                { 300, 77762 }
        };

        public static int XCenter = 159480;
        public static int YCenter = 155083;
        public static int ZCenter = 481762;
        public static int TCenter = 0;

        public static int XPosLimit = 318961;
        public static int YPosLimit = 310167;
        public static int ZPosLimit = 963524;
        public static int TPosLimit = 10494;

        public static int TNegLimit = -12547;


        public static void AddLocations()
        {
            Center = new Dictionary<string, int>() {
                { "x", 159480 },
                { "y", 155083 },
                { "z", 481762 },
                { "t", 0 }
            };
            PosLimit = new Dictionary<string, int>() {
                { "x", 318961 },
                { "y", 310167 },
                { "z", 963524 },
                { "t", 10494 },
                { "tNegative",  -12547 }
            };
            
        }

    }
}
