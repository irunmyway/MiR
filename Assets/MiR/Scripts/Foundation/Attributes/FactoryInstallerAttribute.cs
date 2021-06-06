using System;
using UnityEngine;

namespace Foundation
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FactoryInstallerAttribute : Attribute
    {
        public readonly Type FactoryType;

        public FactoryInstallerAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }
    }
}
