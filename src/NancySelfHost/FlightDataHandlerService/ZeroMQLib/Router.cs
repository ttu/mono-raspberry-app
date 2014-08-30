using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDataHandlerService
{
    public class Router<TRequest, TResponse>
    {
        private List<string> _bindEndPoints;
        private Func<TRequest, TResponse> _process;

        public Router(string endpoint, Func<TRequest, TResponse> process)
        {
            _bindEndPoints = new List<string> { endpoint };
            _process = process;
        }

        public void Start()
        {
            using (var context = NetMQContext.Create())
            {
                //using (var socket = context.CreateResponseSocket())
                using (var socket = context.CreateRouterSocket())
                {
                    foreach (var bindEndPoint in _bindEndPoints)
                        socket.Bind(bindEndPoint);

                    socket.ReceiveReady += _socket_ReceiveReady;

                    var poller = new Poller();
                    poller.AddSocket(socket);
                    poller.Start();
                }
            }
        }

        private async void _socket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMessage();

            // Response socket has index 1, Dealer/Router 2
            var text = Encoding.Unicode.GetString(msg[2].Buffer);

            var request = SerializationMethods.FromByteArray<TRequest>(msg[2].Buffer);

            var response = await DoWork(request);

            var envelope = new NetMQFrame(Encoding.UTF8.GetBytes(response.ToString()));
            var body = new NetMQFrame(response.ToByteArray());

            msg.Append(envelope);
            msg.Append(body);

            e.Socket.SendMessage(msg);
        }

        private async Task<TResponse> DoWork(TRequest request)
        {
            return await Task.Factory.StartNew(() =>
            {
                return _process(request);
            });
        }
    }
}
