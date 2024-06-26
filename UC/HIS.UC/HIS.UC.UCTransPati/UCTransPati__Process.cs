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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.UCTransPati.Config;
using HIS.UC.UCTransPati.Init;

namespace HIS.UC.UCTransPati
{
    public partial class UCTransPati : UserControl
    {
        /// <summary>
        /// Khởi tạo, đổ dữ liệu khởi tạo vào control chứa dữu liệu (combo,...)
        /// </summary>
        private void InitDataToControl()
        {
            try
            {
                DataDefault.LoadDataToComboNoiDKKCBBD(this.cboNoiChuyenDen, DataStore.MediOrgs);
                this.InitUcIcd();
                this.InitUcSubIcd();
                DataDefault.LoadDataToComboChuyenTuyen(this.cboChuyenTuyen);
                DataDefault.LoadDataToCombo(this.cboHinhThucChuyen, DataStore.TranPatiForms);
                DataDefault.LoadDataToComboLyDoChuyen(this.cboLyDoChuyen, DataStore.TranPatiReasons);

                //Set focus
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadNoiChuyenDenCombo(string searchCode)
        {
            try
            {
                this.cboNoiChuyenDen.Properties.Buttons[1].Visible = false;
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(this.cboNoiChuyenDen, true);
                }
                else
                {
                    var data = DataStore.MediOrgs.Where(o => o.MEDI_ORG_CODE.Contains(searchCode)).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.MEDI_ORG_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboNoiChuyenDen.EditValue = searchResult[0].MEDI_ORG_CODE;
                        this.txtMaNoiChuyenDen.Text = searchResult[0].MEDI_ORG_CODE;
                        this.cboNoiChuyenDen.Properties.Buttons[1].Visible = true;
                        this.icdProcessor.FocusControl(ucIcd);

                        this.ProcessLevelOfMediOrg();
                    }
                    else
                    {
                        ResetEditorControl.ResetAndFocus(this.cboNoiChuyenDen, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        ///// <summary>
        ///// Hàm xử lý lấy thông tin chẩn đoán theo mã, sau đó gán kết quả tìm được vào giao diện
        ///// </summary>
        ///// <param name="searchCode"></param>
        ///// <param name="isExpand"></param>
        //private void LoadChuanDoanTDCombo(string searchCode)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            txtMaChuanDoanTD.ErrorText = "";
        //            this.chkHasDialogText.Enabled = false;
        //            this.cboChuanDoanTD.Properties.Buttons[1].Visible = false;
        //            ResetEditorControl.ResetAndFocus(this.cboChuanDoanTD, false);
        //        }
        //        else
        //        {
        //            var data = DataStore.Icds.Where(o => o.ICD_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
        //            var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ICD_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
        //            if (searchResult != null && searchResult.Count == 1)
        //            {
        //                this.cboChuanDoanTD.Properties.Buttons[1].Visible = true;
        //                this.cboChuanDoanTD.EditValue = searchResult[0].ID;
        //                this.txtMaChuanDoanTD.Text = searchResult[0].ICD_CODE;
        //                this.chkHasDialogText.Enabled = true;
        //                this.chkHasDialogText.Checked = (this.autoCheckIcd == "1");
        //                if (this.chkHasDialogText.Checked)
        //                {
        //                    this.txtDialogText.Text = searchResult[0].ICD_NAME;
        //                    this.txtDialogText.Focus();
        //                    this.txtDialogText.SelectAll();
        //                }
        //                else
        //                    this.chkHasDialogText.Focus();
        //                txtMaChuanDoanTD.ErrorText = "";
        //            }
        //            else
        //            {
        //                txtMaChuanDoanTD.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void ChangecboChanDoanTD()
        //{
        //    try
        //    {
        //        MOS.EFMODEL.DataModels.HIS_ICD data = DataStore.Icds.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboChuanDoanTD.EditValue ?? "0").ToString()));
        //        if (data != null)
        //        {
        //            this.cboChuanDoanTD.Properties.Buttons[1].Visible = true;
        //            this.txtMaChuanDoanTD.Text = data.ICD_CODE;
        //            this.chkHasDialogText.Enabled = true;
        //            if (autoCheckIcd == "1")
        //            {
        //                this.chkHasDialogText.Checked = true;
        //            }
        //            if (this.chkHasDialogText.Checked)
        //            {
        //                this.txtDialogText.Text = data.ICD_NAME;
        //                this.txtDialogText.Focus();
        //                this.txtDialogText.SelectAll();
        //            }
        //            else
        //            {
        //                this.chkHasDialogText.Focus();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        /// <summary>
        /// Hàm xử lý lấy thông tin hình thức chuyển tuyến theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        private void LoadTranPatiFormCombo(string searchCode)
        {
            try
            {
                this.cboHinhThucChuyen.Properties.Buttons[1].Visible = false;
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(this.cboHinhThucChuyen, true);
                }
                else
                {
                    var data = DataStore.TranPatiForms.Where(o => o.TRAN_PATI_FORM_CODE.Contains(searchCode)).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.TRAN_PATI_FORM_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboHinhThucChuyen.EditValue = searchResult[0].ID;
                        this.txtMaHinhThucChuyen.Text = searchResult[0].TRAN_PATI_FORM_CODE;
                        this.cboHinhThucChuyen.Properties.Buttons[1].Visible = true;
                        this.txtMaLyDoChuyen.Focus();
                        this.txtMaLyDoChuyen.SelectAll();
                    }
                    else
                    {
                        ResetEditorControl.ResetAndFocus(this.cboHinhThucChuyen, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin lý do chuyển tuyến theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        private void LoadTranPatiReasonCombo(string searchCode)
        {
            try
            {
                this.cboLyDoChuyen.Properties.Buttons[1].Visible = false;
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(this.cboLyDoChuyen, true);
                }
                else
                {
                    var data = DataStore.TranPatiReasons.Where(o => o.TRAN_PATI_REASON_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.TRAN_PATI_REASON_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboLyDoChuyen.EditValue = searchResult[0].ID;
                        this.txtMaLyDoChuyen.Text = searchResult[0].TRAN_PATI_REASON_CODE;
                        this.cboLyDoChuyen.Properties.Buttons[1].Visible = true;
                        if (this.dlgFocusNextUserControl != null)
                            this.dlgFocusNextUserControl(null);
                    }
                    else
                    {
                        ResetEditorControl.ResetAndFocus(this.cboLyDoChuyen, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        //{
        //    try
        //    {
        //        this.chkHasDialogText.Enabled = true;
        //        this.chkHasDialogText.Checked = true;
        //        this.txtDialogText.Text = text;
        //        this.txtDialogText.Focus();
        //        this.txtDialogText.SelectAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
