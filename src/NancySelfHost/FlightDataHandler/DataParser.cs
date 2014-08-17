using FlightDataHandler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightDataHandler
{
    public static class DataParser
    {
        public static List<FlightInfo> Parse(string json)
        {
            var infos = new List<FlightInfo>();

            dynamic j = JsonConvert.DeserializeObject(json);

            foreach (var item in ((IEnumerable<object>)j).ToList())
            {
                var iString = item.ToString();
                if (iString.Contains("["))
                {
                    var parts = iString.Split('[');
                    var id = parts[0].Replace('"', ' ').Replace(':', ' ').Trim();
                    var datas = parts[1].Replace(']', ' ').Replace('"', ' ').Trim().Split(',');

                    for (int i = 0; i < datas.Length; i++)
                    {
                        datas[i] = datas[i].Trim();
                    }

                    var info = new FlightInfo();
                    info.Parse(datas);

                    infos.Add(info);
                }
                else
                { 
                    // TODO: Parse Infos
                }
            }

            return infos;
        }
    }
}