using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm.Results
{
    public class ScalarResult<T> : ResultBase
    {
        public T Value { get; set; }
    }
}
