using System;
using UnityEngine;

namespace SerializeReferenceDropdown.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeReferenceDropdownNameAttribute : PropertyAttribute
    {
        public readonly string name;
        public readonly int[] menuOrder;

        public SerializeReferenceDropdownNameAttribute(string name, params int[] menuOrder)
        {
            this.name = name;
            this.menuOrder = menuOrder;
        }
    }
}