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
    public class FaceRegisterInput
    {
        public string image { get; set; }
        public string unique_name { get; set; }
        public string person_name { get; set; }
        public decimal same_person_thr { get; set; }
        public bool force { get; set; }
        public string info { get; set; }
    }
    public class FaceRegisterOutput
    {
        public MessageRegiter message { get; set; }
        public List<int> face_loc { get; set; }
        public decimal face_angle { get; set; }
        public int result_code { get; set; }
        public string request_id { get; set; }
        public string server_name { get; set; }
        public string version { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }
    }
    public class MessageRegiter
    {
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string info { get; set; }
        public string face_id { get; set; }
        public List<int> face_loc { get; set; }
        public decimal face_angle { get; set; }
        public decimal matched_score { get; set; }
        public decimal same_person_thr { get; set; }
        public string unique_name { get; set; }
        public MessageFound message { get; set; }
    }

    public class MessageFound
    {
        public string error_code { get; set; }
        public string error_message { get; set; }
    }
}
