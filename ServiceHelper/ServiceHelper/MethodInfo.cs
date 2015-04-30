using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHelper
{
    public class MethodInfo
    {
        private string name;
        //private MethodParameter[] inputParameters;
        private MethodParameter[] outputParameters;

        public MethodInfo(string name, /*MethodParameter[] inputParameters,*/ MethodParameter[] outputParameters)
        {
            this.name = name;
            //this.inputParameters = inputParameters;
            this.outputParameters = outputParameters;
        }
        public string Name
        {
            get { return name; }
        }

        /*
        public MethodParameter[] InputParameters
        {
            get { return inputParameters; }
        }
        */
        public MethodParameter[] OutputParameters
        {
            get { return outputParameters; }
        }
    }
}
