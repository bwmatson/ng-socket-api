using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWebSocket.Contract
{
    public class Stock
    {
        public int Open { get; set; }
        public int Close { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public string DateTime { get; set; }
    }
}
