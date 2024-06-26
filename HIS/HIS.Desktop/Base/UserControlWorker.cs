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

//using Inventec.Desktop.Common.Modules.ConfigApplication;
using System;
//using Inventec.Desktop.Common.Modules.EventLog;

namespace HIS.Desktop.Base
{
    static class UserControlWorker
    {
        private static object syncRoot = new Object();
        
        //static UCConfigApplication ucConfigApplication;
        //internal static UCConfigApplication UCConfigApplication
        //{
        //    get
        //    {
        //        if (ucConfigApplication == null)
        //        {
        //            lock (syncRoot)
        //                ucConfigApplication = new UCConfigApplication();
        //        }
        //        return ucConfigApplication;
        //    }
        //    set
        //    {
        //        lock (syncRoot)
        //            ucConfigApplication = value;
        //    }
        //}

        //static UCReportListControl ucReportListControl;
        //internal static UCReportListControl UCReportListControl
        //{
        //    get
        //    {
        //        if (ucReportListControl == null)
        //        {
        //            lock (syncRoot)
        //                ucReportListControl = new UCReportListControl();
        //        }
        //        return ucReportListControl;
        //    }
        //    set
        //    {
        //        lock (syncRoot)
        //            ucReportListControl = value;
        //    }
        //}

        //static UCReportTypeListControl ucReportTypeListControl;
        //internal static UCReportTypeListControl UCReportTypeListControl
        //{
        //    get
        //    {
        //        if (ucReportTypeListControl == null)
        //        {
        //            lock (syncRoot)
        //                ucReportTypeListControl = new UCReportTypeListControl();
        //        }
        //        return ucReportTypeListControl;
        //    }
        //    set
        //    {
        //        lock (syncRoot)
        //            ucReportTypeListControl = value;
        //    }
        //}

        //static UCEventLog ucEventLog;
        //internal static UCEventLog UCEventLog
        //{
        //    get
        //    {
        //        if (ucEventLog == null)
        //        {
        //            lock (syncRoot)
        //                ucEventLog = new UCEventLog();
        //        }
        //        return ucEventLog;
        //    }
        //    set
        //    {
        //        lock (syncRoot)
        //            ucEventLog = value;
        //    }
        //}
    }
}
