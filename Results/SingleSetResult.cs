using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm.Results
{
    public class SingleSetResult<T> : ResultBase
    {
        private readonly IList<T> _data;

        public SingleSetResult(IEnumerable<T> items)
        {
            _data = new List<T>(items);
        }

        public IList<T> Data { get { return _data; } }
    }
}
