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
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.ADO
{
    /// <summary>
    /// Minimal information of a sale export ticket needed to attach/list/delete prescription files.
    /// Built from HIS_EXP_MEST, V_HIS_EXP_MEST or V_HIS_EXP_MEST_2 so every caller screen can use it.
    /// </summary>
    public class ExpMestAttachInfoADO
    {
        public long ID { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string TDL_SERVICE_REQ_CODE { get; set; }
        public long EXP_MEST_STT_ID { get; set; }
        public long? BILL_ID { get; set; }
        public long? DEBT_ID { get; set; }
        /// <summary>v57853 - Ma ho so dieu tri (rong voi phieu ban vang lai) -> DocumentTDO.TreatmentCode</summary>
        public string TDL_TREATMENT_CODE { get; set; }
        /// <summary>v57853 - Ma kho xuat -> khoi "EXP_STOCK:" trong HIS_CODE (EMR tach vao EMR_DOCUMENT.EXP_MEDI_STOCK_CODE)</summary>
        public string MEDI_STOCK_CODE { get; set; }
        /// <summary>v57853 - Ma khoa yeu cau -> khoi "REQ_DEPT:" trong HIS_CODE (EMR_DOCUMENT.REQ_DEPARTMENT_CODE)</summary>
        public string REQ_DEPARTMENT_CODE { get; set; }

        public ExpMestAttachInfoADO() { }

        public ExpMestAttachInfoADO(HIS_EXP_MEST data)
        {
            if (data == null) return;
            this.ID = data.ID;
            this.EXP_MEST_CODE = data.EXP_MEST_CODE;
            this.TDL_SERVICE_REQ_CODE = data.TDL_SERVICE_REQ_CODE;
            this.EXP_MEST_STT_ID = data.EXP_MEST_STT_ID;
            this.BILL_ID = data.BILL_ID;
            this.DEBT_ID = data.DEBT_ID;
            this.TDL_TREATMENT_CODE = data.TDL_TREATMENT_CODE;
        }

        public ExpMestAttachInfoADO(V_HIS_EXP_MEST data)
        {
            if (data == null) return;
            this.ID = data.ID;
            this.EXP_MEST_CODE = data.EXP_MEST_CODE;
            this.TDL_SERVICE_REQ_CODE = data.TDL_SERVICE_REQ_CODE;
            this.EXP_MEST_STT_ID = data.EXP_MEST_STT_ID;
            this.BILL_ID = data.BILL_ID;
            this.DEBT_ID = data.DEBT_ID;
            this.TDL_TREATMENT_CODE = data.TDL_TREATMENT_CODE;
            this.MEDI_STOCK_CODE = data.MEDI_STOCK_CODE;
            this.REQ_DEPARTMENT_CODE = data.REQ_DEPARTMENT_CODE;
        }

        public ExpMestAttachInfoADO(V_HIS_EXP_MEST_2 data)
        {
            if (data == null) return;
            this.ID = data.ID;
            this.EXP_MEST_CODE = data.EXP_MEST_CODE;
            this.TDL_SERVICE_REQ_CODE = data.TDL_SERVICE_REQ_CODE;
            this.EXP_MEST_STT_ID = data.EXP_MEST_STT_ID;
            this.BILL_ID = data.BILL_ID;
            this.DEBT_ID = data.DEBT_ID;
            this.TDL_TREATMENT_CODE = data.TDL_TREATMENT_CODE;
            this.MEDI_STOCK_CODE = data.MEDI_STOCK_CODE;
            this.REQ_DEPARTMENT_CODE = data.REQ_DEPARTMENT_CODE;
        }
    }
}
