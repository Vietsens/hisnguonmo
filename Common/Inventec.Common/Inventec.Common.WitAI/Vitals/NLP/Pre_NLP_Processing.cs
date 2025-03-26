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
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.WitAI.Vitals.NLP
{
    static class Pre_NLP_Processing
    {
        public static string preprocessText(string text)
        {
            // Max length is 255
            if (text.Length > 255)
            {
                text = text.Substring(0, 255);
            }

            // Convert spaces etc
            string modtext = HttpUtility.UrlPathEncode(text);

            return modtext;
        }
    }
}
