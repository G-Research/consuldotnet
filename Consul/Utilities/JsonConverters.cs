// -----------------------------------------------------------------------
//  <copyright file="JsonConverters.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consul
{
    public class NanoSecTimespanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetUInt64(out var result))
                {
                    return TimeSpan.FromTicks((long)(result / 100));
                }
            }
            return Extensions.FromGoDuration(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue((long)value.Value.TotalMilliseconds * 1000000);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    public class DurationTimespanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetUInt64(out var result))
                {
                    return TimeSpan.FromTicks((long)(result / 100));
                }
            }
            return Extensions.FromGoDuration(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToGoDuration());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    public class KVPairConverter : JsonConverter<KVPair>
    {
        static readonly Lazy<string[]> ObjProps = new Lazy<string[]>(() => typeof(KVPair).GetRuntimeProperties().Select(p => p.Name).ToArray());


        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(KVPair);
        }

        public override KVPair Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            KVPair result = new KVPair();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject) { continue; }
                if (reader.TokenType == JsonTokenType.EndObject) { return result; }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string jsonPropName = reader.GetString();
                    if (jsonPropName == null)
                    {
                        continue;
                    }

                    var propName = ObjProps.Value.FirstOrDefault(p => p.Equals(jsonPropName, StringComparison.OrdinalIgnoreCase));
                    if (propName != null)
                    {
                        PropertyInfo pi = result.GetType().GetRuntimeProperty(propName);

                        if (jsonPropName.Equals("Flags", StringComparison.OrdinalIgnoreCase) && reader.Read())
                        {
                            pi.SetValue(result, reader.GetUInt64(), null);
                        }
                        else if (jsonPropName.Equals("Value", StringComparison.OrdinalIgnoreCase) && reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.Null)
                            {
                                continue;
                            }
                            pi.SetValue(result, reader.GetBytesFromBase64(), null);
                        }
                        else if (reader.Read())
                        {
                            pi.SetValue(result, JsonSerializer.Deserialize(ref reader, pi.PropertyType, options), null);
                        }
                    }
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, KVPair value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Key", value.Key);
            writer.WriteNumber("CreateIndex", value.CreateIndex);
            writer.WriteNumber("ModifyIndex", value.ModifyIndex);
            writer.WriteNumber("LockIndex", value.LockIndex);
            writer.WriteNumber("Flags", value.Flags);
            writer.WriteBase64String("Session", value.Value);
            writer.WriteString("Session", value.Session);
            writer.WriteEndObject();
        }
    }
}
