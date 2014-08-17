using System.Net;
using System.Threading.Tasks;

namespace FlightDataHandler
{
    public class DataDownloader
    {
        private const string _url = "http://www.flightradar24.com/zones/full_all.json";
        private readonly WebClient _client;

        public DataDownloader()
        {
            _client = new WebClient();
        }

        public async Task<string> GetData()
        {
            var data = await _client.DownloadDataTaskAsync(_url);

            return System.Text.Encoding.Default.GetString(data);
        }

        public bool IsBusy { get { return _client.IsBusy; } }
    }
}