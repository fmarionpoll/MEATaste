using System;

namespace MEATaste.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RazorWriteLiteralMethodAttribute : Attribute { }
}