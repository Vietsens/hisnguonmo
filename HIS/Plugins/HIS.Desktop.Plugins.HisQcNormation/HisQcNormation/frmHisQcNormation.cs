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
using HIS.Desktop.Utility;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.HisQcNormation.ADO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisQcNormation.Resources;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.HisQcNormation.HisQcNormation
{
    public partial class frmHisQcNormation : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        long NormationAdoID = -1;
        long? QcTypeID = null;
        long MachineId = 0;
        

        //QcTypeADO currentHisQcType;
        //HIS_QC_NORMATION currentHisQcNormation;
        List<QcTypeADO> lstHisQcType;
        List<HIS_QC_NORMATION> lstHisQcNormation;
        List<HIS_MATERIAL_TYPE> lstHisMaterialType;
        HIS_MACHINE currentHisMachine;
        List<MaterialTypeADO> lstMaterialTypeADO;

        List<QcNormationADO> lstQcNormationADO;

        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        public frmHisQcNormation(Inventec.Desktop.Common.Modules.Module module, HIS_MACHINE HisMachine)
            : base(module)
        {
            InitializeComponent();
            pagingGrid = new PagingGrid();
            this.moduleData = module;
            this.currentHisMachine = HisMachine;
            this.MachineId = HisMachine.ID;
        }

        private void frmHisQcNormation_Load(object sender, EventArgs e)
        {
            try
            {
                LoadlblMachine();
                LoadlstHisMaterialType();

                FillDataToControlQcType();

                InitComboMaterialType(repositoryItemCbo_Material_Type_Name, this.lstHisMaterialType);

                //set ngon ngu
                SetCaptionByLanguagekey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisQcNormation.Resources.Lang", typeof(HIS.Desktop.Plugins.HisQcNormation.HisQcNormation.frmHisQcNormation).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisQcNormation.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisQcNormation.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void InitComboMaterialType(object combobox, List<HIS_MATERIAL_TYPE> MaterialType)
        {
            try
            {//repositoryItemCbo_Material_Type_Name
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("MATERIAL_TYPE_NAME", "", 400, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MATERIAL_TYPE_NAME", "ID", columnInfos, false, 450);
                ControlEditorLoader.Load(combobox, MaterialType, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadlstHisMaterialType()
        {
            try
            {
                lstMaterialTypeADO=new List<MaterialTypeADO>();
                lstHisMaterialType = new List<HIS_MATERIAL_TYPE>();
                CommonParam param = new CommonParam();
                HisMaterialTypeFilter filter = new HisMaterialTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                lstHisMaterialType = new BackendAdapter(param).Get<List<HIS_MATERIAL_TYPE>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GET, ApiConsumers.MosConsumer, filter, param);

                this.lstHisMaterialType = lstHisMaterialType.Where(o => o.IS_CHEMICAL_SUBSTANCE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                if (lstHisMaterialType != null)
                {
                    foreach (var item in lstHisMaterialType)
                    {
                        MaterialTypeADO MaterialType = new MaterialTypeADO();
                        MaterialType.ID = item.ID;
                        MaterialType.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        MaterialType.MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        MaterialType.TDL_SERVICE_UNIT_ID = item.TDL_SERVICE_UNIT_ID;
                        MaterialType.Amount = null;

                        lstMaterialTypeADO.Add(MaterialType);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void FillDataToControlQcType()
        {
            try
            {
                lstHisQcType = new List<QcTypeADO>();
                CommonParam param = new CommonParam();
                HisQcTypeFilter filter = new HisQcTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                gridViewHisQcType.BeginUpdate();
                var lstQcType = new BackendAdapter(param).Get<List<HIS_QC_TYPE>>(HisRequestUriStore.HIS_QC_TYPE_GET, ApiConsumers.MosConsumer, filter, param);

                if (lstQcType != null && lstQcType.Count > 0)
                {
                    foreach (var item in lstQcType)
                    {
                        QcTypeADO TypeAdo = new QcTypeADO();
                        TypeAdo.ID = item.ID;
                        TypeAdo.QC_TYPE_CODE = item.QC_TYPE_CODE;
                        TypeAdo.QC_TYPE_NAME = item.QC_TYPE_NAME;
                        TypeAdo.GROUP_CODE = item.GROUP_CODE;
                        TypeAdo.CheckForType = false;

                        this.lstHisQcType.Add(TypeAdo);
                    }

                    gridViewHisQcType.GridControl.DataSource = this.lstHisQcType;
                }
                gridViewHisQcType.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadlblMachine()
        {
            try
            {
                if (this.currentHisMachine != null)
                {
                    lblMachine.Text = this.currentHisMachine.MACHINE_CODE + " - " + this.currentHisMachine.MACHINE_NAME;
                }
                else 
                {
                    lblMachine.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewHisQcType_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    QcTypeADO pData = (QcTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void chkQcType_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (QcTypeADO)gridViewHisQcType.GetFocusedRow();
                if (row.CheckForType == true)
                {
                    row.CheckForType = false;
                    QcTypeID = null;
                    LoadDefaultQcNormation();
                    return;
                }

                foreach (var item in lstHisQcType)
                {
                    item.CheckForType = false;
                    if (item.ID == row.ID)
                    {
                        QcTypeID = row.ID;
                        item.CheckForType = true;
                    }
                }

                gridControlHisQcType.DataSource = lstHisQcType;
                gridControlHisQcType.RefreshDataSource();
                FilldataQcNormation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void LoadDefaultQcNormation()
        {
            try
            {
                //lstHisQcNormation = new List<HIS_QC_NORMATION>();
                lstQcNormationADO = new List<QcNormationADO>();
                gridViewHisQcNormation.BeginUpdate();
                gridViewHisQcNormation.GridControl.DataSource = lstQcNormationADO;
                gridViewHisQcNormation.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void FilldataQcNormation()
        {
            try
            {
                WaitingManager.Show();
                rowCount = 0;
                dataTotal = 0;
                startPage = 0;
                int numPageSize = 0;

                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingHisQcNormation(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                
                ucPaging1.Init(LoadPagingHisQcNormation, param, numPageSize, this.gridControlHisQcNormation);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn("rowCount: " + rowCount + " dataTotal: " + dataTotal);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPagingHisQcNormation(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;

                lstHisQcNormation = new List<HIS_QC_NORMATION>();
                lstQcNormationADO = new List<QcNormationADO>();

                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_QC_NORMATION>> apiResult = null;
                HisQcNormationFilter filter = new HisQcNormationFilter();
                filter.QC_TYPE_ID = QcTypeID;
                filter.MACHINE_ID = this.MachineId;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridViewHisQcNormation.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_QC_NORMATION>>(HisRequestUriStore.HIS_QC_NORMATION_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Info("apiResult: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    lstHisQcNormation = apiResult.Data;

                    if (lstHisQcNormation != null && lstHisQcNormation.Count > 0)
                    {
                        lstQcNormationADO.AddRange((from r in lstHisQcNormation select new QcNormationADO(r, this.lstMaterialTypeADO)).ToList());
                    }
                }
                QcNormationADO AdoNew = new QcNormationADO();
                AdoNew.ID = NormationAdoID;

                lstQcNormationADO.Add(AdoNew);

                if (lstQcNormationADO != null)
                {
                    gridViewHisQcNormation.GridControl.DataSource = lstQcNormationADO;
                    rowCount = (lstQcNormationADO == null ? 0 : lstQcNormationADO.Count - 1);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                gridViewHisQcNormation.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                gridViewHisQcNormation.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisQcNormation_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                QcNormationADO data = null;
                int dem = lstQcNormationADO.Count - 1;
                
                if (e.RowHandle > -1)
                {
                    data = (QcNormationADO)gridViewHisQcNormation.GetRow(e.RowHandle);
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Add_Delete")
                    {
                        if (e.RowHandle == dem)
                        {
                            e.RepositoryItem = repositoryItemBtn_Material_Type_Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtn_Material_Type_Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisQcNormation_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHisQcNormation_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    QcNormationADO pData = (QcNormationADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewHisQcNormation_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var row = (QcNormationADO)gridViewHisQcNormation.GetFocusedRow();
                if (view.FocusedColumn.FieldName == "MATERIAL_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        List<HIS_MATERIAL_TYPE> MaterialType = new List<HIS_MATERIAL_TYPE>();
                        MaterialType.AddRange(lstHisMaterialType);
                        foreach (var item in lstQcNormationADO)
                        {
                            if (item.MATERIAL_TYPE_ID > 0 && row.MATERIAL_TYPE_ID != item.MATERIAL_TYPE_ID)
                            {
                                MaterialType.Remove(this.lstHisMaterialType.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID));
                            }
                        }
                        InitComboMaterialType(editor, MaterialType);
                        editor.EditValue = row.MATERIAL_TYPE_ID;
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetError(int rowHandle, DevExpress.XtraGrid.Columns.GridColumn column)
        {
            try
            {
                if (column.FieldName == "MATERIAL_TYPE_ID")
                {
                    QcNormationADO data = (QcNormationADO)gridViewHisQcNormation.GetRow(rowHandle);
                    if (data == null)
                        return string.Empty;

                    if (data.MATERIAL_TYPE_ID <= 0)
                    {
                        return "Không có thông tin tên hóa chất.";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return string.Empty;
        }

        private void gridViewHisQcNormation_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridColumn onOrderCol = gridViewHisQcNormation.Columns["AMOUNT"];
                    gridViewHisQcNormation.ClearColumnErrors();
                    var data = (QcNormationADO)gridViewHisQcNormation.GetRow(e.RowHandle);
                    if (data != null && data.MATERIAL_TYPE_ID > 0)
                    {
                        if (data.Amount == null)
                        {
                            // e.Valid = false;
                            gridViewHisQcNormation.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
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
                WaitingManager.Show();
                string ErrorAmount = "";
                bool checkMaterialType = false;
                bool check = false;

                CommonParam param = new CommonParam();
                List<MaterialNormationSDO> MaterialNormations = new List<MaterialNormationSDO>();
                var QcTypeCheck = this.lstHisQcType.FirstOrDefault(o => o.CheckForType == true);
                if (QcTypeCheck == null)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(ResourceMessage.BanChuaChonPhong);
                    check = true;
                }

                Inventec.Common.Logging.LogSystem.Info("lstQcNormationADO: "+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstQcNormationADO), lstQcNormationADO));

                foreach (var item in this.lstQcNormationADO)
                {
                    if (item.MATERIAL_TYPE_ID > 0)
                    {
                        if (item.Amount > 0)
                        {
                            MaterialNormationSDO QcSave = new MaterialNormationSDO();
                            QcSave.MaterialTypeId = item.MATERIAL_TYPE_ID;
                            QcSave.Amount = item.Amount.Value;

                            MaterialNormations.Add(QcSave);
                        }
                        else 
                        {
                            ErrorAmount += this.lstMaterialTypeADO.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID).MATERIAL_TYPE_NAME + ", ";
                        }
                    }
                    else if(item.Amount > 0)
                    {
                        checkMaterialType = true;
                    }
                }

                if (!string.IsNullOrEmpty(ErrorAmount))
                {
                    WaitingManager.Hide();
                    ErrorAmount = ErrorAmount.EndsWith(", ") ? ErrorAmount.Substring(0, ErrorAmount.Length - 2) : ErrorAmount;
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongCoSoLuong, ErrorAmount));
                    check = true;
                }

                if (checkMaterialType)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(ResourceMessage.BanChuaChonHoaChat);
                    check = true;
                }
                HisQcNormationSDO lstSaveQc = new HisQcNormationSDO();
                //if (MaterialNormations != null && MaterialNormations.Count > 0)
                //{
                    lstSaveQc.MachineId = currentHisMachine.ID;
                    lstSaveQc.QcTypeId = QcTypeCheck.ID;
                    lstSaveQc.MaterialNormations = MaterialNormations;
                //}
                //else
                //{
                //    WaitingManager.Hide();
                //    MessageBox.Show(ResourceMessage.BanChuaNhapDanhSachDinhMuc);
                //    check = true;
                //}

                if (!check)
                {

                    var ResultData = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_QC_NORMATION_CREATE_SDO, ApiConsumers.MosConsumer, lstSaveQc, param);
                    WaitingManager.Hide();
                    if (ResultData)
                    {
                        FilldataQcNormation();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, ResultData);
                    #endregion
                }
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void repositoryItemBtn_Material_Type_Add_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                NormationAdoID--;
                QcNormationADO careDetail = new QcNormationADO();
                careDetail.ID = NormationAdoID;

                if (this.lstQcNormationADO == null) this.lstQcNormationADO = new List<QcNormationADO>();
                this.lstQcNormationADO.Add(careDetail);

                gridControlHisQcNormation.BeginUpdate();
                gridControlHisQcNormation.DataSource = this.lstQcNormationADO;
                gridControlHisQcNormation.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_Material_Type_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                QcNormationADO detail = (QcNormationADO)gridViewHisQcNormation.GetFocusedRow();
                if (detail != null)
                {
                    this.lstQcNormationADO.Remove(detail);

                    gridControlHisQcNormation.BeginUpdate();
                    gridControlHisQcNormation.DataSource = this.lstQcNormationADO;
                    gridControlHisQcNormation.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCbo_Material_Type_Name_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as GridLookUpEdit;

                    if (cbo != null && cbo.EditValue != null)
                    {
                        var row = (QcNormationADO)gridViewHisQcNormation.GetFocusedRow();

                         long MaterialTypeId = (long)cbo.EditValue;

                         var data = this.lstQcNormationADO.FirstOrDefault(o => o.ID == row.ID);

                         var MaterialADO = this.lstHisMaterialType.FirstOrDefault(o => o.ID == MaterialTypeId);
                         var ServiceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == MaterialADO.TDL_SERVICE_UNIT_ID);

                         foreach (var item in this.lstQcNormationADO)
                         {
                             if (item.ID == row.ID)
                             {
                                 item.MATERIAL_TYPE_ID = MaterialTypeId;
                                 item.MaterialTypeCode = MaterialADO.MATERIAL_TYPE_CODE;
                                 item.ServiceUnitName = ServiceUnit.SERVICE_UNIT_NAME;
                                 item.AMOUNT = data.AMOUNT;
                             }
                         }

                         if (row.ID == NormationAdoID)
                         {
                             NormationAdoID--;
                             QcNormationADO careDetail = new QcNormationADO();
                             careDetail.ID = NormationAdoID;

                             this.lstQcNormationADO.Add(careDetail);
                         }

                         gridViewHisQcNormation.BeginUpdate();
                         gridViewHisQcNormation.GridControl.DataSource = this.lstQcNormationADO;
                         gridViewHisQcNormation.EndUpdate();
                     }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void repositoryItemTxt_Material_Type_Code_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    if (e.KeyChar == ',' || e.KeyChar == '.')
                    {
                        e.Handled = false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

    }
}
