using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormServer
{
    public class Room
    {
        public int active;
        public int count;
        public int index1, index2;
        public string pw="";
        public int loaiphong;
        public int[,] arr;
        public Room()
        {
            active = 0;
            count = 0;
            index1 = -1;
            index2 = -1;
            loaiphong = 0;
            arr = new int[9, 9];
        }        
    }
}
