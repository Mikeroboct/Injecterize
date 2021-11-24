using System;
using os_ms_students.utils;

namespace Injecterize
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class InjecterizeAttribute : Attribute
    {
        public string Name { get; set; }
        public InstanceScope TargetScope { get; set; }
        public Type InterfaceToUse { get; set; }
        
        

        public InjecterizeAttribute(string name = "", InstanceScope scope = InstanceScope.Scope , Type interfaceToUse = null)
        {
            Name = name;
            TargetScope = scope;
            InterfaceToUse = interfaceToUse;
        }
        
        public InjecterizeAttribute()
        {
            Name = "";
            TargetScope = InstanceScope.Scope;
            InterfaceToUse = null;
        }
    }
}