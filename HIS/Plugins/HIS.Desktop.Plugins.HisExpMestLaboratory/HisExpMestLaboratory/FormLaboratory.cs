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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisExpMestLaboratory.ADO;
using HIS.Desktop.Plugins.HisExpMestLaboratory.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.HisExpMestLaboratory.HisExpMestLaboratory
{
    public partial class FormLaboratory : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private UC.UcQc UcQc;
        private UC.UcTest UcTest;

        private int positionHandle = -1;
        private int MaxReq = 500;
        private List<MaterialTypeADO> ListDataSource;
        private ExpMestTestResultSDO expMestTestResultSDO;
        private List<long> lstSereServIds;

        public FormLaboratory(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.moduleData = moduleData;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormLaboratory_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitDataCbo();
                ValidateCboMediStock();
                btnPrint.Enabled = false;
                chkTypeTest.Checked = true;
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

        private void InitDataCbo()
        {
            try
            {
                InitDataCboMediStock();
                InitDataCboMachine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboMachine()
        {
            try
            {
                List<HIS_MACHINE> listDataMachine = BackendDataWorker.Get<HIS_MACHINE>().Where(o => o.IS_ACTIVE == 1 && (o.ROOM_IDS != null && o.ROOM_IDS.Contains(moduleData.RoomId.ToString()))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMachine, listDataMachine, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDataCboMediStock()
        {
            try
            {
                List<V_HIS_MEST_ROOM> mestRoom = BackendDataWorker.Get<V_HIS_MEST_ROOM>().Where(o => o.ROOM_ID == moduleData.RoomId).ToList();
                List<HIS_MEDI_STOCK> listDataStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().Where(o => o.IS_CABINET == 1 && o.IS_ACTIVE == 1 && (mestRoom == null || mestRoom.Count <= 0 || mestRoom.Select(s => s.MEDI_STOCK_ID).Contains(o.ID))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMediStock, listDataStock, controlEditorADO);

                if (listDataStock != null && listDataStock.Count == 1)
                {
                    cboMediStock.EditValue = listDataStock.First().ID;
                }
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Delete.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_MaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_MaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_NationalName.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_NationalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_ManufacturerName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_AvailableAmount.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_AvailableAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Amount.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.gc_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.layoutControl.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAccept.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.btnAccept.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMachine.Properties.NullText = Inventec.Common.Resource.Get.Value("FormLaboratory.cboMachine.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkQc.Properties.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.chkQc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTypeTest.Properties.Caption = Inventec.Common.Resource.Get.Value("FormLaboratory.chkTypeTest.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("FormLaboratory.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStock.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.lciMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.lciNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciType.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.lciType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMachine.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.lciMachine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("FormLaboratory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTypeTest_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTypeTest.Checked)
                {
                    EnableControlMachine(false);
                    chkQc.Checked = false;
                    UcTest = new UC.UcTest();
                    layoutControl.Controls.Clear();
                    layoutControl.Controls.Add(UcTest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkQc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                dxValidationProvider.RemoveControlError(cboMachine);
                dxValidationProvider.SetValidationRule(cboMachine, null);
                lciMachine.AppearanceItemCaption.ForeColor = Color.Black;
                EnableControlMachine(false);
                if (chkQc.Checked)
                {
                    EnableControlMachine(true);
                    chkTypeTest.Checked = false;
                    UcQc = new UC.UcQc();
                    layoutControl.Controls.Clear();
                    layoutControl.Controls.Add(UcQc);
                    lciMachine.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateComboMachine();
                    if (cboMachine.EditValue != null)
                    {
                        UcQc.ReloadCboTypeByMachineId(Inventec.Common.TypeConvert.Parse.ToInt64(cboMachine.EditValue.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateComboMachine()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = cboMachine;
                validate.ErrorText = "Dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboMachine, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateCboMediStock()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = cboMediStock;
                validate.ErrorText = "Dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboMediStock, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                bool valid = true;
                valid = dxValidationProvider.Validate() && valid;

                if (chkTypeTest.Checked && UcTest != null)
                {
                    valid = UcTest.ValidateUc() && valid;
                    if (!valid)
                    {
                        return;
                    }

                    GetDataMaterialBySereServTein();
                }
                else if (chkQc.Checked && UcQc != null)
                {
                    valid = UcQc.ValidateUc() && valid;
                    if (!valid)
                    {
                        return;
                    }

                    GetDataMaterialByQcType();
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
                if (!btnSave.Enabled || !btnSave.Visible)
                    return;

                gridView.PostEditor();
                List<MaterialTypeADO> listDataSource = (List<MaterialTypeADO>)gridControl.DataSource;
                if (listDataSource != null && listDataSource.Count > 0)
                {
                    if (CheckData(listDataSource))
                    {
                        return;
                    }

                    WaitingManager.Show();
                    ExpMestTestSDO sdo = new ExpMestTestSDO();
                    sdo.Description = txtNote.Text;
                    if (chkQc.Checked)
                    {
                        sdo.ExpMestTestType = ExpMestTestTypeEnum.QC;
                    }
                    else if (chkTypeTest.Checked)
                    {
                        sdo.ExpMestTestType = ExpMestTestTypeEnum.TEST;
                        sdo.SereServIds = lstSereServIds;
                    }

                    if (cboMachine.EditValue != null)
                    {
                        sdo.MachineId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMachine.EditValue.ToString());
					}
					

                    if (cboMediStock.EditValue != null)
                    {
                        sdo.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString());
                    }

                    sdo.RequestRoomId = this.moduleData.RoomId;

                    sdo.Materials = new List<ExpMaterialTypeSDO>();
                    foreach (var item in listDataSource)
                    {
                        ExpMaterialTypeSDO materialSdo = new ExpMaterialTypeSDO();
                        materialSdo.Amount = item.AMOUNT;
                        materialSdo.MaterialTypeId = item.ID;
                        sdo.Materials.Add(materialSdo);
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listDataSource), listDataSource));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    CommonParam param = new CommonParam();
                    bool success = false;
                    expMestTestResultSDO = new Inventec.Common.Adapter.BackendAdapter(param).Post<ExpMestTestResultSDO>("api/HisExpMest/TestCreate", ApiConsumers.MosConsumer, sdo, param);
                    if (expMestTestResultSDO != null)
                    {
                        success = true;
                        btnPrint.Enabled = true;
                        btnSave.Enabled = false;
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
                else
                {
                    XtraMessageBox.Show(ResourceLanguageManager.KhongCoDuLieuHoaChat);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                List<MaterialTypeADO> listDataSource = new List<MaterialTypeADO>();
                ReloadDatasource(listDataSource);
                btnPrint.Enabled = false;
                expMestTestResultSDO = null;
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
                if (!btnPrint.Enabled || !btnPrint.Visible)
                    return;

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("MPS000165", deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnAccept_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAccept_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataMaterialByQcType()
        {
            try
            {
                if (UcQc != null)
                {
                    long qcTypeId = 0;
                    long turns = 0;
                    UcQc.GetValue(ref qcTypeId, ref turns);

                    List<MaterialTypeADO> listDataSource = new List<MaterialTypeADO>();

                    CommonParam param = new CommonParam();
                    HisQcNormationFilter filter = new HisQcNormationFilter();
                    filter.QC_TYPE_ID = qcTypeId;
                    filter.MACHINE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMachine.EditValue.ToString());
                    List<HIS_QC_NORMATION> listQc = new BackendAdapter(param).Get<List<HIS_QC_NORMATION>>("/api/HisQcNormation/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listQc != null && listQc.Count > 0)
                    {
                        List<HisMaterialTypeInStockSDO> listMaterialInstock = GetDataMaterialbyType(listQc.Select(s => s.MATERIAL_TYPE_ID).ToList());

                        foreach (var item in listQc)
                        {
                            var material = listMaterialInstock.FirstOrDefault(o => o.Id == item.MATERIAL_TYPE_ID);
                            if (material != null)
                            {
                                listDataSource.Add(new MaterialTypeADO(material, item.AMOUNT * turns));
                            }
                            else
                            {
                                var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                                if (materialType != null)
                                {
                                    listDataSource.Add(new MaterialTypeADO(materialType, item.AMOUNT * turns));
                                }
                            }
                        }
                    }

                    ReloadDatasource(listDataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataMaterialBySereServTein()
        {
            try
            {
                if (UcTest != null)
                {
                    long timeFrom = 0;
                    long timeTo = 0;
                    UcTest.GetValue(ref timeFrom, ref timeTo);

                    List<MaterialTypeADO> listDataSource = new List<MaterialTypeADO>();
                    CommonParam param = new CommonParam();
                    HisSereServTeinAmountByNormationFilter filter = new HisSereServTeinAmountByNormationFilter();
                    filter.FINISH_TIME_FROM = timeFrom;
                    filter.FINISH_TIME_TO = timeTo;
                    filter.EXECUTE_ROOM_ID = moduleData.RoomId;
                    TestMaterialByNormationCollectionSDO listMaterial = new BackendAdapter(param).Get<TestMaterialByNormationCollectionSDO>("/api/HisSereServTein/GetMaterialAmountByNormation", ApiConsumers.MosConsumer, filter, param);
                    if (listMaterial != null)
                    {

						Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMaterial), listMaterial));
                        var lstTestMaterialByNormations = listMaterial.TestMaterialByNormations;
                        lstSereServIds = listMaterial.SereServIds;
                        List<HisMaterialTypeInStockSDO> listMaterialInstock = GetDataMaterialbyType(lstTestMaterialByNormations.Select(o=>o.MATERIAL_TYPE_ID ?? 0).ToList());
						foreach (var lst in lstTestMaterialByNormations)
							{
                               
                                    var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == lst.MATERIAL_TYPE_ID);
                                    if (materialType != null)
                                    {
                                        var material = listMaterialInstock.FirstOrDefault(o => o.Id == materialType.ID);
                                        if (material != null)
                                        {
                                            listDataSource.Add(new MaterialTypeADO(material, lst.NORMATION_AMOUNT ?? 0));
                                        }
                                        else
                                        {
                                            listDataSource.Add(new MaterialTypeADO(materialType, lst.NORMATION_AMOUNT ?? 0));
                                        }
                                    }
                                
                            }                          
                        
                    }

                    ReloadDatasource(listDataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisMaterialTypeInStockSDO> GetDataMaterialbyType(List<long> materialTypeIds)
        {
            List<HisMaterialTypeInStockSDO> result = new List<HisMaterialTypeInStockSDO>();
            try
            {
                if (materialTypeIds != null && materialTypeIds.Count > 0)
                {
                    materialTypeIds = materialTypeIds.Distinct().ToList();

                    int skip = 0;
                    while (materialTypeIds.Count - skip > 0)
                    {
                        var listIds = materialTypeIds.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;
                        CommonParam param = new CommonParam();
                        HisMaterialTypeStockViewFilter filter = new HisMaterialTypeStockViewFilter();
                        filter.MATERIAL_TYPE_IDs = listIds;
                        filter.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStock.EditValue.ToString());
                        List<HisMaterialTypeInStockSDO> listMaterial = new BackendAdapter(param).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialType", ApiConsumers.MosConsumer, filter, param);
                        if (listMaterial != null && listMaterial.Count > 0)
                        {
                            result.AddRange(listMaterial);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<HisMaterialTypeInStockSDO>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ReloadDatasource(List<MaterialTypeADO> listData)
        {
            try
            {
                ListDataSource = listData;
                gridControl.BeginUpdate();
                gridControl.DataSource = listData;
                gridControl.EndUpdate();

                if (ListDataSource != null && ListDataSource.Count > 0)
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void cboMachine_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboMachine.EditValue != null && UcQc != null && chkQc.Checked)
                {
                    UcQc.ReloadCboTypeByMachineId(Inventec.Common.TypeConvert.Parse.ToInt64(cboMachine.EditValue.ToString()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);

                decimal amount = Inventec.Common.TypeConvert.Parse.ToDecimal((vw.GetRowCellValue(e.RowHandle, "AMOUNT") ?? "").ToString());
                decimal availableAmount = Inventec.Common.TypeConvert.Parse.ToDecimal((vw.GetRowCellValue(e.RowHandle, "AVAILABLE_AMOUNT") ?? "").ToString());
                if (amount > availableAmount)
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckData(List<MaterialTypeADO> listDataSource)
        {
            bool resul = true;
            try
            {
                List<string> error = new List<string>();
                if (listDataSource != null && listDataSource.Count > 0)
                {
                    List<string> errorAmount0 = new List<string>();
                    List<string> errorAvailableAmount = new List<string>();
                    foreach (var item in listDataSource)
                    {
                        if (item.AMOUNT <= 0)
                        {
                            errorAmount0.Add(item.MATERIAL_TYPE_CODE);
                        }

                        if (item.AMOUNT > item.AVAILABLE_AMOUNT)
                        {
                            errorAvailableAmount.Add(item.MATERIAL_TYPE_CODE);
                        }
                    }

                    if (errorAmount0.Count > 0)
                    {
                        error.Add(string.Format(ResourceLanguageManager.KhongCoSoLuong, string.Join(",", errorAmount0)));
                    }

                    if (errorAvailableAmount.Count > 0)
                    {
                        error.Add(string.Format(ResourceLanguageManager.SoLuongLonHonKhaDung, string.Join(",", errorAvailableAmount)));
                    }
                }

                if (error.Count > 0)
                {
                    XtraMessageBox.Show(string.Join("\r\n", error));
                }
                else
                {
                    resul = false;
                }
            }
            catch (Exception ex)
            {
                resul = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return resul;
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MaterialTypeADO)gridView.GetFocusedRow();
                if (row != null && ListDataSource != null && ListDataSource.Count > 0)
                {
                    ListDataSource.Remove(row);
                    ReloadDatasource(ListDataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.expMestTestResultSDO == null)
                    return result;

                MPS.Processor.Mps000165.PDO.Mps000165PDO rdo = new MPS.Processor.Mps000165.PDO.Mps000165PDO(
                    this.expMestTestResultSDO.ExpMest,
                    null,
                    this.expMestTestResultSDO.ExpMestMaterials,
                    BackendDataWorker.Get<HIS_MACHINE>());

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void EnableControlMachine(bool IsEnable)
		{
			try
			{
                cboMachine.EditValue = null;
                lciMachine.Enabled = IsEnable;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}            
    }
}
