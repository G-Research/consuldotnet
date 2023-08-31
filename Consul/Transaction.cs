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

using System.Collections.Generic;
using Newtonsoft.Json;

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
        [JsonProperty]
        public int OpIndex { get; private set; }
        [JsonProperty]
        public string What { get; private set; }
    }

    internal class TxnResponse
    {
        [JsonProperty]
        internal List<TxnResult> Results { get; set; }
        [JsonProperty]
        internal List<TxnError> Errors { get; set; }
    }
}
