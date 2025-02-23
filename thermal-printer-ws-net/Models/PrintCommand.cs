using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thermalPrinterWsNet.Models
{
    internal class PrintCommand
    {
        public string Action { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
        public bool Mode { get; set; }
        public string ImagePath { get; set; }
    }
}
