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

        //Viec 63227 (khoa 01412 = 2):
        //- useHeldBedDeferredReload, bedListStale: doi gio nam giuong thi chi danh dau danh sach giuong da cu, nap lai khi roi o
        //  gio hoac khi bam Luu. Khong nap trong luc dang go (khong bo dem): doc DateEdit.DateTime luc dang go se chot nua chung
        //  phan dang go (go gio "1", dung, go "4" ra 04 thay vi 14).
        //- isCheckingBed: dang kiem giuong da chon (co the dang hien hop hoi modal)
        //- bedLogLoadFailCount, lastBedListLoadFailed: lan nap danh sach giuong co tra cuu loi thi so nguoi dang nam khong tin duoc
        //- checkedBed*: giuong dang chon, so nguoi dang nam va khoang nam giuong luc vua kiem (chon giuong, dien san), de nut Luu
        //  kiem lai khi khoang da doi
        private bool useHeldBedDeferredReload;
        private bool bedListStale;
        private bool isCheckingBed;
        private int bedLogLoadFailCount;
        private bool lastBedListLoadFailed;
        private long checkedBedId;
        private long checkedBedAmount;
        private long checkedBedIsKey;
        private string checkedBedWindow;

        private void FormBedInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDefaultValue();

                //Viec 63227: khoa 01412 = 2 thi nguoi dang giu giuong co duoc tinh hay khong phu thuoc gio ket thuc nam giuong,
                //doi gio ket thuc phai tinh lai danh sach giuong. Gan sau LoadDefaultValue de khong nap hai lan khi mo form.
                //Doi gio chi danh dau danh sach giuong da cu; nap lai khi roi o gio (Enter sang o ma giuong, bam chon giuong) hoac khi
                //bam Luu. Khong nap trong luc dang go (khong dung bo dem, khong dung che do Buffered cua DevExpress): doc gio luc dang
                //go se chot nua chung phan dang go (go "1", dung, go "9" ra ngay 09 thay vi 19).
                if (HeldBedProcessor.IsHoldBedOn)
                {
                    this.useHeldBedDeferredReload = true;
                    this.dtBedFinishTime.EditValueChanged += this.dtBedFinishTime_EditValueChanged;
                    this.dtBedStartTime.Leave += this.dtBedTime_LeaveHeldBed;
                    this.dtBedFinishTime.Leave += this.dtBedTime_LeaveHeldBed;
                }

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

        /// <summary>
        /// Cac ho so dang chi dinh, dung de khong tinh chinh cac nguoi benh nay la nguoi dang giu giuong (viec 63227)
        /// </summary>
        private List<long> GetAssigningTreatmentIds()
        {
            List<long> ids = new List<long>();
            try
            {
                if (this.ListTreatment != null)
                {
                    foreach (var item in this.ListTreatment)
                    {
                        if (item != null && item.ID > 0 && !ids.Contains(item.ID))
                            ids.Add(item.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ids;
        }

        /// <summary>
        /// Gio ket thuc nam giuong dang nhap (rong = khong gioi han)
        /// </summary>
        private long? GetFinishTimeFilter()
        {
            if (dtBedFinishTime.EditValue != null && dtBedFinishTime.DateTime != DateTime.MinValue)
            {
                return Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBedFinishTime.DateTime);
            }
            return null;
        }

        /// <summary>
        /// Viec 63227 (khoa 01412 = 1 hoac 2): dien san giuong nguoi benh dang giu (giuong duoc chi dinh gan nhat trong
        /// buong dang nam, chua chuyen khoa, chua ra vien) khi dong dich vu giuong chua co giuong.
        /// Chi dien khi chi dinh cho 1 nguoi benh, giuong co trong danh sach chon duoc va khong co nguoi benh khac dang
        /// nam (truong hop nam ghep phai chon qua danh sach de hoi va tinh so nguoi nam ghep).
        /// Goi trong luc isInit = true nen khong hien cac hop hoi cua viec chon giuong.
        /// </summary>
        private void FillDefaultBedByHeld()
        {
            try
            {
                if (!HeldBedProcessor.IsDefaultBedByLastAssignedOn)
                    return;
                if (this.ListTreatment == null || this.ListTreatment.Count != 1 || this.ListTreatment[0] == null)
                    return;
                if (this.dataBedADOs == null || this.dataBedADOs.Count == 0)
                    return;

                List<MOS.SDO.HisBedHeldSDO> helds = HeldBedProcessor.TakeBedsHeld(null, new List<long> { this.ListTreatment[0].ID });
                if (helds == null || helds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("FillDefaultBedByHeld => nguoi benh chua giu giuong nao, de trong giuong.");
                    return;
                }

                foreach (var held in helds.OrderByDescending(o => o.StartTime))
                {
                    var bed = this.dataBedADOs.FirstOrDefault(o => o.ID == held.BedId);
                    if (bed == null)
                        continue;

                    long otherCount = bed.TREATMENT_BED_ROOM_IDs != null ? bed.TREATMENT_BED_ROOM_IDs.Where(o => o != held.TreatmentBedRoomId).Distinct().LongCount() : 0;
                    if (otherCount > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("FillDefaultBedByHeld => giuong dang giu co nguoi benh khac nam, de trong giuong. " + Inventec.Common.Logging.LogUtil.TraceData("BedId", bed.ID));
                        return;
                    }

                    this.cboBed.EditValue = bed.ID;
                    return;
                }
                Inventec.Common.Logging.LogSystem.Info("FillDefaultBedByHeld => giuong dang giu khong co trong danh sach chon duoc, de trong giuong.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        //Viec 63227: ghi nhan doc ban ghi nam giuong loi (so nguoi dang nam khong tin duoc)
                        if (dataBedLogs == null)
                            this.bedLogLoadFailCount++;

                        //Viec 63227: khoa 01412 = 2 thi tinh ca nguoi dang giu giuong (chua roi buong, chua chuyen khoa,
                        //chua ra vien) ke ca khi chua ke giuong cho ngay tiep theo. Khong tinh cac nguoi benh dang chi dinh.
                        if (HeldBedProcessor.IsHoldBedOn)
                        {
                            dataBedLogs = HeldBedProcessor.AppendHeldBedLogs(dataBedLogs, listIds, this.GetAssigningTreatmentIds(), timeFilter, this.GetFinishTimeFilter());
                        }

                        if (dataBedLogs != null && dataBedLogs.Count > 0)
                        {
                            foreach (var itemADO in result)
                            {
                                var dataByBedLogs = dataBedLogs.Where(p => p.BED_ID == itemADO.ID && (!p.FINISH_TIME.HasValue || (p.FINISH_TIME.HasValue && p.FINISH_TIME.Value > timeFilter))).ToList();

                                if (dataByBedLogs != null && dataByBedLogs.Count > 0)
                                {
                                    //Khoa 01412 = 2: giuong chua khai bao so nguoi nam toi da coi la 1 nguoi, giong Backend
                                    long? capacity = HeldBedProcessor.GetCapacity(itemADO.MAX_CAPACITY);
                                    if (capacity.HasValue)
                                    {
                                        if (dataByBedLogs.Count >= capacity)
                                            itemADO.IsKey = 2;
                                        else
                                            itemADO.IsKey = 1;
                                    }
                                    else
                                        itemADO.IsKey = 1;

                                    itemADO.AMOUNT = dataByBedLogs.Count;
                                    itemADO.AMOUNT_STR = dataByBedLogs.Count + "/" + capacity;
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
                //Viec 63227: dong dich vu giuong chua co giuong thi dien san giuong nguoi benh dang giu
                if (this.sereServADO.BedId == null || this.sereServADO.BedId <= 0)
                {
                    this.FillDefaultBedByHeld();
                }
                this.isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBed_EditValueChanged(object sender, EventArgs e)
        {
            //Viec 63227 (khoa 01412 = 2): danh dau dang kiem giuong de bo dem doi gio khong nap chen vao luc dang hien hop hoi
            bool wasCheckingBed = this.isCheckingBed;
            if (this.useHeldBedDeferredReload)
                this.isCheckingBed = true;
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
                            //Viec 63227: khoa 01412 = 2 thi giuong chua khai bao so nguoi nam toi da coi la 1 nguoi (chi dinh nhieu
                            //nguoi benh cung luc cung phai bi chan nhu Backend). Khoa khac 2 thi bang MAX_CAPACITY nhu cu.
                            long? groupCapacity = HeldBedProcessor.GetCapacity(row.MAX_CAPACITY);
                            //Viec 63227: khoa 01412 = 2 thi giuong dang co nguoi nam (IsKey 1) cung phai du cho moi nguoi benh dang chi dinh
                            //(nhanh else if ben duoi bao giuong da vuot qua suc chua)
                            bool isGroupOverCapacity = HeldBedProcessor.IsHoldBedOn && row.IsKey == 1 && ListTreatment.Count + row.AMOUNT > groupCapacity;
                            if ((row.IsKey == 1 && !isGroupOverCapacity) || (row.IsKey == 0 && !(ListTreatment.Count + row.AMOUNT > groupCapacity)))
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

                                        //Viec 63227: khoa 01412 = 2 thi tinh ca moi nguoi benh dang chi dinh cung giuong
                                        this.spShareCount.Value = row.AMOUNT + (HeldBedProcessor.IsHoldBedOn ? Math.Max(1, ListTreatment.Count) : 1);
                                    }
                                    else if (HeldBedProcessor.IsHoldBedOn && ListTreatment.Count > 1)
                                    {
                                        //Viec 63227: khoa 01412 = 2, giuong trong ma chi dinh nhieu nguoi benh cung giuong thi cac nguoi benh nay nam ghep
                                        this.spShareCount.Value = row.AMOUNT + ListTreatment.Count;
                                    }
                                }
                            }
                            else if (row.IsKey == 2 || (ListTreatment.Count + row.AMOUNT > groupCapacity))
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
            finally
            {
                //Viec 63227: ghi nhan giuong vua kiem (chon giuong, dien san) de nut Luu biet khoang nam giuong da doi hay chua
                if (HeldBedProcessor.IsHoldBedOn)
                    this.SnapshotCheckedBed();
                this.isCheckingBed = wasCheckingBed;
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
                //Viec 63227 (khoa 01412 = 2): nap lai danh sach giuong neu da cu, roi kiem lai giuong da chon neu khoang nam giuong
                //da doi tu luc chon giuong (co the hoi nam ghep, hoi giam so nguoi nam ghep, hoac bao giuong da du nguoi va bo chon)
                if (this.useHeldBedDeferredReload)
                {
                    //Doc khoang truoc: chot phan sua con treo cua o ngay (vd cuon chuot khi o khong co focus) de danh dau danh sach da cu
                    this.GetBedWindowKey();
                    if (this.bedListStale)
                    {
                        this.bedListStale = false;
                        this.ReloadBedList();
                    }
                    this.RecheckSelectedBedBeforeSave();
                }
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                 
                if (dtBedStartTime.EditValue != null && dtBedStartTime.DateTime != DateTime.MinValue
                    && dtBedFinishTime.EditValue != null && dtBedFinishTime.DateTime != DateTime.MinValue
                    && dtBedFinishTime.DateTime <= dtBedStartTime.DateTime)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian kết thúc phải lớn hơn thời gian bắt đầu", "Cảnh báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
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
                //Viec 63227: khoa 01412 = 2 (sau khi mo form) thi chi danh dau danh sach giuong da cu; con lai nap ngay nhu cu
                if (this.useHeldBedDeferredReload)
                    this.bedListStale = true;
                else
                    InitCboBed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtBedFinishTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Viec 63227 (khoa 01412 = 2): chi danh dau danh sach giuong da cu, nap lai khi roi o gio hoac khi bam Luu
                this.bedListStale = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Viec 63227 (khoa 01412 = 2): roi o gio ma danh sach giuong da cu thi nap lai (chi nap danh sach, khong hoi). Luc nay o
        /// ngay da chot phan dang go (MaskBox chot truoc khi DateEdit.Leave chay) nen doc gio khong lam sai gio vua nhap.
        /// </summary>
        private void dtBedTime_LeaveHeldBed(object sender, EventArgs e)
        {
            try
            {
                if (this.bedListStale && !this.isCheckingBed)
                {
                    this.bedListStale = false;
                    this.ReloadBedList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Viec 63227: nap lai danh sach giuong va ghi nhan lan nap co tra cuu loi khong (doc ban ghi nam giuong, nguoi dang
        /// giu giuong, ho so cua chinh nguoi benh). Tra cuu loi thi so nguoi dang nam co the thieu hoac thua.
        /// </summary>
        private void ReloadBedList()
        {
            int heldFailBefore = HeldBedProcessor.HeldLookupFailCount;
            int bedLogFailBefore = this.bedLogLoadFailCount;
            InitCboBed();
            this.lastBedListLoadFailed = HeldBedProcessor.HeldLookupFailCount != heldFailBefore || this.bedLogLoadFailCount != bedLogFailBefore;
        }

        /// <summary>
        /// Khoang nam giuong dang nhap (gio bat dau|gio ket thuc)
        /// </summary>
        private string GetBedWindowKey()
        {
            string start = this.dtBedStartTime.EditValue != null && this.dtBedStartTime.DateTime != DateTime.MinValue ? this.dtBedStartTime.DateTime.ToString("yyyyMMddHHmmss") : "";
            string finish = this.dtBedFinishTime.EditValue != null && this.dtBedFinishTime.DateTime != DateTime.MinValue ? this.dtBedFinishTime.DateTime.ToString("yyyyMMddHHmmss") : "";
            return start + "|" + finish;
        }

        /// <summary>
        /// Viec 63227 (khoa 01412 = 2): ghi nhan giuong dang chon, so nguoi dang nam va khoang nam giuong luc vua kiem
        /// </summary>
        private void SnapshotCheckedBed()
        {
            try
            {
                //Doc khoang truoc: chot phan sua con treo cua o ngay; neu vi vay danh sach giuong thanh cu thi khong ghi khoang de nut
                //Luu nap lai va kiem lai
                string window = this.GetBedWindowKey();
                long bedId = this.cboBed.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(this.cboBed.EditValue.ToString()) : 0;
                HisBedADO row = bedId > 0 && this.dataBedADOs != null ? this.dataBedADOs.FirstOrDefault(o => o.ID == bedId) : null;
                this.checkedBedId = row != null ? row.ID : 0;
                this.checkedBedAmount = row != null ? row.AMOUNT : 0;
                this.checkedBedIsKey = row != null ? row.IsKey : 0;
                this.checkedBedWindow = this.bedListStale ? null : window;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Viec 63227 (khoa 01412 = 2), truoc khi luu: khoang nam giuong da doi tu luc chon giuong thi kiem lai giuong dang chon
        /// theo danh sach giuong moi nhat. Day hon (them nguoi nam hoac thanh du nguoi) thi kiem nhu luc chon giuong: bao giuong
        /// da du nguoi va bo chon, hoac hoi nam ghep va tinh lai so nguoi nam ghep. It nguoi hon thi giu giuong va hoi truoc khi
        /// giam so nguoi nam ghep (anh huong tien giuong; Backend nuot loi doc DB thanh danh sach rong nen may tram khong phan
        /// biet duoc voi "khong co ai nam"). Lan nap co tra cuu loi, hoac gio ket thuc khong sau gio bat dau, thi giu nguyen.
        /// </summary>
        private void RecheckSelectedBedBeforeSave()
        {
            long bedId = this.cboBed.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(this.cboBed.EditValue.ToString()) : 0;
            if (bedId <= 0 || this.lastBedListLoadFailed)
                return;
            //Thieu gio hoac gio ket thuc khong sau gio bat dau: chua kiem lai, kiem tra cua nut Luu se bao truoc
            if (this.dtBedStartTime.EditValue == null || this.dtBedStartTime.DateTime == DateTime.MinValue
                || this.dtBedFinishTime.EditValue == null || this.dtBedFinishTime.DateTime == DateTime.MinValue
                || this.dtBedFinishTime.DateTime <= this.dtBedStartTime.DateTime)
                return;
            if (this.checkedBedId == bedId && this.checkedBedWindow == this.GetBedWindowKey())
                return;
            HisBedADO now = this.dataBedADOs != null ? this.dataBedADOs.FirstOrDefault(o => o.ID == bedId) : null;
            if (now == null)
                return;
            if (this.checkedBedId != bedId || now.AMOUNT > this.checkedBedAmount || now.IsKey > this.checkedBedIsKey)
            {
                this.cboBed_EditValueChanged(this.cboBed, EventArgs.Empty);
            }
            else if (now.AMOUNT < this.checkedBedAmount && this.spShareCount.EditValue != null)
            {
                int assigningCount = this.ListTreatment != null ? Math.Max(1, this.ListTreatment.Count) : 1;
                long shareCount = now.AMOUNT + assigningCount;
                long currentShareCount = (long)this.spShareCount.Value;
                if ((shareCount > 1 && shareCount == currentShareCount) || (shareCount <= 1 && currentShareCount <= 1))
                {
                    //So nguoi nam ghep khong doi: khong hoi
                    this.SnapshotCheckedBed();
                    return;
                }
                string newShareText = shareCount > 1 ? shareCount.ToString() : "không nằm ghép";
                string message = string.Format("Khoảng nằm giường đã đổi: giường {0} còn {1} người bệnh khác nằm cùng. Cập nhật số người nằm ghép từ {2} thành {3}?", now.BED_NAME, now.AMOUNT, this.spShareCount.Value, newShareText);
                if (DevExpress.XtraEditors.XtraMessageBox.Show(message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (shareCount > 1)
                        this.spShareCount.Value = shareCount;
                    else
                        this.spShareCount.EditValue = null;
                }
                this.SnapshotCheckedBed();
            }
        }
	}
}
