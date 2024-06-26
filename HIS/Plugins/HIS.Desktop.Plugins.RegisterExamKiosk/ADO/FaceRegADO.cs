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

namespace HIS.Desktop.Plugins.RegisterExamKiosk.ADO
{
    public class FaceRegInputADO
    {
        public string image { get; set; }
        public int topk { get; set; }
        public string request_id { get; set; }
    }
    public class FaceRegResultADO
    {
        public Message message { get; set; }
        public decimal recognition_time { get; set; }
        public List<RegRessultArr> recognition_result { get; set; }
        public int result_code { get; set; }
        public string request_id { get; set; }
        public string server_name { get; set; }
        public string version { get; set; }
    }
    public class Message
    {
        public string api_version { get; set; }
        public string error_message { get; set; }
        public string error_code { get; set; }
    }
    public class RegRessultArr
    {
        public List<int> face_loc { get; set; }
        public long face_size { get; set; }
        public List<TopK> top_k { get; set; }
    }
    public class TopK
    {
        public decimal compare_score { get; set; }
        public string face_id { get; set; }
        public Info info { get; set; }
        public string person_name { get; set; }
        public string unique_name { get; set; }
    }
    public class Info
    {
        public string unique_name { get; set; }
    }
}
