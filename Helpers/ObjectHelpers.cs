using System;
using System.ComponentModel;
using System.Linq;

namespace ExtremeInsiders.Helpers
{
    public static class ObjectHelpers
    {
        private static string Divider => "======================================";
        
        public static string Dump(this object obj, string prepend = null)
        {
            var firstLine = "| " + (prepend ?? "OBJECT DUMP") + " |" + Divider.Substring(prepend?.Length ?? 0) + "\n";
            var str = firstLine + "";
            if (prepend != null)
                str += "| " + new string('-', prepend.Length) + " |"  + Divider.Substring(prepend.Length) + "\n";
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                var name = descriptor.Name;
                var value = descriptor.GetValue(obj);
                str += "| " + name + ": " + value + "\n";
            }

            str += "| " + new string('=', firstLine.Length - 3);
            return str;
        }
    }
}