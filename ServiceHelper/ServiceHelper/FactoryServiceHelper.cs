using System;
//!
using System.CodeDom.Compiler;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Linq;

namespace ServiceHelper
{
    public class FactoryServiceHelper : ServiceHelper
    {
        private MetadataSet serviceMetadata;

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

            ServiceContractGenerator generator = new ServiceContractGenerator();
            foreach (ContractDescription contract in contracts)
            {
                generator.GenerateServiceContractType(contract);
            }

            serviceAssembly = CompileAssembly(generator);
            serviceDeclarations = serviceAssembly.GetTypes().First();
            
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpointAdresse = new EndpointAddress(adresse.ToString());

            dynamic factory = Activator.CreateInstance(typeof
                (ChannelFactory<>).MakeGenericType(serviceDeclarations), binding, endpointAdresse);
            serviceInstance = factory.CreateChannel();
        }

        private Assembly CompileAssembly(ServiceContractGenerator generator)
        {
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");

            CompilerParameters compilerParameters = new CompilerParameters(
                new string[] { "System.dll", "System.ServiceModel.dll", "System.Runtime.Serialization.dll" });
            compilerParameters.GenerateInMemory = true;

            CompilerResults results = codeDomProvider.CompileAssemblyFromDom(compilerParameters, generator.TargetCompileUnit);

            return results.CompiledAssembly;
        }
    }
}
