/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WitAI.Objects
{
    // Fill this class with the corresponding objects, the names should correspond with the JSON names
    // The structure should also correspond with the JSON structure or they won't be cast

    class O_NLP
    {
        // Default classes
        public class RootObject
        {
            public string _text { get; set; }
            public string msg_id { get; set; }
            public string msg_body { get; set; }            
            public _Outcome outcome { get; set; }
            public _Outcome entities { get; set; }
        }

        public class _Outcome
        {
            public string intent { get; set; }
            public _Entities entities { get; set; }            
            public double confidence { get; set; }
        }

        // You should add your custom entities here
        // Can be single or array, will always be cast to an array
        public class _Entities
        {
            [JsonConverter(typeof(JsonToArrayConverter<_DateTime>))]
            public _DateTime[] datetime { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Object>))]
            public _Object[] _object { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_WolframAlpha>))]
            public _WolframAlpha[] wolfram_search_query { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Math_Expression>))]
            public _Math_Expression[] math_expression { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Agenda_Entry>))]
            public _Agenda_Entry[] agenda_entry { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_On_Off>))]
            public _On_Off[] on_off { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Question>))]
            public _Question[] question { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Message_Body>))]
            public _Message_Body[] message_body { get; set; }

            [JsonConverter(typeof(JsonToArrayConverter<_Contact>))]
            public _Contact[] contact { get; set; }
        }

        public class _DateTime
        {
            public int end { get; set; }
            public int start { get; set; }
            public _Value value { get; set; }
            public string body { get; set; }            
        }

        public class _Value
        {
            public DateTime from { get; set; }
            public DateTime to { get; set; }
        }

        public class _Object
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
        }

        // Custom entities
        public class _Question
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
        }

        public class _WolframAlpha
        {
            public string body { get; set; }
        }

        public class _Math_Expression
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
            public bool suggested { get; set; }
        }

        public class _Agenda_Entry
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
            public bool suggested { get; set; }
        }

        public class _On_Off
        {
            public string value { get; set; }
        }

        public class _Message_Body
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
            public bool suggested { get; set; }
        }

        public class _Contact
        {
            public int end { get; set; }
            public int start { get; set; }
            public string value { get; set; }
            public string body { get; set; }
            public bool suggested { get; set; }
        }

        // Converts single values to arrays
        public class JsonToArrayConverter<T> : CustomCreationConverter<T[]>
        {
            public override T[] Create(Type objectType)
            {
                return new T[0];
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    object result = serializer.Deserialize(reader, objectType);
                    return result;
                }
                else
                {
                    var resultObject = serializer.Deserialize<T>(reader);
                    return new T[] { resultObject };
                }
            }
        }
    }
}
