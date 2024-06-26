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
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ExpMestHcsCreate.Base;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.ExpMestHcsCreate
{
    public partial class UCExpMestHcsCreate : UserControl
    {

        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule = null;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        int rowCount1 = 0;
        int dataTotal1 = 0;
        int startPage1 = 0;

        int positionHandleLeft = -1;

        List<V_HIS_MEDICINE_TYPE> listMedicineType = null;
        List<V_HIS_MATERIAL_TYPE> listMaterialType = null;

        V_HIS_MEDI_STOCK mediStock = null;

        #endregion

        #region Contructor

        public UCExpMestHcsCreate()
        {
            InitializeComponent();
        }

        public UCExpMestHcsCreate(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                if (this.mediStock != null && this.mediStock.IS_CABINET != 1)
                {
                    for (int i = 0; i < layoutControl1.Controls.Count; i++)
                    {
                        layoutControl1.Controls[i].Enabled = false;
                    }
                    DevExpress.XtraEditors.XtraMessageBox.Show("Kho không phải là tủ trực", "Thông báo");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCExpMestHcsCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                listMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();

                ValidationSingleControl(cboMediStock);
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpMestHcsCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestHcsCreate.UCExpMestHcsCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn15.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn16.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn17.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.gridColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreate.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.btnCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxStatus.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.groupBoxStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditNotDone.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.checkEditNotDone.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditDone.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.checkEditDone.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCExpMestHcsCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.Text = "";
                DateTime? TimeNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                checkEditNotDone.CheckState = CheckState.Checked;
                dtImpTimeFrom.EditValue = TimeNow;
                dtImpTimeTo.EditValue = TimeNow;

                LoadMediStock();
                LoadDataImpMestToGrid();
                LoadDataExpMestToGrid();

                gridControlExp.DataSource = null;
                gridControlImp.DataSource = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadMediStock()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> mediStockList = new List<V_HIS_MEDI_STOCK>();
                var data = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.MEDI_STOCK_ID == this.mediStock.ID).ToList();
                if (data != null && data.Count > 0)
                {
                    mediStockList = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => data.Select(p => p.ROOM_ID).Contains(o.ROOM_ID) && o.IS_BUSINESS != 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, mediStockList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataImpMestToGrid()
        {
            try
            {
                WaitingManager.Show();

                List<ImpMestADO> listImpMestAdo = new List<ImpMestADO>();
                CommonParam param = new CommonParam();
                HisImpMestViewFilter hisImpMestViewFilter = new HisImpMestViewFilter();
                SetFilterImpMest(ref hisImpMestViewFilter);
                var listImpMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisImpMestViewFilter, param);

                if (listImpMest != null && listImpMest.Count > 0)
                {
                    foreach (var item in listImpMest)
                    {
                        var ado = new ImpMestADO(item);
                        listImpMestAdo.Add(ado);
                    }
                }
                gridControlImport.BeginUpdate();
                gridControlImport.DataSource = listImpMestAdo;
                gridControlImport.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void SetFilterImpMest(ref HisImpMestViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text;
                filter.IMP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL;

                filter.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT;
                //filter.IMP_MEST_STT_IDs = stt;
                filter.MEDI_STOCK_ID = this.mediStock.ID;
                //filter.DATA_DOMAIN_FILTER = true;
                //filter.WORKING_ROOM_ID = this.currentModule.RoomId;
                if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                    filter.IMP_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                    filter.IMP_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMdd") + "000000");
                if (checkEditDone.Checked || checkEditNotDone.Checked)
                {
                    filter.HAS_XHTT_OR_XBTT = checkEditDone.Checked ? true : false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataExpMestToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pagingSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                GridPagingExport(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPagingExport, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPagingExport(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>> apiResult = null;
                MOS.Filter.HisExpMestViewFilter hisExpMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                hisExpMestViewFilter.ORDER_DIRECTION = "DESC";
                hisExpMestViewFilter.ORDER_FIELD = "EXP_MEST_CODE";
                hisExpMestViewFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HCS;
                gridViewExport.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>
                    (ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisExpMestViewFilter, paramCommon);
                if (apiResult != null)
                {
                    var listExpMest = apiResult.Data;
                    if (listExpMest != null && listExpMest.Count > 0)
                    {
                        gridControlExport.DataSource = listExpMest;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlExport.DataSource = null;
                        rowCount = (listExpMest == null ? 0 : listExpMest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewExport.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpToGrid(V_HIS_EXP_MEST expMestData)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<ExpDetailADO> listExpDetailAdo = new List<ExpDetailADO>();
                //Get ImpMestMedicine
                HisExpMestMetyReqViewFilter metyFilter = new HisExpMestMetyReqViewFilter();
                metyFilter.EXP_MEST_ID = expMestData.ID;

                var listExpMetyReq = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, metyFilter, param);

                if (listExpMetyReq != null && listExpMetyReq.Count > 0)
                {
                    foreach (var item in listExpMetyReq)
                    {
                        var ado = new ExpDetailADO(item);
                        listExpDetailAdo.Add(ado);
                    }
                }

                //Get ImpMestMaterial
                HisExpMestMatyReqViewFilter matyFilter = new HisExpMestMatyReqViewFilter();
                matyFilter.EXP_MEST_ID = expMestData.ID;

                var listExpMatyReq = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, matyFilter, param);

                if (listExpMatyReq != null && listExpMatyReq.Count > 0)
                {
                    foreach (var item in listExpMatyReq)
                    {
                        var ado = new ExpDetailADO(item);
                        listExpDetailAdo.Add(ado);
                    }
                }

                var groupImp = listExpDetailAdo.GroupBy(o => new { o.IsMedicine, o.ID }).ToList();
                List<ExpDetailADO> listExpDetailAdoGroup = new List<ExpDetailADO>();

                foreach (var item in groupImp)
                {
                    ExpDetailADO ado = new ExpDetailADO();
                    ado.AMOUNT = item.Sum(o => o.AMOUNT);
                    ado.IsMedicine = item.First().IsMedicine;
                    ado.ID = item.First().ID;
                    ado.DESCRIPTION = item.First().DESCRIPTION;

                    listExpDetailAdoGroup.Add(ado);
                }

                gridControlExp.BeginUpdate();
                gridControlExp.DataSource = listExpDetailAdoGroup;
                gridControlExp.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataToImpGrid(ImpMestADO impMestData)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<ImpDetailADO> listImpDetailAdo = new List<ImpDetailADO>();
                //Get ImpMestMedicine
                HisImpMestMedicineViewFilter medicineFilter = new HisImpMestMedicineViewFilter();
                medicineFilter.IMP_MEST_ID = impMestData.ID;

                var listImpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, param);

                if (listImpMestMedicine != null && listImpMestMedicine.Count > 0)
                {
                    foreach (var item in listImpMestMedicine)
                    {
                        var ado = new ImpDetailADO(item);
                        listImpDetailAdo.Add(ado);
                    }
                }

                //Get ImpMestMaterial
                HisImpMestMaterialViewFilter materialFilter = new HisImpMestMaterialViewFilter();
                materialFilter.IMP_MEST_ID = impMestData.ID;

                var listImpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, param);

                if (listImpMestMaterial != null && listImpMestMaterial.Count > 0)
                {
                    foreach (var item in listImpMestMaterial)
                    {
                        var ado = new ImpDetailADO(item);
                        listImpDetailAdo.Add(ado);
                    }
                }

                var groupImp = listImpDetailAdo.GroupBy(o => new { o.IsMedicine, o.ID }).ToList();
                List<ImpDetailADO> listImpDetailAdoGroup = new List<ImpDetailADO>();

                foreach (var item in groupImp)
                {
                    ImpDetailADO ado = new ImpDetailADO();
                    ado.NAME = item.First().NAME;
                    ado.PACKAGE_NUMBER = item.First().PACKAGE_NUMBER;
                    ado.SERVICE_UNIT_NAME = item.First().SERVICE_UNIT_NAME;
                    ado.CODE = item.First().CODE;
                    ado.AMOUNT = item.Sum(o => o.AMOUNT);
                    ado.IsMedicine = item.First().IsMedicine;
                    ado.ID = item.First().ID;

                    listImpDetailAdoGroup.Add(ado);
                }

                gridControlImp.BeginUpdate();
                gridControlImp.DataSource = listImpDetailAdoGroup;
                gridControlImp.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
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

        #endregion

        #region Phim Tat

        public void Search()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Refresh()
        {
            try
            {
                if (btnRefresh.Enabled)
                {
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void dtImpTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtImpTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtImpTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkEditDone.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataImpMestToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataImpMestToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var impMestSource = (List<ImpMestADO>)gridControlImport.DataSource;               

                bool success = false;
                positionHandleLeft = -1;

                if (!dxValidationProvider1.Validate())
                    return;
                if (impMestSource == null || (impMestSource != null && impMestSource.Count(o => o.Check) <= 0))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn phiếu nhập nào");
                    return;
                }

                var impMestList = impMestSource.Where(o => o.Check).ToList();

                WaitingManager.Show();

                CommonParam param = new CommonParam();
                HisExpMestHcsSDO hisExpMestHcsSDO = new HisExpMestHcsSDO();
                hisExpMestHcsSDO.ImpMediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString());
                hisExpMestHcsSDO.ImpMestMobaIds = impMestList.Select(o => o.ID).ToList();
                hisExpMestHcsSDO.ReqRoomId = this.currentModule.RoomId;
                hisExpMestHcsSDO.MediStockId = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId).ID;

                var hisExpMestResultSDO = new BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/HcsCreate", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestHcsSDO, param);

                if (hisExpMestResultSDO != null)
                {
                    success = true;
                    LoadDataImpMestToGrid();
                    LoadDataExpMestToGrid();
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }

        }

        #endregion

        #region Grid view Import

        private void gridViewImport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ImpMestADO data = (ImpMestADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Check")
                    {
                        e.RepositoryItem = data.XBTT_Or_XHTT ? checkEnable : checkDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }

        }

        private void gridViewImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewImport_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (ImpMestADO)gridViewImport.GetFocusedRow();
                if (row != null)
                {
                    LoadDataToImpGrid(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewImport_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var data = (ImpMestADO)gridViewImport.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.XBTT_Or_XHTT)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        #endregion

        #region Grid view Export

        private void gridViewExport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewExport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExport_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST)gridViewExport.GetFocusedRow();
                if (row != null)
                {
                    LoadDataExpToGrid(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid view Imp

        private void gridViewImp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewImp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpDetailADO data = (ImpDetailADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImp_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        #endregion

        #region Grid view Exp

        private void gridViewExp_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewExp_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExpDetailADO data = (ExpDetailADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage1;
                        }
                        if (e.Column.FieldName == "CODE")
                        {
                            if (data.IsMedicine)
                            {
                                e.Value = listMedicineType.FirstOrDefault(o => o.ID == data.ID).MEDICINE_TYPE_CODE;
                            }
                            else
                            {
                                e.Value = listMaterialType.FirstOrDefault(o => o.ID == data.ID).MATERIAL_TYPE_CODE;
                            }
                        }
                        if (e.Column.FieldName == "NAME")
                        {
                            if (data.IsMedicine)
                            {
                                e.Value = listMedicineType.FirstOrDefault(o => o.ID == data.ID).MEDICINE_TYPE_NAME;
                            }
                            else
                            {
                                e.Value = listMaterialType.FirstOrDefault(o => o.ID == data.ID).MATERIAL_TYPE_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExp_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        #endregion

        #region Validate

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

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
      
    }
}
