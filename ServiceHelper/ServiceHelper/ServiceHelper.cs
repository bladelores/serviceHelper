using System;
using System.Collections.Generic;
using System.Reflection;

namespace ServiceHelper
{
    public class ServiceHelper
    {
        protected Uri adresse;
        protected Assembly serviceAssembly;
        protected object serviceInstance;
        protected Type serviceDeclarations;
        protected string serviceName;

        public List<MethodInformation> GetServiceMethods()
        {
            List<MethodInformation> methods = new List<MethodInformation>();
            MethodInfo[] methodsInfo = serviceDeclarations.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (System.Reflection.MethodInfo methodInfo in methodsInfo)
            {
                List<MethodParameter> inputParameters = new List<MethodParameter>();
                foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                {
                    inputParameters.Add(new MethodParameter(paramInfo.Name, paramInfo.ParameterType.ToString()));
                }

                MethodParameter outputParameter = new MethodParameter(methodInfo.ReturnParameter.Name, methodInfo.ReturnType.ToString());

                if (inputParameters.Count == 0)
                    methods.Add(new MethodInformation(methodInfo.Name, /*inputParameters.ToArray(),*/ outputParameter));
            }

            return methods;
        }

        public T CallMethod<T>(string methodName)
        {
            return (T)serviceDeclarations.InvokeMember(methodName, BindingFlags.InvokeMethod, null, serviceInstance, null);
        }
     
    }
}
