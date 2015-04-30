using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Services.Description;
using System.Net;
using System.IO;

namespace ServiceHelper
{
    public class ServiceHelper
    {
        private Uri adresse;
        private ServiceDescription serviceInfo;
        private string hostName;
        private string serviceName;
        private string serviceType;

        public ServiceHelper(Uri url)
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
                serviceInfo = ServiceDescription.Read(stream);

                hostName = serviceInfo.Types.Schemas[0].TargetNamespace;
                serviceName = serviceInfo.PortTypes[0].Name;
                serviceType = uriBuilder.Query == "WSDL" ? "asmx" : "svc";
            }

        }

        private List<MethodParameter> GetMethodParameters(string messagePartName) {
            List<MethodParameter> parameters = new List<MethodParameter>();

            Types types = serviceInfo.Types;
            System.Xml.Schema.XmlSchema xmlSchema = types.Schemas[0];

            foreach (System.Xml.Schema.XmlSchemaElement item in xmlSchema.Items)
            {
                System.Xml.Schema.XmlSchemaElement schemaElement = item;
                if (schemaElement != null)
                {
                    if (schemaElement.Name == messagePartName)
                    {
                        System.Xml.Schema.XmlSchemaType schemaType = schemaElement.SchemaType;
                        System.Xml.Schema.XmlSchemaComplexType complexType = schemaType as System.Xml.Schema.XmlSchemaComplexType;
                        if (complexType != null)
                        {
                            System.Xml.Schema.XmlSchemaParticle particle = complexType.Particle;
                            System.Xml.Schema.XmlSchemaSequence sequence = particle as System.Xml.Schema.XmlSchemaSequence;
                            if (sequence != null)
                            {
                                foreach (System.Xml.Schema.XmlSchemaElement childElement in sequence.Items)
                                {
                                    string parameterName = childElement.Name;
                                    string parameterType = childElement.SchemaTypeName.Name;
                                    parameters.Add(new MethodParameter(parameterName, parameterType));
                                }
                            }
                        }
                    }
                }
            }
            return parameters;
        }

        public List<MethodInfo> GetServiceMethods()
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (PortType portType in serviceInfo.PortTypes)
            {
                foreach (Operation operation in portType.Operations)
                {
                    string inputMessageName = operation.Messages.Input.Message.Name;
                    string outputMessageName = operation.Messages.Output.Message.Name;

                    string inputMessagePartName =
                        serviceInfo.Messages[inputMessageName].Parts[0].Element.Name;
                    string outputMessagePartName =
                        serviceInfo.Messages[outputMessageName].Parts[0].Element.Name;

                    List<MethodParameter> inputParameters = GetMethodParameters(inputMessagePartName);
                    List<MethodParameter> outputParameters = GetMethodParameters(outputMessagePartName);                  

                    if (inputParameters.Count == 0)
                        methods.Add(new MethodInfo(operation.Name, /*inputParameters.ToArray(),*/ outputParameters.ToArray()));
                }
            }
            return methods;
        }

        public StreamReader CallMethod(string methodName)
        {         
            string requestString = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body>{0}</s:Body></s:Envelope>";  
            string soapAction = "\"" + hostName;
            soapAction += serviceType == "svc" ? (serviceName + '/' + methodName + '\"') : (methodName + '\"');
            
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(adresse.ToString());
            request.Headers.Add("SOAPAction", soapAction);
            request.ContentType = "text/xml; charset=utf-8";
            request.Method = "POST";

            using (Stream reqStream = request.GetRequestStream())
            {
                requestString = string.Format(requestString, string.Format("<{0} xmlns=\"{1}\"></{0}>", methodName, hostName));
                using (StreamWriter sw = new StreamWriter(reqStream))
                {
                    sw.Write(requestString);
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            return responseStream;
        }
        /*
        public StreamReader CallMethod(string methodName, Dictionary<string, object> methodParameters)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(adresse.ToString());
            string requestString = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><{0} xmlns=""http://tempuri.org/"">{1}</{0}></s:Body></s:Envelope>";

            request.Method = "POST";
            request.Headers.Add("SOAPAction", "\"http://tempuri.org/" + serviceInfo.PortTypes[0].Name + '/' + methodName + '\"');
            request.ContentType = "text/xml; charset=utf-8";

            using (Stream reqStream = request.GetRequestStream())
            {
                string parameters = "";
                foreach (var param in methodParameters)
                {
                    parameters += string.Format("<{0}>{1}</{0}>", param.Key, param.Value.ToString());
                }

                requestString = string.Format(requestString, methodName, parameters);
                
                using (StreamWriter sw = new StreamWriter(reqStream))
                {
                    sw.Write(requestString);
                }
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            return responseStream;
        }*/
    }
}
