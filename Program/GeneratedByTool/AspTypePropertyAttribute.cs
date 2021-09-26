using System;

namespace MEATaste.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AspTypePropertyAttribute : Attribute
    {
        public bool CreateConstructorReferences { get; }

        public AspTypePropertyAttribute(bool createConstructorReferences)
        {
            CreateConstructorReferences = createConstructorReferences;
        }
    }
}