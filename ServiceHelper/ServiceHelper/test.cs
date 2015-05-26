using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ServiceHelper
{
    [TestFixture]
    class test
    {
        [Test]
        public void TestMethod()
        {
            ReflectionServiceHelper refSH = new ReflectionServiceHelper(new Uri("http://localhost:1473/Service1.svc"));
            var methodsWithoutInput = refSH.GetServiceMethods();
            string test = refSH.CallMethod<string>(methodsWithoutInput.FirstOrDefault().Name);
            //304 millisceonds

            FactoryServiceHelper facSH = new FactoryServiceHelper(new Uri("http://localhost:1473/Service1.svc"));
            var methodsWithoutInput2 = facSH.GetServiceMethods();
            string test2 = facSH.CallMethod<string>(methodsWithoutInput.FirstOrDefault().Name);
            //667 milliseconds
        }
    }
}
