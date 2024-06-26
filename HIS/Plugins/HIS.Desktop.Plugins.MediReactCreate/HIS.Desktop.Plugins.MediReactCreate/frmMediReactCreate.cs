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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MediReactCreate.Resources;
using HIS.Desktop.Plugins.MediReactCreate.Validation;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediReactCreate
{

    public partial class frmMediReactCreate : HIS.Desktop.Utility.FormBase
    {
        CommonParam param = new CommonParam();
        Inventec.Desktop.Common.Modules.Module moduleData;
        MediReactCreateADO data;
        List<MEDITYPE> glstMediType = new List<MEDITYPE>();
        List<MEDITYPE> glstMediTypeSort = new List<MEDITYPE>();
        List<V_HIS_MEDICINE> glstMedicine = new List<V_HIS_MEDICINE>();
        List<V_HIS_MEDI_REACT> ListMediReact = new List<V_HIS_MEDI_REACT>();
        int positionHandleControl = -1;
        internal int ActionType = GlobalVariables.ActionAdd;
        int positionHandle = -1;
        long idCombo = 0;
        long IDUpdate = 0;
        public frmMediReactCreate(Inventec.Desktop.Common.Modules.Module moduleData, MediReactCreateADO data)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.data = data;

        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            bool notHandler = false;
            try
            {

                V_HIS_MEDI_REACT dataMediReact = (V_HIS_MEDI_REACT)gridView1.GetFocusedRow();
                HIS_MEDI_REACT datasend = new HIS_MEDI_REACT();
                datasend.ID = dataMediReact.ID;
                if (dataMediReact != null)
                {
                    if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();

                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_MEDI_REACT_DELETE, ApiConsumers.MosConsumer, datasend, null);
                        //HisTreatmentLog.HisTreatmentLogLogic(param).DepartmentDelete<bool>(dataTreatmentLog.HisDepartmentTran.ID);
                        WaitingManager.Hide();
                        if (success) { Loaddatatogrid(); }


                        WaitingManager.Hide();


                    }
                    else
                    {
                        notHandler = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }

            if (!notHandler)
            {
                #region Show message
                MessageManager.Show(this, param, success);
                param = new CommonParam();
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
        }

        private void frmMediReactCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIcon();
                SetDefaultControl();
                Loaddatatogrid();
                LoadDatatoComboMedi(this.cboMedicineType);
                LoadDatatoCombo(this.cboMedicineType, this.cboMediReact, this.cboReqUsername, this.cboExeUsername, this.cboCheckUsername);
                //SetCaptionByLanguageKey();

                ValidControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediReactCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MediReactCreate.frmMediReactCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmMediReactCreate.STT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Delete.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.MedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.MedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.MediReactType.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.MediReactType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CheckResult.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.CheckResult.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.RequestUsername.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.RequestUsername.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Executor.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.Executor.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CheckUsername.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.CheckUsername.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ExeTime.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.ExeTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CheckTime.Caption = Inventec.Common.Resource.Get.Value("frmMediReactCreate.CheckTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediReact.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMediReactCreate.cboMediReact.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMedicineType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMediReactCreate.cboMedicineType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboReqUsername.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMediReactCreate.cboReqUsername.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExeUsername.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMediReactCreate.cboExeUsername.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCheckUsername.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMediReactCreate.cboCheckUsername.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMediReactCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidationSingleControl(txtMedicineName);
                ValidationSingleControl(cboReqUsername);
                ValidationSingleControl(txtMediReact);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                this.dateEdit1.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                this.dateEdit2.DateTime = DateTime.Now;
                cboMedicineType.EditValue = cboMediReact.EditValue = cboReqUsername.EditValue = cboExeUsername.EditValue = cboCheckUsername.EditValue = null;
                txtMedicine.Text = "";
                txtMedicineName.Text = "";
                txtMedicineName.Enabled = true;
                txtPackage.Text = "";
                txtPackage.Enabled = true;
                txtExpiredDate.EditValue = null;
                txtExpiredDate.Enabled = true;
                txtMediReact.Text = "";
                textEdit1.Text = "";
                txtMediReact.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDatatoCombo(
            DevExpress.XtraEditors.GridLookUpEdit lookUpEdit1,
            DevExpress.XtraEditors.GridLookUpEdit gridLookUpEdit1,
            DevExpress.XtraEditors.GridLookUpEdit lookUpEdit5,
            DevExpress.XtraEditors.GridLookUpEdit lookUpEdit6,
            DevExpress.XtraEditors.GridLookUpEdit lookUpEdit7)
        {
            try
            {
                WaitingManager.Show();
                //Tên thuốc
                //HisExpMestFilter expMestfilter = new HisExpMestFilter();
                //expMestfilter.TDL_TREATMENT_ID = this.data.treatmentId;
                //expMestfilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                //var _ExpMests = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                //List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                //if (_ExpMests != null && _ExpMests.Count > 0)
                //{
                //    List<V_HIS_EXP_MEST_MEDICINE> dataAlls = new List<V_HIS_EXP_MEST_MEDICINE>();
                //    HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                //    mediFilter.EXP_MEST_IDs = _ExpMests.Select(p => p.ID).ToList();
                //    dataAlls = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(
                //              "api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                //    if (dataAlls != null && dataAlls.Count > 0)
                //    {
                //        var dataGroups = dataAlls.GroupBy(p => new { p.MEDICINE_ID, p.TDL_MEDICINE_TYPE_ID }).Select(p => p.ToList()).ToList();
                //        foreach (var item in dataGroups)
                //        {
                //            V_HIS_EXP_MEST_MEDICINE ado = new V_HIS_EXP_MEST_MEDICINE();
                //            ado = item[0];
                //            _ExpMestMedicines.Add(ado);
                //        }
                //        _ExpMestMedicines = _ExpMestMedicines.OrderBy(p => p.MEDICINE_TYPE_NAME).ToList();
                //    }
                //}

                //lookUpEdit1.Properties.DataSource 
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "MEDICINE_ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboMedicineType, _ExpMestMedicines, controlEditorADO);

                // phápPhương
                List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
                columnInfos2.Add(new ColumnInfo("MEDI_REACT_TYPE_CODE", "", 100, 1));
                columnInfos2.Add(new ColumnInfo("MEDI_REACT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO1 = new ControlEditorADO("MEDI_REACT_TYPE_NAME", "ID", columnInfos2, false, 350);
                ControlEditorLoader.Load(cboMediReact, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE>(), controlEditorADO1);


                //Người chỉ định

                HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                // filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.IS_ACTIVE = 1;
                filter.ROOM_ID = this.moduleData.RoomId;
                var thisUserRoom = new BackendAdapter(new CommonParam()).Get<List<V_HIS_USER_ROOM>>(HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, null);
                List<string> _loginNames = new List<string>();
                _loginNames = thisUserRoom.Select(p => p.LOGINNAME).ToList();
                var data1 = BackendDataWorker.Get<ACS_USER>().Where(o => _loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).OrderBy(p => p.LOGINNAME).ToList();

                List<ColumnInfo> columnInfos3 = new List<ColumnInfo>();
                columnInfos3.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos3.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO3 = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos3, false, 350);
                ControlEditorLoader.Load(cboReqUsername,
                    data1, controlEditorADO3);

                //Người thử
                //lookUpEdit6.Properties.DataSource = BackendDataWorker.Get<ACS_USER>().Where(o => thisUserRoom.Select(p => p.LOGINNAME).Contains(o.LOGINNAME));
                // var data2 = BackendDataWorker.Get<ACS_USER>().Where(o => thisUserRoom.Select(p => p.LOGINNAME).Contains(o.LOGINNAME)).ToList();
                List<ColumnInfo> columnInfos4 = new List<ColumnInfo>();
                columnInfos4.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos4.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO4 = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos4, false, 350);
                ControlEditorLoader.Load(cboExeUsername,
                    data1, controlEditorADO4);
                //Người đọc
                //lookUpEdit7.Properties.DataSource = BackendDataWorker.Get<ACS_USER>().Where(o => thisUserRoom.Select(p => p.LOGINNAME).Contains(o.LOGINNAME));
                //var data3 = BackendDataWorker.Get<ACS_USER>().Where(o => thisUserRoom.Select(p => p.LOGINNAME).Contains(o.LOGINNAME)).ToList();
                List<ColumnInfo> columnInfos5 = new List<ColumnInfo>();
                columnInfos5.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos5.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO5 = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos5, false, 350);
                ControlEditorLoader.Load(cboCheckUsername,
                    data1, controlEditorADO5);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<MEDITYPE> FillDataCombo()
        {
            try
            {
                List<MEDITYPE> glstMedi = new List<MEDITYPE>();
                ////Thuốc trong kho
                HisExpMestMedicineView6Filter filter = new HisExpMestMedicineView6Filter();
                filter.TDL_TREATMENT_ID = this.data.treatmentId;

                List<V_HIS_EXP_MEST_MEDICINE_6> expMestMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_6>>("api/HisExpMestMedicine/GetView6", ApiConsumers.MosConsumer, filter, null);

                foreach (V_HIS_EXP_MEST_MEDICINE_6 exp in expMestMedicine)
                {
                    MEDITYPE medi = new MEDITYPE();
                    medi.INSTRUCTION_TIME = exp.TDL_INTRUCTION_TIME;
                    medi.MEDICINE_ID = exp.MEDICINE_ID;
                    medi.MEDICINE_TYPE_CODE = exp.MEDICINE_TYPE_CODE;
                    medi.MEDICINE_TYPE_NAME = exp.MEDICINE_TYPE_NAME;
                    medi.AMOUNT = exp.AMOUNT;
                    medi.SPEED = exp.SPEED;
                    medi.PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                    medi.SERVICE_UNIT_ID = exp.SERVICE_UNIT_ID;
                    medi.SERVICE_UNIT_NAME = exp.SERVICE_UNIT_NAME;
                    medi.EXPIRED_DATE = exp.EXPIRED_DATE;
                    medi.LOGGINNAME = exp.REQ_LOGINNAME;
                    medi.ngoaikho = false;
                    glstMedi.Add(medi);
                }

                //////Lấy thuốc kê ngoài
                ////Trường hợp 1: đơn chưa tạo phiếu xuất bán -> lấy chi tiết thuốc của đơn
                List<V_HIS_SERVICE_REQ_METY> glstMetyReq = null;
                List<V_HIS_EXP_MEST_MEDICINE_6> outMestMedicines = null;
                HisServiceReqMetyViewFilter filter2 = new HisServiceReqMetyViewFilter();
                filter2.TREATMENT_ID = this.data.treatmentId;
                glstMetyReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/GetView", ApiConsumers.MosConsumer, filter2, null);

                if (glstMetyReq != null && glstMetyReq.Count > 0)
                {
                    HisExpMestMedicineView6Filter hisExpMestMedicineView6Filter2 = new HisExpMestMedicineView6Filter();
                    hisExpMestMedicineView6Filter2.PRESCRIPTION_IDs = glstMetyReq.Select(s => s.SERVICE_REQ_ID).Distinct().ToList();
                    outMestMedicines = new BackendAdapter(this.param).Get<List<V_HIS_EXP_MEST_MEDICINE_6>>("api/HisExpMestMedicine/GetView6", ApiConsumers.MosConsumer, hisExpMestMedicineView6Filter2, null);
                }
                if (outMestMedicines != null && outMestMedicines.Count > 0)
                {
                    foreach (V_HIS_EXP_MEST_MEDICINE_6 exp in outMestMedicines)
                    {
                        MEDITYPE mEDITYPE = new MEDITYPE();
                        mEDITYPE.INSTRUCTION_TIME = exp.TDL_INTRUCTION_TIME;
                        mEDITYPE.MEDICINE_ID = exp.MEDICINE_ID;
                        mEDITYPE.MEDICINE_TYPE_CODE = exp.MEDICINE_TYPE_CODE;
                        mEDITYPE.MEDICINE_TYPE_NAME = exp.MEDICINE_TYPE_NAME;
                        mEDITYPE.PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                        mEDITYPE.SERVICE_UNIT_ID = exp.SERVICE_UNIT_ID;
                        mEDITYPE.SERVICE_UNIT_NAME = exp.SERVICE_UNIT_NAME;
                        mEDITYPE.EXPIRED_DATE = exp.EXPIRED_DATE;
                        mEDITYPE.LOGGINNAME = exp.REQ_LOGINNAME;
                        mEDITYPE.AMOUNT = exp.AMOUNT;
                        mEDITYPE.ngoaikho = true;
                        V_HIS_SERVICE_REQ_METY v_HIS_SERVICE_REQ_METY = glstMetyReq.FirstOrDefault(o => o.MEDICINE_TYPE_ID == exp.MEDICINE_TYPE_ID && o.SERVICE_REQ_ID == exp.PRESCRIPTION_ID);
                        if (v_HIS_SERVICE_REQ_METY != null)
                        {
                            mEDITYPE.SPEED = v_HIS_SERVICE_REQ_METY.SPEED;
                        }
                        else
                        {
                            mEDITYPE.SPEED = exp.SPEED;
                        }
                        glstMedi.Add(mEDITYPE);
                    }
                }
                if (glstMetyReq != null && glstMetyReq.Count > 0)
                {
                    foreach (V_HIS_SERVICE_REQ_METY serq in glstMetyReq)
                    {
                        bool hasSale = false;
                        if (serq.MEDICINE_TYPE_ID.HasValue && outMestMedicines != null)
                        {
                            hasSale = outMestMedicines.Any(a => a.PRESCRIPTION_ID == serq.SERVICE_REQ_ID && a.MEDICINE_TYPE_ID == serq.MEDICINE_TYPE_ID);
                        }

                        if (!hasSale)
                        {
                            MEDITYPE mediType = new MEDITYPE();
                            mediType.INSTRUCTION_TIME = serq.INTRUCTION_TIME;
                            mediType.LOGGINNAME = serq.REQUEST_LOGINNAME;

                            mediType.MEDICINE_TYPE_NAME = serq.MEDICINE_TYPE_NAME;
                            mediType.AMOUNT = serq.AMOUNT;
                            mediType.SPEED = serq.SPEED;
                            mediType.SERVICE_UNIT_NAME = serq.UNIT_NAME;
                            long id = serq.MEDICINE_TYPE_ID ?? 0;
                            if (id > 0)
                            {
                                HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(md => md.ID == id);
                                if (medicineType != null)
                                {
                                    mediType.MEDICINE_TYPE_CODE = medicineType.MEDICINE_TYPE_CODE;
                                    mediType.SERVICE_UNIT_ID = medicineType.TDL_SERVICE_UNIT_ID;
                                    mediType.EXPIRED_DATE = mediType.EXPIRED_DATE;
                                }
                            }
                            mediType.ngoaikho = true;
                            glstMedi.Add(mediType);
                        }

                    }
                }

                if (glstMedi != null)
                {
                    glstMedi = (from m in glstMedi
                                group m by new { m.MEDICINE_ID, m.MEDICINE_TYPE_CODE, m.MEDICINE_TYPE_NAME, m.PACKAGE_NUMBER,  m.EXPIRED_DATE,  m.ngoaikho } into g
                                select new MEDITYPE
                                {
                                    ID = "MEDITYPE" + (idCombo++),
                                    MEDICINE_ID = g.Key.MEDICINE_ID,
                                    MEDICINE_TYPE_CODE = g.Key.MEDICINE_TYPE_CODE,
                                    MEDICINE_TYPE_NAME = g.Key.MEDICINE_TYPE_NAME,
                                    PACKAGE_NUMBER = g.Key.PACKAGE_NUMBER,
                                    EXPIRED_DATE = g.Key.EXPIRED_DATE,
                                    ngoaikho = g.Key.ngoaikho

                                }).ToList<MEDITYPE>();
                }

                foreach (MEDITYPE medi in glstMedi)
                {
                    if (medi.EXPIRED_DATE > 0)
                        medi.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(medi.EXPIRED_DATE.ToString()));
                    if (medi.INSTRUCTION_TIME > 0)
                    {
                        medi.INSTRUCTION_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(medi.INSTRUCTION_TIME.ToString()));
                    }
                }

                //if (glstMedi != null)
                //    glstMedi.Sort((emp1, emp2) => Convert.ToDateTime(emp2.INSTRUCTION_TIME_STR).CompareTo(Convert.ToDateTime(emp1.INSTRUCTION_TIME_STR)));
                return glstMedi;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

        }

        private void LoadDatatoComboMedi(DevExpress.XtraEditors.GridLookUpEdit lookUpEdit1)
        {
            try
            {
                glstMediType = FillDataCombo();
                lookUpEdit1.Properties.DataSource = glstMediType;
                lookUpEdit1.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                lookUpEdit1.Properties.ValueMember = "ID";
                gridLookUpEditView.RowStyle += gridLookUpEditView_RowStyle;
                lookUpEdit1.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                lookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                lookUpEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                lookUpEdit1.Properties.ImmediatePopup = true;
                lookUpEdit1.ForceInitialize();
                //lookUpEdit1.Properties.View.Columns.Clear();
                lookUpEdit1.Properties.View.OptionsView.ShowColumnHeaders = true;


                GridColumn aColumnCode = lookUpEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = lookUpEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                GridColumn aColumnPackageNumber = lookUpEdit1.Properties.View.Columns.AddField("PACKAGE_NUMBER");
                aColumnPackageNumber.Caption = "Số lô";
                aColumnPackageNumber.Visible = true;
                aColumnPackageNumber.VisibleIndex = 3;
                aColumnPackageNumber.Width = 40;


                GridColumn aColumnExpiredDate = lookUpEdit1.Properties.View.Columns.AddField("EXPIRED_DATE_STR");
                aColumnExpiredDate.Caption = "Hạn sử dụng";
                aColumnExpiredDate.Visible = true;
                aColumnExpiredDate.VisibleIndex = 4;
                aColumnExpiredDate.Width = 120;

                //SetDefaultControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Loaddatatogrid()
        {
            try
            {
                HisMediReactViewFilter MediReactfilter = new HisMediReactViewFilter();
                MediReactfilter.MEDI_REACT_SUM_ID = data.MediReactSumId;
                ListMediReact = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDI_REACT>>(HisRequestUriStore.HIS_MEDI_REACT_GETVIEW, ApiConsumers.MosConsumer, MediReactfilter, param);
                if (ListMediReact != null && ListMediReact.Count > 0)
                {
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = ListMediReact;
                    //Lấy thông tin medicine type
                    HisMedicineViewFilter filterMedicine = new HisMedicineViewFilter();
                    filterMedicine.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filterMedicine.IDs = ListMediReact.Select(mt => mt.MEDICINE_ID ?? 0).ToList();
                    glstMedicine = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filterMedicine, null);
                    gridControl1.EndUpdate();
                }
               
                
                //.ToList().OrderByDescending(o => o.CREATE_TIME)
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        //e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXE_TIME_ST")
                    {
                        try
                        {
                            string StartTime = (view.GetRowCellValue(e.ListSourceRowIndex, "EXECUTE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(StartTime));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXECUTE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CHECK_TIME_ST")
                    {
                        try
                        {
                            string FinishTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CHECK_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(FinishTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            string ExpriredDate = (view.GetRowCellValue(e.ListSourceRowIndex, "EXPIRED_DATE") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(ExpriredDate));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao EXPIRED_DATE", ex);
                        }
                    }

                }
                //gridControlPatientList.RefreshDataSource();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridView1.PostEditor();
            CommonParam paramCommon = new CommonParam();
            bool success = false;
            try
            {
                if (!btnSave.Enabled && this.ActionType == GlobalVariables.ActionView)
                    return;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                HIS_MEDI_REACT mediReactsend = new HIS_MEDI_REACT();
                mediReactsend.MEDI_REACT_SUM_ID = data.MediReactSumId;
                if (cboMedicineType.EditValue != null)
                {
                    var mdType = glstMediType.FirstOrDefault(md => md.ID.Equals(cboMedicineType.EditValue));
                    if (mdType != null && mdType.MEDICINE_ID != null)
                    {
                        mediReactsend.MEDICINE_ID = mdType.MEDICINE_ID.Value;
                    }
                }
                mediReactsend.MEDI_REACT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMediReact.EditValue ?? 0).ToString());
                if (mediReactsend.MEDI_REACT_TYPE_ID == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Mã phương pháp không tồn tại", "Thông báo");
                    return;
                }
                if (String.IsNullOrEmpty(txtMedicineName.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập tên thuốc", "Thông báo");
                    return;
                }
                else
                {
                    mediReactsend.MEDICINE_TYPE_NAME = txtMedicineName.Text;
                }
                if (!String.IsNullOrEmpty(txtPackage.Text))
                {
                    mediReactsend.PACKAGE_NUMBER = txtPackage.Text;
                }
                if (txtExpiredDate.EditValue != null)
                {
                    mediReactsend.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtExpiredDate.DateTime);
                }
                if (cboReqUsername.EditValue != null)
                {
                    mediReactsend.REQUEST_USERNAME = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == this.cboReqUsername.EditValue.ToString()).First().USERNAME;
                    mediReactsend.REQUEST_LOGINNAME = this.cboReqUsername.EditValue.ToString();
                }
                if (cboExeUsername.EditValue != null)
                {
                    mediReactsend.EXECUTE_USERNAME = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == this.cboExeUsername.EditValue.ToString()).First().USERNAME;
                    mediReactsend.EXECUTE_LOGINNAME = this.cboExeUsername.EditValue.ToString();
                }
                if (cboCheckUsername.EditValue != null)
                {
                    mediReactsend.CHECK_USERNAME = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == this.cboCheckUsername.EditValue.ToString()).First().USERNAME;
                    mediReactsend.CHECK_LOGINNAME = this.cboCheckUsername.EditValue.ToString();
                }
                mediReactsend.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateEdit1.DateTime);
                //Convert.ToInt64(dateEdit1.DateTime.ToString("yyyyMMddhhmm") + "00");
                mediReactsend.CHECK_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateEdit2.DateTime);
                //Convert.ToInt64(dateEdit2.DateTime.ToString("yyyyMMddhhmm") + "00");
                if (textEdit1.EditValue != null && textEdit1.Text != "")
                {
                    mediReactsend.CHECK_RESULT = textEdit1.Text;
                }
                WaitingManager.Show();
                HIS_MEDI_REACT result = null;
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDI_REACT>(
                        ApiConsumer.HisRequestUriStore.HIS_MEDI_REACT_CREATE, ApiConsumer.ApiConsumers.MosConsumer, mediReactsend, param);
                }
                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    mediReactsend.ID = IDUpdate;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEDI_REACT>("api/HisMediReact/Update", ApiConsumer.ApiConsumers.MosConsumer, mediReactsend, param);
                    //if (result != null) this.ActionType = GlobalVariables.ActionAdd;
                }
                if (result != null)
                {
                    this.ActionType = GlobalVariables.ActionAdd;
                    success = true;

                    btnRefesh_Click(null, null);
                }
                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                param = new CommonParam();
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineType.EditValue != null)
                    {
                        cboMedicineType.Properties.Buttons[1].Visible = true;
                        var department = glstMediType.Where(f => f.ID.Equals(cboMedicineType.EditValue)).FirstOrDefault();

                        if (department != null)
                        {
                            txtMedicine.Text = department.MEDICINE_TYPE_CODE;
                            txtMedicineName.Text = department.MEDICINE_TYPE_NAME;
                            if (department.PACKAGE_NUMBER != null)
                                txtPackage.Text = department.PACKAGE_NUMBER;
                            else
                                txtPackage.EditValue = null;

                            //cboReqUsername.EditValue = department.LOGGINNAME;
                            //cboExeUsername.EditValue = null;
                            if (department.EXPIRED_DATE != null && department.EXPIRED_DATE > 0)
                                txtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)department.EXPIRED_DATE);
                            else
                                txtExpiredDate.EditValue = null;

                            if (department.ngoaikho == false)
                            {
                                txtMedicineName.Enabled = false;
                                txtPackage.Enabled = false;
                                txtExpiredDate.Enabled = false;

                            }
                            else
                            {
                                txtMedicineName.Enabled = true;
                                txtPackage.Enabled = true;
                                txtExpiredDate.Enabled = true;
                            }

                        }
                        //SendKeys.Send("{TAB}");


                    }

                    else
                    {
                        cboMedicineType.Focus();
                        //cboMedicineType.ShowPopup();
                        txtMedicine.EditValue = null;
                        txtMedicine.Enabled = true;
                        txtPackage.Enabled = true;
                    }
                }
                cboReqUsername.Focus();
                cboReqUsername.ShowPopup(); ;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider);
                ActionType = GlobalVariables.ActionAdd;
                SetDefaultControl();
                Loaddatatogrid();
                //LoadDatatoCombo(this.cboMedicineType, this.cboMediReact, this.cboReqUsername, this.cboExeUsername, this.cboCheckUsername);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefesh_Click(null, null);
        }



        private void dateEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dateEdit2.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dateEdit2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.textEdit1.Focus();
                    e.Handled = true;
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
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediReact_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboMediReact.EditValue != null)
                    {

                        var department = BackendDataWorker.Get<HIS_MEDI_REACT_TYPE>().FirstOrDefault(f => f.ID == long.Parse(cboMediReact.EditValue.ToString()));
                        if (department != null)
                        {
                            txtMediReact.Text = department.MEDI_REACT_TYPE_CODE;
                        }

                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                    else
                    {
                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCheckUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCheckUsername.EditValue != null)
                    {
                        cboCheckUsername.Properties.Buttons[1].Visible = true;
                    }
                    this.dateEdit1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {

                var row = (V_HIS_MEDI_REACT)gridView1.GetFocusedRow();
                if (row != null)
                {
                    IDUpdate = row.ID;
                    //if (row.MEDICINE_ID.HasValue)
                    //{
                    //    var mediType = glstMediType.Where(mdi => mdi.MEDICINE_ID == row.MEDICINE_ID).ToList();
                    //    if (mediType != null && mediType.Count > 0)
                    //    {
                    //        cboMedicineType.EditValue = mediType.FirstOrDefault().ID;
                    //        txtMedicineName.Enabled = false;
                    //        txtPackage.Enabled = false;
                    //    }
                    //}
                    if (row.MEDICINE_ID.HasValue)
                    {
                        var mediType = glstMediType.Where(mdi => mdi.MEDICINE_ID == row.MEDICINE_ID).ToList();
                        if (mediType != null && mediType.Count > 0)
                            cboMedicineType.EditValue = mediType.FirstOrDefault().ID;
                        var medicine = glstMedicine.Where(md => md.ID == row.MEDICINE_ID).ToList();
                        if (medicine != null && medicine.Count > 0)
                        {
                            txtMedicine.Text = medicine.FirstOrDefault().MEDICINE_TYPE_CODE;
                            txtMedicineName.Text = medicine.FirstOrDefault().MEDICINE_TYPE_NAME;
                        }
                        txtMedicineName.Enabled = false;
                        txtPackage.Enabled = false;
                    }
                    else
                    {
                        cboMedicineType.EditValue = null;
                        txtMedicine.Text = "";
                        txtMedicineName.Text = row.MEDICINE_TYPE_NAME;
                        txtMedicineName.Enabled = true;
                        txtPackage.Enabled = true;
                    }
                    //cboMedicineType.EditValue = row.MEDICINE_ID;
                    //txtMedicine.Text = row.MEDICINE_TYPE_CODE;
                    //txtMedicineName.Text = row.MEDICINE_TYPE_NAME;
                    txtPackage.Text = row.PACKAGE_NUMBER;
                    if (row.EXPIRED_DATE != null)
                    {
                        txtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.EXPIRED_DATE);
                    }
                    else {
                        txtExpiredDate.EditValue = null;
                    }
                    cboMediReact.EditValue = row.MEDI_REACT_TYPE_ID;
                    txtMediReact.Text = row.MEDI_REACT_TYPE_CODE;
                    cboReqUsername.EditValue = row.REQUEST_LOGINNAME;
                    cboExeUsername.EditValue = row.EXECUTE_LOGINNAME;
                    cboCheckUsername.EditValue = row.CHECK_LOGINNAME;
                    dateEdit1.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.EXECUTE_TIME);
                    dateEdit2.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)row.CHECK_TIME);
                    
                    textEdit1.Text = row.CHECK_RESULT;
                    ActionType = GlobalVariables.ActionEdit;
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboReqUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.cboExeUsername.Focus();
                    this.cboExeUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboExeUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExeUsername.EditValue != null)
                    {
                        cboExeUsername.Properties.Buttons[1].Visible = true;
                    }
                    this.cboCheckUsername.Focus();
                    this.cboCheckUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtMediReact_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {

        }

        private void txtMediReact_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtMediReact.Text))
                    {
                        cboMediReact.EditValue = null;
                        cboMediReact.Focus();
                        cboMediReact.ShowPopup();
                    }
                    else
                    {
                        var listData = BackendDataWorker.Get<HIS_MEDI_REACT_TYPE>().Where(o => o.MEDI_REACT_TYPE_CODE.Contains(txtMediReact.Text) || o.MEDI_REACT_TYPE_NAME.Contains(txtMediReact.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtMediReact.Text = listData.First().MEDI_REACT_TYPE_CODE;
                            cboMediReact.EditValue = listData.First().ID;
                            System.Windows.Forms.SendKeys.Send("{TAB}");
                        }
                        else if (listData != null && listData.Count > 1)
                        {
                            cboMediReact.Properties.DataSource = BackendDataWorker.Get<HIS_MEDI_REACT_TYPE>().Where(o => o.MEDI_REACT_TYPE_CODE.Contains(txtMediReact.Text) || o.MEDI_REACT_TYPE_NAME.Contains(txtMediReact.Text)).ToList();
                            cboMediReact.Focus();
                            cboMediReact.ShowPopup();
                        }
                        else
                        {
                            cboMediReact.Focus();
                            cboMediReact.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediReact_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMediReact.EditValue != null)
                    {
                        HIS_MEDI_REACT_TYPE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_REACT_TYPE>().SingleOrDefault(
                            o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediReact.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboMedicineType.Focus();
                            cboMediReact.ShowPopup();
                        }
                    }
                    else
                    {
                        cboMediReact.ShowPopup();
                    }
                }
                else
                {
                    cboMediReact.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (cboMedicineType.EditValue != null)
            //        {
            //            V_HIS_MEDICINE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE>().SingleOrDefault(
            //                o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediReact.EditValue.ToString()));
            //            if (gt != null)
            //            {
            //                cboReqUsername.Focus();
            //                cboMedicineType.ShowPopup();
            //            }
            //        }
            //        else
            //        {
            //            cboMedicineType.ShowPopup();
            //        }
            //    }
            //    else
            //    {
            //        cboMedicineType.ShowPopup();
            //    }
            //}
            //catch (Exception ex)
            //{

            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboReqUsername_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboReqUsername.EditValue != null)
                    {
                        ACS_USER gt = BackendDataWorker.Get<ACS_USER>().SingleOrDefault(
                            o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediReact.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboExeUsername.Focus();
                            cboExeUsername.ShowPopup();
                        }
                    }
                    else
                    {
                        cboReqUsername.ShowPopup();
                    }
                }
                else
                {
                    cboReqUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeUsername_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExeUsername.EditValue != null)
                    {
                        ACS_USER gt = BackendDataWorker.Get<ACS_USER>().SingleOrDefault(
                            o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediReact.EditValue.ToString()));
                        if (gt != null)
                        {
                            cboCheckUsername.Focus();
                            cboExeUsername.ShowPopup();
                        }
                    }
                    else
                    {
                        cboExeUsername.ShowPopup();
                    }
                }
                else
                {
                    cboReqUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCheckUsername_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCheckUsername.EditValue != null)
                    {
                        ACS_USER gt = BackendDataWorker.Get<ACS_USER>().SingleOrDefault(
                            o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediReact.EditValue.ToString()));
                        if (gt != null)
                        {
                            dateEdit1.Focus();
                            cboCheckUsername.ShowPopup();
                        }
                    }
                    else
                    {
                        cboCheckUsername.ShowPopup();
                    }
                }
                else
                {
                    cboCheckUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeUsername_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExeUsername.Properties.Buttons[1].Visible = false;
                    cboExeUsername.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCheckUsername_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCheckUsername.Properties.Buttons[1].Visible = false;
                    cboCheckUsername.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEditView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                var data = (MEDITYPE)gridLookUpEditView.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.ngoaikho)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FindByMedicineType(List<MEDITYPE> glst)
        {
            if (String.IsNullOrEmpty(txtMedicine.Text))
            {
                cboMedicineType.EditValue = null;
                cboMedicineType.Focus();
                cboMedicineType.ShowPopup();
            }
            else
            {
                string key = Inventec.Common.String.Convert.UnSignVNese(this.txtMedicine.Text.ToLower().Trim());

                if (glstMediTypeSort != null && glst.Count > 0)
                {
                    var listData = glst.Where(o =>
                        (!String.IsNullOrEmpty(o.MEDICINE_TYPE_CODE) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_CODE.ToLower()).Contains(key))
                        || (!String.IsNullOrEmpty(o.MEDICINE_TYPE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_NAME.ToLower()).Contains(key))).ToList();
                    if (listData != null && listData.Count == 1)
                    {
                        if (listData.FirstOrDefault().ngoaikho == false)
                        {
                            txtMedicineName.Enabled = false;
                            txtPackage.Enabled = false;
                            txtExpiredDate.Enabled = false;
                        }
                        else
                        {
                            txtMedicineName.Enabled = true;
                            txtPackage.Enabled = true;
                            txtExpiredDate.Enabled = true;
                        }
                        txtMedicine.Text = listData.FirstOrDefault().MEDICINE_TYPE_CODE;
                        cboMedicineType.EditValue = listData.FirstOrDefault().ID;
                        txtMedicineName.Text = listData.First().MEDICINE_TYPE_NAME;
                        txtPackage.Text = listData.FirstOrDefault().PACKAGE_NUMBER;

                        if (listData.FirstOrDefault().EXPIRED_DATE != null && listData.First().EXPIRED_DATE > 0)
                            txtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)listData.FirstOrDefault().EXPIRED_DATE);

                        //HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO inputService = new HIS.UC.ServiceUnit.ADO.ServiceUnitInputADO();
                        //if (listData.FirstOrDefault().SERVICE_UNIT_ID != null && listData.FirstOrDefault().SERVICE_UNIT_ID > 0)
                        //{
                        //    inputService.SERVICE_UNIT_NAME = listData.FirstOrDefault().SERVICE_UNIT_NAME;
                        //    inputService.SERVICE_UNIT_ID = listData.FirstOrDefault().SERVICE_UNIT_ID;
                        //}
                        //else
                        //    inputService.SERVICE_UNIT_NAME = listData.FirstOrDefault().SERVICE_UNIT_NAME;
                        //serviceUnitProcessor.Reload(ucServiceUnit, inputService);
                        cboReqUsername.EditValue = listData.First().LOGGINNAME;
                        cboExeUsername.EditValue = null;
                        //SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        glst = glst.Where(o =>
                        (!String.IsNullOrEmpty(o.MEDICINE_TYPE_CODE) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_CODE.ToLower()).Contains(key))
                        || (!String.IsNullOrEmpty(o.MEDICINE_TYPE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.MEDICINE_TYPE_NAME.ToLower()).Contains(key))).ToList();
                        if (glst != null && glst.Count > 0)
                        {
                            cboMedicineType.Properties.DataSource = glst;
                        }
                        cboMedicineType.Focus();
                        cboMedicineType.ShowPopup();
                    }
                }
            }
        }

        private void txtMedicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    glstMediType = FillDataCombo();
                    cboMedicineType.Properties.DataSource = glstMediType;
                    FindByMedicineType(glstMediType);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicineType.EditValue != null)
                    {
                        var department = glstMediType.Where(f => f.ID.Equals(cboMedicineType.EditValue)).FirstOrDefault();
                        if (department != null)
                        {
                            if (department.ngoaikho == false)
                            {
                                txtMedicineName.Enabled = false;
                                txtPackage.Enabled = false;
                                txtExpiredDate.Enabled = false;
                            }
                            else
                            {
                                txtMedicineName.Enabled = true;
                                txtPackage.Enabled = true;
                                txtExpiredDate.Enabled = true;
                            }
                            txtMedicine.Text = department.MEDICINE_TYPE_CODE;
                            txtMedicineName.Text = department.MEDICINE_TYPE_NAME;
                            txtPackage.Text = department.PACKAGE_NUMBER;
                            //ServiceUnitInputADO serviceUnit = new ServiceUnitInputADO();
                            //if (department.SERVICE_UNIT_ID != null)
                            //{
                            //    serviceUnit.SERVICE_UNIT_ID = department.SERVICE_UNIT_ID;
                            //    serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            //}
                            //else
                            //    serviceUnit.SERVICE_UNIT_NAME = department.SERVICE_UNIT_NAME;
                            //serviceUnitProcessor.Reload(ucServiceUnit, serviceUnit);
                            if (department.EXPIRED_DATE != null && department.EXPIRED_DATE > 0)
                                txtExpiredDate.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)department.EXPIRED_DATE);
                            cboReqUsername.EditValue = department.LOGGINNAME;

                        }
                        //cboServiceUnitName.Focus();

                    }
                    else
                    {
                        txtMedicineName.EditValue = null;
                        txtMedicineName.Enabled = true;
                        txtPackage.Enabled = true;
                        txtMedicineName.Focus();

                    }

                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMedicineType.Properties.Buttons[1].Visible = false;
                    cboMedicineType.EditValue = null;
                    txtMedicineName.EditValue = null;
                    txtMedicine.EditValue = null;
                    txtMedicineName.Enabled = true;
                    txtPackage.Enabled = true;
                    txtExpiredDate.Enabled = true;
                    txtPackage.EditValue = null;
                    txtExpiredDate.EditValue = null;
                    //cboExeUsername.EditValue = null;
                    //cboReqUsername.EditValue = null;
                    glstMediType = FillDataCombo();
                    cboMedicineType.Properties.DataSource = glstMediType;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPackage.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExpiredDate.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExpiredDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExeUsername.Focus();
                    cboExeUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
