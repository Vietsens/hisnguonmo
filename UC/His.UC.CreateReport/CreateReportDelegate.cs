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

namespace His.UC.CreateReport
{
    public class CreateReportDelegate
    {
        private static ProcessCreateReport processCreateReport;
        public static ProcessCreateReport ProcessCreateReport
        {
            get
            {
                return processCreateReport;
            }
            set
            {
                processCreateReport = value;
            }
        }
        private static ProcessCreateReportViewAway processCreateReportViewAway;
        public static ProcessCreateReportViewAway ProcessCreateReportViewAway
        {
            get
            {
                return processCreateReportViewAway;
            }
            set
            {
                processCreateReportViewAway = value;
            }
        }

        private static StatusReport delegateStatusReport;
        public static StatusReport DelegateStatusReport
        {
            get
            {
                return delegateStatusReport;
            }
            set
            {
                delegateStatusReport = value;
            }
        }
    }
}
