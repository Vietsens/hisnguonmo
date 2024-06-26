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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Base
{
    class GlobalStore
    {
        internal const string HIS_PRESCRIPTION = "api/HisPrescription/Get";
        internal const string HIS_TREATMENT_OUT_GET = "api/HisTreatmentOut/Get";
        internal const string HIS_APPOINTMENT_GET = "api/HisAppointment/Get";
        internal const string HIS_TRAN_PATI_GET = "api/HisTranPati/Get";
        internal const string HIS_DEATH_GET = "api/HisDeath/Get";
        internal const string HIS_MEDI_ORG_GET = "api/HisMediOrg/Get";
        internal const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";

        private const string END_ORDER = "MOS.HIS_TREATMENT.IS_MANUAL_END_CODE";

        internal const short IS_TRUE = 1;

        internal static void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string captionCode, string name, string captionName, string value, object data)
        {
            try
            {
                comboEdit.Properties.DataSource = data;
                comboEdit.Properties.DisplayMember = name;
                comboEdit.Properties.ValueMember = value;
                comboEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboEdit.Properties.ImmediatePopup = true;
                comboEdit.ForceInitialize();
                comboEdit.Properties.View.Columns.Clear();
                comboEdit.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnCode = comboEdit.Properties.View.Columns.AddField(code);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = comboEdit.Properties.View.Columns.AddField(name);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE> HisTreatmentEndTypes
        {
            get
            {
                return BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.IS_ACTIVE == 1).OrderByDescending(o => o.TREATMENT_END_TYPE_CODE).ToList();
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT> HisTreatmentResults
        {
            get
            {
                return BackendDataWorker.Get<HIS_TREATMENT_RESULT>().Where(o => o.IS_ACTIVE == 1).OrderByDescending(o => o.TREATMENT_RESULT_CODE).ToList();
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> HisTranPatiReasons
        {
            get
            {
                return BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.TRAN_PATI_REASON_CODE).ToList();
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> HisTranPatiForms
        {
            get
            {
                return BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().Where(o => o.IS_ACTIVE == 1).OrderByDescending(o => o.TRAN_PATI_FORM_CODE).ToList();
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH> TranPatiTechs
        {
            get
            {
                return BackendDataWorker.Get<HIS_TRAN_PATI_TECH>().Where(o => o.IS_ACTIVE == 1).OrderByDescending(o => o.TRAN_PATI_TECH_CODE).ToList();
            }
        }

        public static List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT> TreatmentEndTypeExts
        {
            get
            {
                return BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>().Where(o => o.IS_ACTIVE == 1).OrderByDescending(o => o.TREATMENT_END_TYPE_EXT_CODE).ToList();
            }
        }

        public static string END_ORDER_STR
        {
            get
            {
                return HisConfigs.Get<string>(END_ORDER);
            }
        }
    }
}
