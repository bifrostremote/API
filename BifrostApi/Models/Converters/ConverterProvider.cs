using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.Converters
{
    public static class ConverterProvider
    {
        public static ValueConverter<bool, BitArray> GetBoolToBitArrayConverter()
        {
            return new ValueConverter<bool, BitArray>(
                value => new BitArray(new[] { value }),
                value => value.Get(0));
        }
    }
}
