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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.BedRoomPartial.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.BedRoomPartial.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using AutoMapper;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.BedRoomPartial.Resources;
using EMR.EFMODEL.DataModels;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    public partial class FrmFollow : Form
    {
        int positionHandleControl = -1;
        L_HIS_TREATMENT_BED_ROOM Treatment = new L_HIS_TREATMENT_BED_ROOM();
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string ModuleLinkName = "HIS.Desktop.Plugins.BedRoomPartial";
        HIS.Desktop.Common.DelegateRefreshData RefreshData;
        public FrmFollow(L_HIS_TREATMENT_BED_ROOM Treatment_, HIS.Desktop.Common.DelegateRefreshData RefreshData_)
        {
            Treatment = Treatment_;
            RefreshData = RefreshData_;
            InitializeComponent();
        }


        private void FrmFollow_Load(object sender, EventArgs e)
        {
            SetIcon();
            this.SetCaptionByLanguageKey();
            txtDateFrom.DateTime = DateTime.Now;
            ValidSpinPulse(txtDateFrom, txtDateTo);
            ValidSpinPulse(txtDateTo, txtDateTo);

            InitControlState();

            btnSave.Select();
            btnSave.Focus();
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {

                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidSpinPulse(DateEdit txt_From, DateEdit txt_To)
        {
            ValidateTime spin = new ValidateTime();
            spin.txt = txt_From;
            spin.txtTo = txt_To;
            if (txt_From.DateTime > txt_To.DateTime)
            {
                spin.ErrorText = Resources.ResourceMessage.ThoiGianDenPhaiLonHonThoiGianTu;
            }
            else
            {
                spin.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
            }

            spin.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txt_From, spin);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dxValidationProvider1.Validate())
                return;

            CommonParam param = new CommonParam();
            bool success = false;


            MOS.SDO.ObservedTimeSDO sdo = new ObservedTimeSDO();

            sdo.ObservedTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtDateFrom.DateTime) ?? 0;
            sdo.ObservedTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtDateTo.DateTime) ?? 0;
            sdo.IsUnobserved = false;
            sdo.TreatmentBedRoomId = Treatment.ID;

            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatmentBedRoom/SetObservedTime", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            WaitingManager.Hide();
            if (success)
            {
                RefreshData();
            }
            #region Show message
            MessageManager.Show(this, param, success);
            #endregion

            #region Process has exception
            SessionManager.ProcessTokenLost(param);
            #endregion
        }

        private void chk24_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chk24.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddDays(1);
                }
                else if (chk12.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddHours(12);
                }
                else if (chkKhac.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime;
                }
                else
                {
                    txtDateTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chk12_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chk24.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddDays(1);
                }
                else if (chk12.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddHours(12);
                }
                else if (chkKhac.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime;
                }
                else
                {
                    txtDateTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chk24.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddDays(1);
                }
                else if (chk12.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime.AddHours(12);
                }
                else if (chkKhac.Checked)
                {
                    txtDateTo.DateTime = txtDateFrom.DateTime;
                }
                else
                {
                    txtDateTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            try
            {

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chk24.Name)
                        {
                            chk24.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chk12.Name)
                        {
                            chk12.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkKhac.Name)
                        {
                            chkKhac.Checked = item.VALUE == "1";
                        }
                    }

                }
                else
                {
                    chk24.Checked = true;

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FrmFollow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // khac
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateKhac = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chkKhac" && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdateKhac != null)
                {
                    csAddOrUpdateKhac.VALUE = (chkKhac.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdateKhac = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateKhac.KEY = chkKhac.Name;
                    csAddOrUpdateKhac.VALUE = (chkKhac.Checked ? "1" : "");
                    csAddOrUpdateKhac.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateKhac);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // 12h 

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate12 = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chk12" && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate12 != null)
                {
                    csAddOrUpdate12.VALUE = (chk12.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate12 = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate12.KEY = chk12.Name;
                    csAddOrUpdate12.VALUE = (chk12.Checked ? "1" : "");
                    csAddOrUpdate12.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate12);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);

                // 24h
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate24 = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "chk24" && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate24 != null)
                {
                    csAddOrUpdate24.VALUE = (chk24.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate24 = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate24.KEY = chk24.Name;
                    csAddOrUpdate24.VALUE = (chk24.Checked ? "1" : "");
                    csAddOrUpdate24.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate24);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);


                RefreshData = null;
                ModuleLinkName = null;
                currentControlStateRDO = null;
                controlStateWorker = null;
                Treatment = null;
                positionHandleControl = 0;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.chkKhac.CheckedChanged -= new System.EventHandler(this.chkKhac_CheckedChanged);
                this.chk12.CheckedChanged -= new System.EventHandler(this.chk12_CheckedChanged);
                this.chk24.CheckedChanged -= new System.EventHandler(this.chk24_CheckedChanged);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.FrmFollow_FormClosed);
                this.Load -= new System.EventHandler(this.FrmFollow_Load);
                barButtonItem1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                emptySpaceItem1 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                chk24 = null;
                chk12 = null;
                chkKhac = null;
                txtDateFrom = null;
                txtDateTo = null;
                btnSave = null;
                layoutControl1 = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FrmFollow
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__FrmFollow = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(FrmFollow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FrmFollow.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.chkKhac.Properties.Caption = Inventec.Common.Resource.Get.Value("FrmFollow.chkKhac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.chk12.Properties.Caption = Inventec.Common.Resource.Get.Value("FrmFollow.chk12.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.chk24.Properties.Caption = Inventec.Common.Resource.Get.Value("FrmFollow.chk24.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FrmFollow.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FrmFollow.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FrmFollow.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FrmFollow.Text", Resources.ResourceLanguageManager.LanguageResource__FrmFollow, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
