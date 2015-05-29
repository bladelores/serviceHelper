using System;
//!
using System.CodeDom.Compiler;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ServiceHelper
{
    public class FactoryServiceHelper : ServiceHelper
    {
        private MetadataSet serviceMetadata;
        private string targetNamespace;
        private IRequestChannel serviceChannel;
        private ContractDescription serviceInfo;

        public FactoryServiceHelper(Uri url)
        {
            adresse = url;
            Uri mexAddress = new Uri(adresse.ToString() + "?wsdl");
            //for MEX endpoints use a MEX address and a mexMode of .MetadataExchange
            MetadataExchangeClientMode mexMode = MetadataExchangeClientMode.HttpGet;
            MetadataExchangeClient mexClient = new MetadataExchangeClient(mexAddress, mexMode);
            mexClient.ResolveMetadataReferences = true;
            serviceMetadata = mexClient.GetMetadata();

            WsdlImporter importer = new WsdlImporter(serviceMetadata);
            System.Collections.ObjectModel.Collection<ContractDescription> contracts = importer.ImportAllContracts();
            
            BasicHttpBinding Binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress(adresse.ToString());

            ChannelFactory<IRequestChannel> factory =
                new ChannelFactory<IRequestChannel>(Binding, address);

            serviceChannel = factory.CreateChannel();
            serviceInfo = contracts[0];
            targetNamespace = serviceInfo.Namespace;
            serviceName = serviceInfo.Name;
        }

        public List<MethodInformation> GetServiceMethods()
        {
            List<MethodInformation> methods = new List<MethodInformation>();
            foreach (OperationDescription operationDescription in serviceInfo.Operations)
                if (operationDescription.Messages[0].Body.Parts.Count == 0)
                    methods.Add(new MethodInformation(operationDescription.Name, new MethodParameter("", "")));
            //тип выходного параметра задаётся пустым, т.к. почему-то type в returnValue == null, однако в private baseType тип задаётся
            return methods;
        }
        
        public Message CallMethod(string methodName)
        {          
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(string.Format(@"<{0} xmlns=""{1}""></{0}>", methodName, targetNamespace));
            streamWriter.Flush();
            memoryStream.Position = 0;
            XmlDictionaryReader bodyReader = XmlDictionaryReader.CreateTextReader(memoryStream, new XmlDictionaryReaderQuotas());

            string soapAction = string.Format("{0}{1}/{2}", targetNamespace, serviceName, methodName);
            serviceChannel.Open();          
            Message request = Message.CreateMessage(MessageVersion.Soap11, soapAction, bodyReader);
            Message reply = serviceChannel.Request(request);
            serviceChannel.Close();

            return reply;
        }

    }
}
