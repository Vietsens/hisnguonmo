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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ConfirmPresBlood.ADO;
using HIS.Desktop.Plugins.ConfirmPresBlood.Resources;
using HIS.Desktop.Plugins.ConfirmPresBlood.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConfirmPresBlood
{
    public partial class frmConfirmPresBlood : HIS.Desktop.Utility.FormBase
    {
        private void FillDataToGridExpMestBlty()
        {
            try
            {
                listExpMestBlty = new List<VHisExpMestBltyADO>();
                HisExpMestBltyReqView1Filter expMestBltyFilter = new HisExpMestBltyReqView1Filter();
                expMestBltyFilter.EXP_MEST_ID = this.expMestId;
                var listExpMestBltyReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>("api/HisExpMestBltyReq/GetView1", ApiConsumers.MosConsumer, expMestBltyFilter, null);
                if (listExpMestBltyReq != null && listExpMestBltyReq.Count > 0)
                {
                    foreach (var item in listExpMestBltyReq)
                    {
                        VHisExpMestBltyADO ado = new VHisExpMestBltyADO(item);
                        ado.AmountReq = ado.AMOUNT;
                        listExpMestBlty.Add(ado);
                    }
                }
                gridControlExpMestBlty.BeginUpdate();
                gridControlExpMestBlty.DataSource = listExpMestBlty;
                gridControlExpMestBlty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodAndPatyMediStockId()
        {
            try
            {
                listBlood = new List<V_HIS_BLOOD>();
                HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                bloodFilter.MEDI_STOCK_ID = this.mediStockId;
                bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumers.MosConsumer, bloodFilter, null);
                if (listBlood != null && listBlood.Count > 0)
                {
                    MOS.Filter.HisServicePatyFilter filter = new HisServicePatyFilter();
                    filter.SERVICE_IDs = listBlood.Select(p => p.SERVICE_ID).ToList();
                    var servicePatys = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_PATY>>("api/HisServicePaty/Get", ApiConsumers.MosConsumer, filter, null);
                    if (servicePatys != null && servicePatys.Count > 0)
                    {
                        List<long> serviceIds = servicePatys.Select(p => p.SERVICE_ID).ToList();
                        listBlood = listBlood.Where(p => serviceIds.Contains(p.SERVICE_ID)).ToList();
                    }
                    else
                    {
                        listBlood = new List<V_HIS_BLOOD>();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataBlood()
        {
            try
            {
                dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();
                dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();
                if (listBlood != null && listBlood.Count > 0)
                {
                    foreach (var item in listBlood)
                    {
                        dicBloodCode[item.BLOOD_CODE] = item;
                        dicCurrentBlood[item.ID] = item;
                        if (!dicBloodAdo.ContainsKey(item.ID))
                            dicShowBlood[item.ID] = item;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridBlood()
        {
            try
            {

                gridControlBlood.BeginUpdate();
                if (listExpMestBlty == null || listExpMestBlty.Count == 0)
                {
                    return;
                }
                gridControlBlood.DataSource = dicShowBlood.Select(s => s.Value).OrderBy(o =>
                  o.BLOOD_TYPE_NAME).ThenBy(o => o.VOLUME).ThenBy(o => o.BLOOD_CODE).ToList();
                gridControlBlood.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlByExpMestBlty()
        {
            try
            {
                if (this.currentBlty != null)
                {
                    var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(o => o.ID == this.currentBlty.BLOOD_TYPE_ID);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void setGridLookUpClick()
        {
            try
            {
                gridLookUpVolume.EditValue = this.currentBlty.BLOOD_VOLUME_ID;
                gridLookUpBloodAboCode.EditValue = this.currentBlty.BLOOD_ABO_ID;
                gridLookUpBloodRhCode.EditValue = this.currentBlty.BLOOD_RH_ID;
                cboBloodType.EditValue = this.currentBlty.BLOOD_TYPE_ID;
                checkBtnRefresh = false;
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataGridViewBlood()
        {
            try
            {
                //Review 
                List<V_HIS_BLOOD> dataNew = new List<V_HIS_BLOOD>();
                if (this.listBlood != null && this.listBlood.Count > 0)
                {
                    dataNew.AddRange(this.listBlood);
                    long maxId = 0;
                    if (bloodVolume != null && bloodVolume.Count > 0)
                    {
                        var listID = bloodVolume.Select(o => o.ID).ToList();
                        maxId = listID.Max();
                        if (gridLookUpVolume.EditValue != null)
                        {
                            long bloodVolumeId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpVolume.EditValue.ToString());
                            if (bloodVolumeId != maxId)
                                dataNew = dataNew.Where(p => p.BLOOD_VOLUME_ID == bloodVolumeId).ToList();
                        }

                        if (cboBloodType.EditValue != null)
                        {
                            long bloodTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodType.EditValue.ToString());
                            dataNew = dataNew.Where(p => p.BLOOD_TYPE_ID == bloodTypeId).ToList();
                        }
                        if (gridLookUpBloodAboCode.EditValue != null)
                        {
                            long bloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodAboCode.EditValue.ToString());
                            dataNew = dataNew.Where(p => p.BLOOD_ABO_ID == bloodAboId).ToList();
                        }
                        if (gridLookUpBloodRhCode.EditValue != null)
                        {
                            long bloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpBloodRhCode.EditValue.ToString());
                            dataNew = dataNew.Where(p => p.BLOOD_RH_ID == bloodRhId).ToList();
                        }
                    }
                }

                List<V_HIS_BLOOD> dataGrid = new List<V_HIS_BLOOD>();
                if (dataNew != null && dataNew.Count > 0 && this.dicShowBlood != null && this.dicShowBlood.Count > 0)
                {
                    List<long> bloodIds = dataNew.Select(p => p.ID).ToList();
                    dataGrid = this.dicShowBlood.Where(p => bloodIds.Contains(p.Key)).Select(p => p.Value).ToList();
                }
                gridViewBlood.GridControl.DataSource = dataGrid;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region LoadCombo
        private void frmExpMestBlood_Plus_GridLookup()
        {
            try
            {
                initGridLookupVolume();
                initGridLookupAbo();
                initGridLookupRh();
                initGridLookupBloodType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupVolume()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodVolumeFilter filter = new MOS.Filter.HisBloodVolumeFilter();
                filter.KEY_WORD = gridLookUpVolume.Text;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "VOLUME";
                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_VOLUME>>
                  ("api/HisBloodVolume/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                bloodVolume = new List<BloodVolumeADO>();
                BloodVolumeADO addAll = new BloodVolumeADO();
                addAll.ID = 0;// obj.FirstOrDefault().ID + 1;
                addAll.Blood_Volume_Str = "Tất cả";
                bloodVolume.Add(addAll);
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {
                        BloodVolumeADO blood = new BloodVolumeADO(item);
                        bloodVolume.Add(blood);
                    }
                }

                // bloodVolume = bloodVolume.OrderBy(o => o.ID).ThenBy(p => p.VOLUME).ToList();

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("Blood_Volume_Str", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("Blood_Volume_Str", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpVolume, bloodVolume, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupAbo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodAboFilter filter = new MOS.Filter.HisBloodAboFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>>
                  ("api/HisBloodAbo/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodAboCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupRh()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>>
                  ("api/HisBloodRh/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_RH_CODE", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfo, false);
                ControlEditorLoader.Load(gridLookUpBloodRhCode, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void initGridLookupBloodType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodTypeFilter filter = new MOS.Filter.HisBloodTypeFilter();

                var obj = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>
                  ("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 0, 1));
                columnInfo.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 0, 1));

                ControlEditorADO ado = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfo, false);
                ControlEditorLoader.Load(cboBloodType, obj, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region GridBlood_Control
        private void gridViewExpMestBlty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_EXP_MEST_BLTY_REQ_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "REQ_BLOOD_TYPE_CODE_DISPLAY")
                        {
                            e.Value = !String.IsNullOrWhiteSpace(data.REQ_BLOOD_TYPE_CODE) ? data.REQ_BLOOD_TYPE_CODE : data.BLOOD_TYPE_CODE;
                        }
                        else if (e.Column.FieldName == "REQ_BLOOD_TYPE_NAME_DISPLAY")
                        {
                            e.Value = !String.IsNullOrWhiteSpace(data.REQ_BLOOD_TYPE_NAME) ? data.REQ_BLOOD_TYPE_NAME : data.BLOOD_TYPE_NAME;
                        }
                        else if (e.Column.FieldName == "REQ_BLOOD_ABO_CODE_DISPLAY")
                        {
                            e.Value = !String.IsNullOrWhiteSpace(data.REQ_BLOOD_ABO_CODE) ? data.REQ_BLOOD_ABO_CODE : data.BLOOD_ABO_CODE;
                        }
                        else if (e.Column.FieldName == "REQ_BLOOD_HR_CODE_DISPLAY")
                        {
                            e.Value = !String.IsNullOrWhiteSpace(data.REQ_BLOOD_RH_CODE) ? data.REQ_BLOOD_RH_CODE : data.BLOOD_RH_CODE;
                        }
                        else if (e.Column.FieldName == "REQ_VOLUME_DISPLAY")
                        {
                            e.Value = data.REQ_VOLUME > 0 ? data.REQ_VOLUME : data.VOLUME;
                        }
                        else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            e.Value = data.REQ_AMOUNT.HasValue ? data.REQ_AMOUNT.Value : data.AMOUNT;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                        {
                            if (data.EXPIRED_DATE > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        else if (e.Column.FieldName == "IMP_TIME_STR")
                        {
                            if (data.IMP_TIME > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewExMestBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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

        private void gridViewExpMestBlty_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentBlty = null;
                this.currentBlty = (V_HIS_EXP_MEST_BLTY_REQ_1)gridViewExpMestBlty.GetFocusedRow();
                this.SetControlByExpMestBlty();
                setGridLookUpClick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditAddBlood_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider1.Validate())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCanChonYeuCauXuatTruocKhiChonTuiMau, "Thông báo");
                    return;
                }
                var blood = (V_HIS_BLOOD)gridViewBlood.GetFocusedRow();
                if (blood != null)
                {
                    if (blood.BLOOD_TYPE_ID != this.currentBlty.BLOOD_TYPE_ID && this.AllowExportBloodOverRequestCFG == "0")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TuiMauKhongThuocLoaiMauDangChon,
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            WaitingManager.Hide();
                            return;
                        }
                    }
                    var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                    if (count >= this.currentBlty.AMOUNT && this.AllowExportBloodOverRequestCFG == "0")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DaDuSoLuongMauYeuCau, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                        return;
                    }

                    WaitingManager.Show();
                    VHisBloodADO ado = new VHisBloodADO(blood);
                    ado.PATIENT_TYPE_ID = this.currentBlty.PATIENT_TYPE_ID;
                    ado.PATIENT_TYPE_CODE = this.currentBlty.PATIENT_TYPE_CODE;
                    ado.PATIENT_TYPE_NAME = this.currentBlty.PATIENT_TYPE_NAME;
                    ado.ExpMestBltyId = currentBlty.ID;
                    dicBloodAdo[ado.ID] = ado;
                    if (dicShowBlood.ContainsKey(ado.ID))
                    {
                        dicShowBlood.Remove(ado.ID);
                    }
                    fillDataGridViewBlood();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region focus

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
                positionHandle = -1;
                if (this.currentBlty == null || this.resultExpMest != null || !dxValidationProvider2.Validate())
                    return;

                var count = dicBloodAdo.Select(s => s.Value).ToList().Where(o => o.ExpMestBltyId == this.currentBlty.ID).ToList().Count();
                if (count >= this.currentBlty.AMOUNT)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DaDuSoLuongMauYeuCau, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                    return;
                    //if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.DaCoTrongDanhSachDuyet_BanCoMuonThayThe, this.currentBlty.BLOOD_TYPE_NAME), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    //{
                    //    return;
                    //}
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodType.EditValue != null)
                    {
                        fillDataGridViewBlood();
                        gridLookUpVolume.Focus();
                        gridLookUpVolume.SelectAll();
                    }
                    else
                    {
                        cboBloodType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBloodType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                fillDataGridViewBlood();
                cboBloodType.Properties.Buttons[1].Visible = true;
                gridLookUpVolume.Focus();
                gridLookUpVolume.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                gridLookUpVolume.Properties.Buttons[1].Visible = true;
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
                gridLookUpBloodAboCode.Properties.Buttons[1].Visible = true;
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
                gridLookUpBloodRhCode.Properties.Buttons[1].Visible = true;
                fillDataGridViewBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region validate
        private void ValidControl()
        {
            try
            {
                ValidControlBloodCode();
                //ValidControlExpiredDate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBloodCode()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void ValidControlExpiredDate()
        //{
        //    try
        //    {
        //        ExpiredDateValidationRule expiredDateRule = new ExpiredDateValidationRule();
        //        expiredDateRule.dtExpiredDate = dtExpiredDate;
        //        dxValidationProvider1.SetValidationRule(txtExpiredDate, expiredDateRule);
        //        dxValidationProvider2.SetValidationRule(txtExpiredDate, expiredDateRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

    }
}
