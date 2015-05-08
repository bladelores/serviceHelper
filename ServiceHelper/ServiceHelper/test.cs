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

            string test = sh.CallMethod<string>(methodsWithoutInput.FirstOrDefault().Name);
        }
    }
}
