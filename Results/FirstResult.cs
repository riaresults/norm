using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm.Results
{
    public class FirstResult<T> : ResultBase
    {
        private readonly T _data;

        public FirstResult(T data)
        {
            _data = data;
        }

        public T Data { get { return _data; } }
    }
}
