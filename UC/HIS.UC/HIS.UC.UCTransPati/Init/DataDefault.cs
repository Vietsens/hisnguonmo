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
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Columns;
using HIS.UC.UCTransPati.ADO;

namespace HIS.UC.UCTransPati.Init
{
    public class DataDefault
    {
        public static void LoadDataToComboLyDoChuyen(DevExpress.XtraEditors.LookUpEdit cboLyDoChuyen, List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON> data)
        {
            try
            {
                cboLyDoChuyen.Properties.DataSource = data;
                cboLyDoChuyen.Properties.DisplayMember = "TRAN_PATI_REASON_NAME";
                cboLyDoChuyen.Properties.ValueMember = "ID";
                cboLyDoChuyen.Properties.ForceInitialize();
                cboLyDoChuyen.Properties.Columns.Clear();
                cboLyDoChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_REASON_CODE", "", 50));
                cboLyDoChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_REASON_NAME", "", 400));
                cboLyDoChuyen.Properties.ShowHeader = false;
                cboLyDoChuyen.Properties.ImmediatePopup = true;
                cboLyDoChuyen.Properties.DropDownRows = 20;
                cboLyDoChuyen.Properties.PopupWidth = 450;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboHinhThucChuyen, List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM> data)
        {
            try
            {
                cboHinhThucChuyen.Properties.DataSource = data;
                cboHinhThucChuyen.Properties.DisplayMember = "TRAN_PATI_FORM_NAME";
                cboHinhThucChuyen.Properties.ValueMember = "ID";
                cboHinhThucChuyen.Properties.ForceInitialize();
                cboHinhThucChuyen.Properties.Columns.Clear();
                cboHinhThucChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_FORM_CODE", "", 50));
                cboHinhThucChuyen.Properties.Columns.Add(new LookUpColumnInfo("TRAN_PATI_FORM_NAME", "", 400));
                cboHinhThucChuyen.Properties.ShowHeader = false;
                cboHinhThucChuyen.Properties.ImmediatePopup = true;
                cboHinhThucChuyen.Properties.DropDownRows = 20;
                cboHinhThucChuyen.Properties.PopupWidth = 450;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboChanDoanTD, List<MOS.EFMODEL.DataModels.HIS_ICD> data)
        {
            try
            {
                cboChanDoanTD.Properties.DataSource = data;
                cboChanDoanTD.Properties.DisplayMember = "ICD_NAME";
                cboChanDoanTD.Properties.ValueMember = "ID";

                cboChanDoanTD.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboChanDoanTD.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboChanDoanTD.Properties.ImmediatePopup = true;
                cboChanDoanTD.ForceInitialize();
                cboChanDoanTD.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboChanDoanTD.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboChanDoanTD.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 400;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboNoiDKKCBBD(DevExpress.XtraEditors.GridLookUpEdit cboMediOrg, List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG> data)
        {
            try
            {
                cboMediOrg.Properties.DataSource = data;
                cboMediOrg.Properties.DisplayMember = "MEDI_ORG_NAME";
                cboMediOrg.Properties.ValueMember = "MEDI_ORG_CODE";

                cboMediOrg.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboMediOrg.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboMediOrg.Properties.ImmediatePopup = true;
                cboMediOrg.ForceInitialize();
                cboMediOrg.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboMediOrg.Properties.View.Columns.AddField("MEDI_ORG_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;

                GridColumn aColumnName = cboMediOrg.Properties.View.Columns.AddField("MEDI_ORG_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboChuyenTuyen(DevExpress.XtraEditors.GridLookUpEdit cboChuyenTuyen)
        {
            try
            {
                List<ChuyenTuyenADO> data = new List<ChuyenTuyenADO>();
                ChuyenTuyenADO dungTuyen = new ChuyenTuyenADO();
                dungTuyen.CHUYENTUYEN_ID = 1;
                dungTuyen.CHUYENTUYEN_NAME = "Chuyển đúng tuyến";
                dungTuyen.CHUYENTUYEN_MOTA = "Chuyển đúng tuyến CMKT gồm các trường hợp chuyển người bệnh theo đúng quy định tại các khoản 1, 2, 3, 4  Điều 5 Thông tư";
                data.Add(dungTuyen);
                ChuyenTuyenADO vuotTuyen = new ChuyenTuyenADO();
                vuotTuyen.CHUYENTUYEN_ID = 2;
                vuotTuyen.CHUYENTUYEN_NAME = "Chuyển vượt tuyến";
                vuotTuyen.CHUYENTUYEN_MOTA = "Chuyển vượt tuyến CMKT gồm các trường hợp chuyển người bệnh không theo đúng quy định tại các khoản 1, 2, 3, 4  Điều 5 Thông tư";
                data.Add(vuotTuyen);

                cboChuyenTuyen.Properties.DataSource = data;
                cboChuyenTuyen.Properties.DisplayMember = "CHUYENTUYEN_NAME";
                cboChuyenTuyen.Properties.ValueMember = "CHUYENTUYEN_ID";

                cboChuyenTuyen.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboChuyenTuyen.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboChuyenTuyen.Properties.ImmediatePopup = true;
                cboChuyenTuyen.ForceInitialize();
                cboChuyenTuyen.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboChuyenTuyen.Properties.View.Columns.AddField("CHUYENTUYEN_NAME");
                aColumnCode.Caption = "Hình thức chuyển";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboChuyenTuyen.Properties.View.Columns.AddField("CHUYENTUYEN_MOTA");
                aColumnName.Caption = "Trường hợp áp dụng";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 270;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
