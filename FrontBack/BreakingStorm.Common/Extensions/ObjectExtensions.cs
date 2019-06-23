using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BreakingStorm.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static T MakeClone<T>(this object source)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

        public static object MakeClone(this object source)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source),source.GetType());
        }

        public static string ToJSON(this object source)
        {
            return Helpers.HelperSerialization.JSONSerialization(source);
        }

    }
}
