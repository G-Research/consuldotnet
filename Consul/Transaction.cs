// -----------------------------------------------------------------------
//  <copyright file="Transaction.cs" company="PlayFab Inc">
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Consul
{
    internal class TxnOp
    {
        public KVTxnOp KV { get; set; }
    }

    internal class TxnResult
    {
        public KVPair KV { get; set; }
    }

    public class TxnError
    {
        [JsonInclude]
        public int OpIndex { get; private set; }
        [JsonInclude]
        public string What { get; private set; }
    }

    public class TxnResponseConverter : JsonConverter<KVTxnResponse>
    {
        public override KVTxnResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new KVTxnResponse() { Results = new List<KVPair>() };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName
                    && reader.GetString() == "Results"
                    && reader.Read()
                    && reader.TokenType == JsonTokenType.StartArray)
                {
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.EndObject)
                        {
                            continue;
                        }

                        if (reader.TokenType == JsonTokenType.PropertyName && reader.Read())
                        {
                            var resultPair = JsonSerializer.Deserialize<KVPair>(ref reader, options);
                            result.Results.Add(resultPair);
                        }
                    }
                }

                if (reader.TokenType == JsonTokenType.PropertyName
                    && reader.GetString() == "Errors"
                    && reader.Read()
                    && reader.TokenType == JsonTokenType.StartArray)
                {
                    result.Errors = JsonSerializer.Deserialize<List<TxnError>>(ref reader, options);
                }
            }

            if (result.Errors == null)
            {
                result.Errors = new List<TxnError>(0);
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, KVTxnResponse value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
