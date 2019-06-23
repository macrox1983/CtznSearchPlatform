using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Helpers
{
    public static class HelperCollections
    {
        public static List<TEnum> FlagEnumToList<TEnum>(TEnum enum_val)
        {            
            return
                enum_val
                .ToString()
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) 
                .Select(strenum => (TEnum)Enum.Parse(typeof(TEnum), strenum))
                .ToList();
        }

    }
}
