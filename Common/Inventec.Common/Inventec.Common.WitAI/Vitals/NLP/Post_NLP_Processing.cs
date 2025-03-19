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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data;

namespace Inventec.Common.WitAI.Vitals.NLP
{
    static class Post_NLP_Processing
    {
        public static Objects.O_NLP.RootObject ParseData(string text)
        {
            // HTML-decode the string, in case it has been HTML encoded
            string jsonText = System.Web.HttpUtility.HtmlDecode(text);

            //Since object is reserved, put a _ in front
            jsonText = jsonText.Replace("\"object\" : {", "\"_object\" : {");

            //Deserialize into our class
            var rootObject = JsonConvert.DeserializeObject<Objects.O_NLP.RootObject>(jsonText);

            return rootObject;
        }
    }
}
