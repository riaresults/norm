using System;

// ReSharper disable CheckNamespace
namespace D10.Norm.ObjectBuilder
// ReSharper restore CheckNamespace
{
    internal static class PopulatorFactory
    {
        private static readonly CompositePopulator _composite = new CompositePopulator();
        private static readonly PrimitivePopulator _primitive = new PrimitivePopulator();
        private static readonly EnumPopulator _enum = new EnumPopulator();

        public static IPopulator GetPopulator(Type t)
        {
            if (_primitive.IsCompatible(t)) return _primitive;

            if (_enum.IsCompatible(t)) return _enum;

            return _composite;
        }
    }
}
