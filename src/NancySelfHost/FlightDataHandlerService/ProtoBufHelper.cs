using FlightDataHandler.Models;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlightDataHandlerService
{
    public static class ProtoBufHelper
    {
        public static void BasicPrepare()
        {
            RuntimeTypeModel.Default.AutoAddMissingTypes = true;

            Prepare(
                typeof(Object),
                typeof(DataRequest),
                typeof(PushMessage),
                typeof(FlightDataHandler.Models.FlightInfo));
        }

        // http://stackoverflow.com/questions/11569593/protobuf-net-implicit-contracts
        public static void Prepare(params Type[] types)
        {
            foreach (var type in types)
            {
                if (type != null && !RuntimeTypeModel.Default.IsDefined(type))
                {
                    // Object is not defined by default
                    if (type.Namespace.StartsWith("System") && type != typeof(Object))
                        continue;

                    Debug.WriteLine("Preparing: " + type.FullName);
                    // note this has no defined sort, so invent one
                    var props = type.GetProperties();
                    Array.Sort(props, (x, y) => string.Compare(
                        x.Name, y.Name, StringComparison.Ordinal));
                    var meta = RuntimeTypeModel.Default.Add(type, false);
                    int fieldNum = 1;
                    for (int i = 0; i < props.Length; i++)
                        if (props[i].CanWrite)
                        {
                            meta.Add(fieldNum++, props[i].Name);

                            if (!RuntimeTypeModel.Default.IsDefined(props[i].PropertyType))
                                if (props[i].PropertyType.HasElementType)
                                    Prepare(props[i].PropertyType.GetElementType()); //T[]
                                else if (props[i].PropertyType.IsGenericType)
                                    Prepare(props[i].PropertyType.GetGenericArguments()); //List<T>
                                else
                                    Prepare(props[i].PropertyType);
                        }
                }
            }
        }
    }
}