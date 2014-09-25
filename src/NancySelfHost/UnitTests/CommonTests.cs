using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebuggingHelper
{
    [TestClass]
    public class CommonTests
    {
        [TestMethod]
        public void MonoCheck()
        {
            var runningOnMono = NancySelfHost.MonoCheck.IsRunningOnMono();
            Assert.IsFalse(runningOnMono);
        }
    }
}
