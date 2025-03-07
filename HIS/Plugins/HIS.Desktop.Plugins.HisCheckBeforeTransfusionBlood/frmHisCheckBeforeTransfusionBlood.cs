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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.InputExpMestId;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using MOS.SDO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood
{
    public partial class frmHisCheckBeforeTransfusionBlood : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private V_HIS_PATIENT CurrentPatient = null;
        private V_HIS_EXP_MEST expMest = null;
        private long? ExpMestId = null;
        private DelegateSelectData delegateSelect = null;
        private int positionHandle = -1;
        private BindingList<ExpBloodADO> records = null;
        ExpBloodADO curentSelect = null;
        #endregion

        public frmHisCheckBeforeTransfusionBlood(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {
            frmInputExpMestId frmExpMestId = new frmInputExpMestId();
            frmExpMestId.ShowDialog();
            this.expMest = frmExpMestId.ExpMest;
            if (this.expMest != null)
            {
                this.ExpMestId = this.expMest.ID;
            }
            InitializeComponent();

            this.delegateSelect = delegateData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public frmHisCheckBeforeTransfusionBlood(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData, long? expMestId)
            : base(module)
        {

            InitializeComponent();
            this.delegateSelect = delegateData;
            this.ExpMestId = expMestId;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public frmHisCheckBeforeTransfusionBlood(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                frmInputExpMestId frmExpMestId = new frmInputExpMestId();
                frmExpMestId.ShowDialog();
                this.expMest = frmExpMestId.ExpMest;
                if (this.expMest != null)
                {
                    this.ExpMestId = this.expMest.ID;
                }
                InitializeComponent();

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisCheckBeforeTransfusionBlood(Inventec.Desktop.Common.Modules.Module module, long? expMestId)
            : base(module)
        {

            try
            {
                InitializeComponent();
                this.ExpMestId = expMestId;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Loadform
        private void frmHisCheckBeforeTransfusionBlood_Load(object sender, EventArgs e)
        {
            try
            {

                WaitingManager.Show();

                Config.ConfigKey.GetConfigKey();

                ValidateForm();

                LoadCurrentPatient();

                LoadComboAC(cboAC);

                LoadComboAC(cboAC2);
                //loadExpMest();
                btnPrint.Enabled = (this.expMest != null && this.expMest.EXP_MEST_STT_ID == 5);

                LoadDataToCombo();

                LoadDataToComboboxEnvironment();

                BuidTreeList();

                InitBtnPrint();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void loadExpMest()
        {
            try
            {
                if (this.ExpMestId != null)
                {
                    HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.ID = this.ExpMestId;
                    var data = new BackendAdapter(new CommonParam()).Get<V_HIS_EXP_MEST>("/api/HisExpMest/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (data != null)
                    {
                        this.expMest = data;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        private void LoadComboAC(GridLookUpEdit cbo)
        {
            try
            {
                List<ADO> lstAdo = new List<ADO>();
                ADO ado = new ADO(0, "Âm tính");
                lstAdo.Add(ado);
                ADO ado2 = new ADO((decimal)0.5, "0.5+");
                lstAdo.Add(ado2);
                ADO ado3 = new ADO(1, "1+");
                lstAdo.Add(ado3);
                ADO ado4 = new ADO(2, "2+");
                lstAdo.Add(ado4);
                ADO ado5 = new ADO(3, "3+");
                lstAdo.Add(ado5);
                ADO ado6 = new ADO(4, "4+");
                lstAdo.Add(ado6);
                ADO ado7 = new ADO(5, "5+");
                lstAdo.Add(ado7);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VALUE", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VALUE", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cbo, lstAdo, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatient()
        {
            try
            {
                if (this.ExpMestId.HasValue)
                {
                    if (this.expMest == null)
                    {
                        HisExpMestViewFilter expfilter = new HisExpMestViewFilter();
                        expfilter.ID = this.ExpMestId.Value;
                        List<V_HIS_EXP_MEST> lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expfilter, null);

                        if (lstExpMest != null && lstExpMest.Count > 0)
                        {
                            this.expMest = lstExpMest.FirstOrDefault();
                        }
                    }
                    if (this.expMest != null)
                    {
                        HisPatientViewFilter patientfilter = new HisPatientViewFilter();
                        patientfilter.ID = this.expMest.TDL_PATIENT_ID ?? 0;
                        var listPatient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientfilter, null);

                        if (listPatient != null && listPatient.Count > 0)
                        {
                            this.CurrentPatient = listPatient.First();
                        }
                    }
                }
                cboOldAbo.Enabled = false;
                cboOldRH.Enabled = false;
                if (this.CurrentPatient != null)
                {
                    cboOldAbo.EditValue = CurrentPatient.BLOOD_ABO_CODE;
                    cboOldRH.EditValue = CurrentPatient.BLOOD_RH_CODE;
                    if (Config.ConfigKey.IsNotAllowEditBloodInformation == "1")
                    {
                        if (this.CurrentPatient.BLOOD_ABO_CODE != null)
                            cboNewAbo.Enabled = false;
                        if (this.CurrentPatient.BLOOD_RH_CODE != null)
                            cboNewRH.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodAboFilter HisBloodAbofilter = new HisBloodAboFilter();
                HisBloodAbofilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var listBloodAbo = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>>(HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.HisRequestUriStore.MOSHIS_HIS_BLOOD_ABO_GET, ApiConsumers.MosConsumer, HisBloodAbofilter, param);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                InitComboAbo(cboNewAbo, listBloodAbo);
                InitComboAbo(cboOldAbo, listBloodAbo);

                HisBloodRhFilter HisBloodRHfilter = new HisBloodRhFilter();
                HisBloodRHfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var listBloodRH = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>>(HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.HisRequestUriStore.MOSHIS_HIS_BLOOD_RH_GET, ApiConsumers.MosConsumer, HisBloodRHfilter, param);

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                InitComboRH(cboNewRH, listBloodRH);
                InitComboRH(cboOldRH, listBloodRH);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboboxEnvironment()
        {
            try
            {
                List<ComboboxADO> datas = new List<ComboboxADO>();

                ComboboxADO ado5 = new ComboboxADO();
                ado5.Id = 5;
                ado5.ItemName = "Âm tính";
                datas.Add(ado5);

                ComboboxADO ado1 = new ComboboxADO();
                ado1.Id = 1;
                ado1.ItemName = "1+";
                datas.Add(ado1);

                ComboboxADO ado2 = new ComboboxADO();
                ado2.Id = 2;
                ado2.ItemName = "2+";
                datas.Add(ado2);

                ComboboxADO ado3 = new ComboboxADO();
                ado3.Id = 3;
                ado3.ItemName = "3+";
                datas.Add(ado3);

                ComboboxADO ado4 = new ComboboxADO();
                ado4.Id = 4;
                ado4.ItemName = "4+";
                datas.Add(ado4);

                ComboboxADO ado6 = new ComboboxADO();
                ado6.Id = 6;
                ado6.ItemName = "0.5+";
                datas.Add(ado6);

                ComboboxADO ado7 = new ComboboxADO();
                ado7.Id = 7;
                ado7.ItemName = "5+";
                datas.Add(ado7);

                InitComboEnvi(cboAntiGlobulin, datas);
                InitComboEnvi(cboAntiGlobulinTwo, datas);
                InitComboEnvi(cboSaltEnvi, datas);
                InitComboEnvi(cboSaltEnviTwo, datas);
                cboAntiGlobulin.EditValue = 0;
                cboAntiGlobulinTwo.EditValue = 0;
                cboSaltEnvi.EditValue = 0;
                cboSaltEnviTwo.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuidTreeList()
        {
            try
            {
                List<ExpBloodADO> datas = new List<ExpBloodADO>();
                if (ExpMestId.HasValue)
                {
                    HisExpMestBloodViewFilter filter = new HisExpMestBloodViewFilter();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.EXP_MEST_ID = ExpMestId.Value;
                    List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, filter, null);

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("expMestBloods___", expMestBloods));
                    HisExpBltyServiceFilter bltyService = new HisExpBltyServiceFilter();
                    bltyService.EXP_MEST_ID = this.ExpMestId.Value;
                    List<V_HIS_EXP_BLTY_SERVICE> bltyServices = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("api/HisExpBltyService/GetView", ApiConsumers.MosConsumer, bltyService, null);

                    if (expMestBloods != null && expMestBloods.Count > 0)
                    {
                        var Groups = expMestBloods.GroupBy(g => g.BLOOD_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            List<V_HIS_EXP_MEST_BLOOD> list = group.ToList();
                            V_HIS_EXP_MEST_BLOOD first = list.FirstOrDefault();
                            ExpBloodADO adoParent = new ExpBloodADO();
                            adoParent.AMOUNT = list.Count;
                            adoParent.BloodTypeId = group.Key;
                            adoParent.Key = String.Format("BLTY_{0}", group.Key);
                            adoParent.SERVICE_BLOOD_CODE = first.BLOOD_TYPE_CODE;
                            adoParent.SERVICE_BLOOD_NAME = first.BLOOD_TYPE_NAME;
                            adoParent.VOLUME = first.VOLUME;
                            adoParent.is_Sevrvice_Blood = false;
                            //adoParent.PREPARATIONS_BLOOD_NAME = first.PREPARATIONS_BLOOD_NAME;
                            datas.Add(adoParent);

                            //Xu ly blood (tui mau)
                            foreach (V_HIS_EXP_MEST_BLOOD bl in list)
                            {
                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("bl___", bl));
                                ExpBloodADO adoBlood = new ExpBloodADO();
                                adoBlood.AMOUNT = 1;
                                adoBlood.BloodTypeId = group.Key;
                                adoBlood.BLOOD_ABO_CODE = bl.BLOOD_ABO_CODE;
                                adoBlood.BLOOD_HR_CODE = bl.BLOOD_RH_CODE;
                                if (!String.IsNullOrWhiteSpace(adoBlood.BLOOD_ABO_CODE) && !String.IsNullOrWhiteSpace(adoBlood.BLOOD_HR_CODE))
                                {
                                    adoBlood.BLOOD_ABO_HR_CODE = String.Format("{0} {1}", adoBlood.BLOOD_ABO_CODE, adoBlood.BLOOD_HR_CODE);
                                }
                                else
                                {
                                    adoBlood.BLOOD_ABO_HR_CODE = String.IsNullOrWhiteSpace(adoBlood.BLOOD_ABO_CODE) ? (adoBlood.BLOOD_HR_CODE ?? "") : adoBlood.BLOOD_ABO_CODE;
                                }
                                adoBlood.BLOOD_CODE = bl.BLOOD_CODE;
                                adoBlood.EXPIRED_DATE = bl.EXP_DATE;
                                if (bl.EXP_DATE.HasValue)
                                {
                                    adoBlood.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(bl.EXP_DATE.Value);
                                }
                                adoBlood.ExpMestBloodId = bl.ID;
                                adoBlood.GIVE_CODE = bl.GIVE_CODE;
                                adoBlood.GIVE_NAME = bl.GIVE_NAME;
                                adoBlood.Key = String.Format("EXP_BLOOD_{0}", bl.BLOOD_ID);
                                adoBlood.ParentKey = adoParent.Key;
                                adoBlood.SERVICE_BLOOD_CODE = first.BLOOD_TYPE_CODE;
                                adoBlood.SERVICE_BLOOD_NAME = first.BLOOD_TYPE_NAME;
                                adoBlood.VOLUME = first.VOLUME;
                                adoBlood.ANTI_GLOBULIN = bl.ANTI_GLOBULIN_ENVI;
                                adoBlood.ANTI_GLOBULIN_TWO = bl.ANTI_GLOBULIN_ENVI_TWO;
                                adoBlood.COOMBS = bl.COOMBS;
                                adoBlood.PATIENT_BLOOD_ABO_CODE = bl.PATIENT_BLOOD_ABO_CODE;
                                adoBlood.PATIENT_BLOOD_RH_CODE = bl.PATIENT_BLOOD_RH_CODE;
                                adoBlood.PUC = bl.PUC;
                                adoBlood.SALT_ENVI = bl.SALT_ENVI;
                                adoBlood.SALT_ENVI_TWO = bl.SALT_ENVI_TWO;
                                adoBlood.SCANGEL_GELCARD = bl.SCANGEL_GELCARD;
                                adoBlood.TEST_TUBE = bl.TEST_TUBE;
                                adoBlood.TEST_TUBE_TWO = bl.TEST_TUBE_TWO;
                                adoBlood.AC_SELF_ENVIDENCE = bl.AC_SELF_ENVIDENCE;
                                adoBlood.AC_SELF_ENVIDENCE_SECOND = bl.AC_SELF_ENVIDENCE_SECOND;
                                adoBlood.PREPARATIONS_BLOOD_NAME = bl.PREPARATIONS_BLOOD_NAME;
                                adoBlood.NUM_ORDER = bl.NUM_ORDER??0;
                                datas.Add(adoBlood);
                            }

                            //Xu ly service (dich vu can test)
                            List<V_HIS_EXP_BLTY_SERVICE> byBloodTypes = bltyServices != null ? bltyServices.Where(o => o.BLOOD_TYPE_ID == group.Key).ToList() : null;

                            if (byBloodTypes != null && byBloodTypes.Count > 0)
                            {
                                ExpBloodADO adoParentService = new ExpBloodADO();
                                adoParentService.AMOUNT = byBloodTypes.Sum(s => s.SERVICE_AMOUNT);
                                adoParentService.BloodTypeId = group.Key;
                                adoParentService.Key = String.Format("SERVICE_{0}", group.Key);
                                adoParentService.ParentKey = adoParent.Key;
                                adoParentService.SERVICE_BLOOD_NAME = "Dịch vụ xét nghiệm";
                                adoParentService.is_Sevrvice_Blood = false;
                                datas.Add(adoParentService);

                                byBloodTypes = byBloodTypes.OrderBy(o => (o.NUM_ORDER ?? 99999999999)).ToList();
                                foreach (V_HIS_EXP_BLTY_SERVICE serv in byBloodTypes)
                                {
                                    ExpBloodADO adoService = new ExpBloodADO();
                                    adoService.AMOUNT = serv.SERVICE_AMOUNT;
                                    adoService.BloodTypeId = group.Key;
                                    adoService.HisExpId = serv.ID;
                                    adoService.Key = String.Format("EXP_SERVICE_{0}", serv.SERVICE_ID);
                                    adoService.ParentKey = adoParentService.Key;
                                    adoService.SERVICE_BLOOD_CODE = serv.SERVICE_CODE;
                                    adoService.SERVICE_BLOOD_NAME = serv.SERVICE_NAME;
                                    adoService.ServiceId = serv.SERVICE_ID;
                                    adoService.NUM_ORDER = serv.NUM_ORDER;
                                    adoService.is_Sevrvice_Blood = true;
                                    adoService.SERVICE_RESULT = serv.SERVICE_RESULT;
                                    adoService.EXP_MEST_ID = serv.EXP_MEST_ID;
                                    adoService.SERVICE_AMOUNT = serv.SERVICE_AMOUNT;
                                    
                                    datas.Add(adoService);
                                }
                            }
                        }
                    }
                }
                records = new BindingList<ExpBloodADO>(datas);
                treeListExpBlood.DataSource = records;
                treeListExpBlood.ExpandAll();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboEnvi(DevExpress.XtraEditors.LookUpEdit cbo, List<ComboboxADO> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ItemName", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ItemName", "Id", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationControlMaxLength(txtPuc, 100);
                ValidationControlMaxLength(txtTestTube, 100);
                ValidationControlMaxLength(txtTestTubeTwo, 100);
                ValidationControlMaxLength(txtScangelGelcard, 100);
                ValidationControlMaxLength(txtCoombs, 100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                //validate.IsRequired = true;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("txtCoombs", 7);
                //dicOrderTabIndexControl.Add("txtScangelGelcard", 6);
                //dicOrderTabIndexControl.Add("txtTestTube", 5);
                //dicOrderTabIndexControl.Add("txtPuc", 4);
                //dicOrderTabIndexControl.Add("cboOldRH", 3);
                //dicOrderTabIndexControl.Add("cboOldAbo", 2);
                //dicOrderTabIndexControl.Add("cboNewRH", 1);
                //dicOrderTabIndexControl.Add("cboNewAbo", 0);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, layoutControl1);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void InitComboAbo(DevExpress.XtraEditors.GridLookUpEdit cbo, List<HIS_BLOOD_ABO> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "BLOOD_ABO_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRH(DevExpress.XtraEditors.GridLookUpEdit cbo, List<HIS_BLOOD_RH> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "BLOOD_RH_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region event
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            UpdateExpMestBlood();
            //UpdatePatient();
        }

        private void UpdateExpMestBlood()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                bool check = true;

                MOS.SDO.HisExpMestTestInfoSDO updateDTO = new MOS.SDO.HisExpMestTestInfoSDO();
                if (this.curentSelect != null && this.curentSelect.ExpMestBloodId.HasValue)
                {
                    UpdateDTOFromDataForm(ref updateDTO);
                    if (updateDTO != null && updateDTO.ExpMestBloods != null && updateDTO.ExpMestBloods.Count > 0 && (string.IsNullOrEmpty(updateDTO.ExpMestBloods.First().PatientBloodAboCode) || string.IsNullOrEmpty(updateDTO.ExpMestBloods.First().PatientBloodRhCode)))
                    {
                        if (MessageBox.Show("Bạn chưa điền đủ thông tin \"Nhóm máu BN\". Bạn có muốn lưu lại không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            check = false;
                        }
                    }

                    else if (updateDTO != null && updateDTO.ExpMestBloods != null && updateDTO.ExpMestBloods.Count > 0 && (updateDTO.ExpMestBloods.First().PatientBloodAboCode != this.CurrentPatient.BLOOD_ABO_CODE || updateDTO.ExpMestBloods.First().PatientBloodRhCode != this.CurrentPatient.BLOOD_RH_CODE))
                    {
                        if (MessageBox.Show("Bạn đã sửa thông tin nhóm máu của bệnh nhân. Bạn có muốn lưu lại không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            check = false;
                        }
                    }
                    
                    if (check)
                    {

                        WaitingManager.Show();
                        var resultData = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/UpdateTestInfo", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            BuidTreeList();
                            LoadCurrentPatient();
                        }

                        if (success)
                        {
                            SetFocusEditor();
                        }
                        WaitingManager.Hide();

                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, success);
                        #endregion

                        #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                        SessionManager.ProcessTokenLost(param);
                        #endregion

                    }

                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn Máu/ Chế phẩm", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                cboNewAbo.Focus();
                cboNewAbo.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void RefeshDataAfterSave(MOS.SDO.HisExpMestResultSDO data)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void UpdateDTOFromDataForm(ref MOS.SDO.HisExpMestTestInfoSDO currentDTO)
        {
            try
            {
                currentDTO.ExpMestBloods = new List<MOS.SDO.ExpTestBloodSDO>();
                currentDTO.ExpMestId = this.ExpMestId.Value;

                currentDTO.RequestRoomId = this.currentModuleBase.RoomId;
                currentDTO.ExpMestBloods = new List<MOS.SDO.ExpTestBloodSDO>();
                MOS.SDO.ExpTestBloodSDO data = new MOS.SDO.ExpTestBloodSDO();
                data.ExpMestBloodId = this.curentSelect.ExpMestBloodId.Value;
                data.PatientBloodAboCode = (string)cboNewAbo.EditValue;
                data.PatientBloodRhCode = (string)cboNewRH.EditValue;
                data.Puc = txtPuc.Text;
                data.ScangelGelcard = txtScangelGelcard.Text;
                data.Coombs = txtCoombs.Text;

                data.TestTube = txtTestTube.Text;
                data.TestTubeTwo = txtTestTubeTwo.Text;

                if (cboSaltEnvi.EditValue != null)
                {
                    data.SaltEnvironment = Convert.ToInt64(cboSaltEnvi.EditValue);
                }
                if (cboAntiGlobulin.EditValue != null)
                {
                    data.AntiGlobulinEnvironment = Convert.ToInt64(cboAntiGlobulin.EditValue);
                }
                if (cboSaltEnviTwo.EditValue != null)
                {
                    data.SaltEnvironmentTwo = Convert.ToInt64(cboSaltEnviTwo.EditValue);
                }
                if (cboAntiGlobulinTwo.EditValue != null)
                {
                    data.AntiGlobulinEnvironmentTwo = Convert.ToInt64(cboAntiGlobulinTwo.EditValue);
                }
                data.AcSelfEnvidence = cboAC.EditValue != null ? (decimal?)cboAC.EditValue : null;
                data.AcSelfEnvidenceSecond = cboAC2.EditValue != null ? (decimal?)cboAC2.EditValue : null;
                currentDTO.ExpMestBloods.Add(data);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToEditorControl(ExpBloodADO data)
        {
            try
            {
                var dataPare = BackendDataWorker.Get<HIS_PREPARATIONS_BLOOD>();
                if (data != null)
                {
                    cboNewAbo.EditValue = data.PATIENT_BLOOD_ABO_CODE ?? (this.CurrentPatient != null ? this.CurrentPatient.BLOOD_ABO_CODE : null);
                    cboNewRH.EditValue = data.PATIENT_BLOOD_RH_CODE ?? (this.CurrentPatient != null ? this.CurrentPatient.BLOOD_RH_CODE : null);
                    txtPuc.Text = data.PUC ?? "";
                    txtScangelGelcard.Text = data.SCANGEL_GELCARD ?? "";
                    txtCoombs.Text = data.COOMBS ?? "";

                    if (!String.IsNullOrWhiteSpace(data.TEST_TUBE)
                        || !String.IsNullOrWhiteSpace(data.TEST_TUBE_TWO)
                        || data.SALT_ENVI.HasValue
                        || data.SALT_ENVI_TWO.HasValue
                        || data.ANTI_GLOBULIN.HasValue
                        || data.ANTI_GLOBULIN_TWO.HasValue)
                    {
                        txtTestTube.Text = data.TEST_TUBE ?? "";
                        cboSaltEnvi.EditValue = data.SALT_ENVI ?? 0;
                        cboAntiGlobulin.EditValue = data.ANTI_GLOBULIN ?? 0;


                        txtTestTubeTwo.Text = data.TEST_TUBE_TWO ?? "";
                        cboSaltEnviTwo.EditValue = data.SALT_ENVI_TWO ?? 0;
                        cboAntiGlobulinTwo.EditValue = data.ANTI_GLOBULIN_TWO ?? 0;
                    }
                    else
                    {
                        txtTestTube.Text = data.TEST_TUBE ?? "";
                        cboSaltEnvi.EditValue = (long)5;
                        cboAntiGlobulin.EditValue = (long)5;


                        txtTestTubeTwo.Text = data.TEST_TUBE_TWO ?? "";
                        cboSaltEnviTwo.EditValue = (long)5;
                        cboAntiGlobulinTwo.EditValue = (long)5;
                    }


                    cboAC.EditValue = (decimal?)data.AC_SELF_ENVIDENCE;
                    cboAC2.EditValue = (decimal?)data.AC_SELF_ENVIDENCE_SECOND;
                    if (data.BloodTypeId > 0)
                    {
                        var bloodType = BackendDataWorker.Get<HIS_BLOOD_TYPE>().Where(o => o.ID == data.BloodTypeId).FirstOrDefault();
                        if (bloodType != null)
                        {
                            var pre = dataPare.FirstOrDefault(o => o.ID == bloodType.PREPARATIONS_BLOOD_ID);
                            if (pre != null)
                            {
                                if (pre.PREPARATIONS_BLOOD_CODE == "3" || pre.PREPARATIONS_BLOOD_CODE == "4" || pre.PREPARATIONS_BLOOD_CODE == "5" || pre.PREPARATIONS_BLOOD_CODE == "1" || pre.PREPARATIONS_BLOOD_CODE == "6")
                                {
                                    cboSaltEnvi.EditValue = null;
                                    cboAntiGlobulin.EditValue = null;
                                    cboSaltEnviTwo.EditValue = null;
                                    cboAntiGlobulinTwo.EditValue = null;
                                    cboAC.EditValue = null;
                                    cboAC2.EditValue = null;
                                }
                                if (pre.PREPARATIONS_BLOOD_CODE == "3" || pre.PREPARATIONS_BLOOD_CODE == "4" || pre.PREPARATIONS_BLOOD_CODE == "5")
                                {
                                    cboSaltEnviTwo.EditValue = (long)5;
                                }
                                if (pre.PREPARATIONS_BLOOD_CODE == "6")
                                {
                                    cboSaltEnvi.EditValue = (long)5;
                                    cboSaltEnviTwo.EditValue = (long)5;
                                    cboAntiGlobulin.EditValue = (long)5;
                                }
                                if (pre.PREPARATIONS_BLOOD_CODE == "1")
                                {
                                    cboSaltEnvi.EditValue = (long)5;
                                    cboAntiGlobulin.EditValue = (long)5;
                                }
                            }
                        }
                        long? bloodGroupId = bloodType != null ? bloodType.BLOOD_GROUP_ID : null;
                        var bloodGroup = BackendDataWorker.Get<HIS_BLOOD_GROUP>().Where(o => o.ID == bloodGroupId).FirstOrDefault();
                        if (bloodGroup != null)
                        {
                            if (bloodGroup.BLOOD_ERYTHROCYTE == 1 && bloodGroup.BLOOD_PLASMA == 1)
                            {
                                txtTestTube.Enabled = false;
                                txtTestTubeTwo.Enabled = false;
                            }
                            else if (bloodGroup.BLOOD_ERYTHROCYTE == 1)
                            {
                                txtTestTube.Enabled = true;
                                txtTestTubeTwo.Enabled = false;
                            }
                            else if (bloodGroup.BLOOD_PLASMA == 1)
                            {
                                txtTestTube.Enabled = false;
                                txtTestTubeTwo.Enabled = true;
                            }
                            else
                            {
                                txtTestTube.Enabled = true;
                                txtTestTubeTwo.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    cboNewAbo.EditValue = null;
                    cboNewRH.EditValue = null;
                    txtPuc.Text = "";
                    txtScangelGelcard.Text = "";
                    txtCoombs.Text = "";
                    txtTestTube.Text = "";
                    cboSaltEnvi.EditValue = null;
                    cboAntiGlobulin.EditValue = null;


                    txtTestTubeTwo.Text = "";
                    cboSaltEnviTwo.EditValue = null;
                    cboAntiGlobulinTwo.EditValue = null;
                    cboAC.EditValue = null;
                    cboAC2.EditValue = null;


                    txtTestTube.Enabled = true;
                    txtTestTubeTwo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void cboNewAboCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboNewRH.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboNewRH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtPuc.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtPuc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtTestTube_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtScangelGelcard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtCoombs.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtCoombs_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ShortCut
        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled)
            {
                btnPrint_Click(null, null);
            }
        }
        #endregion

        private void cboNewAbo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNewAbo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNewRH_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNewRH.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListExpBlood_Click(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("treeListExpBlood_Click.1");
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                ExpBloodADO data = (ExpBloodADO)treeListExpBlood.GetDataRecordByNode(hi.Node);
                if (data != null && data.ExpMestBloodId.HasValue)
                {
                    this.curentSelect = data;
                }
                else
                {
                    this.curentSelect = null;
                }

                this.FillDataToEditorControl(this.curentSelect);
                LogSystem.Debug("treeListExpBlood_Click.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnvi_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSaltEnvi.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulin_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAntiGlobulin.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnviTwo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSaltEnviTwo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulinTwo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAntiGlobulinTwo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnvi_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnvi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulin_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTestTubeTwo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnviTwo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSaltEnviTwo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulinTwo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAntiGlobulinTwo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListExpBlood_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            try
            {
                ExpBloodADO data = (ExpBloodADO)treeListExpBlood.GetDataRecordByNode(e.Node);
                if (data != null && e.Node.ParentNode == null)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || !this.ExpMestId.HasValue) return;

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                store.RunPrintTemplate("Mps000421", delegateProcessPrintExp);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateProcessPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case "Mps000421":
                            InPhieuTruyenMauVaPhatMau(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuTruyenMauVaPhatMau(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();

                MPS.ProcessorBase.Core.PrintData printData = null;

                HisExpMestBloodViewFilter bloodViewFilter = new HisExpMestBloodViewFilter();
                bloodViewFilter.EXP_MEST_ID = this.ExpMestId.Value;
                List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("/api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodViewFilter, null);

                HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                filter.EXP_MEST_BLOOD_IDs = expMestBloods.Select(o => o.ID).ToList();
                List<V_HIS_TRANSFUSION_SUM> v_HIS_TRANSFUSION_SUMs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSFUSION_SUM>>("/api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, null);

                HisTransfusionFilter filterTrans = new HisTransfusionFilter();
                filterTrans.IDs = v_HIS_TRANSFUSION_SUMs.Select(o => o.ID).ToList();
                List<HIS_TRANSFUSION> hIS_TRANSFUSION = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSFUSION>>("/api/HisTransfusion/GetView", ApiConsumers.MosConsumer, filterTrans, null);


                HisExpBltyServiceViewFilter bltyServiceFilter = new HisExpBltyServiceViewFilter();
                bltyServiceFilter.EXP_MEST_ID = this.ExpMestId.Value;
                List<V_HIS_EXP_BLTY_SERVICE> expBltyServices = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("/api/HisExpBltyService/GetView", ApiConsumers.MosConsumer, bltyServiceFilter, null);

                V_HIS_TREATMENT treatment = null;

                if (this.expMest != null)
                {
                    if (this.expMest.TDL_TREATMENT_ID.HasValue)
                    {
                        HisExpMestViewFilter filterExp = new HisExpMestViewFilter();
                        filterExp.EXP_MEST_TYPE_ID = 12;
                        filterExp.EXP_MEST_STT_ID = 5;
                        filterExp.TDL_TREATMENT_CODE__EXACT = expMest.TDL_TREATMENT_CODE;
                        
                        listExp = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("/api/HisExpMest/Get", ApiConsumers.MosConsumer, filterExp, null);

                        HisTreatmentViewFilter treatFilter = new HisTreatmentViewFilter();
                        treatFilter.ID = this.expMest.TDL_TREATMENT_ID.Value;
                        List<V_HIS_TREATMENT> lstTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatFilter, null);
                        treatment = lstTreatment != null ? lstTreatment.FirstOrDefault() : null;
                    }

                }



                WaitingManager.Hide();
                foreach (var item in expMestBloods)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.expMest != null ? this.expMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                    if (inputADO != null && treatment != null && this.expMest != null)
                    {
                        string treatmentCode = "TREATMENT_CODE:" + treatment.TREATMENT_CODE;
                        string expMestCode = "EXP_MEST_CODE:" + this.expMest.EXP_MEST_CODE;
                        string bloodCode = "";
                        if (item != null)
                        {
                            bloodCode = "BLOOD_CODE:" + item.BLOOD_CODE;
                        }
                        inputADO.HisCode = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, expMestCode, bloodCode);
                    }

                    List<V_HIS_EXP_MEST_BLOOD> list = new List<V_HIS_EXP_MEST_BLOOD>();
                    list.Add(item);
                    List<V_HIS_EXP_BLTY_SERVICE> listService = expBltyServices != null ? expBltyServices.ToList() : null;

                    MPS.Processor.Mps000421.PDO.Mps000421PDO mps000421PDO = new MPS.Processor.Mps000421.PDO.Mps000421PDO(treatment, this.CurrentPatient, this.expMest, list, listService, v_HIS_TRANSFUSION_SUMs, listExp, hIS_TRANSFUSION );

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000421PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000421PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    if (printData != null)
                    {
                        printData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(printData);
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListExpBlood_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            var data = (ExpBloodADO)treeListExpBlood.GetDataRecordByNode(e.Node);
            if (data != null)
            {
                if (e.Column.FieldName == "SERVICE_RESULT")
                {
                    if (data.is_Sevrvice_Blood)//Xét nghiệm
                    { e.RepositoryItem = repositoryItemTextEdit; }
                    else
                    {
                        e.RepositoryItem = repositoryItemTextEdit1;
                    }
                }

            }
        }

        private void treeListExpBlood_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            var data = (ExpBloodADO)treeListExpBlood.GetDataRecordByNode(e.Node);
            if (data != null)
            {
                if (e.Column.FieldName == "SERVICE_RESULT")
                {
                    if (data.is_Sevrvice_Blood)//Xét nghiệm
                        e.Value = data.SERVICE_RESULT;
                }

            }
        }

        private void treeListExpBlood_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            var data = (ExpBloodADO)treeListExpBlood.GetDataRecordByNode(e.Node);
            if (data != null)
            {
                if (e.Column.FieldName == "SERVICE_RESULT")
                {
                    if (data.is_Sevrvice_Blood)//Xét nghiệm
                        //data.SERVICE_RESULT = repositoryItemTextEdit;
                        ProcessSaveBedLog(data);
                }

            }
        }
        public void ProcessSaveBedLog(ExpBloodADO row)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();

                if (row != null)
                {
                    HIS_EXP_BLTY_SERVICE hisexp = new HIS_EXP_BLTY_SERVICE();
                    hisexp.BLOOD_TYPE_ID = row.BloodTypeId;
                    hisexp.SERVICE_ID = row.ServiceId ?? 0;
                    hisexp.SERVICE_RESULT = row.SERVICE_RESULT;
                    hisexp.ID = row.HisExpId;
                    hisexp.EXP_MEST_ID = row.EXP_MEST_ID;
                    HisExpBltyServiceFilter bltyService = new HisExpBltyServiceFilter();
                    bltyService.BLOOD_TYPE_ID = row.BloodTypeId;

                    var bltyServices = new BackendAdapter(new CommonParam()).Post<HIS_EXP_BLTY_SERVICE>("api/HisExpBltyService/Update", ApiConsumers.MosConsumer, hisexp, null);

                    if (bltyServices != null)
                    {
                        //treeListExpBlood.DataSource =  ;
                        success = true;
                        MessageManager.Show(this, param, success);
                        treeListExpBlood.ExpandAll();
                        treeListExpBlood.Refresh();
                    }
                    else
                    {
                        MessageManager.Show(this, param, success);
                    }
                }
                if ((param.BugCodes != null && param.BugCodes.Count > 0) || (param.Messages != null && param.Messages.Count > 0))
                {
                    #region Show message

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextEdit_Enter(object sender, EventArgs e)
        {

        }
        private Type GetDataType(ColumnView view, DevExpress.XtraEditors.BaseEdit editor)
        {
            Type t = null;
            if (!Object.Equals(editor, null) && editor is TextEdit)
            {
                Type columnType = view.FocusedColumn.ColumnType;
            }
            return t;
        }

        private void repositoryItemTextEdit_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cboAC_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAC2_CloseUp(object sender, CloseUpEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAC_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAC2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAC_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAC.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAC2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAC2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region Custom btnPrint
        private void InitBtnPrint()
        {
            try
            {
                PopupMenu popupMenu = new PopupMenu();
                popupMenu.Manager = new BarManager();
                popupMenu.ItemLinks.Add(new BarButtonItem(popupMenu.Manager, "Phiếu truyền máu theo phiếu xuất"));
                popupMenu.ItemLinks.Add(new BarButtonItem(popupMenu.Manager, "Phiếu truyền máu theo mã vạch"));
                btnPrint.DropDownControl = popupMenu;

                foreach (BarItemLink link in popupMenu.ItemLinks)
                {
                    link.Item.ItemClick += (sender, e) =>
                    {
                        string menuItemText = e.Item.Caption;

                        // Xử lý dựa trên văn bản của các mục menu
                        switch (menuItemText)
                        {
                            case "Phiếu truyền máu theo phiếu xuất":
                                // Xử lý khi người dùng nhấp vào mục "Phiếu truyền máu theo phiếu xuất"
                                if (!btnPrint.Enabled || !this.ExpMestId.HasValue) return;
                                Inventec.Common.RichEditor.RichEditorStore store1 = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store1.RunPrintTemplate("Mps000421", delegateProcessPrintExp);
                                break;
                            case "Phiếu truyền máu theo mã vạch":
                                // Xử lý khi người dùng nhấp vào mục "Phiếu truyền máu theo mã vạch"
                                if (!btnPrint.Enabled || !this.ExpMestId.HasValue) return;
                                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store.RunPrintTemplate("Mps000421", delegateProcessPrint);
                                break;
                            default:
                                break;
                        }
                    };
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        private bool delegateProcessPrintExp(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case "Mps000421":
                            InTheoPhieuXuat(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private List<HIS_EXP_MEST> listExp = new List<HIS_EXP_MEST>();
        private void InTheoPhieuXuat(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();

                MPS.ProcessorBase.Core.PrintData printData = null;
                
                HisExpMestBloodViewFilter bloodViewFilter = new HisExpMestBloodViewFilter();
                bloodViewFilter.EXP_MEST_ID = this.ExpMestId.Value;
                List<V_HIS_EXP_MEST_BLOOD> expMestBloods = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("/api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, bloodViewFilter, null);

                HisTransfusionSumViewFilter filter = new HisTransfusionSumViewFilter();
                filter.EXP_MEST_BLOOD_IDs = expMestBloods.Select(o => o.ID).ToList();
                List<V_HIS_TRANSFUSION_SUM> v_HIS_TRANSFUSION_SUMs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSFUSION_SUM>>("/api/HisTransfusionSum/GetView", ApiConsumers.MosConsumer, filter, null);

                HisTransfusionFilter filterTrans = new HisTransfusionFilter();
                filterTrans.IDs = v_HIS_TRANSFUSION_SUMs.Select(o => o.ID).ToList();
                List<HIS_TRANSFUSION> hIS_TRANSFUSION = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSFUSION>>("/api/HisTransfusion/GetView", ApiConsumers.MosConsumer, filterTrans, null);


                HisExpBltyServiceViewFilter bltyServiceFilter = new HisExpBltyServiceViewFilter();
                bltyServiceFilter.EXP_MEST_ID = this.ExpMestId.Value;
                List<V_HIS_EXP_BLTY_SERVICE> expBltyServices = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_BLTY_SERVICE>>("/api/HisExpBltyService/GetView", ApiConsumers.MosConsumer, bltyServiceFilter, null);

                V_HIS_TREATMENT treatment = null;
                
                if (this.expMest != null)
                {
                    if (this.expMest.TDL_TREATMENT_ID.HasValue)
                    {
                        #region load list exp mest

                        HisExpMestViewFilter filterExp = new HisExpMestViewFilter();
                        filterExp.EXP_MEST_TYPE_ID = 12;
                        filterExp.EXP_MEST_STT_ID = 5;
                        filterExp.TDL_TREATMENT_CODE__EXACT = expMest.TDL_TREATMENT_CODE;
                        
                        listExp = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("/api/HisExpMest/Get", ApiConsumers.MosConsumer, filterExp, null);
                        

                        #endregion

                        HisTreatmentViewFilter treatFilter = new HisTreatmentViewFilter();
                        treatFilter.ID = this.expMest.TDL_TREATMENT_ID.Value;
                        List<V_HIS_TREATMENT> lstTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatFilter, null);
                        treatment = lstTreatment != null ? lstTreatment.FirstOrDefault() : null;
                    }

                }

                WaitingManager.Hide();
                List<V_HIS_EXP_MEST_BLOOD> list = new List<V_HIS_EXP_MEST_BLOOD>();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.expMest != null ? this.expMest.TDL_TREATMENT_CODE : "", printTypeCode, this.currentModuleBase.RoomId);
                foreach (var item in expMestBloods)
                {
                    if (inputADO != null && treatment != null && this.expMest != null)
                    {
                        string treatmentCode = "TREATMENT_CODE:" + treatment.TREATMENT_CODE;
                        string expMestCode = "EXP_MEST_CODE:" + this.expMest.EXP_MEST_CODE;
                        string bloodCode = "";
                        if (item != null)
                        {
                            bloodCode = "BLOOD_CODE:" + item.BLOOD_CODE;
                        }
                        inputADO.HisCode = String.Format("{0} {1} {2} {3}", printTypeCode, treatmentCode, expMestCode, bloodCode);
                    }   

                    
                    list.Add(item);  
                }
                List<V_HIS_EXP_BLTY_SERVICE> listService = expBltyServices != null ? expBltyServices.ToList() : null;
                //MPS.Processor.Mps000421.PDO.Mps000421PDO mps000421PDO = new MPS.Processor.Mps000421.PDO.Mps000421PDO(treatment, this.CurrentPatient, this.expMest, list, listService);
                MPS.Processor.Mps000421.PDO.Mps000421PDO mps000421PDO = new MPS.Processor.Mps000421.PDO.Mps000421PDO(treatment, this.CurrentPatient, this.expMest, list, listService, v_HIS_TRANSFUSION_SUMs, listExp, hIS_TRANSFUSION );
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000421PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000421PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                if (printData != null)
                {
                    printData.EmrInputADO = inputADO;
                    result = MPS.MpsPrinter.Run(printData);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
