using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDataHandlerService
{
    public class Dealer<TRequest, TResponse>
    {
        private NetMQContext _context;

        private string _bindEndPoint;

        public Dealer(string endpoint)
        {
            _bindEndPoint = endpoint;
            _context = NetMQContext.Create();
        }

        public TResponse SendRequest(TRequest request)
        {
            using (var socket = _context.CreateDealerSocket())
            {
                var clientId = Guid.NewGuid();
                socket.Options.Identity = Encoding.Unicode.GetBytes(clientId.ToString());

                socket.Connect(_bindEndPoint);

                var envelope = new NetMQFrame(Encoding.UTF8.GetBytes(request.ToString()));
                var body = new NetMQFrame(request.ToByteArray());

                var msq = new NetMQMessage();
                msq.Append(envelope);
                msq.Append(body);

                socket.SendMessage(msq);

                var responseMsg = socket.ReceiveMessage();

                return SerializationMethods.FromByteArray<TResponse>(responseMsg[3].Buffer);
            }
        }
    }
}
