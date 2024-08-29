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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.PublicMedicineGeneral.ADO;
using HIS.Desktop.Plugins.PublicMedicineGeneral.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000177.PDO;
using MPS.Processor.Mps000486.PDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineGeneral
{
    public partial class FormPublicMedicineGeneral : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        System.Globalization.CultureInfo cultureLang;
        MOS.EFMODEL.DataModels.V_HIS_ROOM currentRoom;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandleLeft = -1;
        List<ExpMestMediAndMateADO> _ExpMestMediAndMateADOs = new List<ExpMestMediAndMateADO>();

        List<PatientADO> lstPatient = new List<PatientADO>();
        List<Mps000177DAY> Mps000177DAY = new List<Mps000177DAY>();
        List<Mps000177MediMate> Mps000177MediMate = new List<Mps000177MediMate>();
        string departmentName = "";
        Dictionary<long, V_HIS_TREATMENT_BED_ROOM> bedRoomName = new Dictionary<long, V_HIS_TREATMENT_BED_ROOM>();
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private V_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom { get; set; }
        private List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRooms { get; set; }
        List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoomsTmp { get; set; }
        #endregion

        #region Construct
        public FormPublicMedicineGeneral()
        {
            InitializeComponent();
            Resources.ResourceLanguageManager.InitResourceLanguageManager();
            cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
        }

        public FormPublicMedicineGeneral(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            Resources.ResourceLanguageManager.InitResourceLanguageManager();
            cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

            // TODO: Complete member initialization
            this.currentModule = currentModule;
            this.Text = currentModule.text;
        }
        #endregion

        #region Private method
        #region load
        private void FormPublicMedicineGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataPatient();

                InitControlState();

                SetIcon();

                LoadKeysFromlanguage();

                SetPrintTypeToMps();

                SetDefaultValueControl();

                FillDataToCbo();

                Validation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatient()
        {
            try
            {
                WaitingManager.Show();
                ListTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                this.gridControlTreatmentBedRoom.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                treatFilter.ORDER_DIRECTION = "ASC";
                treatFilter.ORDER_FIELD = "TDL_PATIENT_FIRST_NAME";
                treatFilter.IS_IN_ROOM = true;
                long bedRoomId = 0;
                MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                if (data != null)
                    bedRoomId = data.ID;
                treatFilter.BED_ROOM_ID = bedRoomId;

                var rs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    ListTreatmentBedRooms = (List<V_HIS_TREATMENT_BED_ROOM>)rs.OrderBy(p => p.TDL_PATIENT_FIRST_NAME).ToList();
                }
                gridControlTreatmentBedRoom.BeginUpdate();
                gridControlTreatmentBedRoom.DataSource = ListTreatmentBedRooms;
                gridControlTreatmentBedRoom.EndUpdate();
                gridViewTreatmentBedRoom.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkDate.Name)
                        {
                            chkDate.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkServiceReq.Name)
                        {
                            chkServiceReq.Checked = item.VALUE == "1";
                        }
                    }
                }
                else
                {
                    chkDate.Checked = true;
                }
                width6 = layoutControlItem6.Size.Width;
                VisibleTextBoxExpMestCode(!chkServiceReq.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void VisibleTextBoxExpMestCode(bool IsVisible)
        {
            try
            {
                layoutControlItem6.Visibility = IsVisible ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem3.Size = IsVisible ? new Size(width6, layoutControlItem3.Size.Height) : new Size(width6 + layoutControlItem3.Size.Width - 10, layoutControlItem3.Size.Height);
                txtExpMestCode.Text = IsVisible ? txtExpMestCode.Text : null;
                btnFind_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_PUBLIC_MEDICINE_GENERAL__LCI_FROM_TIME",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value(
                     "IVT_LANGUAGE_KEY__FORM_PUBLIC_MEDICINE_GENERAL__LCI_TO_TIME",
                     Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                     cultureLang);
                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                     "IVT_LANGUAGE_KEY__FORM_PUBLIC_MEDICINE_GENERAL__TXT_EXP_MEST_CODE",
                     Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                     cultureLang);
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                     "IVT_LANGUAGE_KEY__FORM_PUBLIC_MEDICINE_GENERAL__TXT_PATIENT_CODE",
                     Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                     cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                dtFromTime.DateTime = DateTime.Now;
                dtToTime.DateTime = DateTime.Now;
                txtExpMestCode.Text = "";
                txtPatientCode.Text = "";
                chkMedicine.Checked = chkBlood.Checked = chkMedical.Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCbo()
        {
            try
            {
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                if (room != null)
                {
                    currentRoom = room;
                    departmentName = room.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidationFromTime();
                ValidationToTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationFromTime()
        {
            try
            {
                Validation.DateValidationRule excuteRoomRule = new Validation.DateValidationRule();
                excuteRoomRule.DateEdit = dtFromTime;
                excuteRoomRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                excuteRoomRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtFromTime, excuteRoomRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationToTime()
        {
            try
            {
                Validation.DateValidationRule excuteRoomRule = new Validation.DateValidationRule();
                excuteRoomRule.DateEdit = dtToTime;
                excuteRoomRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                excuteRoomRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtToTime, excuteRoomRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region event
        private void barButtonItemCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtPatientCode.Text = code;
                        }
                    }
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        string code = txtExpMestCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtExpMestCode.Text = code;
                        }
                    }
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void GetDataByDepartment(long p, ref bool success, ref CommonParam param)
        {
            try
            {
                success = false;
                MOS.Filter.HisBedRoomViewFilter filter = new HisBedRoomViewFilter();
                filter.DEPARTMENT_ID = p;
                var bedroom = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>(Base.GlobaStore.HisBedRoomGetView, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (bedroom != null && bedroom.Count > 0)
                {
                    var bedroomId = bedroom.Select(s => s.ID).ToList();
                    List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
                    foreach (var item in bedroomId)
                    {
                        MOS.Filter.HisTreatmentBedRoomFilter bedFilter = new HisTreatmentBedRoomFilter();
                        bedFilter.BED_ROOM_ID = item;
                        var bed = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(Base.GlobaStore.HisTreatmentBedRoomGetview, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, param);
                        if (bed != null && bed.Count > 0)
                        {
                            treatmentBedRoom.AddRange(bed);
                        }
                    }
                    var listTreatmentId = treatmentBedRoom.Select(s => s.TREATMENT_ID).Distinct().ToList();

                    if (listTreatmentId != null && listTreatmentId.Count > 0)
                    {
                        //Review
                        //List<V_HIS_PRESCRIPTION_1> prescription = new List<V_HIS_PRESCRIPTION_1>();
                        //int start = 0;
                        //int count = listTreatmentId.Count;
                        //while (count > 0)
                        //{
                        //    int limit = (count <= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM) ? count : Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        //    var listSub = listTreatmentId.Skip(start).Take(limit).ToList();

                        //    MOS.Filter.HisPrescriptionView1Filter presFilter = new MOS.Filter.HisPrescriptionView1Filter();
                        //    presFilter.TREATMENT_IDs = listSub;
                        //    if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                        //        presFilter.INTRUCTION_TIME__FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        //            dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");
                        //    if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                        //        presFilter.INTRUCTION_TIME__TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        //            dtToTime.DateTime.ToString("yyyyMMdd") + "235959");
                        //    var prescriptions = new BackendAdapter(param).Get<List<V_HIS_PRESCRIPTION_1>>(Base.GlobaStore.HisPrescriptionGetview1, ApiConsumer.ApiConsumers.MosConsumer, presFilter, param);
                        //    if (prescriptions != null && prescriptions.Count > 0)
                        //    {
                        //        prescription.AddRange(prescriptions);
                        //    }

                        //    start += Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        //    count -= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        //}

                        //if (prescription != null && prescription.Count > 0)
                        //{
                        //    CreateThreadLoadData(prescription);
                        //    PrintProcess();
                        //    success = true;
                        //}
                        //else
                        //{
                        //    success = false;
                        //    param.Messages.Add(Resources.ResourceMessage.KhongCoDonThuoc);
                        //}
                    }
                    else
                    {
                        success = false;
                        param.Messages.Add(Resources.ResourceMessage.KhongCoBenhNhan);
                    }
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.KhoaKhongCoBuongBenh);
                    success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public List<V_HIS_TREATMENT_BED_ROOM> GetSelectedRows()
        {
            List<V_HIS_TREATMENT_BED_ROOM> result = new List<V_HIS_TREATMENT_BED_ROOM>();
            try
            {
                int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var mediMatyTypeADO = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(selectRows[i]);
                        result.Add(mediMatyTypeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        Dictionary<long, HIS_EXP_MEST> dicExpMest = new Dictionary<long, HIS_EXP_MEST>();
        private int width6;
        private List<HIS_EXP_MEST> _ExpMests;

        private void GetDataByFilter(bool IsPrint)
        {
            try
            {
                _ExpMestMediAndMateADOs = new List<ADO.ExpMestMediAndMateADO>();
                MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                _ExpMests = new List<HIS_EXP_MEST>();
                lstPatient = new List<PatientADO>();
                string expMestCode = null;
                string patientCode = null;
                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    expMestCode = txtExpMestCode.Text.Trim();
                    if (expMestCode.Length < 12 && checkDigit(expMestCode))
                    {
                        expMestCode = string.Format("{0:000000000000}", Convert.ToInt64(expMestCode));
                        txtExpMestCode.Text = expMestCode;
                    }
                }
                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    patientCode = txtPatientCode.Text.Trim();
                    if (patientCode.Length < 10 && checkDigit(patientCode))
                    {
                        patientCode = string.Format("{0:0000000000}", Convert.ToInt64(patientCode));
                        txtPatientCode.Text = patientCode;
                    }
                }

                if (IsPrint)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        filter.TDL_PATIENT_CODE__EXACT = txtPatientCode.Text;
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT;//Review
                        if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                            filter.TDL_INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                                dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");

                        if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                            filter.TDL_INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                                dtToTime.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                    else if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        filter.EXP_MEST_CODE__EXACT = txtExpMestCode.Text;
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL;
                    }
                    else
                    {
                        if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                            filter.TDL_INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                                dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");

                        if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                            filter.TDL_INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                                dtToTime.DateTime.ToString("yyyyMMdd") + "000000");
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        filter.EXP_MEST_CODE__EXACT = txtExpMestCode.Text;
                        filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL;
                    }
                }
                if (currentRoom != null)
                {
                    filter.REQ_DEPARTMENT_ID = currentRoom.DEPARTMENT_ID;
                }

                if (IsPrint)
                {
                    CallApiExpMest(filter, true);
                    foreach (var item in _ExpMests)
                    {
                        if (!dicExpMest.ContainsKey(item.ID))
                        {
                            dicExpMest[item.ID] = new HIS_EXP_MEST();
                            dicExpMest[item.ID] = item;
                        }
                    }

                    int start = 0;
                    int count = _ExpMests.Count;
                    while (count > 0)
                    {
                        int limit = (count <= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM) ? count : Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        var listSub = _ExpMests.Skip(start).Take(limit).ToList();
                        CreateThreadLoadData_New(listSub);

                        start += Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        count -= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        
                    }
                    PrintProcess();

                    if (_ExpMests.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoDonThuoc, "Cảnh báo", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    CallApiExpMest(filter, false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallApiExpMest(HisExpMestFilter filter, bool IsPrint)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> treatmentIds = GetSelectedRows().Select(o => o.TREATMENT_ID).ToList();
                var _ExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(Base.GlobaStore.HisExpMestGet, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (_ExpMests != null && _ExpMests.Count > 0)
                {
                    HisExpMestFilter aggFilter = new HisExpMestFilter();
                    aggFilter.AGGR_EXP_MEST_IDs = _ExpMests.Select(o => o.ID).ToList();
                    if (IsPrint)
                    {
                        if (string.IsNullOrEmpty(txtExpMestCode.Text) || !string.IsNullOrEmpty(txtPatientCode.Text))
                        {
                            this._ExpMests.AddRange(_ExpMests.Where(o=>treatmentIds.Exists(p=>p == o.TDL_TREATMENT_ID)).ToList());
                            return;
                        }
                        int start = 0;
                        int count = treatmentIds.Count;
                        while (count > 0)
                        {
                            int limit = (count <= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM) ? count : Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                            var listSub = treatmentIds.Skip(start).Take(limit).ToList();
                            aggFilter.TDL_TREATMENT_IDs = listSub;
                            var _AggExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(Base.GlobaStore.HisExpMestGet, ApiConsumer.ApiConsumers.MosConsumer, aggFilter, param);
                            if (_AggExpMests != null && _AggExpMests.Count > 0)
                            {
                                this._ExpMests.AddRange(_AggExpMests);
                            }
                            start += Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                            count -= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        }
                    }
                    else
                    {
                        var _AggExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(Base.GlobaStore.HisExpMestGet, ApiConsumer.ApiConsumers.MosConsumer, aggFilter, param);
                        if (_AggExpMests != null && _AggExpMests.Count > 0)
                        {
                            this._ExpMests.AddRange(_AggExpMests);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Trong Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData_New(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));
            Thread threadBlood = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataBloodNewThread));
            Thread threadPatient = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataPatientNewThread));
            Thread threadTreatmentBedRoom = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataTreatmentBedRoomNewThread));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadBlood.Start(param);
                threadPatient.Start(param);
                threadTreatmentBedRoom.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
                threadBlood.Join();
                threadPatient.Join();
                threadTreatmentBedRoom.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadBlood.Abort();
                threadPatient.Abort();
                threadTreatmentBedRoom.Abort();
            }
        }

        private void LoadDataMedicineNewThread(object param)
        {
            try
            {
                LoadDataMedicine(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Load V_HIS_EXP_MEST_MEDICINE start");
                List<HIS_EXP_MEST> _expMests = (List<HIS_EXP_MEST>)data;

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter expMestMediFilter = new HisExpMestMedicineViewFilter();
                expMestMediFilter.EXP_MEST_IDs = _expMests.Select(p => p.ID).Distinct().ToList();
                var _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMediFilter, param);


                var _MedicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var expMestMedicineGroups = _ExpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.PRICE, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                foreach (var itemGroups in expMestMedicineGroups)
                {
                    if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                    {
                        //V_HIS_MEDICINE_TYPE mediType = (_MedicineTypes != null && _MedicineTypes.Count > 0) ? _MedicineTypes.FirstOrDefault(p => p.ID == itemGroups[0].MEDICINE_TYPE_ID) : null;

                        var expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                        if (expMest != null)//mediType != null && expMest != null)
                        {
                            ExpMestMediAndMateADO expMedi = new ExpMestMediAndMateADO();
                            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, ExpMestMediAndMateADO>();
                            expMedi = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);
                            //expMedi.MEDICINE_TYPE_NAME = mediType.MEDICINE_TYPE_NAME;
                            //expMedi.MEDICINE_TYPE_CODE = mediType.MEDICINE_TYPE_CODE;
                            //expMedi.SERVICE_UNIT_CODE = mediType.SERVICE_UNIT_CODE;
                            //expMedi.SERVICE_UNIT_NAME = mediType.SERVICE_UNIT_NAME;
                            expMedi.INTRUCTION_DATE = expMest.TDL_INTRUCTION_DATE ?? 0;
                            expMedi.INTRUCTION_TIME = expMest.TDL_INTRUCTION_TIME ?? 0;

                            expMedi.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                            expMedi.AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                            expMedi.TREATMENT_ID = expMest.TDL_TREATMENT_ID ?? 0;
                            _ExpMestMediAndMateADOs.Add(expMedi);
                        }

                    }
                }

                Inventec.Common.Logging.LogSystem.Info("Loaded V_HIS_EXP_MEST_MEDICINE end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialNewThread(object param)
        {
            try
            {
                LoadDataMaterial(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Load V_HIS_EXP_MEST_MATERIAL start");
                List<HIS_EXP_MEST> _expMests = (List<HIS_EXP_MEST>)data;

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter expMestMateFilter = new HisExpMestMaterialViewFilter();
                expMestMateFilter.EXP_MEST_IDs = _expMests.Select(p => p.ID).ToList();
                var _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMateFilter, param);


                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var _MaterialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();

                    var expMestMaterialGroups = _ExpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.PRICE, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in expMestMaterialGroups)
                    {
                        if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                        {
                            //V_HIS_MATERIAL_TYPE mediType = (_MaterialTypes != null && _MaterialTypes.Count > 0) ? _MaterialTypes.FirstOrDefault(p => p.ID == itemGroups[0].MATERIAL_TYPE_ID) : null;

                            var expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                            if (expMest != null)
                            {
                                ExpMestMediAndMateADO expMate = new ExpMestMediAndMateADO();
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, ExpMestMediAndMateADO>();
                                expMate = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);
                                expMate.MEDICINE_TYPE_NAME = itemGroups[0].MATERIAL_TYPE_NAME;
                                expMate.MEDICINE_TYPE_CODE = itemGroups[0].MATERIAL_TYPE_CODE;
                                expMate.MEDICINE_TYPE_ID = itemGroups[0].MATERIAL_TYPE_ID;
                                expMate.MEDICINE_ID = itemGroups[0].MATERIAL_ID;
                                //expMate.SERVICE_UNIT_CODE = mediType.SERVICE_UNIT_CODE;
                                //expMate.SERVICE_UNIT_NAME = mediType.SERVICE_UNIT_NAME;
                                expMate.INTRUCTION_DATE = expMest.TDL_INTRUCTION_DATE ?? 0;
                                expMate.INTRUCTION_TIME = expMest.TDL_INTRUCTION_TIME ?? 0;

                                expMate.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                                expMate.AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                expMate.IS_CHEMICAL_SUBSTANCE = itemGroups[0].IS_CHEMICAL_SUBSTANCE;
                                expMate.TREATMENT_ID = expMest.TDL_TREATMENT_ID ?? 0;
                                _ExpMestMediAndMateADOs.Add(expMate);
                            }
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("Loaded V_HIS_EXP_MEST_MATERIAL end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodNewThread(object param)
        {
            try
            {
                LoadDataBlood(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBlood(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Load V_HIS_EXP_MEST_BLOOD start");
                List<HIS_EXP_MEST> _expMests = (List<HIS_EXP_MEST>)data;

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_IDs = _expMests.Select(p => p.ID).ToList();
                var _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);

                if (_ExpMestBloods != null && _ExpMestBloods.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestBllodGroups = _ExpMestBloods.GroupBy(p => new { p.BLOOD_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in expMestBllodGroups)
                    {
                        if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID))
                        {
                            var expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID];
                            if (expMest != null)
                            {
                                ExpMestMediAndMateADO expMestBlood = new ExpMestMediAndMateADO();
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD, ExpMestMediAndMateADO>();
                                expMestBlood = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);
                                expMestBlood.AMOUNT = itemGroups.Count;
                                expMestBlood.PRICE = itemGroups[0].PRICE;
                                expMestBlood.MEDICINE_TYPE_ID = itemGroups[0].BLOOD_TYPE_ID;
                                expMestBlood.MEDICINE_ID = itemGroups[0].BLOOD_ID;
                                expMestBlood.INTRUCTION_DATE = expMest.TDL_INTRUCTION_DATE ?? 0;
                                expMestBlood.INTRUCTION_TIME = expMest.TDL_INTRUCTION_TIME ?? 0;

                                expMestBlood.MEDICINE_TYPE_CODE = itemGroups[0].BLOOD_TYPE_CODE;
                                expMestBlood.MEDICINE_TYPE_NAME = itemGroups[0].BLOOD_TYPE_NAME; ;
                                expMestBlood.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                                expMestBlood.TREATMENT_ID = expMest.TDL_TREATMENT_ID ?? 0;
                                _ExpMestMediAndMateADOs.Add(expMestBlood);
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("Loaded V_HIS_EXP_MEST_BLOOD end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatientNewThread(object param)
        {
            try
            {
                LoadDataPatient(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataPatient(object data)
        {
            try
            {
                List<HIS_EXP_MEST> _expMests = (List<HIS_EXP_MEST>)data;
                MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.IDs = _expMests.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                var treatments = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>(Base.GlobaStore.HisTreatmentGetView, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (treatments != null && treatments.Count > 0)
                {
                    var patient_ids = treatments.Select(s => s.PATIENT_ID).ToList();
                    MOS.Filter.HisPatientViewFilter paFilter = new HisPatientViewFilter();
                    filter.IDs = patient_ids;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(Base.GlobaStore.HisPatientGetview, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        foreach (var item in patients)
                        {
                            PatientADO ado = new PatientADO(item);
                            var treat = treatments.FirstOrDefault(o => o.PATIENT_ID == item.ID);
                            ado.treatment_id = treat.ID;
                            PatientADOadd(ado, treat);
                            lstPatient.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentBedRoomNewThread(object param)
        {
            try
            {
                LoadDataTreatmentBedRoom(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentBedRoom(object data)
        {
            try
            {
                List<HIS_EXP_MEST> _expMests = (List<HIS_EXP_MEST>)data;

                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_IDs = _expMests.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                var bedRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>(Base.GlobaStore.HisTreatmentBedRoomGetview, ApiConsumer.ApiConsumers.MosConsumer, bedFilter, null);
                if (bedRooms != null && bedRooms.Count > 0)
                {
                    bedRooms = bedRooms.OrderByDescending(o => o.CREATE_TIME).ToList();
                    foreach (var bed in bedRooms)
                    {
                        if (!bedRoomName.ContainsKey(bed.TREATMENT_ID))
                            bedRoomName.Add(bed.TREATMENT_ID, bed);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PatientADOadd(PatientADO ado, V_HIS_TREATMENT data)
        {
            try
            {
                if (data != null && ado != null)
                {
                    ado.CLINICAL_IN_TIME = data.CLINICAL_IN_TIME;
                    ado.DATA_STORE_CODE = data.DATA_STORE_CODE;
                    ado.DATA_STORE_ID = data.DATA_STORE_ID;
                    ado.DATA_STORE_NAME = data.DATA_STORE_NAME;
                    ado.END_DEPARTMENT_CODE = data.END_DEPARTMENT_CODE;
                    ado.END_DEPARTMENT_ID = data.END_DEPARTMENT_ID;
                    ado.END_DEPARTMENT_NAME = data.END_DEPARTMENT_NAME;
                    ado.END_LOGINNAME = data.END_LOGINNAME;
                    //ado.END_ORDER = data.END_ORDER;
                    //ado.END_ORDER_REQUEST = data.END_ORDER_REQUEST;
                    ado.END_ROOM_CODE = data.END_ROOM_CODE;
                    ado.END_ROOM_ID = data.END_ROOM_ID;
                    ado.END_ROOM_NAME = data.END_ROOM_NAME;
                    ado.END_USERNAME = data.END_USERNAME;
                    ado.FEE_LOCK_DEPARTMENT_ID = data.FEE_LOCK_DEPARTMENT_ID;
                    ado.FEE_LOCK_ORDER = data.FEE_LOCK_ORDER;
                    ado.FEE_LOCK_ROOM_ID = data.FEE_LOCK_ROOM_ID;
                    ado.FEE_LOCK_TIME = data.FEE_LOCK_TIME;
                    ado.ICD_CODE = data.ICD_CODE;
                    //ado.ICD_ID = data.ICD_ID;
                    ado.ICD_MAIN_TEXT = data.ICD_NAME;
                    //ado.ICD_NAME = data.ICD_NAME;
                    ado.ICD_SUB_CODE = data.ICD_SUB_CODE;
                    ado.ICD_TEXT = data.ICD_TEXT;
                    ado.IN_CODE = data.IN_CODE;
                    ado.IN_DEPARTMENT_ID = data.IN_DEPARTMENT_ID;
                    //ado.IN_ORDER = data.IN_ORDER;
                    //ado.IN_ORDER_REQUEST = data.IN_ORDER_REQUEST;
                    ado.IN_ROOM_ID = data.IN_ROOM_ID;
                    ado.IN_TIME = data.IN_TIME;
                    //ado.LOCK_TIME = data.LOCK_TIME;
                    ado.OUT_TIME = data.OUT_TIME;
                    ado.OWE_MODIFY_TIME = data.OWE_MODIFY_TIME;
                    ado.OWE_TYPE_CODE = data.OWE_TYPE_CODE;
                    ado.OWE_TYPE_ID = data.OWE_TYPE_ID;
                    ado.OWE_TYPE_NAME = data.OWE_TYPE_NAME;
                    ado.PROGRAM_ID = data.PROGRAM_ID;
                    ado.STORE_CODE = data.STORE_CODE;
                    ado.STORE_TIME = data.STORE_TIME;
                    ado.TREATMENT_CODE = data.TREATMENT_CODE;
                    //ado.TREATMENT_END_NO_ID = data.TREATMENT_END_NO_ID;
                    ado.TREATMENT_END_TYPE_CODE = data.TREATMENT_END_TYPE_CODE;
                    ado.TREATMENT_END_TYPE_ID = data.TREATMENT_END_TYPE_ID;
                    ado.TREATMENT_END_TYPE_NAME = data.TREATMENT_END_TYPE_NAME;
                    ado.TREATMENT_RESULT_CODE = data.TREATMENT_RESULT_CODE;
                    ado.TREATMENT_RESULT_ID = data.TREATMENT_RESULT_ID;
                    ado.TREATMENT_RESULT_NAME = data.TREATMENT_RESULT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleLeft == -1)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleLeft > edit.TabIndex)
                {
                    positionHandleLeft = edit.TabIndex;
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
        #endregion

        #region print
        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps000177PDO.printTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case Mps000177PDO.printTypeCode:
                        PrintMps177(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000486":
                        PrintMps486(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
            return result;
        }
        
            
        private void PrintMps486(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                List<HIS_SERVICE_REQ> listserviceReqs = new List<HIS_SERVICE_REQ>();
                List<V_HIS_SERE_SERV_2> listSereServView2 = new List<V_HIS_SERE_SERV_2>();
                List<HIS_SERVICE_REQ_METY> listServiceReqMety = new List<HIS_SERVICE_REQ_METY>();
                List<HIS_SERVICE_REQ_MATY> listServiceReqMaty = new List<HIS_SERVICE_REQ_MATY>();
                Mps000486ADO ado = new Mps000486ADO();
                ado.ADO_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                     Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                ado.ADO_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                       Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                CommonParam paramCommon = new CommonParam();
                var treatmentIds = GetSelectedRows().Select(o => o.TREATMENT_ID).ToList();
                int start = 0;
                int count = treatmentIds.Count;
                while (count > 0)
                {
                    int limit = (count <= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM) ? count : Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                    var listSub = treatmentIds.Skip(start).Take(limit).ToList();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_IDs = listSub;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                    filter.INTRUCTION_DATE_FROM = ado.ADO_TIME_FROM;
                    filter.INTRUCTION_DATE_TO = ado.ADO_TIME_TO;
                    var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        listserviceReqs.AddRange(serviceReqs);
                        int startSr = 0;
                        int countSr = serviceReqs.Count;
                        while (countSr > 0)
                        {
                            int limitSr = (countSr <= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM) ? countSr : Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                            var listSubSr = serviceReqs.Skip(startSr).Take(limitSr).ToList();
                            HisSereServView2Filter SereServView2Filter = new HisSereServView2Filter();
                            SereServView2Filter.SERVICE_REQ_IDs = listSubSr.Select(o => o.ID).ToList();
                            var SereServView2 = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_2>>("api/HisSereServ/GetView2", ApiConsumers.MosConsumer, SereServView2Filter, paramCommon);
                            if (SereServView2 != null && SereServView2.Count > 0)
                            {
                                listSereServView2.AddRange(SereServView2);
                            }
                            HisServiceReqMetyFilter ServiceReqMetyFilter = new HisServiceReqMetyFilter();
                            ServiceReqMetyFilter.SERVICE_REQ_IDs = listSubSr.Select(o => o.ID).ToList();
                            var ServiceReqMety = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, ServiceReqMetyFilter, paramCommon);
                            if (ServiceReqMety != null && ServiceReqMety.Count > 0)
                            {
                                listServiceReqMety.AddRange(ServiceReqMety);
                            }
                            HisServiceReqMatyFilter ServiceReqMatyFilter = new HisServiceReqMatyFilter();
                            ServiceReqMatyFilter.SERVICE_REQ_IDs = listSubSr.Select(o => o.ID).ToList();
                            var ServiceReqMaty = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, ServiceReqMatyFilter, paramCommon);
                            if (ServiceReqMaty != null && ServiceReqMaty.Count > 0)
                            {
                                listServiceReqMaty.AddRange(ServiceReqMaty);
                            }
                            startSr += Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                            countSr -= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                        }
                    }
                    start += Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                    count -= Base.GlobaStore.MAX_REQUEST_LENGTH_PARAM;
                }
                if (listserviceReqs == null || listserviceReqs.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongCoYLenh, "Cảnh báo", MessageBoxButtons.OK);
                    return;
                }
                Mps000486PDO pdo = new Mps000486PDO(
                                listserviceReqs,
                                listSereServView2,
                                listServiceReqMety,
                                listServiceReqMaty,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                                ado
                                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null);
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
        private void PrintMps177(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                AddDataToDatetime();
                List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterial = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_BLOOD> listExpMestBlood = new List<V_HIS_EXP_MEST_BLOOD>();
                CommonParam param = new CommonParam();
                var treatmentIds = bedRoomName.Select(s=>s.Value).Select(o=>o.TREATMENT_ID).ToList();

                if (chkMedicine.Checked)
                {
                    if(_ExpMestMediAndMateADOs != null)
                    {
                        var listThuoc = _ExpMestMediAndMateADOs.Where(s => s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                        Mapper.CreateMap<ExpMestMediAndMateADO,V_HIS_EXP_MEST_MEDICINE >();
                        listExpMestMedicine = Mapper.Map<List<ExpMestMediAndMateADO>, List<V_HIS_EXP_MEST_MEDICINE>>(listThuoc);

                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MEDICINE>(listExpMestMedicine, _ExpMestMediAndMateADOs.Where(s=>s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC));
                    }
                }
                if (chkMedical.Checked)
                {
                    if (_ExpMestMediAndMateADOs != null)
                    {
                        var listThuoc = _ExpMestMediAndMateADOs.Where(s => s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                        foreach(var expMaterial in listThuoc)
                        {
                            V_HIS_EXP_MEST_MATERIAL expMate = new V_HIS_EXP_MEST_MATERIAL();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MATERIAL>(expMate, expMaterial);
                            expMate.MATERIAL_TYPE_NAME = expMaterial.MEDICINE_TYPE_NAME;
                            expMate.MATERIAL_TYPE_CODE = expMaterial.MEDICINE_TYPE_CODE;
                            expMate.TDL_MATERIAL_TYPE_ID = expMaterial.MEDICINE_TYPE_ID;
                            expMate.MATERIAL_ID = expMaterial.MEDICINE_ID;
                            expMate.TDL_INTRUCTION_DATE = expMaterial.INTRUCTION_DATE;
                            expMate.TDL_INTRUCTION_TIME = expMaterial.INTRUCTION_TIME;
                            expMate.TDL_TREATMENT_ID = expMaterial.TREATMENT_ID;
                            
                            listExpMestMaterial.Add(expMate);
                        }
                        //Mapper.CreateMap<ExpMestMediAndMateADO, V_HIS_EXP_MEST_MATERIAL>();
                        //listExpMestMaterial = Mapper.Map<List<ExpMestMediAndMateADO>, List<V_HIS_EXP_MEST_MATERIAL>>(listThuoc);
                        ////Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_MATERIAL>(listExpMestMaterial, _ExpMestMediAndMateADOs.Where(s => s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
                    }
                }
                if (chkBlood.Checked)
                {
                    if (_ExpMestMediAndMateADOs != null)
                    {
                        var listThuoc = _ExpMestMediAndMateADOs.Where(s => s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                        foreach (var expMaterial in listThuoc)
                        {
                            V_HIS_EXP_MEST_BLOOD expMate = new V_HIS_EXP_MEST_BLOOD();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_BLOOD>(expMate, expMaterial);
                            expMate.BLOOD_TYPE_NAME = expMaterial.MEDICINE_TYPE_NAME;
                            expMate.BLOOD_TYPE_CODE = expMaterial.MEDICINE_TYPE_CODE;
                            expMate.TDL_BLOOD_TYPE_ID = expMaterial.MEDICINE_TYPE_ID;
                            expMate.BLOOD_ID = expMaterial.MEDICINE_ID??0;
                            expMate.TDL_INTRUCTION_DATE = expMaterial.INTRUCTION_DATE;
                            expMate.TDL_INTRUCTION_TIME = expMaterial.INTRUCTION_TIME;
                            expMate.TDL_TREATMENT_ID = expMaterial.TREATMENT_ID;
                            listExpMestBlood.Add(expMate);
                        }

                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST_BLOOD>(listExpMestBlood, _ExpMestMediAndMateADOs.Where(s => s.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU));
                    }
                }
                long config_day_size = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.PHIEU_CONG_KHAI_THUOC.DAY_SIZE"); //BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY == "HIS.PHIEU_CONG_KHAI_THUOC.DAY_SIZE").FirstOrDefault();
                
                Mps000177PDO pdo = new Mps000177PDO(
                    lstPatient,
                    Mps000177DAY,
                    Mps000177MediMate,
                    departmentName,
                    bedRoomName,
                    listExpMestMedicine,
                    listExpMestMaterial,
                    listExpMestBlood,
                    config_day_size
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null);
                }

                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddDataToDatetime()
        {
            try
            {
                if (_ExpMestMediAndMateADOs != null && _ExpMestMediAndMateADOs.Count > 0)
                {
                    var groups = _ExpMestMediAndMateADOs.GroupBy(g => new { g.TREATMENT_ID }).ToList();
                    Mps000177DAY = new List<Mps000177DAY>();
                    Mps000177MediMate = new List<Mps000177MediMate>();
                    foreach (var gr in groups)
                    {
                        List<string> distinctDates = gr
                        .Select(o => o.INTRUCTION_TIME.ToString().Substring(0, 8))
                        .Distinct().OrderBy(t => t).ToList();
                        var sereServGroups = gr;
                        int index = 0;
                        #region ThuatToan
                        while (index < distinctDates.Count)
                        {
                            Mps000177DAY sdo = new Mps000177DAY();
                            sdo.treatment_id = gr.First().TREATMENT_ID;
                            sdo.Day1 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day2 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day3 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day4 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day5 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day6 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day7 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day8 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day9 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day10 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day11 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day12 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day13 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day14 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day15 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day16 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day17 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day18 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day19 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day20 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day21 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day22 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day23 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            sdo.Day24 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                            foreach (var group in sereServGroups)
                            {
                                Mps000177MediMate sereServPrint = new Mps000177MediMate();
                                List<ExpMestMediAndMateADO> sereServs = new List<ExpMestMediAndMateADO>();
                                sereServs.Add(group);
                                Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, Mps000177MediMate>();
                                sereServPrint = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, Mps000177MediMate>(group);
                                var amount = sereServs.FirstOrDefault().AMOUNT.ToString();
                                sereServPrint.type = group.type;
                                sereServPrint.treatment_id = group.TREATMENT_ID;
                                sereServPrint.Day1 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day1).Any() ? amount : "";
                                sereServPrint.Day2 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day2).Any() ? amount : "";
                                sereServPrint.Day3 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day3).Any() ? amount : "";
                                sereServPrint.Day4 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day4).Any() ? amount : "";
                                sereServPrint.Day5 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day5).Any() ? amount : "";
                                sereServPrint.Day6 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day6).Any() ? amount : "";
                                sereServPrint.Day7 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day7).Any() ? amount : "";
                                sereServPrint.Day8 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day8).Any() ? amount : "";
                                sereServPrint.Day9 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day9).Any() ? amount : "";
                                sereServPrint.Day10 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day10).Any() ? amount : "";
                                sereServPrint.Day11 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day11).Any() ? amount : "";
                                sereServPrint.Day12 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day12).Any() ? amount : "";
                                sereServPrint.Day13 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day13).Any() ? amount : "";
                                sereServPrint.Day14 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day14).Any() ? amount : "";
                                sereServPrint.Day15 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day15).Any() ? amount : "";
                                sereServPrint.Day16 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day16).Any() ? amount : "";
                                sereServPrint.Day17 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day17).Any() ? amount : "";
                                sereServPrint.Day18 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day18).Any() ? amount : "";
                                sereServPrint.Day19 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day19).Any() ? amount : "";
                                sereServPrint.Day20 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day20).Any() ? amount : "";
                                sereServPrint.Day21 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day21).Any() ? amount : "";
                                sereServPrint.Day22 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day22).Any() ? amount : "";
                                sereServPrint.Day23 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day23).Any() ? amount : "";
                                sereServPrint.Day24 = sereServs.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.INTRUCTION_TIME) == sdo.Day24).Any() ? amount : "";
                                Mps000177MediMate.Add(sereServPrint);
                            }
                            #endregion
                            //sdo.Mps000177MediMateADOs = Mps000177MediMate;
                            Mps000177DAY.Add(sdo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                lstTreatmentBedRoomsTmp = new List<V_HIS_TREATMENT_BED_ROOM>();
                if (ListTreatmentBedRooms != null && ListTreatmentBedRooms.Count > 0)
                {
                    if (!string.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtPatientCode.Text = code;
                        }
                        lstTreatmentBedRoomsTmp = ListTreatmentBedRooms.Where(o => o.TDL_PATIENT_CODE.Contains(txtPatientCode.Text.Trim())).ToList();
                    }
                    else if (!string.IsNullOrEmpty(txtExpMestCode.Text.Trim()))
                    {
                        GetDataByFilter(false);
                        if (_ExpMests != null && _ExpMests.Count > 0)
                            lstTreatmentBedRoomsTmp = ListTreatmentBedRooms.Where(o => _ExpMests.Select(p => p.TDL_TREATMENT_ID).Distinct().ToList().Exists(p => p == o.TREATMENT_ID)).ToList();
                    }
                    else
                    {
                        lstTreatmentBedRoomsTmp = ListTreatmentBedRooms;
                    }
                    gridControlTreatmentBedRoom.BeginUpdate();
                    gridControlTreatmentBedRoom.DataSource = lstTreatmentBedRoomsTmp;
                    gridControlTreatmentBedRoom.EndUpdate();
                    gridViewTreatmentBedRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleLeft = -1;
                if (!dxValidationProvider1.Validate()) return;
                int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();
                if (!(selectRows != null && selectRows.Count() > 0))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaChonBenhNhan, "Cảnh báo", MessageBoxButtons.OK);
                    return;
                }

                if (chkDate.Checked)
                    GetDataByFilter(true);
                else
                    Print486();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Print486()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000486", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = (V_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TIME_str")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatmentBedRoom.IN_TIME);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkServiceReq_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                VisibleTextBoxExpMestCode(!chkServiceReq.Checked);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkServiceReq.Name && o.MODULE_LINK == ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkServiceReq.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkServiceReq.Name;
                    csAddOrUpdate.VALUE = (chkServiceReq.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkDate.Name && o.MODULE_LINK == ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkDate.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkDate.Name;
                    csAddOrUpdate.VALUE = (chkDate.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExpMestCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
