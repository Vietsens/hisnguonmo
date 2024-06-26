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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InputSave
{
    public partial class frmInputSave : Form
    {
        HIS.Desktop.Common.DelegateSelectData selectData;

        public frmInputSave(HIS.Desktop.Common.DelegateSelectData data)
        {
            InitializeComponent();
            try
            {
                this.selectData = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmInputSave_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadCombo()
        {
            try
            {
                InitCombo(cboHinhThucChuyen, BackendDataWorker.Get<HIS_TRAN_PATI_FORM>(), "TRAN_PATI_FORM_NAME", "ID", "TRAN_PATI_FORM_CODE", "TRAN_PATI_FORM_NAME");
                InitCombo(cboIcd, BackendDataWorker.Get<HIS_ICD>(), "ICD_NAME", "ICD_CODE", "ICD_CODE", "ICD_NAME");
                InitCombo(cboNoiChuyenDen, BackendDataWorker.Get<HIS_MEDI_ORG>(), "MEDI_ORG_NAME", "MEDI_ORG_CODE", "MEDI_ORG_CODE", "MEDI_ORG_NAME");
                InitCombo(cboLyDoChuyen, BackendDataWorker.Get<HIS_TRAN_PATI_REASON>(), "TRAN_PATI_REASON_NAME", "TRAN_PATI_REASON_CODE", "TRAN_PATI_REASON_CODE", "TRAN_PATI_REASON_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCombo(GridLookUpEdit cbo, object data, string displayMember, string valueMember, string column1, string column2)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(column1, "", 100, 1));
                columnInfos.Add(new ColumnInfo(column2, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.selectData != null)
                {
                    InputSaveADO ado = new InputSaveADO();
                    if (cboIcd.EditValue != null)
                    {
                        ado.IcdCode = cboIcd.EditValue.ToString();
                        ado.IcdName = cboIcd.Text;
                    }
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        ado.MediOrgCode = cboNoiChuyenDen.EditValue.ToString();
                        ado.MediOrgName = cboNoiChuyenDen.Text;
                    }
                    if (cboLyDoChuyen.EditValue != null)
                    {
                        ado.ReasonId = Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyen.EditValue.ToString());
                    }
                    if (cboHinhThucChuyen.EditValue != null)
                    {
                        ado.FormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboHinhThucChuyen.EditValue.ToString());
                    }

                    this.selectData(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtNoiChuyenDen.Text))
                    {
                        cboNoiChuyenDen.EditValue = null;
                        cboNoiChuyenDen.Focus();
                        cboNoiChuyenDen.ShowPopup();
                    }
                    else
                    {
                        List<HIS_MEDI_ORG> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.MEDI_ORG_CODE.ToUpper().Contains(txtNoiChuyenDen.Text.ToUpper()) && o.IS_ACTIVE == 1).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.MEDI_ORG_CODE.ToUpper() == txtNoiChuyenDen.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtNoiChuyenDen.Text = searchs[0].MEDI_ORG_CODE;
                            cboNoiChuyenDen.EditValue = searchs[0].MEDI_ORG_CODE;
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                        else
                        {
                            cboNoiChuyenDen.Focus();
                            cboNoiChuyenDen.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == (cboNoiChuyenDen.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                    }
                    else
                    {
                        cboNoiChuyenDen.Focus();
                        cboNoiChuyenDen.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtIcdCode.Text))
                    {
                        cboIcd.EditValue = null;
                        cboIcd.Focus();
                        cboIcd.ShowPopup();
                    }
                    else
                    {
                        List<HIS_ICD> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_ICD>().Where(o => o.ICD_CODE.ToUpper().Contains(txtIcdCode.Text.ToUpper()) && o.IS_ACTIVE == 1).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ICD_CODE.ToUpper() == txtIcdCode.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtIcdCode.Text = searchs[0].ICD_CODE;
                            cboIcd.EditValue = searchs[0].ICD_CODE;
                            txtHinhThucChuyen.Focus();
                            txtHinhThucChuyen.SelectAll();
                        }
                        else
                        {
                            cboIcd.Focus();
                            cboIcd.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboIcd.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_ICD>().SingleOrDefault(o => o.ICD_CODE == (cboIcd.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            txtHinhThucChuyen.Focus();
                            txtHinhThucChuyen.SelectAll();
                        }
                    }
                    else
                    {
                        cboIcd.Focus();
                        cboIcd.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHinhThucChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtHinhThucChuyen.Text))
                    {
                        cboHinhThucChuyen.EditValue = null;
                        cboHinhThucChuyen.Focus();
                        cboHinhThucChuyen.ShowPopup();
                    }
                    else
                    {
                        List<HIS_TRAN_PATI_FORM> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().Where(o => o.TRAN_PATI_FORM_CODE.ToUpper().Contains(txtHinhThucChuyen.Text.ToUpper()) && o.IS_ACTIVE == 1).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.TRAN_PATI_FORM_CODE.ToUpper() == txtHinhThucChuyen.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtHinhThucChuyen.Text = searchs[0].TRAN_PATI_FORM_CODE;
                            cboHinhThucChuyen.EditValue = searchs[0].ID;
                            txtLyDoChuyen.Focus();
                            txtLyDoChuyen.SelectAll();
                        }
                        else
                        {
                            cboHinhThucChuyen.Focus();
                            cboHinhThucChuyen.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHinhThucChuyen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboHinhThucChuyen.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                            txtLyDoChuyen.Focus();
                            txtLyDoChuyen.SelectAll();
                        }
                    }
                    else
                    {
                        cboHinhThucChuyen.Focus();
                        cboHinhThucChuyen.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLyDoChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtLyDoChuyen.Text))
                    {
                        cboLyDoChuyen.EditValue = null;
                        cboLyDoChuyen.Focus();
                        cboLyDoChuyen.ShowPopup();
                    }
                    else
                    {
                        List<HIS_TRAN_PATI_REASON> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().Where(o => o.TRAN_PATI_REASON_CODE.ToUpper().Contains(txtLyDoChuyen.Text.ToUpper()) && o.IS_ACTIVE == 1).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.TRAN_PATI_REASON_CODE.ToUpper() == txtLyDoChuyen.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtLyDoChuyen.Text = searchs[0].TRAN_PATI_REASON_CODE;
                            cboLyDoChuyen.EditValue = searchs[0].ID;
                            btnSave.Focus();
                        }
                        else
                        {
                            cboLyDoChuyen.Focus();
                            cboLyDoChuyen.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLyDoChuyen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLyDoChuyen.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtLyDoChuyen.Text = data.TRAN_PATI_REASON_CODE;
                            btnSave.Focus();
                        }
                    }
                    else
                    {
                        cboLyDoChuyen.Focus();
                        cboLyDoChuyen.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoiChuyenDen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNoiChuyenDen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_MEDI_ORG>().SingleOrDefault(o => o.MEDI_ORG_CODE == (cboNoiChuyenDen.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtNoiChuyenDen.Text = data.MEDI_ORG_CODE;
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcd_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboIcd.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_ICD>().SingleOrDefault(o => o.ICD_CODE == (cboIcd.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            txtIcdCode.Text = data.ICD_CODE;
                            txtHinhThucChuyen.Focus();
                            txtHinhThucChuyen.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLyDoChuyen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLyDoChuyen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboLyDoChuyen.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtLyDoChuyen.Text = data.TRAN_PATI_REASON_CODE;
                            btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHinhThucChuyen_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHinhThucChuyen.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboHinhThucChuyen.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtHinhThucChuyen.Text = data.TRAN_PATI_FORM_CODE;
                            txtLyDoChuyen.Focus();
                            txtLyDoChuyen.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
