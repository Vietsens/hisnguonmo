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
using System.Windows.Forms;

namespace Inventec.VoiceCommand
{
    public class ResultCommandADO
    {
        public ResultCommandADO() { }

        public string commandActionLink { get; set; }
        public int commandType { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string messageError { get; set; }
        public bool success { get; set; }
        public string text { get; set; }
        public string _text { get; set; }
        public object entities { get; set; }
        public object traits { get; set; }
        public object[] intents { get; set; }
        public object control { get; set; }
        public object id { get; set; }
    }
}
