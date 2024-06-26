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
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.BedInfo
{
    public partial class FormBedInfo : FormBase
    {
        private LocalStorage.BackendData.ADO.SereServADO sereServADO;
        private long IntructionTime;
        private List<HIS_BED_BSTY> hisBedBstys;
        private Inventec.Desktop.Common.Modules.Module module;
        private HIS_DEPARTMENT CurrentDepartment;
        private List<HisBedADO> dataBedADOs;
        private bool isInit;

        const int MaxReq = 500;
        private int positionHandleControl;
        private List<HIS_TREATMENT> ListTreatment;

        public FormBedInfo(Inventec.Desktop.Common.Modules.Module module, HIS_DEPARTMENT currentDepartment, long intructionTime, LocalStorage.BackendData.ADO.SereServADO sereServADO, List<HIS_TREATMENT> treatment)
            : base(module)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.module = module;
            this.sereServADO = sereServADO;
            this.IntructionTime = intructionTime;
            this.CurrentDepartment = currentDepartment;
            this.IsUseApplyFormClosingOption = false;
            this.ListTreatment = treatment;
        }

        private void FormBedInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefaultValue();

                ValidateControl();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormBedInfo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(FormBedInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBed.Properties.NullText = Inventec.Common.Resource.Get.Value("FormBedInfo.cboBed.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedStartTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormBedInfo.lciBedStartTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedStartTime.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.lciBedStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedFinishTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormBedInfo.lciBedFinishTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedFinishTime.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.lciBedFinishTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciBedCode.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.LciBedCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciShareCount.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.lciShareCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormBedInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        private void ValidateControl()
        {
            try
            {
                ValidationSingleControl(dtBedStartTime, dxValidationProvider1);
                ValidationSingleControl(dtBedFinishTime, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboBed, txtBedCode, dxValidationProvider1);
                ValidationDate(dtBedStartTime, dtBedFinishTime, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		private void ValidationDate(DateEdit dtBedStartTime, DateEdit dtBedFinishTime, DXValidationProvider dxValidationProvider1)
		{
			try
			{
                DateValid validRule = new DateValid();
                validRule.dte1 = dtBedStartTime;
                validRule.dte2 = dtBedFinishTime;
                dxValidationProvider1.SetValidationRule(dtBedStartTime, validRule);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}

        private void InitCboBed()
        {
            try
            {
                this.hisBedBstys = BackendDataWorker.Get<HIS_BED_BSTY>().Where(o => o.IS_ACTIVE == 1 && o.BED_SERVICE_TYPE_ID == this.sereServADO.SERVICE_ID).ToList();
                var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.DEPARTMENT_ID == this.CurrentDepartment.ID).ToList();

                var listBed = BackendDataWorker.Get<V_HIS_BED>().Where(o => bedRoom.Exists(s => s.ID == o.BED_ROOM_ID) && this.hisBedBstys.Exists(s => s.BED_ID == o.ID)).ToList();

                this.dataBedADOs = ProcessDataBedAdo(listBed);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BED_NAME", "", 250, 2));
                columnInfos.Add(new ColumnInfo("AMOUNT_STR", "", 50, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboBed, this.dataBedADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisBedADO> ProcessDataBedAdo(List<V_HIS_BED> datas)
        {
            List<HisBedADO> result = null;
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    result = new List<HisBedADO>();
                    result.AddRange((from r in datas select new HisBedADO(r)).ToList());

                    long? timeFilter = null;
                    if (dtBedStartTime.EditValue != null && dtBedStartTime.DateTime != DateTime.MinValue)
                    {
                        timeFilter = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBedStartTime.DateTime) ?? 0;
                    }

                    List<long> bedIds = datas.Select(p => p.ID).Distinct().ToList();

                    int skip = 0;
                    while (bedIds.Count - skip > 0)
                    {
                        var listIds = bedIds.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;

                        MOS.Filter.HisBedLogFilter filter = new MOS.Filter.HisBedLogFilter();
                        filter.BED_IDs = listIds;
                        if (dtBedStartTime.EditValue != null && dtBedStartTime.DateTime != DateTime.MinValue)
                        {
                            filter.START_TIME_TO = timeFilter;
                            filter.FINISH_TIME_FROM__OR__NULL = timeFilter;
                        }
                        CommonParam param = new CommonParam();
                        var dataBedLogs = new BackendAdapter(param).Get<List<HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (dataBedLogs != null && dataBedLogs.Count > 0)
                        {
                            foreach (var itemADO in result)
                            {
                                var dataByBedLogs = dataBedLogs.Where(p => p.BED_ID == itemADO.ID && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value > timeFilter))).ToList();

                                if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                                {
                                    if (itemADO.MAX_CAPACITY.HasValue)
                                    {
                                        if (dataByBedLogs.Count >= itemADO.MAX_CAPACITY)
                                            itemADO.IsKey = 2;
                                        else
                                            itemADO.IsKey = 1;
                                    }
                                    else
                                        itemADO.IsKey = 1;

                                    itemADO.AMOUNT = dataByBedLogs.Count;
                                    itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + itemADO.MAX_CAPACITY;
                                    itemADO.TREATMENT_BED_ROOM_IDs = dataByBedLogs.Select(o => o.TREATMENT_BED_ROOM_ID).ToList();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDefaultValue()
        {
            try
            {
                this.isInit = true;
                //gán thông tin giường sau để khi đổi thời gian bắt đầu mới load thông tin giường vào combo
                this.dtBedFinishTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServADO.BedFinishTime ?? 0);
                this.dtBedStartTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServADO.BedStartTime ?? this.IntructionTime);
                this.spShareCount.EditValue = this.sereServADO.ShareCount;
                this.cboBed.EditValue = this.sereServADO.BedId;
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtBedCode.Text = "";
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && dataBedADOs != null && dataBedADOs.Count > 0)
                {
                    HisBedADO row = dataBedADOs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "").ToString()));
                    if (row != null)
                    {
                        this.txtBedCode.Text = row.BED_CODE;
                        this.spShareCount.EditValue = null;

                        if (!this.isInit)
                        {
                            var treatmentTYpe = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => ListTreatment.Select(p => p.TDL_TREATMENT_TYPE_ID).ToList().Exists(p => p == o.ID)).ToList();
                            if (row.IsKey == 1 || (row.IsKey == 0 && !(ListTreatment.Count + row.AMOUNT > row.MAX_CAPACITY)))
                            {
                                if (treatmentTYpe != null)
                                {
                                    if ((ListTreatment.Count > 1  || row.AMOUNT > 0) && treatmentTYpe.Exists(o=>o.IS_NOT_ALLOW_SHARE_BED == 1))
                                    {
                                        var lst = ListTreatment.Where(o => treatmentTYpe.Where(p => p.IS_NOT_ALLOW_SHARE_BED == 1).ToList().Exists(p=> p.ID == o.TDL_TREATMENT_TYPE_ID)).Select(o => new {o.TDL_PATIENT_NAME,o.TDL_TREATMENT_TYPE_ID});
                                        string msg = null;
                                        int count = 0;
                                        foreach (var item in lst)
                                        {
                                            count++;
                                            msg += item.TDL_PATIENT_NAME + " - " + treatmentTYpe.FirstOrDefault(o => o.ID == item.TDL_TREATMENT_TYPE_ID).TREATMENT_TYPE_NAME;
                                            if (count != lst.Count())
                                                msg += "; ";
                                        }
                                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(Resources.ResourceMessage.KhongDuocNamGhepGiuong, msg), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                                        cbo.EditValue = null;
                                        cbo.ShowPopup();
                                        return;
                                    }
                                    else if(row.IsKey == 1)
                                    {
                                        if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.GiuongDaCoBenhNhanNam, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                                        {
                                            cboBed.EditValue = null;
                                            cboBed.ShowPopup();
                                            return;
                                        }

                                        this.spShareCount.Value = row.AMOUNT + 1;
                                    }
                                }
                            }
                            else if (row.IsKey == 2 || (ListTreatment.Count + row.AMOUNT > row.MAX_CAPACITY))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.GiuongDaVuotQuaSucChua, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                cboBed.EditValue = null;
                                cboBed.ShowPopup();
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

        private void dtBedStartTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtBedFinishTime.Focus();
                    this.dtBedFinishTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtBedFinishTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtBedCode.Focus();
                    this.txtBedCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBedCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboBed.EditValue = null;
                        cboBed.Focus();
                        cboBed.ShowPopup();
                    }
                    else
                    {
                        var data = dataBedADOs.Where(o => o.BED_CODE.Contains(searchCode)).ToList();
                        List<HisBedADO> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BED_CODE == searchCode).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboBed.EditValue = result[0].ID;
                            txtBedCode.Text = result[0].BED_CODE;
                            spShareCount.Focus();
                            spShareCount.SelectAll();
                        }
                        else
                        {
                            cboBed.EditValue = null;
                            cboBed.Focus();
                            cboBed.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spShareCount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1View_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    long IsKey = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IsKey") ?? "0").ToString());
                    if (IsKey == 1)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (IsKey == 2)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }

                this.sereServADO.BedFinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBedFinishTime.DateTime);
                this.sereServADO.BedStartTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBedStartTime.DateTime);
                this.sereServADO.BedId = Inventec.Common.TypeConvert.Parse.ToInt64(cboBed.EditValue.ToString());
                if (spShareCount.Value > 0)
                {
                    this.sereServADO.ShareCount = (long)spShareCount.Value;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtBedStartTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitCboBed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
	}
}
