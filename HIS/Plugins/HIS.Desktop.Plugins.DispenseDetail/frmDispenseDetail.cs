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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.DispenseDetail.ADO;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using MOS.Filter;

namespace HIS.Desktop.Plugins.DispenseDetail
{
    public partial class frmDispenseDetail : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        MOS.EFMODEL.DataModels.HIS_DISPENSE DispenseDetail;
        List<MOS.EFMODEL.DataModels.HIS_IMP_MEST> impMests;
        List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> expMests;
        List<MPS.Processor.Mps000119.PDO.HisBidMetyADO> bidprintAdo;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<ImpMestExpMestAdo> ImpMestExpMestAdos;
        #endregion

        #region Construct
        public frmDispenseDetail(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                this.moduleData = moduleData;
                SetPrintTypeToMps();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmDispenseDetail(MOS.EFMODEL.DataModels.HIS_DISPENSE data, Inventec.Desktop.Common.Modules.Module moduleData)
            : this(moduleData)
        {
            try
            {
                this.DispenseDetail = data;
                bidprintAdo = new List<MPS.Processor.Mps000119.PDO.HisBidMetyADO>();
                this.Text = moduleData.text;
                this.moduleData = moduleData;
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DispenseDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.DispenseDetail.frmDispenseDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPrint.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.cboPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Column_GcStt.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ImpMestExpMest_GcStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpExpTimeStr.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.gridColumn_ImpExpTimeStr.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpExpLoginname.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.gridColumn_ImpExpLoginname.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ColoumnDispenseCode.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ColoumnDispenseCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ColumnType.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ColumnType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ColumnSttName.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ColumnSttName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ImpMestExpMest_GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ImpMestExpMest_GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ImpMestExpMest_GcCreator.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ImpMestExpMest_GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ImpMestExpMest_GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ImpMestExpMest_GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ImpMestExpMest_GcModifier.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.ImpMestExpMest_GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("FormDispenseDetail.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControlDispense.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.groupControlDispense.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDispenseCode.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.lciDispenseCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDispenseTime.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.lciDispenseTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedistock.Text = Inventec.Common.Resource.Get.Value("FormDispenseDetail.lciMedistock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormDispenseDetail_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                //LoadKeysFromlanguage();
                SetCaptionByLanguageKey();
                FillDataToCommonInfo(this.DispenseDetail);
                //Load du lieu
                FillDataToGridImpExpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private void FillDataToCommonInfo(MOS.EFMODEL.DataModels.HIS_DISPENSE dispense)
        {
            try
            {
                lblDispenseCode.Text = dispense.DISPENSE_CODE;
                lblDispenseTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dispense.DISPENSE_TIME);

                var medistock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == dispense.MEDI_STOCK_ID);
                if (medistock != null)
                {
                    lblMedistock.Text = medistock.MEDI_STOCK_CODE + " - " + medistock.MEDI_STOCK_NAME;
                }
                else
                {
                    lblMedistock.Text = "";
                }
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

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToGridImpExpMest()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.DispenseDetail == null)
                {
                    return;
                }
                WaitingManager.Show();
                this.ImpMestExpMestAdos = new List<ImpMestExpMestAdo>();

                MOS.Filter.HisImpMestFilter filter = new MOS.Filter.HisImpMestFilter();
                filter.DISPENSE_ID = this.DispenseDetail.ID;
                impMests = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                foreach (var impMest in impMests)
                {
                    ImpMestExpMestAdo ImpMestExpMestAdo = new ImpMestExpMestAdo(impMest);
                    this.ImpMestExpMestAdos.Add(ImpMestExpMestAdo);
                }

                MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                expMestFilter.DISPENSE_ID = this.DispenseDetail.ID;
                expMests = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, param);
                foreach (var expMest in expMests)
                {
                    ImpMestExpMestAdo ImpMestExpMestAdo = new ImpMestExpMestAdo(expMest);
                    this.ImpMestExpMestAdos.Add(ImpMestExpMestAdo);
                }

                gridControlImpExpMest.DataSource = this.ImpMestExpMestAdos;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicine_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpMestExpMestAdo data = (ImpMestExpMestAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "ImpExpTimeStr")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ImpExpTime ?? 0);
                        }
                        else if (e.Column.FieldName == "ImpExpLoginnameExtend")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ImpExpTime ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region in
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000244":
                        this.InPhieuBaoCheThuoc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000319":
                        this.InPhieuDongGoiVatTu(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InPhieuBaoCheThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                V_HIS_IMP_MEST impMest = new V_HIS_IMP_MEST();
                CommonParam param = new CommonParam();
                WaitingManager.Show();


                if (this.impMests != null && this.impMests.Count > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                    impMestMedicineViewFilter.IMP_MEST_IDs = this.impMests.Select(o => o.ID).Distinct().ToList();
                    impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                }

                if (this.expMests != null && this.expMests.Count > 0)
                {
                    MOS.Filter.HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                    hisExpMestMaterialViewFilter.EXP_MEST_IDs = this.expMests.Select(o => o.ID).Distinct().ToList();
                    expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);

                    MOS.Filter.HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = this.expMests.Select(o => o.ID).Distinct().ToList();
                    expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, param);
                }

                MPS.Processor.Mps000244.PDO.Mps000244PDO rdo = new MPS.Processor.Mps000244.PDO.Mps000244PDO(
                    this.DispenseDetail,
                      BackendDataWorker.Get<HIS_MEDI_STOCK>(),
                      impMestMedicines,
                      expMestMaterials,
                      expMestMedicines);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuDongGoiVatTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                WaitingManager.Show();


                if (this.impMests != null && this.impMests.Count > 0)
                {
                    MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                    impMestMaterialViewFilter.IMP_MEST_IDs = this.impMests.Select(o => o.ID).Distinct().ToList();
                    impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                }

                if (this.expMests != null && this.expMests.Count > 0)
                {
                    MOS.Filter.HisExpMestMaterialViewFilter hisExpMestMaterialViewFilter = new HisExpMestMaterialViewFilter();
                    hisExpMestMaterialViewFilter.EXP_MEST_IDs = this.expMests.Select(o => o.ID).Distinct().ToList();
                    expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisExpMestMaterialViewFilter, param);
                }

                MPS.Processor.Mps000319.PDO.Mps000319PDO rdo = new MPS.Processor.Mps000319.PDO.Mps000319PDO(
                    this.DispenseDetail,
                      BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.DispenseDetail.MEDI_STOCK_ID),
                      impMestMaterials,
                      expMestMaterials);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DispenseDetail.DISPENSE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_DISPENSE_TYPE.ID__DISPENSE_MEDICINE)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                    richEditor.RunPrintTemplate("Mps000244", DelegateRunPrinter);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditor = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                    richEditor.RunPrintTemplate("Mps000319", DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cboPrint_Click(null, null);
        }
        #endregion

        private void ButtonEdit_Detail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (ImpMestExpMestAdo)gridViewImpExpMest.GetFocusedRow();
                if (focus != null)
                {
                    if (focus.IsImpMest)
                    {
                        HIS.Desktop.ADO.ImpMestViewDetailADO impMestViewDetailADO = new HIS.Desktop.ADO.ImpMestViewDetailADO(focus.ImpExpMestId, focus.ImpExpTypeId, focus.ImpExpMestSttId);
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ImpMestViewDetail").FirstOrDefault();
                        if (moduleData == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ImpMestViewDetail");
                            MessageManager.Show(Resources.ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(impMestViewDetailADO);
                            listArgs.Add(this.moduleData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataAfterSave);
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisExpMestViewFilter expMestViewFilter = new MOS.Filter.HisExpMestViewFilter();
                        expMestViewFilter.ID = focus.ImpExpMestId;
                        var expMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestViewFilter, param).FirstOrDefault();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestViewDetail").FirstOrDefault();
                        if (moduleData == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestViewDetail");
                            MessageManager.Show(Resources.ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(expMest);
                            listArgs.Add(this.moduleData);
                            listArgs.Add((HIS.Desktop.Common.DelegateSelectData)FillDataAfterSave);
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            WaitingManager.Hide();
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show(Resources.ResourceLanguageManager.TaiKhoanKhongCoQuyenThucHienChucNang);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataAfterSave(object data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToGridImpExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridViewImpExpMest_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ImpMestExpMestAdo)gridViewImpExpMest.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IsImpMest)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Public method

        #endregion
    }
}
