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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AggrMobaImpMests.ADO;
using HIS.Desktop.Plugins.AggrMobaImpMests.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.AggrMobaImpMests;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrMobaImpMests
{
    public partial class frmAggrMobaImpMests : HIS.Desktop.Utility.FormBase
    {
        internal List<V_HIS_IMP_MEST_2> lstMobaImpMestChecks { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        HIS.Desktop.Common.RefeshReference refeshData;
        internal List<MediMateTypeADO> _OddNumbers { get; set; }
        int positionHandleControlBedInfo = -1;
        internal List<long> medicineGroupGnHt = new List<long>();

        // List<MediMateTypeADO> _MediMateChecks { get; set; }

        public frmAggrMobaImpMests()
        {
            InitializeComponent();
            HisConfig.LoadConfig();
            medicineGroupGnHt.Add(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
            medicineGroupGnHt.Add(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
        }

        public frmAggrMobaImpMests(Inventec.Desktop.Common.Modules.Module currentModule, List<V_HIS_IMP_MEST_2> lstMobaImpMestChecks, HIS.Desktop.Common.RefeshReference refeshData)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                HisConfig.LoadConfig();
                this.lstMobaImpMestChecks = lstMobaImpMestChecks;
                this.currentModule = currentModule;
                this.refeshData = refeshData;
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                medicineGroupGnHt.Add(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                medicineGroupGnHt.Add(IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmAggrMobaImpMests(List<V_HIS_IMP_MEST_2> lstMobaImpMestChecks, HIS.Desktop.Common.RefeshReference refeshData)
        {
            InitializeComponent();
            this.lstMobaImpMestChecks = lstMobaImpMestChecks;
            this.refeshData = refeshData;
        }

        private void frmAggrMobaImpMests_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                ValidateBedForm();
                LoadDataToComboImpMedistock();
                ChecksImpMestOddNumber();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AggrMobaImpMests.Resources.Lang", typeof(HIS.Desktop.Plugins.AggrMobaImpMests.frmAggrMobaImpMests).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColMediStockName.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.gridColMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStockImport.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.cboMediStockImport.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton_Save.Caption = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.barButton_Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAggrMobaImpMests.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChecksImpMestOddNumber()
        {
            try
            {
                if (lstMobaImpMestChecks != null && lstMobaImpMestChecks.Count > 0)
                {
                //    long meidStockId = 0;
                //    foreach (var item in lstMobaImpMestChecks)
                //    {
                //        if (meidStockId == 0)
                //        {
                //            meidStockId = item.MEDI_STOCK_ID;
                //        }
                //        else if (meidStockId != item.MEDI_STOCK_ID)
                //        {
                //            MessageManager.Show("Danh sách truyền vào yêu cầu phải cùng kho" + "\r\n");// + "Chọn lại để tiếp tục");
                //            this.Close();
                //            return;
                //        }
                //    }


                    _OddNumbers = new List<MediMateTypeADO>();
                    List<long> _ImpMestIds = lstMobaImpMestChecks.Select(p => p.ID).ToList();

                    //#20597
                    List<MediMateTypeADO> _MediMateChecks = new List<MediMateTypeADO>();
                    _MediMateChecks = LoadDetailImpMestMatyMetyByImpMestId(_ImpMestIds);

                    //_MediMateChecks = new List<MediMateTypeADO>();
                    //if (LoadImpMestMedicine(_ImpMestIds))
                    //    LoadImpMestMaterial(_ImpMestIds);
                    //else
                    //{
                    //    this.Close();
                    //    return;
                    //}

                    if (_MediMateChecks != null && _MediMateChecks.Count > 0)
                    {
                        //Group theo MEDICINE_ID
                        var _DataGroups = _MediMateChecks.GroupBy(x => new { x.MEDICINE_TYPE_ID, x.IsMedicine }).Select(grc => grc.ToList()).ToList();
                        List<MediMateTypeADO> ImpMestMedicineChecks = new List<MediMateTypeADO>();
                        foreach (var item_Group in _DataGroups)
                        {
                            MediMateTypeADO impMestMedi = new MediMateTypeADO();
                            impMestMedi = item_Group.FirstOrDefault();
                            impMestMedi.AMOUNT = item_Group.Sum(p => p.AMOUNT);
                            if (item_Group[0].IsMedicine)
                            {
                                var type = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item_Group[0].MEDICINE_TYPE_ID);

                                //Neu cau hinh quan ly le thuoc gay nghien huong than thi bo cac thuoc gay nghien huong than ra
                                if (HisConfig.OddManagerOption == (int)HisConfig.OddOption.ADDICTIVE_PSYCHOACITVE
                                    && type.MEDICINE_GROUP_ID.HasValue
                                    && medicineGroupGnHt.Contains(type.MEDICINE_GROUP_ID.Value))
                                {
                                    continue;
                                }
                                if (type != null && type.IS_ALLOW_EXPORT_ODD != 1)
                                {
                                    ImpMestMedicineChecks.Add(impMestMedi);
                                }
                            }
                            else
                            {
                                var type = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item_Group[0].MEDICINE_TYPE_ID);
                                if (type != null && type.IS_ALLOW_EXPORT_ODD != 1)
                                {
                                    ImpMestMedicineChecks.Add(impMestMedi);
                                }
                            }
                        }

                        //Ktra xem có lẻ hay k?
                        foreach (var item_oddNumber in ImpMestMedicineChecks)
                        {
                            decimal x = Math.Abs(Math.Round(item_oddNumber.AMOUNT, 6) - Math.Floor(item_oddNumber.AMOUNT));//Math.Round(item_oddNumber.AMOUNT, 3)
                            if (x > 0)
                            {
                                MediMateTypeADO oddNumber = new MediMateTypeADO();
                                oddNumber = item_oddNumber;
                                oddNumber.AMOUNT = x;
                                _OddNumbers.Add(oddNumber); //lẻ
                            }
                        }
                    }
                    gridControlMobaImpMests.DataSource = null;
                    if (_OddNumbers != null && _OddNumbers.Count > 0)
                    {
                        gridControlMobaImpMests.DataSource = _OddNumbers;
                    }
                    else
                    {
                        SaveAggrByImpMestIds(_ImpMestIds, null, null, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Lưu luôn khi không có lẻ
        private void SaveAggrByImpMestIds(List<long> _impMestIds, List<OddMedicineTypeSDO> _OddMedicineTypes, List<OddMaterialTypeSDO> _OddMaterialTypes, long _mediStockId)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                MOS.SDO.HisImpMestAggrSDO impMestSDO = new MOS.SDO.HisImpMestAggrSDO();
                impMestSDO.ImpMestIds = _impMestIds;
                impMestSDO.RequestRoomId = this.currentModule.RoomId;
                impMestSDO.OddMedicineTypes = new List<OddMedicineTypeSDO>();
                impMestSDO.OddMaterialTypes = new List<OddMaterialTypeSDO>();
                if (_OddMedicineTypes != null && _OddMedicineTypes.Count > 0)
                {
                    impMestSDO.OddMedicineTypes = _OddMedicineTypes;
                }
                if (_OddMaterialTypes != null && _OddMaterialTypes.Count > 0)
                {
                    impMestSDO.OddMaterialTypes = _OddMaterialTypes;
                }
                if (_mediStockId > 0)
                {
                    impMestSDO.OddMediStockId = _mediStockId;
                }
                var currentAggrImpMes = new BackendAdapter(param).Post<List<V_HIS_IMP_MEST>>("api/HisImpMest/AggrCreate", ApiConsumers.MosConsumer, impMestSDO, param);
                WaitingManager.Hide();
                if (currentAggrImpMes != null)
                {
                    success = true;
                    this.Close();
                    this.refeshData();
                }
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static List<MediMateTypeADO> LoadDetailImpMestMatyMetyByImpMestId(List<long> impMestIds)
        {

            List<MediMateTypeADO> impMestManuMetyMatys = new List<MediMateTypeADO>();//luu tat ca thuoc va vat tu
            try
            {
                CommonParam param = new CommonParam();
                //string ODD_MEDICINE_MANAGEMENT_OPTION_CFG = "MOS.HIS_MEDI_STOCK.ODD_MEDICINE_MANAGEMENT_OPTION";
                //string ODD_MATERIAL_MANAGEMENT_OPTION_CFG = "MOS.HIS_MEDI_STOCK.ODD_MATERIAL_MANAGEMENT_OPTION";
                //long keyMedi = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ODD_MEDICINE_MANAGEMENT_OPTION_CFG);
                //long keyMate = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(ODD_MATERIAL_MANAGEMENT_OPTION_CFG);
                //if (keyMedi == 3)
                //{
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_IDs = impMestIds;
                var impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMestMedicines != null && impMestMedicines.Count > 0)
                {
                    impMestManuMetyMatys.AddRange(from r in impMestMedicines select new MediMateTypeADO(r));
                }
                //  }

                //if (keyMate == 3)
                //{
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_IDs = impMestIds;
                var impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMestMaterials != null && impMestMaterials.Count > 0)
                {
                    impMestManuMetyMatys.AddRange(from r in impMestMaterials select new MediMateTypeADO(r));
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return impMestManuMetyMatys;
        }

        private void LoadImpMestMaterial(List<long> impMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_IDs = impMestIds;
                var impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMestMaterials != null && impMestMaterials.Count > 0)
                {
                    // _MediMateChecks.AddRange(from r in impMestMaterials select new MediMateTypeADO(r));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool LoadImpMestMedicine(List<long> impMestIds)
        {
            bool success = true;
            try
            {
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_IDs = impMestIds;
                var impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (impMestMedicines != null && impMestMedicines.Count > 0)
                {
                    List<V_HIS_IMP_MEST_MEDICINE> _MedicineOld = new List<V_HIS_IMP_MEST_MEDICINE>();

                    //Ktra xem có lẻ hay k?
                    foreach (var item in impMestMedicines)
                    {
                        decimal x = Math.Abs(Math.Round(item.AMOUNT, 3) - Math.Floor(item.AMOUNT));
                        if (x > 0)
                        {
                            //x : sl lẻ
                            _MedicineOld.Add(item);
                        }
                    }

                    if (_MedicineOld != null && _MedicineOld.Count > 0)
                    {
                        var dataGroups = _MedicineOld.GroupBy(p => p.IMP_MEST_ID).Select(p => p.ToList()).ToList();
                        string pressMessages = "";
                        if (dataGroups.Count == 1)
                        {
                            pressMessages += string.Format("Phiếu {0} có các thuốc sau có số lượng lẻ: \r\n", dataGroups[0][0].IMP_MEST_CODE);
                            for (int i = 0; i < dataGroups.Count; i++)
                            {
                                pressMessages += " - " + dataGroups[0][i].MEDICINE_TYPE_NAME + "(" + dataGroups[0][i].AMOUNT + ")" + "\r\n";
                            }
                        }
                        else
                        {
                            pressMessages += "Các thuốc sau có số lượng lẻ: \r\n";
                            foreach (var itemGr in dataGroups)
                            {
                                List<string> _mess = new List<string>();

                                for (int i = 0; i < itemGr.Count; i++)
                                {
                                    // pressMessages += " - " + itemGr[i].MEDICINE_TYPE_NAME + "(" + itemGr[i].AMOUNT + ")" + "\r\n";
                                    _mess.Add(itemGr[i].MEDICINE_TYPE_NAME + "(" + itemGr[i].AMOUNT + ")");
                                }
                                pressMessages += string.Format(" - Phiếu {0}: {1} \r\n", itemGr[0].IMP_MEST_CODE, string.Join(",", _mess));
                            }
                        }

                        if (DevExpress.XtraEditors.XtraMessageBox.Show(pressMessages + "Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            success = false;
                        }
                    }
                    if (success)
                    {
                        // _MediMateChecks.AddRange(from r in impMestMedicines select new MediMateTypeADO(r));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void LoadDataToComboImpMedistock()
        {
            CommonParam param = new CommonParam();
            try
            {
                //Lây các kho được cấu hình là kho lẻ thuộc khoa yêu cầu tổng hợp phiếu trả.
                List<V_HIS_MEDI_STOCK> _DataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.IS_ODD == 1 && p.IS_ACTIVE == 1 && WorkPlace.WorkPlaceSDO.FirstOrDefault(s => s.RoomId == this.currentModule.RoomId).DepartmentId == p.DEPARTMENT_ID).ToList();

                cboMediStockImport.Properties.DataSource = _DataMediStocks;
                cboMediStockImport.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStockImport.Properties.ValueMember = "ID";
                cboMediStockImport.Properties.ForceInitialize();
                cboMediStockImport.Properties.Columns.Clear();
                cboMediStockImport.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                cboMediStockImport.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStockImport.Properties.ShowHeader = false;
                cboMediStockImport.Properties.ImmediatePopup = true;
                cboMediStockImport.Properties.PopupWidth = 300;
                if (_DataMediStocks != null && _DataMediStocks.Count > 0)
                {
                    _DataMediStocks = _DataMediStocks.OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    cboMediStockImport.EditValue = _DataMediStocks[0].ID;
                    txtMediStockImportCode.Text = _DataMediStocks[0].MEDI_STOCK_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }






        private void cboMediStockImport_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediStockImport.EditValue != null)
                    {
                        var mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                            .FirstOrDefault(o =>
                                o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockImport.EditValue ?? 0).ToString()));
                        if (mediStockImp != null)
                        {
                            txtMediStockImportCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        }
                    }
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockImport_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboMediStockImport.EditValue != null)
                {
                    var mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>()
                            .FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockImport.EditValue ?? 0).ToString())
                            && o.ROOM_TYPE_ID == WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomTypeId);
                    if (mediStockImp != null)
                    {
                        txtMediStockImportCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMobaImpMests_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                try
                {
                    if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                    {
                        MediMateTypeADO data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "MEDI_STOCK_NAME")
                        {
                            var lstMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>();
                            e.Value = lstMediStock.Where(p => p.ID == data.MEDI_STOCK_ID).FirstOrDefault().MEDI_STOCK_NAME;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
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
                this.positionHandleControlBedInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                List<MediMateTypeADO> _DataOddNumbers = (List<MediMateTypeADO>)gridControlMobaImpMests.DataSource;
                List<OddMedicineTypeSDO> _medi = new List<OddMedicineTypeSDO>();
                List<OddMaterialTypeSDO> _mate = new List<OddMaterialTypeSDO>();
                if (_DataOddNumbers != null && _DataOddNumbers.Count > 0)
                {
                    foreach (var item in _DataOddNumbers)
                    {
                        if (item.AMOUNT > 0)
                        {
                            if (item.IsMedicine)
                            {
                                OddMedicineTypeSDO ado = new OddMedicineTypeSDO();
                                ado.Amount = item.AMOUNT;
                                ado.MedicineTypeId = item.MEDICINE_TYPE_ID;
                                _medi.Add(ado);
                            }
                            else
                            {
                                OddMaterialTypeSDO ado = new OddMaterialTypeSDO();
                                ado.Amount = item.AMOUNT;
                                ado.MaterialTypeId = item.MEDICINE_TYPE_ID;
                                _mate.Add(ado);
                            }
                        }
                    }
                }
                WaitingManager.Hide();
                List<long> _ImpMestIds = lstMobaImpMestChecks.Select(p => p.ID).ToList();
                long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockImport.EditValue.ToString());
                SaveAggrByImpMestIds(_ImpMestIds, _medi, _mate, mediStockId);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewMobaImpMests.PostEditor();
                btnSave_Click(null, null);
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

                if (positionHandleControlBedInfo == -1)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlBedInfo > edit.TabIndex)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
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

        private void ValidateBedForm()
        {
            ValidateLookupWithTextEdit(cboMediStockImport, txtMediStockImportCode);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit txtTextEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = txtTextEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTextEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockImportCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadStockCombo(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadStockCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboMediStockImport.EditValue = null;
                    cboMediStockImport.Focus();
                    cboMediStockImport.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.MEDI_STOCK_CODE.Contains(searchCode)).ToList();
                    if (data != null && data.Count > 0)
                    {
                        if (data.Count == 1)
                        {
                            cboMediStockImport.EditValue = data[0].ID;
                            txtMediStockImportCode.Text = data[0].MEDI_STOCK_CODE;
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.MEDI_STOCK_CODE == searchCode);
                            if (search != null)
                            {
                                cboMediStockImport.EditValue = search.ID;
                                txtMediStockImportCode.Text = search.MEDI_STOCK_CODE;
                            }
                            else
                            {
                                cboMediStockImport.EditValue = null;
                                cboMediStockImport.Focus();
                                cboMediStockImport.ShowPopup();
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

        private void gridViewMobaImpMests_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                //if (e.RowHandle < 0 || e.Column.FieldName != "AMOUNT")
                //    return;
                //var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //if (data != null)
                //{

                //}
                //gridControlMobaImpMests.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewMobaImpMests.GetFocusedRow();
                if (data != null)
                {
                    _OddNumbers.Remove(data);
                }
                gridControlMobaImpMests.BeginUpdate();
                gridControlMobaImpMests.DataSource = _OddNumbers;
                gridControlMobaImpMests.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
