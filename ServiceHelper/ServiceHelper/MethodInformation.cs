using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHelper
{
    public class MethodInformation
    {
        private string name;
        //private MethodParameter[] inputParameters;
        private MethodParameter outputParameter;

        public MethodInformation(string name, /*MethodParameter[] inputParameters,*/ MethodParameter outputParameter)
        {
            this.name = name;
            //this.inputParameters = inputParameters;
            this.outputParameter = outputParameter;
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
        public MethodParameter OutputParameter
        {
            get { return outputParameter; }
        }
    }
}
