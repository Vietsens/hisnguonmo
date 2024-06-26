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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExportBlood.ADO;
using HIS.Desktop.Plugins.ExportBlood.Base;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportBlood
{
    public partial class frmExpMestBlood : Form
    {
        private void gridViewExpMestBlty_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentBlty = null;
                this.currentBlty = (V_HIS_EXP_MEST_BLTY)gridViewExpMestBlty.GetFocusedRow();
                this.SetControlByExpMestBlty();
                setGridLookUpClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBlty_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_EXP_MEST_BLTY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_BLOOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExMestBlood_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExMestBlood_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                if (e.Column.FieldName != "EXPIRED_DATE_STR")
                    return;
                gridViewExMestBlood.PostEditor();
                DateTime? dt = null;
                if (gridViewExMestBlood.EditingValue != null)
                {
                    dt = (DateTime)gridViewExMestBlood.EditingValue;
                }
                if (!dt.HasValue || dt.Value == DateTime.MinValue)
                {
                    MessageManager.Show(Base.ResourceMessageManager.TruongDuLieuBatBuoc);
                }
                else if (dt.Value < DateTime.Now)
                {
                    MessageManager.Show(ResourceMessageManager.HanDungKhongDuocBeHonHienTai);
                }
                else
                {
                    var data = (VHisBloodADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        data.EXPIRED_DATE = Convert.ToInt64(dt.Value.ToString("yyyyMMdd") + "235959");
                    }
                }
                gridControlExpMestBlood.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();
                        txtBloodCode.Focus();
                        txtBloodCode.SelectAll();
                    }
                    else
                    {
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = Base.ResourceMessageManager.NguoiDungNhapNgayKhongHopLe;
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Leave(object sender, EventArgs e)
        {
            try
            {
                dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                txtBloodCode.Focus();
                txtBloodCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        dtExpiredDate.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                        txtBloodCode.Focus();
                        txtBloodCode.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtExpiredDate.EditValue = dt;
                        dtExpiredDate.Update();
                        txtBloodCode.Focus();
                        txtBloodCode.SelectAll();
                    }
                    else
                    {
                        dtExpiredDate.Visible = true;
                        dtExpiredDate.Focus();
                        dtExpiredDate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpiredDate_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();

                if (!String.IsNullOrEmpty(currentValue))
                {
                    int day = Int16.Parse(currentValue.Substring(0, 2));
                    int month = Int16.Parse(currentValue.Substring(3, 2));
                    int year = Int16.Parse(currentValue.Substring(6, 4));
                    if (day < 0 || day > 31 || month < 0 || month > 12 || year < 1000 || year > DateTime.Now.Year)
                    {
                        //e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtExpiredDate.Visible = false;
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpiredDate.Visible = false;
                    dtExpiredDate.Update();
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpiredDate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtExpiredDate.Text = "";
                if (dtExpiredDate.EditValue != null && dtExpiredDate.DateTime != DateTime.MinValue)
                {
                    txtExpiredDate.Text = dtExpiredDate.DateTime.ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBloodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessAddBloodIntoExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAddBloodIntoExpMest()
        {
            try
            {
                positionHandleControl = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider2.Validate())
                    return;
                string bloodCode = txtBloodCode.Text.Trim();
                if (!dicBloodCode.ContainsKey(bloodCode))
                {
                    MessageManager.Show(Base.ResourceMessageManager.MaVachKhongChinhXac);
                    return;
                }

                var blood = dicBloodCode[bloodCode];
                if (blood != null)
                {
                    var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                    if (count >= this.currentBlty.AMOUNT)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Base.ResourceMessageManager.SoLuongTuiMauCuaLoaiLonHonSoLuongYeuCau, this.currentBlty.BLOOD_TYPE_NAME), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    WaitingManager.Show();
                    VHisBloodADO ado = new VHisBloodADO(blood);
                    if (this.currentBlty.PATIENT_TYPE_ID.HasValue)
                    {
                        HisBloodPatyFilter bloodPatyFilter = new HisBloodPatyFilter();
                        bloodPatyFilter.BLOOD_ID = blood.ID;
                        bloodPatyFilter.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID.Value;
                        var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_PATY>>("api/HisBloodPaty/Get", ApiConsumers.MosConsumer, bloodPatyFilter, null);
                        if (datas == null || datas.Count <= 0)
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(String.Format(Base.ResourceMessageManager.TuiMauKhongCoChinhSachGiaChoDoiTuong, this.currentBlty.PATIENT_TYPE_NAME));
                            return;
                        }
                    }
                    ado.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = this.currentBlty.PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = this.currentBlty.PATIENT_TYPE_NAME;
                    ado.ExpMestBltyId = this.currentBlty.ID;
                    ado.EXPIRED_DATE = Convert.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "235959");
                    dicBloodAdo[ado.ID] = ado;
                    if (dicShowBlood.ContainsKey(ado.ID))
                    {
                        dicShowBlood.Remove(ado.ID);
                    }
                    FillDataToGridBlood();
                    FillDataToGridExpMestBlood();
                    txtBloodCode.Text = "";
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpVolume_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpVolume.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpBloodAboCode.Focus();
                        gridLookUpBloodAboCode.SelectAll();
                    }
                    else
                    {
                        gridLookUpVolume.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpBloodAboCode.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpBloodRhCode.Focus();
                        gridLookUpBloodRhCode.SelectAll();
                    }
                    else
                    {
                        gridLookUpBloodAboCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpBloodRhCode.EditValue != null)
                    {
                        fillDataGridViewBlood();

                    }
                    else
                    {
                        gridLookUpBloodRhCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpVolume_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                gridLookUpBloodAboCode.Focus();
                gridLookUpBloodAboCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                gridLookUpBloodRhCode.Focus();
                gridLookUpBloodRhCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
