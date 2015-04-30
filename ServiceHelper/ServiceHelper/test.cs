using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using System.IO;

namespace ServiceHelper
{
    [TestFixture]
    class test
    {
        [Test]
        public void TestMethod()
        {
            ServiceHelper sh = new ServiceHelper(new Uri("http://localhost:1473/Service1.svc"));

            var methodsWithoutInput = sh.GetServiceMethods();

            StreamReader responseStream = sh.CallMethod(methodsWithoutInput.FirstOrDefault().Name);

            /*
            Dictionary<string, object> parameters = new Dictionary<string,object>();
            parameters.Add("value","HOLY STIH");

            StreamReader responseStream = sh.CallMethod("GetData", parameters);
            */
        }
    }
}
