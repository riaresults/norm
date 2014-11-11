using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D10.Norm.Results;

namespace D10.Norm.ObjectBuilder
{
    internal interface IPopulator
    {
        bool RequiresTypeDescription();
        bool IsCompatible(Type t);
        T BuildInstance<T>(SqlDataRecord row, TypeDescription description);
    }
}
