using FlightDataHandler;
using FlightDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightDataHandlerService
{
    public class DataHandlerWrapper
    {
        private IDataHandler _dataHandler = new DataHandler();

        private Router<DataRequest, List<FlightInfo>> _rep;
        private Push<Tuple<string, List<FlightInfo>>> _push;
        private Pull<PushMessage> _pull;

        public DataHandlerWrapper()
        {
            ProtoBufHelper.Prepare(
               typeof(Object),
               typeof(DataRequest),
               typeof(PushMessage),
               typeof(FlightDataHandler.Models.FlightInfo));
 
            _rep = new Router<DataRequest, List<FlightInfo>>("tcp://127.0.0.1:5656", ExecuteRequest);
            _push = new Push<Tuple<string, List<FlightInfo>>>("tcp://127.0.0.1:5657");
            _pull = new Pull<PushMessage>("tcp://127.0.0.1:5658", PullAction);

            Task.Factory.StartNew(_rep.Start);
            Task.Factory.StartNew(_pull.Start);

            _dataHandler.UpdatedClientData += _dataHandler_UpdatedClientData;
        }

        private List<FlightInfo> ExecuteRequest(DataRequest request)
        {
            // TODO: Validate request object

            Task<List<FlightInfo>> task;

            if (request.Method == 0)
            {
                task = _dataHandler.RequestFlights();
            }
            else
            {
                var parameters = request.Value as Tuple<double, double, double>;
                task = _dataHandler.RequestFlights(parameters.Item1, parameters.Item2, parameters.Item3);
            }

            task.Wait();

            return task.Result;
        }

        private void PullAction(PushMessage message)
        {
            if (message.Method == 0)
            {
                _dataHandler.Subscribe((string)message.ClientId);
            }
            else if (message.Method == 1)
            {
                _dataHandler.UnSubscribe((string)message.ClientId);
            }
            else
            {
                var parameters = message.Values as Tuple<double, double, double>;
                _dataHandler.SetClientLocation(message.ClientId, parameters.Item1, parameters.Item2, parameters.Item3);
            }
        }

        private void _dataHandler_UpdatedClientData(object sender, Tuple<string, List<FlightInfo>> e)
        {
            _push.Send(e);
        }
    }
}