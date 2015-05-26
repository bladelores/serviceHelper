using System;
using System.Web.Services.Description;
using System.Net;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;


namespace ServiceHelper
{
    public class ReflectionServiceHelper : ServiceHelper
    {
        private ServiceDescription serviceInfo;

        public ReflectionServiceHelper(Uri url)
        {
            adresse = url;
            UriBuilder uriBuilder = new UriBuilder(adresse);
            uriBuilder.Query = Path.GetExtension(adresse.ToString()) == ".svc" ? "singleWSDL" : "WSDL";
            
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            webRequest.Accept = "text/xml";

            CookieContainer cookieContainer = new CookieContainer(); 
            webRequest.CookieContainer = cookieContainer;

            using (System.Net.WebResponse response = webRequest.GetResponse())
            using (System.IO.Stream stream = response.GetResponseStream())
            {
                serviceInfo = System.Web.Services.Description.ServiceDescription.Read(stream);
            }
           
            ServiceDescriptionImporter descriptionImporter = new ServiceDescriptionImporter();
            descriptionImporter.AddServiceDescription(serviceInfo, null, null);

            serviceName = serviceInfo.Services[0].Name;
            serviceAssembly = CompileAssembly(descriptionImporter);
            serviceInstance = serviceAssembly.CreateInstance(serviceName);
            serviceDeclarations = serviceInstance.GetType();
        }

        private Assembly CompileAssembly(ServiceDescriptionImporter descriptionImporter)
        {
            CodeNamespace codeNamespace = new CodeNamespace();
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            codeUnit.Namespaces.Add(codeNamespace);

            ServiceDescriptionImportWarnings importWarnings = descriptionImporter.Import(codeNamespace, codeUnit);

            CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
            string[] references = new string[2] { "System.Web.Services.dll", "System.Xml.dll" };

            CompilerParameters parameters = new CompilerParameters(references);
            CompilerResults results = compiler.CompileAssemblyFromDom(parameters, codeUnit);

            return results.CompiledAssembly;
        }
       
    }
}
