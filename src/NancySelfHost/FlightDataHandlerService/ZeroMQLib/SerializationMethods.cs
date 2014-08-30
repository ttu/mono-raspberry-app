using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;

namespace FlightDataHandlerService
{
    public static class SerializationMethods
    {
        private static Lazy<TypeModel> _model = new Lazy<TypeModel>(() =>
        {
            var model = TypeModel.Create();
            model.Add(typeof(Object), true);
            model.Add(typeof(DataRequest), true).Add("Method", "Value");
            model.Add(typeof(PushMessage), true);
            model.Add(typeof(FlightDataHandler.Models.FlightInfo), true);

            return model;
        });

        public static byte[] ToByteArray<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                //_model.Value.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
                //return (T)_model.Value.Deserialize(stream, null, typeof(T));
            }
        }
    }
}