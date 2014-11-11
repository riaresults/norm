using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm.Results
{
    public abstract class ResultBase
    {
        public TimeSpan Duration { get; set; }

        public int RecordsAffected { get; set; }
    }
}


