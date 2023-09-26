using System;
using UnityEngine;

namespace SerializeReferenceDropdown.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeReferenceDropdownNameAttribute : PropertyAttribute
    {
        public readonly string Name;
    
        public SerializeReferenceDropdownNameAttribute(string name)
        {
            Name = name;
        }
    }
}