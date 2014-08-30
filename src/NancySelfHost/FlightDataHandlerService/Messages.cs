using System;

namespace FlightDataHandlerService
{
    //[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class DataRequest
    {
        public int Method { get; set; }

        public Tuple<double, double, double> Value { get; set; }
    }

    public class PushMessage
    {
        public int Method { get; set; }

        public string ClientId { get; set; }

        public Tuple<double, double, double> Values { get; set; }
    }
}