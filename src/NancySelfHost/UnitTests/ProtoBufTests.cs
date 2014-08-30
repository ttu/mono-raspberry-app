using FlightDataHandlerService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using System;
using System.Diagnostics;

namespace DebugginHelper
{
    [TestClass]
    public class ProtoBufTests
    {
        [TestMethod]
        public void ToAndFromByteArray()
        {
            var dh = new DataRequest { Method = 2 };

            ProtoBufHelper.Prepare(typeof(Object), typeof(DataRequest));

            var bytes = dh.ToByteArray();

            Assert.IsTrue(bytes.Length > 0, "Length is zero");

            var deserialized = SerializationMethods.FromByteArray<DataRequest>(bytes);

            Assert.AreEqual(dh.Method, deserialized.Method, "Not serialized correctly");
        }

        [TestMethod]
        public void DeepCopy()
        {
            ProtoBufHelper.Prepare(typeof(Object), typeof(DataRequest));

            var dh = new DataRequest { Method = 2 };

            var dh2 = Serializer.DeepClone(dh);

            dh.Method = 1;

            Assert.AreNotEqual(dh.Method, dh2.Method);
        }
    }
}