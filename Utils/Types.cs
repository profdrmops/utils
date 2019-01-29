using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Types
    {
        internal static readonly Type[] GuessableTypes = {typeof(double), typeof(int), typeof(bool), typeof(string)};
        
        public static Type Guess(string str)
        {
            foreach (Type t in GuessableTypes)
            {
                var converter = TypeDescriptor.GetConverter(t);
                if(converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        object temp = converter.ConvertFromInvariantString(str);
                        if (temp != null)
                            return t;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;
        }
    }
}
