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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SummaryInforTreatmentRecordsUpdate
{
    public partial class frmSummaryInforTreatmentRecordsUpdate : Form
    {
        private HIS_MEDI_RECORD SetDataMedirecord()
        {
            HIS_MEDI_RECORD mediRecord = new HIS_MEDI_RECORD();
            try
            {
                //mediRecord.MEDI_RECORD_CODE = ;
            }
            catch (Exception ex)
            {
                mediRecord = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return mediRecord;
        }

        private HIS_TREATMENT_LOG SetDataTreatmentLog()
        {
            HIS_TREATMENT_LOG TreatmentLog = new HIS_TREATMENT_LOG();
            try
            {
                TreatmentLog.TREATMENT_LOG_TYPE_ID = HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__MEDI_RECORD;
                TreatmentLog.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime)??0;
            }
            catch (Exception ex)
            {
                TreatmentLog = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return TreatmentLog;
        }
    }
}
