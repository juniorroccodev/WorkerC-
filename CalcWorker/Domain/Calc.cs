using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcWorker.Domain
{
    public class Calc
    {
      

        public long Id { get; set; }
        public decimal number1 { get; set; }
        public decimal number2 { get; set; }
        public string status { get; set; }
        public decimal resultado { get; set; }

    }
}
