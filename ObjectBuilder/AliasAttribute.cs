using System;

namespace D10.Norm
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AliasAttribute : Attribute
    {
        private readonly string[] _aliases;

        public AliasAttribute(params string[] aliases)
        {
            _aliases = aliases;
        }

        public string[] Aliases
        {
            get { return _aliases; }
        }
    }
}