﻿using System;
using Newtonsoft.Json;

namespace R8.Lib.JsonConverters
{
    public class JsonGuidConverter : JsonConverter<Guid?>
    {
        public override void WriteJson(JsonWriter writer, Guid? value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteValue((string)null);
            else
                writer.WriteValue(value.ToString());
        }

        public override Guid? ReadJson(JsonReader reader, Type objectType, Guid? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            if (string.IsNullOrEmpty(s))
                return null;

            return Guid.Parse(s);
        }
    }
}