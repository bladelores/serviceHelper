using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHelper
{
    public class MethodParameter
    {
        public string Name;
        public string Type;
        public MethodParameter(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
