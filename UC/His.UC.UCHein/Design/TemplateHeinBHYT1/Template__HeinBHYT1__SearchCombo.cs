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
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein.Base;
using His.UC.UCHein.Config;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.HisPatient;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.LibraryHein.Bhyt;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        private void ChangecboChanDoanTD()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_ICD data = DataStore.Icds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboChanDoanTD.EditValue ?? "0").ToString()));
                if (data != null)
                {
                    this.cboChanDoanTD.Properties.Buttons[1].Visible = true;
                    this.txtMaChanDoanTD.Text = data.ICD_CODE;
                    this.chkHasDialogText.Enabled = true;
                    if (autoCheckIcd == "1")
                    {
                        this.chkHasDialogText.Checked = true;
                    }
                    if (this.chkHasDialogText.Checked)
                    {
                        this.txtDialogText.Text = data.ICD_NAME;
                        this.txtDialogText.Focus();
                        this.txtDialogText.SelectAll();
                    }
                    else
                    {
                        this.txtDialogText.Text = data.ICD_NAME;
                        this.chkHasDialogText.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        {
            try
            {
                this.chkHasDialogText.Enabled = true;
                this.chkHasDialogText.Checked = true;
                this.txtDialogText.Text = text;
                this.txtDialogText.Focus();
                this.txtDialogText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin chẩn đoán theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
        private void LoadChuanDoanTDCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    txtMaChanDoanTD.ErrorText = "";
                    this.chkHasDialogText.Enabled = false;
                    this.cboChanDoanTD.Properties.Buttons[1].Visible = false;
                    ResetEditorControl.ResetAndFocus(this.cboChanDoanTD, false);
                }
                else
                {
                    var data = DataStore.Icds.Where(o => o.ICD_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ICD_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboChanDoanTD.Properties.Buttons[1].Visible = true;
                        this.cboChanDoanTD.EditValue = searchResult[0].ID;
                        this.txtMaChanDoanTD.Text = searchResult[0].ICD_CODE;
                        this.chkHasDialogText.Enabled = true;
                        this.chkHasDialogText.Checked = (this.autoCheckIcd == "1");
                        if (this.chkHasDialogText.Checked)
                        {
                            this.txtDialogText.Text = searchResult[0].ICD_NAME;
                            this.txtDialogText.Focus();
                            this.txtDialogText.SelectAll();
                        }
                        else
                        {
                            this.txtDialogText.Text = searchResult[0].ICD_NAME;
                            this.chkHasDialogText.Focus();
                        }
                        txtMaChanDoanTD.ErrorText = "";
                    }
                    else
                    {
                        txtMaChanDoanTD.ErrorText = ResourceMessage.MaBenhChinhKhongHopLe;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin loại đúng tuyến theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
        private void LoadHeinRightRouterTypeCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(cboHeinRightRoute, true);
                }
                else
                {
                    var data = DataStore.HeinRightRouteTypes.Where(o => o.HeinRightRouteTypeCode.Contains(searchCode)).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.HeinRightRouteTypeCode.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                        this.cboHeinRightRoute.EditValue = searchResult[0].HeinRightRouteTypeCode;
                        this.txtHeinRightRouteCode.Text = searchResult[0].HeinRightRouteTypeCode;
                        if (searchResult[0].HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC, true);
                        }
                        else if (searchResult[0].HeinRightRouteTypeCode == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT)
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT, true);
                        }
                        else
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT, true);
                        }
                    }
                    else
                    {
                        ResetEditorControl.ResetAndFocus(cboHeinRightRoute, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                        this.FocusMoveOut();
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

        /// <summary>
        /// Hàm xử lý lấy thông tin cơ sở kcb theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
        private void LoadNoiDKKCBBDCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(this.cboDKKCBBD, false, true);
                }
                else
                {
                    var data = DataStore.MediOrgs.Where(o => o.MEDI_ORG_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG one = null;
                        one = (data.Count == 1 ? data[0] : DataStore.MediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE.Equals(searchCode)));
                        if (one != null)
                        {
                            this.cboDKKCBBD.EditValue = one.MEDI_ORG_CODE;
                            this.txtMaDKKCBBD.Text = one.MEDI_ORG_CODE;
                            this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());
                            string heinCardNumber = txtSoThe.Text;
                            heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                            heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                            this.CheckExamHistoryFromBHXHApi(heinCardNumber);
                        }
                        else
                        {
                            ResetEditorControl.ResetAndFocus(this.cboDKKCBBD, false, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin nơi chuyển đến theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
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
                        this.ProcessLevelOfMediOrg();
                        this.txtMaChanDoanTD.Focus();
                        this.txtMaChanDoanTD.SelectAll();
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
    }
}
