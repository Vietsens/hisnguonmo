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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.BaseCompensationCreate.ADO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using AutoMapper;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using MOS.SDO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.Logging;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.BaseCompensationCreate
{
    public partial class UCBaseCompensationCreate : UserControlBase
    {
        private V_HIS_MEDI_STOCK stock = null;
        private bool isReadOnly = false;
        private bool isCheckAllPres = true;
        private bool isCheckAllCom = true;
        private bool isCheckAllDetail = true;
        private int start = 0;
        private int limit = 0;
        private int dataTotal = 0;
        private int rowCount = 0;

        private HIS_EXP_MEST expMestPrint = null;

        private int lastRowHandle = -1;
        private ToolTipControlInfo lastInfo = null;

        private const long TYPE_METY = (long)1;
        private const long TYPE_MATY = (long)2;

        private List<V_HIS_MEDI_STOCK_METY> mediStockMetys = null;
        private List<V_HIS_MEDI_STOCK_MATY> mediStockMatys = null;

        private List<HisExpMestADO> listPresCabinet = new List<HisExpMestADO>();
        private List<HisExpMestADO> listCompensation = new List<HisExpMestADO>();
        private List<MetyMatyADO> listExpMestDetails = new List<MetyMatyADO>();
        private List<MetyMatyADO> listExpMestDetails_ = new List<MetyMatyADO>();
        List<MetyMatyADO> _KhoXuatADOiSelecteds = new List<MetyMatyADO>();
        private Inventec.Desktop.Common.Modules.Module _currentModule = null;


        public UCBaseCompensationCreate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            this._currentModule = moduleData;
            InitializeComponent();
        }

        private void UCBaseCompensationCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                this.isReadOnly = (this.stock == null) || (this.stock.IS_CABINET != 1);
                if (this.isReadOnly)
                {
                    btnFind.Enabled = false;
                    btnRefresh.Enabled = false;
                    btnSave.Enabled = false;
                    XtraMessageBox.Show("Không phải tủ trực", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                if (this.stock.CABINET_MANAGE_OPTION == 2)
                {
                    btnTimKiemS.Enabled = true;
                    cboKhoXuat.Enabled = true;
                }
                else
                {
                    btnTimKiemS.Enabled = false;
                    cboKhoXuat.Enabled = false;
                }
                this.gridControlExpMestBcs.ToolTipController = this.toolTipController1;
                this.mediStockMatys = BackendDataWorker.Get<V_HIS_MEDI_STOCK_MATY>();
                this.mediStockMetys = BackendDataWorker.Get<V_HIS_MEDI_STOCK_METY>();
                this.mediStockMatys = this.mediStockMatys != null ? this.mediStockMatys.Where(o => o.MEDI_STOCK_ID == this.stock.ID).ToList() : null;
                this.mediStockMetys = this.mediStockMetys != null ? this.mediStockMetys.Where(o => o.MEDI_STOCK_ID == this.stock.ID).ToList() : null;
                this.gridColumn_PresCabinet_Check.Image = imageListIcon.Images[6];
                this.gridColumn_Compensation_Check.Image = imageListIcon.Images[6];
                this.gridColumn_ExpMestDetail_Check.Image = imageListIcon.Images[6];

                InitCheck(cboKhoXuat, SelectionGrid__KhoXuat);
                InitCombo(cboKhoXuat, "MEDI_STOCK_NAME", "ID");

                this.SetDefaultValue();
                this.InitComboMediStock();
                this.InitThreadLoadData();
                IsReasonRequired = LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__IS_REASON_REQUIRED) == "1";
                LoadDataToComboReasonRequired();
                lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Black;
                if (IsReasonRequired)
                {
                    lciReasonRequired.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(cboReasonRequired);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboReasonRequired()
        {
            try
            {
                var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboReasonRequired, reason, controlEditorADO);
                cboReasonRequired.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtKeyword.Text = "";
                dtExportDateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtExportDateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                cboKhoXuat.EditValue = null;
                checkIsNotCompensation.Checked = true;
                checkIsCompensation.Checked = false;
                gridControlPreCabinet.DataSource = null;
                gridControlBcsDetail.DataSource = null;
                gridControlExpMestBcs.DataSource = null;
                gridControlExpMestDetail.DataSource = null;
                gridControlCompensation.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> _mediStocks = new List<V_HIS_MEDI_STOCK>();
                var datas = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(p => p.ROOM_ID == this.currentModuleBase.RoomId && p.IS_ACTIVE == 1).ToList();
                if (datas != null)
                {
                    List<long> mediStockIds = datas.Select(p => p.MEDI_STOCK_ID).ToList();
                    _mediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p =>
                        mediStockIds.Contains(p.ID)
                        && this.CheckBusiness(p)
                        && p.ID != this.stock.ID
                        && p.IS_ACTIVE == 1).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, _mediStocks, controlEditorADO);
                if (_mediStocks != null && _mediStocks.Count > 0)
                {
                    _mediStocks.OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    this.cboMediStock.EditValue = _mediStocks[0].ID;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboMediStock_()
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 1));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "MEDI_STOCK_NAME", columnInfos, false, 250);

                List<MetyMatyADO> mediStock = new List<MetyMatyADO>();

                foreach (var item in this.listExpMestDetails.Select(o => new { o.MEDI_STOCK_ID, o.MEDI_STOCK_NAME }).Distinct()) //[MEDI_STOCK_NAME,MEDI_STOCK_ID]
                {
                    MetyMatyADO medi = new MetyMatyADO();
                    if (!string.IsNullOrEmpty(item.MEDI_STOCK_NAME) && !string.IsNullOrWhiteSpace(item.MEDI_STOCK_NAME))
                    {
                        medi.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                        medi.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        mediStock.Add(medi);
                    }
                }
                _KhoXuatADOiSelecteds = new List<MetyMatyADO>();
                ResetCombo(cboKhoXuat);
                cboKhoXuat.Text = "";
                mediStock = mediStock.Distinct().ToList();
                // ControlEditorLoader.Load(cboKhoXuat, mediStock, controlEditorADO);
                //ResetCombo(cboKhoXuat);
                cboKhoXuat.Properties.DataSource = mediStock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

            }
        }


        private bool CheckBusiness(V_HIS_MEDI_STOCK p)
        {
            try
            {
                if (this.stock.IS_BUSINESS == 1)
                {
                    return (p.IS_BUSINESS == 1);
                }
                else
                {
                    return (p.IS_BUSINESS != 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void InitThreadLoadData()
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoadDataToControl));
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadDataToControl()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.LoadExpMestPresCabinet();
                        this.LoadExpMestCompensation();
                        this.LoadExpMestDetail();
                        this.LoadData();
                    }));
                }
                else
                {
                    this.LoadExpMestPresCabinet();
                    this.LoadExpMestCompensation();
                    this.LoadExpMestDetail();
                    this.LoadData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestPresCabinet()
        {

            try
            {
                this.listPresCabinet = new List<HisExpMestADO>();
                HisExpMestView5Filter filter = new HisExpMestView5Filter();
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "FINISH_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.HAS_XBTT_EXP_MEST_ID = false;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

                if (checkIsNotCompensation.Checked && !checkIsCompensation.Checked)
                {
                    filter.IS_NOT_FULL_BCS_REQ_AMOUNT = true;
                }
                else if (!checkIsNotCompensation.Checked && checkIsCompensation.Checked)
                {
                    filter.IS_NOT_FULL_BCS_REQ_AMOUNT = false;
                }

                filter.MEDI_STOCK_ID = this.stock.ID;

                #region DateTime
                if (dtExportDateFrom.EditValue != null && dtExportDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_FROM = Convert.ToInt64(dtExportDateFrom.DateTime.ToString("yyyyMMddHHmmss"));
                }
                if (dtExportDateTo.EditValue != null && dtExportDateTo.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_TO = Convert.ToInt64(dtExportDateTo.DateTime.ToString("yyyyMMddHHmmss"));
                }
                #endregion

                filter.ColumnParams = new List<string>()
                {
                    "ID",
                    "EXP_MEST_STT_ID",
                    "EXP_MEST_TYPE_ID",
                    "IMP_MEDI_STOCK_ID",
                    "IS_ACTIVE",
                    "IS_DELETE",
                    "EXP_MEST_CODE",
                    "IS_EXECUTE_KIDNEY_PRES",
                    "IS_EXPORT_EQUAL_APPROVE",
                    "IS_EXPORT_EQUAL_REQUEST",
                    "IS_HAS_BCS_REQ_AMOUNT",
                    "IS_NOT_FULL_BCS_REQ_AMOUNT",
                    "IS_OLD_BCS",
                    "MEDI_STOCK_ID",
                    "MEDI_STOCK_CODE",
                    "REQ_DEPARTMENT_ID",
                    "REQ_LOGINNAME",
                    "REQ_USERNAME",
                    "SERVICE_REQ_ID",
                    "TDL_INTRUCTION_TIME",
                    "XBTT_EXP_MEST_ID",
                    "MEDI_STOCK_NAME",
                    "FINISH_TIME",
                    "FINISH_DATE",
                    "TDL_TREATMENT_CODE",
                    "TDL_SERVICE_REQ_CODE",
                    "TDL_PATIENT_NAME"
                };

                CommonParam param = new CommonParam();
                LogSystem.Info("LoadExpMestPresCabinet. Begin Call Api");
                List<V_HIS_EXP_MEST_5> rs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_5>>("api/HisExpMest/GetView5Dynamic", ApiConsumers.MosConsumer, filter, param);
                LogSystem.Info("LoadExpMestPresCabinet. End Call Api");
                if (rs != null && rs.Count > 0)
                {
                    Mapper.CreateMap<V_HIS_EXP_MEST_5, HisExpMestADO>();
                    this.listPresCabinet = Mapper.Map<List<HisExpMestADO>>(rs);
                    this.listPresCabinet.ForEach(o => o.IsAllowCheck = (o.IS_NOT_FULL_BCS_REQ_AMOUNT == 1));
                }
                isCheckAllPres = true;
                gridColumn_PresCabinet_Check.Image = imageListIcon.Images[6];
                LogSystem.Info("LoadExpMestPresCabinet. Begin Update");
                gridControlPreCabinet.BeginUpdate();
                gridControlPreCabinet.DataSource = this.listPresCabinet;
                gridControlPreCabinet.EndUpdate();
                LogSystem.Info("LoadExpMestPresCabinet. End Update");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestCompensation()
        {

            try
            {
                this.listCompensation = new List<HisExpMestADO>();
                HisExpMestView5Filter filter = new HisExpMestView5Filter();
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "FINISH_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.HAS_XBTT_EXP_MEST_ID = false;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;

                if (checkIsNotCompensation.Checked && !checkIsCompensation.Checked)
                {
                    filter.IS_NOT_FULL_BCS_REQ_AMOUNT = true;
                }
                else if (!checkIsNotCompensation.Checked && checkIsCompensation.Checked)
                {
                    filter.IS_NOT_FULL_BCS_REQ_AMOUNT = false;
                }

                filter.IMP_MEDI_STOCK_ID = this.stock.ID;

                #region DateTime
                if (dtExportDateFrom.EditValue != null && dtExportDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_FROM = Convert.ToInt64(dtExportDateFrom.DateTime.ToString("yyyyMMddHHmmss"));
                }
                if (dtExportDateTo.EditValue != null && dtExportDateTo.DateTime != DateTime.MinValue)
                {
                    filter.FINISH_TIME_TO = Convert.ToInt64(dtExportDateTo.DateTime.ToString("yyyyMMddHHmmss"));
                }
                #endregion

                filter.ColumnParams = new List<string>()
                {
                    "ID",
                    "CREATE_TIME",
                    "EXP_MEST_STT_ID",
                    "EXP_MEST_TYPE_ID",
                    "IMP_MEDI_STOCK_ID",
                    "IS_ACTIVE",
                    "IS_DELETE",
                    "EXP_MEST_CODE",
                    "IS_EXECUTE_KIDNEY_PRES",
                    "IS_EXPORT_EQUAL_APPROVE",
                    "IS_EXPORT_EQUAL_REQUEST",
                    "IS_HAS_BCS_REQ_AMOUNT",
                    "IS_NOT_FULL_BCS_REQ_AMOUNT",
                    "IS_OLD_BCS",
                    "MEDI_STOCK_ID",
                    "MEDI_STOCK_CODE",
                    "REQ_DEPARTMENT_ID",
                    "REQ_LOGINNAME",
                    "REQ_USERNAME",
                    "SERVICE_REQ_ID",
                    "TDL_INTRUCTION_TIME",
                    "XBTT_EXP_MEST_ID",
                    "MEDI_STOCK_NAME",
                    "FINISH_TIME",
                    "FINISH_DATE"
                };

                CommonParam param = new CommonParam();
                LogSystem.Info("LoadExpMestCompensation. Begin Call Api");
                List<V_HIS_EXP_MEST_5> rs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_5>>("api/HisExpMest/GetView5Dynamic", ApiConsumers.MosConsumer, filter, param);
                LogSystem.Info("LoadExpMestCompensation. End Call Api");
                if (rs != null && rs.Count > 0)
                {
                    Mapper.CreateMap<V_HIS_EXP_MEST_5, HisExpMestADO>();
                    this.listCompensation = Mapper.Map<List<HisExpMestADO>>(rs);
                    this.listCompensation.ForEach(o => o.IsAllowCheck = (o.IS_NOT_FULL_BCS_REQ_AMOUNT == 1));
                }

                isCheckAllCom = true;
                gridColumn_Compensation_Check.Image = imageListIcon.Images[6];
                LogSystem.Info("LoadExpMestCompensation. Begin Update");
                gridControlCompensation.BeginUpdate();
                gridControlCompensation.DataSource = this.listCompensation;
                gridControlCompensation.EndUpdate();
                LogSystem.Info("LoadExpMestCompensation. End Update");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestDetail()
        {
            try
            {
                LogSystem.Info("LoadExpMestDetail. Begin Process");
                this.listExpMestDetails = new List<MetyMatyADO>();
                List<long> preChecks = this.listPresCabinet != null ? this.listPresCabinet.Where(o => o.IsCheck).Select(s => s.ID).ToList() : null;

                List<long> comChecks = this.listCompensation != null ? this.listCompensation.Where(o => o.IsCheck).Select(s => s.ID).ToList() : null;

                List<HIS_EXP_MEST_MEDICINE> medicines = new List<HIS_EXP_MEST_MEDICINE>();
                List<HIS_EXP_MEST_MATERIAL> materials = new List<HIS_EXP_MEST_MATERIAL>();

                List<HIS_EXP_MEST_METY_REQ> metyReqs = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_MATY_REQ> matyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

                if (preChecks != null && preChecks.Count > 0)
                {
                    int start = 0;
                    int limit = 100;
                    int count = preChecks.Count;
                    while (count > 0)
                    {
                        limit = (count >= 100) ? 100 : count;
                        List<long> ids = preChecks.Skip(start).Take(limit).ToList();

                        HisExpMestMedicineFilter mediFilter = new HisExpMestMedicineFilter();
                        mediFilter.EXP_MEST_IDs = ids;
                        List<HIS_EXP_MEST_MEDICINE> medis = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, mediFilter, null);
                        if (medis != null && medis.Count > 0)
                        {
                            medicines.AddRange(medis);
                        }

                        HisExpMestMaterialFilter mateFilter = new HisExpMestMaterialFilter();
                        mateFilter.EXP_MEST_IDs = ids;
                        List<HIS_EXP_MEST_MATERIAL> mates = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, mateFilter, null);
                        if (mates != null && mates.Count > 0)
                        {
                            materials.AddRange(mates);
                        }
                        start += 100;
                        count -= 100;
                    }
                }

                if (comChecks != null && comChecks.Count > 0)
                {
                    int start = 0;
                    int limit = 100;
                    int count = comChecks.Count;
                    while (count > 0)
                    {
                        limit = (count >= 100) ? 100 : count;
                        List<long> ids = comChecks.Skip(start).Take(limit).ToList();

                        HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                        metyFilter.EXP_MEST_IDs = ids;
                        List<HIS_EXP_MEST_METY_REQ> metys = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, null);
                        if (metys != null && metys.Count > 0)
                        {
                            metyReqs.AddRange(metys);
                        }

                        HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                        matyFilter.EXP_MEST_IDs = ids;
                        List<HIS_EXP_MEST_MATY_REQ> matys = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyFilter, null);
                        if (matys != null && matys.Count > 0)
                        {
                            matyReqs.AddRange(matys);
                        }
                        start += 100;
                        count -= 100;
                    }
                }
                medicines = medicines.Where(o => o.TDL_MEDICINE_TYPE_ID.HasValue && (o.AMOUNT - (o.BCS_REQ_AMOUNT ?? 0)) > 0).ToList();
                materials = materials.Where(o => o.TDL_MATERIAL_TYPE_ID.HasValue && (o.AMOUNT - (o.BCS_REQ_AMOUNT ?? 0)) > 0).ToList();
                metyReqs = metyReqs.Where(o => (o.AMOUNT - ((o.DD_AMOUNT ?? 0) + (o.BCS_REQ_AMOUNT ?? 0))) > 0).ToList();
                matyReqs = matyReqs.Where(o => (o.AMOUNT - ((o.DD_AMOUNT ?? 0) + (o.BCS_REQ_AMOUNT ?? 0))) > 0).ToList();
                if (medicines.Count > 0)
                {
                    var Groups = medicines.GroupBy(g => g.TDL_MEDICINE_TYPE_ID.Value).ToList();
                    foreach (var group in Groups)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        V_HIS_MEDI_STOCK_METY stockMety = this.mediStockMetys != null ? this.mediStockMetys.FirstOrDefault(o => o.MEDICINE_TYPE_ID == group.Key) : null;
                        MetyMatyADO ado = this.listExpMestDetails.FirstOrDefault(o => o.TYPE == TYPE_METY && o.METY_MATY_ID == group.Key);
                        if (ado == null)
                        {

                            ado = new MetyMatyADO();
                            ado.METY_MATY_ID = group.Key;
                            ado.METY_MATY_CODE = medicineType.MEDICINE_TYPE_CODE;
                            ado.METY_MATY_NAME = medicineType.MEDICINE_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            ado.TYPE = TYPE_METY;
                            this.listExpMestDetails.Add(ado);
                        }
                        ado.AMOUNT += group.Sum(s => (s.AMOUNT - (s.BCS_REQ_AMOUNT ?? 0)));
                        ado.ExpMestMedicines = group.ToList();
                        ado.IsCheck = true;
                        ado.REQ_AMOUNT = ado.AMOUNT;
                        if (stockMety != null && stockMety.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = stockMety.EXP_MEDI_STOCK_NAME;
                            ado.MEDI_STOCK_ID = stockMety.EXP_MEDI_STOCK_ID;
                        }
                    }
                }

                if (metyReqs.Count > 0)
                {
                    var Groups = metyReqs.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        V_HIS_MEDI_STOCK_METY stockMety = this.mediStockMetys != null ? this.mediStockMetys.FirstOrDefault(o => o.MEDICINE_TYPE_ID == group.Key) : null;
                        MetyMatyADO ado = this.listExpMestDetails.FirstOrDefault(o => o.TYPE == TYPE_METY && o.METY_MATY_ID == group.Key);
                        if (ado == null)
                        {

                            ado = new MetyMatyADO();
                            ado.METY_MATY_ID = group.Key;
                            ado.METY_MATY_CODE = medicineType.MEDICINE_TYPE_CODE;
                            ado.METY_MATY_NAME = medicineType.MEDICINE_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            ado.TYPE = TYPE_METY;
                            this.listExpMestDetails.Add(ado);
                        }
                        ado.AMOUNT += group.Sum(s => (s.AMOUNT - ((s.DD_AMOUNT ?? 0) + (s.BCS_REQ_AMOUNT ?? 0))));
                        ado.ExpMestMetyReqs = group.ToList();
                        ado.IsCheck = true;
                        ado.REQ_AMOUNT = ado.AMOUNT;
                        if (stockMety != null && stockMety.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = stockMety.EXP_MEDI_STOCK_NAME;
                            ado.MEDI_STOCK_ID = stockMety.EXP_MEDI_STOCK_ID;
                        }
                    }
                }

                if (materials.Count > 0)
                {
                    var Groups = materials.GroupBy(g => g.TDL_MATERIAL_TYPE_ID.Value).ToList();
                    foreach (var group in Groups)
                    {
                        V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        V_HIS_MEDI_STOCK_MATY stockMaty = this.mediStockMatys != null ? this.mediStockMatys.FirstOrDefault(o => o.MATERIAL_TYPE_ID == group.Key) : null;
                        MetyMatyADO ado = this.listExpMestDetails.FirstOrDefault(o => o.TYPE == TYPE_MATY && o.METY_MATY_ID == group.Key);
                        if (ado == null)
                        {

                            ado = new MetyMatyADO();
                            ado.METY_MATY_ID = group.Key;
                            ado.METY_MATY_CODE = materialType.MATERIAL_TYPE_CODE;
                            ado.METY_MATY_NAME = materialType.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            ado.TYPE = TYPE_MATY;
                            this.listExpMestDetails.Add(ado);
                        }
                        ado.AMOUNT += group.Sum(s => (s.AMOUNT - (s.BCS_REQ_AMOUNT ?? 0)));
                        ado.ExpMestMaterials = group.ToList();
                        ado.IsCheck = true;
                        ado.REQ_AMOUNT = ado.AMOUNT;
                        if (stockMaty != null && stockMaty.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = stockMaty.EXP_MEDI_STOCK_NAME;
                            ado.MEDI_STOCK_ID = stockMaty.EXP_MEDI_STOCK_ID;
                        }
                    }
                }

                if (matyReqs.Count > 0)
                {
                    var Groups = matyReqs.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                    foreach (var group in Groups)
                    {
                        V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                        V_HIS_MEDI_STOCK_MATY stockMaty = this.mediStockMatys != null ? this.mediStockMatys.FirstOrDefault(o => o.MATERIAL_TYPE_ID == group.Key) : null;
                        MetyMatyADO ado = this.listExpMestDetails.FirstOrDefault(o => o.TYPE == TYPE_MATY && o.METY_MATY_ID == group.Key);
                        if (ado == null)
                        {

                            ado = new MetyMatyADO();
                            ado.METY_MATY_ID = group.Key;
                            ado.METY_MATY_CODE = materialType.MATERIAL_TYPE_CODE;
                            ado.METY_MATY_NAME = materialType.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            ado.TYPE = TYPE_MATY;
                            this.listExpMestDetails.Add(ado);
                        }
                        ado.AMOUNT += group.Sum(s => (s.AMOUNT - ((s.DD_AMOUNT ?? 0) + (s.BCS_REQ_AMOUNT ?? 0))));
                        ado.ExpMestMatyReqs = group.ToList();
                        ado.IsCheck = true;
                        ado.REQ_AMOUNT = ado.AMOUNT;
                        if (stockMaty != null && stockMaty.EXP_MEDI_STOCK_ID.HasValue)
                        {
                            ado.MEDI_STOCK_NAME = stockMaty.EXP_MEDI_STOCK_NAME;
                            ado.MEDI_STOCK_ID = stockMaty.EXP_MEDI_STOCK_ID;
                        }
                    }
                }
                if (this.listExpMestDetails != null && this.listExpMestDetails.Count > 0)
                {
                    isCheckAllDetail = false;
                    gridColumn_ExpMestDetail_Check.Image = imageListIcon.Images[5];
                }
                else
                {
                    isCheckAllDetail = true;
                    gridColumn_ExpMestDetail_Check.Image = imageListIcon.Images[6];
                }
                InitComboMediStock_();



                LogSystem.Info("LoadExpMestDetail. End Process");
                LogSystem.Info("LoadExpMestDetail. Begin Update");
                gridControlExpMestDetail.BeginUpdate();

                if (_KhoXuatADOiSelecteds != null && _KhoXuatADOiSelecteds.Count > 0)
                {

                    //foreach (var item in _KhoXuatADOiSelecteds)
                    //{
                    //    listExpMestDetails_.AddRange(this.listExpMestDetails.Where(o => o.MEDI_STOCK_NAME == item.MEDI_STOCK_NAME).ToList());
                    //}

                    //if (listExpMestDetails_ != null && listExpMestDetails_.Count() > 0)
                    //{
                    this.listExpMestDetails = this.listExpMestDetails.Where(o => _KhoXuatADOiSelecteds.Exists(p => p.MEDI_STOCK_NAME == o.MEDI_STOCK_NAME)).ToList();
                    // }
                    gridControlExpMestDetail.DataSource = this.listExpMestDetails;
                }
                else
                {
                    gridControlExpMestDetail.DataSource = this.listExpMestDetails;
                }

                gridControlExpMestDetail.EndUpdate();
                LogSystem.Info("LoadExpMestDetail. End Update");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadDataExpMestBcs(); }));
                }
                else
                {
                    this.LoadDataExpMestBcs();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataExpMestBcs()
        {
            try
            {
                int pageSize = ucPagingExpMestBcs.pagingGrid != null ? ucPagingExpMestBcs.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingExpMestBCS(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingExpMestBcs.Init(PagingExpMestBCS, param, pageSize, gridControlExpMestBcs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PagingExpMestBCS(object param)
        {
            try
            {
                List<HIS_EXP_MEST> listData = new List<HIS_EXP_MEST>();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlExpMestBcs.DataSource = null;
                HisExpMestFilter bcsFilter = new HisExpMestFilter();
                bcsFilter.ORDER_FIELD = "EXP_MEST_CODE";
                bcsFilter.ORDER_DIRECTION = "DESC";
                bcsFilter.IMP_MEDI_STOCK_ID = this.stock.ID;
                bcsFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS;
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, bcsFilter, paramCommon);
                if (apiResult != null)
                {
                    listData = (List<HIS_EXP_MEST>)apiResult.Data;
                    if (listData != null)
                    {
                        rowCount = listData.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
                gridControlExpMestBcs.BeginUpdate();
                gridControlExpMestBcs.DataSource = listData;
                gridControlExpMestBcs.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadExpMestPresCabinet();
                this.LoadExpMestCompensation();
                this.LoadExpMestDetail();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultValue();
                this.LoadExpMestPresCabinet();
                this.LoadExpMestCompensation();
                this.LoadExpMestDetail();
                this.LoadData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
                _KhoXuatADOiSelecteds = new List<MetyMatyADO>();
                cboKhoXuat.ShowPopup();
                cboKhoXuat.ClosePopup();
                cboKhoXuat.Text = "";
                cboKhoXuat.EditValue = null;

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                lastRowHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this._KhoXuatADOiSelecteds != null && this._KhoXuatADOiSelecteds.Count > 0)
                {

                    listExpMestDetails_ = this.listExpMestDetails.Where(o => this._KhoXuatADOiSelecteds.Exists(p => p.MEDI_STOCK_NAME == o.MEDI_STOCK_NAME)).ToList();
                }
                else
                {
                    listExpMestDetails_ = this.listExpMestDetails;
                }
                List<MetyMatyADO> checkDatas = listExpMestDetails_ != null ? listExpMestDetails_.Where(o => o.IsCheck).ToList() : null;
                if (checkDatas == null || checkDatas.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn thuốc/vật tư cần bù", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                var existsError = checkDatas.Where(o => o.REQ_AMOUNT > o.AMOUNT).ToList();
                if (existsError == null || existsError.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư có số lượng yêu cầu bù lớn hơn số lượng được phép bù: {0}", String.Join(",", existsError.Select(s => s.METY_MATY_NAME).ToList()));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                existsError = checkDatas.Where(o => o.REQ_AMOUNT <= 0).ToList();
                if (existsError == null || existsError.Count > 0)
                {
                    string message = String.Format("Các thuốc/vật tư có số lượng yêu cầu bù nhỏ hơn hoặc bằng 0: {0}", String.Join(",", existsError.Select(s => s.METY_MATY_NAME).ToList()));
                    XtraMessageBox.Show(message, "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                if (checkDatas.Any(a => String.IsNullOrWhiteSpace(a.MEDI_STOCK_NAME)) && cboMediStock.EditValue == null)
                {
                    XtraMessageBox.Show("Tồn tại thuốc chưa được thiết lập kho xuất. Yêu cầu bạn chọn kho xuất", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                CabinetBaseCompensationSDO sdo = new CabinetBaseCompensationSDO();
                sdo.CabinetMediStockId = stock.ID;
                if (cboReasonRequired.EditValue != null)
                    sdo.ExpMestReasonId = Int64.Parse(cboReasonRequired.EditValue.ToString());
                if (cboMediStock.EditValue != null)
                {
                    sdo.MediStockId = Convert.ToInt64(cboMediStock.EditValue);
                }
                sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                foreach (var item in checkDatas)
                {
                    if (item.TYPE == TYPE_METY)
                    {
                        if (sdo.MedicineTypes == null) sdo.MedicineTypes = new List<BaseMedicineTypeSDO>();
                        BaseMedicineTypeSDO mt = new BaseMedicineTypeSDO();
                        mt.Amount = item.REQ_AMOUNT;
                        if (item.ExpMestMedicines != null && item.ExpMestMedicines.Count > 0)
                        {
                            mt.ExpMestMedicineIds = item.ExpMestMedicines.Select(s => s.ID).ToList();
                        }
                        if (item.ExpMestMetyReqs != null && item.ExpMestMetyReqs.Count > 0)
                        {
                            mt.ExpMestMetyReqIds = item.ExpMestMetyReqs.Select(s => s.ID).ToList();
                        }
                        mt.MedicineTypeId = item.METY_MATY_ID;
                        sdo.MedicineTypes.Add(mt);
                    }
                    else if (item.TYPE == TYPE_MATY)
                    {
                        if (sdo.MaterialTypes == null) sdo.MaterialTypes = new List<BaseMaterialTypeSDO>();
                        BaseMaterialTypeSDO mt = new BaseMaterialTypeSDO();
                        mt.Amount = item.REQ_AMOUNT;
                        if (item.ExpMestMaterials != null && item.ExpMestMaterials.Count > 0)
                        {
                            mt.ExpMestMaterialIds = item.ExpMestMaterials.Select(s => s.ID).ToList();
                        }
                        if (item.ExpMestMatyReqs != null && item.ExpMestMatyReqs.Count > 0)
                        {
                            mt.ExpMestMatyReqIds = item.ExpMestMatyReqs.Select(s => s.ID).ToList();
                        }
                        mt.MaterialTypeId = item.METY_MATY_ID;
                        sdo.MaterialTypes.Add(mt);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                List<HIS_EXP_MEST> rs = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/BaseCompensationCreate", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null && rs.Count > 0)
                {
                    success = true;

                    btnRefresh_Click(null, null);

                    cboKhoXuat.Text = "";
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPresCabinet_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HisExpMestADO row = (HisExpMestADO)gridViewPresCabinet.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "IsCheck")
                    {
                        if (row.IsAllowCheck)
                        {
                            e.RepositoryItem = repositoryItemCheckPresCabinet_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckPresCabinet_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPresCabinet_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HisExpMestADO row = (HisExpMestADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
                {
                    if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(row.TDL_INTRUCTION_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPresCabinet_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            WaitingManager.Show();
                            gridColumn_PresCabinet_Check.Image = imageListIcon.Images[5];
                            gridControlPreCabinet.BeginUpdate();
                            if (this.listPresCabinet == null)
                                this.listPresCabinet = new List<HisExpMestADO>();
                            if (isCheckAllPres)
                            {
                                listPresCabinet.ForEach(o => o.IsCheck = o.IsAllowCheck);
                                isCheckAllPres = false;
                            }
                            else
                            {
                                gridColumn_PresCabinet_Check.Image = imageListIcon.Images[6];
                                listPresCabinet.ForEach(o => o.IsCheck = false);
                                isCheckAllPres = true;
                            }
                            //_KhoXuatADOiSelecteds = new List<MetyMatyADO>();
                            //ResetCombo(cboKhoXuat);
                            //cboKhoXuat.Text = "";
                            gridControlPreCabinet.EndUpdate();
                            this.LoadExpMestDetail();
                            WaitingManager.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HisExpMestADO row = (HisExpMestADO)gridViewCompensation.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "IsCheck")
                    {
                        if (row.IsAllowCheck)
                        {
                            e.RepositoryItem = repositoryItemCheckCompensation_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckCompensation_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HisExpMestADO row = (HisExpMestADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(row.CREATE_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewCompensation_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            WaitingManager.Show();
                            gridColumn_Compensation_Check.Image = imageListIcon.Images[5];
                            gridControlCompensation.BeginUpdate();
                            if (this.listCompensation == null)
                                this.listCompensation = new List<HisExpMestADO>();
                            if (isCheckAllCom)
                            {
                                this.listCompensation.ForEach(o => o.IsCheck = o.IsAllowCheck);
                                isCheckAllCom = false;
                            }
                            else
                            {
                                gridColumn_Compensation_Check.Image = imageListIcon.Images[6];
                                this.listCompensation.ForEach(o => o.IsCheck = false);
                                isCheckAllCom = true;
                            }
                            gridControlCompensation.EndUpdate();
                            this.LoadExpMestDetail();
                            WaitingManager.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewExpMestDetail.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (e.Column.FieldName == "REQ_AMOUNT")
                    {
                        if (row.IsCheck)
                        {
                            e.RepositoryItem = repositoryItemSpinReqAmount_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemSpinReqAmount_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewExpMestDetail.GetRow(e.RowHandle);
                if (row != null)
                {
                    if (row.IsCheck)
                    {
                        if (row.TYPE == TYPE_MATY)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "REQ_AMOUNT")
                    return;
                var data = (MetyMatyADO)gridViewExpMestDetail.GetRow(e.RowHandle);
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.IsCheck)
                    {
                        if (data.REQ_AMOUNT <= 0)
                        {
                            valid = false;
                            message = "Số lượng yêu cầu bù phải lớn hơn 0";
                        }
                        else if (data.REQ_AMOUNT > data.AMOUNT)
                        {
                            valid = false;
                            message = String.Format("Số lượng yêu cầu bù {0} lớn hơn số lượng cho phép {1}", data.REQ_AMOUNT, data.AMOUNT);
                        }

                    }
                    if (!valid)
                        gridViewExpMestDetail.SetColumnError(gridViewExpMestDetail.FocusedColumn, message);
                    else
                        gridViewExpMestDetail.ClearColumnErrors();
                }
                gridControlExpMestDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestDetail_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumn_ExpMestDetail_Check.Image = imageListIcon.Images[5];
                            gridControlExpMestDetail.BeginUpdate();
                            if (this.listExpMestDetails == null)
                                this.listExpMestDetails = new List<MetyMatyADO>();
                            if (isCheckAllDetail)
                            {
                                this.listExpMestDetails.ForEach(o =>
                                {
                                    o.IsCheck = true;
                                    o.REQ_AMOUNT = o.AMOUNT;
                                });
                                isCheckAllDetail = false;
                            }
                            else
                            {
                                gridColumn_ExpMestDetail_Check.Image = imageListIcon.Images[6];
                                this.listExpMestDetails.ForEach(o =>
                                {
                                    o.IsCheck = false;
                                    o.REQ_AMOUNT = 0;
                                });
                                isCheckAllDetail = true;
                            }
                            gridControlExpMestDetail.EndUpdate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewExpMestBcs_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (HIS_EXP_MEST)gridViewExpMestBcs.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "DELETE")
                    {
                        if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                            || data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                            e.RepositoryItem = repositoryItemButtonDelete_Enable;
                        else
                            e.RepositoryItem = repositoryItemButtonDelete_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBcs_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                HIS_EXP_MEST row = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(row.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MEDI_STOCK_NAME")
                    {
                        var mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == row.MEDI_STOCK_ID);
                        if (mediStock != null)
                        {
                            e.Value = mediStock.MEDI_STOCK_NAME;
                        }
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        //trang: dang gui YC : màu vàng
                        if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu xanh
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (row.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestBcs_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS_EXP_MEST row = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                LoadBcsDetail(row);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBcsDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                MetyMatyADO row = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (row != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBcsDetail(HIS_EXP_MEST row)
        {
            try
            {
                List<MetyMatyADO> listData = new List<MetyMatyADO>();
                if (row != null)
                {
                    HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = row.ID;
                    List<HIS_EXP_MEST_MATY_REQ> matyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyFilter, null);

                    HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = row.ID;
                    List<HIS_EXP_MEST_METY_REQ> metyReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, null);

                    if (metyReqs != null && metyReqs.Count > 0)
                    {
                        var Groups = metyReqs.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                            MetyMatyADO ado = new MetyMatyADO();
                            ado.AMOUNT = group.Sum(s => s.AMOUNT);
                            ado.METY_MATY_ID = group.Key;
                            if (medicineType != null)
                            {
                                ado.METY_MATY_CODE = medicineType.MEDICINE_TYPE_CODE;
                                ado.METY_MATY_NAME = medicineType.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = medicineType.SERVICE_UNIT_NAME;
                            }
                            ado.TYPE = TYPE_METY;
                            listData.Add(ado);
                        }
                    }

                    if (matyReqs != null && matyReqs.Count > 0)
                    {
                        var Groups = matyReqs.GroupBy(g => g.MATERIAL_TYPE_ID).ToList();
                        foreach (var group in Groups)
                        {
                            V_HIS_MATERIAL_TYPE materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == group.Key);
                            MetyMatyADO ado = new MetyMatyADO();
                            ado.AMOUNT = group.Sum(s => s.AMOUNT);
                            ado.METY_MATY_ID = group.Key;
                            if (materialType != null)
                            {
                                ado.METY_MATY_CODE = materialType.MATERIAL_TYPE_CODE;
                                ado.METY_MATY_NAME = materialType.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            }
                            ado.TYPE = TYPE_MATY;
                            listData.Add(ado);
                        }
                    }
                }
                gridControlBcsDetail.BeginUpdate();
                gridControlBcsDetail.DataSource = listData;
                gridControlBcsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataRow = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                if (dataRow != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Bạn có chắc muốn hủy dữ liệu?",
                        "Thông báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        HisExpMestSDO sdo = new HisExpMestSDO();
                        sdo.ExpMestId = dataRow.ID;
                        sdo.ReqRoomId = this.currentModuleBase.RoomId;
                        if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/BaseCompensationDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadData();
                            }
                        }
                        else if (dataRow.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__BASE)
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>("/api/HisExpMest/CompensationByBaseDelete", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadData();
                            }
                        }
                        else
                        {
                            var apiresul = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_DELETE, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                            if (apiresul)
                            {
                                success = true;
                                LoadData();
                            }
                        }

                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.expMestPrint = (HIS_EXP_MEST)gridViewExpMestBcs.GetFocusedRow();
                if (this.expMestPrint != null)
                {
                    MOS.Filter.HisExpMestViewFilter filter = new HisExpMestViewFilter();
                    filter.ID = this.expMestPrint.ID;
                    var expMests = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    PrintAggregateExpMest(expMests.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestBcs)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlExpMestBcs.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle)
                        {
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                long stt = (long)view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_ID");
                                var name = BackendDataWorker.Get<HIS_EXP_MEST_STT>().FirstOrDefault(p => p.ID == stt);
                                if (name != null)
                                    text = name.EXP_MEST_STT_NAME;
                                else
                                    text = "";
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPrintPhieuXuatBCS(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("MPS000215", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case "MPS000215":
                            InPhieuXuatBCS(ref result, printTypeCode, fileName);
                            break;
                        case "MPS000254":
                            MPS000254(ref result, printTypeCode, fileName);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DC_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCGNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCHTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TCs { get; set; }

        V_HIS_EXP_MEST _BcsExpMest { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_TREATMENT> ListTreatment { get; set; }
        HisExpMestBcsMoreInfoSDO MoreInfo { get; set; }
        public bool IsReasonRequired { get; private set; }

        private void InPhieuXuatBCS(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                long keyOrder = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODER_OPTION));
                #region TT Chung
                WaitingManager.Show();
                HisExpMestViewFilter bcsFilter = new HisExpMestViewFilter();
                bcsFilter.ID = this.expMestPrint.ID;
                var listBcsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, bcsFilter, null);
                if (listBcsExpMest == null || listBcsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                _BcsExpMest = listBcsExpMest.First();

                CommonParam param = new CommonParam();

                _ExpMestMetyReqs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReqs = new List<HIS_EXP_MEST_MATY_REQ>();

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                ListTreatment = new List<HIS_TREATMENT>();
                List<long> treatmentIds = new List<long>();
                MoreInfo = null;

                if (_BcsExpMest != null)
                {
                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/get", ApiConsumers.MosConsumer, metyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMetyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_ID = _BcsExpMest.ID;
                    _ExpMestMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/get", ApiConsumers.MosConsumer, matyReqFilter, param);
                    treatmentIds.AddRange(_ExpMestMatyReqs.Select(s => s.TREATMENT_ID ?? 0).ToList());

                    if (_BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || _BcsExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = _BcsExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    }

                    if (treatmentIds != null && treatmentIds.Count > 0)
                    {
                        int skip = 0;
                        while (treatmentIds.Count - skip > 0)
                        {
                            var listIds = treatmentIds.Skip(skip).Take(100).ToList();
                            skip += 100;

                            MOS.Filter.HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                            treatFilter.IDs = listIds;
                            var treat = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                            if (treat != null && treat.Count > 0)
                            {
                                ListTreatment.AddRange(treat);
                            }
                        }
                    }

                    if (_BcsExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES
                        || _BcsExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL)
                    {
                        HisExpMestBcsMoreInfoFilter moreFilter = new HisExpMestBcsMoreInfoFilter();
                        moreFilter.BCS_EXP_MEST_ID = _BcsExpMest.ID;
                        MoreInfo = new BackendAdapter(new CommonParam()).Get<HisExpMestBcsMoreInfoSDO>("api/HisExpMest/GetBcsMoreInfo", ApiConsumers.MosConsumer, moreFilter, null);
                    }
                }
                #endregion

                {
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();
                    List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();

                    #region --- Xu Ly Tach GN_HT -----
                    if (_ExpMestMetyReqs != null && _ExpMestMetyReqs.Count > 0)
                    {
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                        var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                        bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);


                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in _ExpMestMetyReqs)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCGNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCHTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                                {
                                    _ExpMestMetyReq_TCs.Add(item);
                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    WaitingManager.Hide();
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("MPS000254", delegatePrintTemplate);

                    #region ----VatTu----
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 _ExpMestMatyReqs,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.vattu,
                  ListTreatment,
                  MoreInfo,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region ----- Thuong ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.thuong,
                  ListTreatment,
                  MoreInfo,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- TC ----
                    if (_ExpMestMetyReq_TCs != null && _ExpMestMetyReq_TCs.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 _BcsExpMest,
                 _ExpMestMedicines,
                 null,
                 _ExpMestMetyReq_TCs,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                  MPS.Processor.Mps000215.PDO.keyTitles.tienchat,
                  ListTreatment,
                  MoreInfo,
                  keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MPS000254(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_BcsExpMest != null ? _BcsExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GOP GN HT -----
                    if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GN_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.tonghop,
                 ListTreatment,
                 MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                    #region ---- GOP DC GN HT -----
                    if (_ExpMestMetyReq_DC_GN_HTs != null && _ExpMestMetyReq_DC_GN_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDODC = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                      (
                        this._BcsExpMest,
                       _ExpMestMedicines,
                       _ExpMestMetyReq_DC_GN_HTs,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                       BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                       BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000254.PDO.keyTitles.tonghopHc,
                        ListTreatment,
                    MoreInfo
                        );

                        MPS.ProcessorBase.Core.PrintData PrintDataDC = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintDataDC);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_GNs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.gaynghien,
                 ListTreatment,
                 MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT -----
                    if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                _BcsExpMest,
                _ExpMestMedicines,
                _ExpMestMetyReq_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.huongthan,
                 ListTreatment,
                 MoreInfo
                 );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCGN ----
                    if (_ExpMestMetyReq_DCGNs != null && _ExpMestMetyReq_DCGNs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._BcsExpMest,
                            _ExpMestMedicines,
                            _ExpMestMetyReq_DCGNs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcGayNghien,
                             ListTreatment,
                         MoreInfo
                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCHT -----
                    if (_ExpMestMetyReq_DCHTs != null && _ExpMestMetyReq_DCHTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                                  this._BcsExpMest,
                            _ExpMestMedicines,
                            _ExpMestMetyReq_DCHTs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcHuongThan,
                             ListTreatment,
                         MoreInfo
                             );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ----- TD -----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_TDs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocdoc,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX -----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_PXs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocphongxa,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO -----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_COs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Corticoid,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT -----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO(
                        _BcsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMetyReq_DTs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000254.PDO.keyTitles.DichTruyen,
                        ListTreatment,
                        MoreInfo
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS -----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                    (
                        _BcsExpMest,
                        _ExpMestMedicines,
                        _ExpMestMetyReq_KSs,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000254.PDO.keyTitles.KhangSinh,
                         ListTreatment,
                         MoreInfo
                     );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO -----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
            _BcsExpMest,
            _ExpMestMedicines,
            _ExpMestMetyReq_LAOs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Lao,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FIND()");
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("REFRESH()");
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SAVE()
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


        public void btnTimKiemS_()
        {

            try
            {
                btnTimKiemS_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repositoryItemCheckPresCabinet_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridViewPresCabinet.PostEditor();
                HisExpMestADO row = (HisExpMestADO)gridViewPresCabinet.GetFocusedRow();
                if (row != null)
                {
                    this.LoadExpMestDetail();
                }
                gridControlPreCabinet.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckCompensation_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridViewCompensation.PostEditor();
                HisExpMestADO row = (HisExpMestADO)gridViewCompensation.GetFocusedRow();
                if (row != null)
                {
                    this.LoadExpMestDetail();
                }
                gridControlCompensation.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckExpMestDetail_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewExpMestDetail.PostEditor();
                MetyMatyADO row = (MetyMatyADO)gridViewExpMestDetail.GetFocusedRow();
                if (row != null)
                {
                    if (row.IsCheck)
                    {
                        row.REQ_AMOUNT = row.AMOUNT;
                    }
                    else
                    {
                        row.REQ_AMOUNT = 0;
                    }
                }
                gridControlExpMestDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST currentAggExpMest = null;
        internal void PrintAggregateExpMest(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {
                //Review
                this.currentAggExpMest = currentAggExpMest;
                DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                barManager1.Form = this;

                ExpMestBCSPopupMenuProcessor processor = new ExpMestBCSPopupMenuProcessor(this.currentAggExpMest, ExpMestBCSMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExpMestBCSMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ExpMestBCSPopupMenuProcessor.PrintType)
                {
                    var moduleType = (ExpMestBCSPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ExpMestBCSPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(Convert.ToInt64(6));
                            break;
                        case ExpMestBCSPopupMenuProcessor.PrintType.PhieuBuCoSo:
                            onClickPrintPhieuXuatBCS(null, null);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormFilter(long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentAggExpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._currentModule.RoomId, this._currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this._currentModule.RoomId, this._currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTimKiemS_Click(object sender, EventArgs e)
        {

            try
            {
                LogSystem.Info("LoadExpMestDetail. End Process");
                LogSystem.Info("LoadExpMestDetail. Begin Update");
                gridControlExpMestDetail.BeginUpdate();

                if (this._KhoXuatADOiSelecteds != null && this._KhoXuatADOiSelecteds.Count > 0)
                {

                    //foreach (var item in _KhoXuatADOiSelecteds)
                    //{
                    //    listExpMestDetails_.AddRange(this.listExpMestDetails.Where(o => o.MEDI_STOCK_NAME == item.MEDI_STOCK_NAME).ToList());
                    //}

                    //if (listExpMestDetails_ != null && listExpMestDetails_.Count() > 0)
                    //{
                    //  this.listExpMestDetails = this.listExpMestDetails.Where(o => this._KhoXuatADOiSelecteds.Exists(p => p.MEDI_STOCK_NAME == o.MEDI_STOCK_NAME)).ToList();
                    // }
                    gridControlExpMestDetail.DataSource = this.listExpMestDetails.Where(o => this._KhoXuatADOiSelecteds.Exists(p => p.MEDI_STOCK_NAME == o.MEDI_STOCK_NAME)).ToList();
                }
                else
                {
                    gridControlExpMestDetail.DataSource = this.listExpMestDetails;
                }

                gridControlExpMestDetail.EndUpdate();
                LogSystem.Info("LoadExpMestDetail. End Update");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboKhoXuat_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {

                    cboKhoXuat.Text = "";
                    _KhoXuatADOiSelecteds.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoXuat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, string DisplayValue, string ValueMember)
        {
            try
            {

                //cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(/*cbo.Properties.DataSource*/null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        private void SelectionGrid__KhoXuat(object sender, EventArgs e)
        {
            try
            {
                _KhoXuatADOiSelecteds = new List<MetyMatyADO>();
                foreach (MetyMatyADO rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        if (!string.IsNullOrEmpty(rv.MEDI_STOCK_NAME) && !string.IsNullOrWhiteSpace(rv.MEDI_STOCK_NAME))
                        {
                            _KhoXuatADOiSelecteds.Add(rv);
                        }
                }
                //  LoadExpMestDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKhoXuat_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                cboKhoXuat.Text = "";
                e.DisplayText = "";
                string khoX = "";
                if (this._KhoXuatADOiSelecteds != null && this._KhoXuatADOiSelecteds.Count > 0)
                {
                    foreach (var item in this._KhoXuatADOiSelecteds)
                    {
                        khoX += item.MEDI_STOCK_NAME + ", ";
                    }
                }
                else
                {
                    khoX = "";
                }
                e.DisplayText = khoX;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (lastRowHandle == -1)
                {
                    lastRowHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (lastRowHandle > edit.TabIndex)
                {
                    lastRowHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
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
