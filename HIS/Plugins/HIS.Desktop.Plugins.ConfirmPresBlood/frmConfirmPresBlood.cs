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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.ConfirmPresBlood.ADO;
using HIS.Desktop.Plugins.ConfirmPresBlood.Config;
using HIS.Desktop.Plugins.ConfirmPresBlood.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.ConfirmPresBlood
{
    public partial class frmConfirmPresBlood : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long expMestId = 0;
        long requestRoomId = 0;
        long mediStockId = 0;
        long expMestTypeID = 0;
        long idGrid = 1;
        int positionHandle = -1;

        List<VHisExpMestBltyADO> listExpMestBlty;
        List<V_HIS_BLOOD> listBlood = new List<V_HIS_BLOOD>();
        Dictionary<string, V_HIS_BLOOD> dicBloodCode = new Dictionary<string, V_HIS_BLOOD>();

        Dictionary<long, V_HIS_BLOOD> dicCurrentBlood = new Dictionary<long, V_HIS_BLOOD>();
        Dictionary<long, V_HIS_BLOOD> dicShowBlood = new Dictionary<long, V_HIS_BLOOD>();

        Dictionary<long, VHisBloodADO> dicBloodAdo = new Dictionary<long, VHisBloodADO>();
        V_HIS_EXP_MEST_BLTY_REQ_1 currentBlty = null;
        List<BloodVolumeADO> bloodVolume;

        HIS_EXP_MEST resultExpMest = null;
        HIS_EXP_MEST ChmsExpMest = new HIS_EXP_MEST();

        bool checkBtnRefresh = true;

        List<HisBloodTypeInStockSDO> listBloodTypeInStock = new List<HisBloodTypeInStockSDO>();
        HisMediStockReplaceSDO replaceSDO = null;

        DelegateSelectData delegateSelectData = null;

        string AllowExportBloodOverRequestCFG = "";
        HIS_EXP_MEST rsSave = null;
        CabinetBaseResultSDO cabinetBaseResultSDO = null;
        V_HIS_EXP_MEST_2 expMest;
        CallApiType callApiType;

        enum CallApiType
        {
            cabinet,
            Other
        }

        public frmConfirmPresBlood()
        {
            InitializeComponent();
            HisConfig.LoadConfig();
        }

        public frmConfirmPresBlood(Inventec.Desktop.Common.Modules.Module currentModule, long expMestId, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            try
            {
                this.currentModule = currentModule;
                this.expMestId = expMestId;
                this.requestRoomId = currentModule.RoomId;
                delegateSelectData = _delegateSelectData;
                HisConfig.LoadConfig();
                this.callApiType = CallApiType.Other;
                //gridColMedicineTypeName.ColumnEdit = repositoryItemGridMedicineTypeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// mở duyệt từ danh sách bổ sung/thu hồi cơ số
        /// </summary>
        /// <param name="currentModule">module</param>
        /// <param name="_expMest">phiếu xuất view 4</param>
        /// <param name="_delegateSelectData">delegate</param>
        public frmConfirmPresBlood(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_EXP_MEST_2 _expMest, DelegateSelectData _delegateSelectData)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            try
            {
                this.currentModule = currentModule;
                this.expMestId = _expMest.ID;
                this.mediStockId = _expMest.MEDI_STOCK_ID;
                this.expMest = _expMest;
                this.callApiType = CallApiType.cabinet;
                this.requestRoomId = currentModule.RoomId;
                delegateSelectData = _delegateSelectData;
                HisConfig.LoadConfig();
                //gridColMedicineTypeName.ColumnEdit = repositoryItemGridMedicineTypeName;
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

        private void frmBrowseExportTicket_Load(object sender, EventArgs e)
        {
            try
            {
                GetConfig();
                this.ValidControl();
                
                FillDataToGridExpMestBlty();
                this.LoadDataBloodAndPatyMediStockId();
                this.ProcessDataBlood();
                this.InitComboBloodType();
                this.FillDataToGridBlood();
                this.SetControlByExpMestBlty();
                frmExpMestBlood_Plus_GridLookup();
                InitMenuToButtonPrint();
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBloodType()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE> bloodTypeList = BackendDataWorker.Get<HIS_BLOOD_TYPE>();
                if (listBlood != null && listBlood.Count > 0)
                {
                    bloodTypeList = bloodTypeList.Where(o => listBlood.Select(p => p.BLOOD_TYPE_ID).Contains(o.ID)).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, false, 350);

                ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_BloodType, bloodTypeList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByExpMest()
        {
            try
            {
                if (this.expMest != null && this.expMest.ID > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMetyReqFilter metyFilter = new HisExpMestMetyReqFilter();
                    metyFilter.EXP_MEST_ID = this.expMest.ID;

                    _DataMetys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);

                    MOS.Filter.HisExpMestMatyReqFilter matyFilter = new HisExpMestMatyReqFilter();
                    matyFilter.EXP_MEST_ID = this.expMest.ID;

                    _DataMatys = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, metyFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_EXP_MEST chmsExpMest { get; set; }
        List<HIS_MEDICINE> _Medicines { get; set; }
        List<HIS_MATERIAL> _Materials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _DataMetys { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _DataMatys { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();   
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

        private void onClickPrint(object sender, EventArgs e)
        {
            try
            {
                LoadDataByExpMest();
                if (
                    !((this._DataMetys != null && this._DataMetys.Count > 0)
                    || (this._DataMatys != null && this._DataMatys.Count > 0))
                    )
                    return;

                #region TT Chung
                WaitingManager.Show();
                chmsExpMest = new V_HIS_EXP_MEST();
                _Medicines = new List<HIS_MEDICINE>();
                _Materials = new List<HIS_MATERIAL>();
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

                long _expMestId = this.expMest.ID;
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = _expMestId;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();

                long _EXP_MEST_STT_ID = chmsExpMest.EXP_MEST_STT_ID;


                if (_EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    || _EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _MedicineIds;
                        _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                    matyFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = _MaterialIds;
                        _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    }
                }

                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);

                foreach (var item in this._DataMetys)
                {
                    var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (dataType != null)
                    {
                        if (dataType.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMetyReq_HCs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        {
                            _ExpMestMetyReq_GNs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                        {
                            _ExpMestMetyReq_HTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                        {
                            _ExpMestMetyReq_TDs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                        {
                            _ExpMestMetyReq_PXs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                        {
                            _ExpMestMetyReq_COs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                        {
                            _ExpMestMetyReq_DTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                        {
                            _ExpMestMetyReq_KSs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                        {
                            _ExpMestMetyReq_LAOs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                        {
                            _ExpMestMetyReq_TC.Add(item);
                        }
                        else
                        {
                            _ExpMestMetyReq_Ts.Add(item);
                        }
                    }
                }

                foreach (var item in this._DataMatys)
                {
                    var dataMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                WaitingManager.Hide();
                #endregion

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.expMest.CHMS_TYPE_ID == 2)
                {
                    store.RunPrintTemplate("Mps000346", delegatePrintTemplate);
                }
                else if (this.expMest.CHMS_TYPE_ID == 1)
                {
                    store.RunPrintTemplate("Mps000347", delegatePrintTemplate);
                }
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
                        case "Mps000346":
                            Mps000346(ref result, printTypeCode, fileName);
                            break;
                        case "Mps000347":
                            Mps000347(ref result, printTypeCode, fileName);
                            break;
                        default:
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

        private void Mps000346(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TD ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "ĐỘC"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PX ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                     (
                      chmsExpMest,
                      _ExpMestMedicines,
                         null,
         _ExpMestMetyReq_LAOs,
          null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
                #region ----- TC -----

                if (_ExpMestMetyReq_TC != null && _ExpMestMetyReq_TC.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TC,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,    
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "PHIẾU TRẢ CƠ SỐ THUỐC TIỀN CHẤT"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000347(ref bool result, string printTypeCode, string fileName)
        {
            try
            {

                #region ---Thuoc Thuong ----
                if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_Ts,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "THUỐC THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---Vat tu Thuong ----
                if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         null,
         _ExpMestMatyReq_VTs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "VẬT TƯ THƯỜNG"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HuongThan ----
                if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_HTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "HƯỚNG THẦN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- GayNghien ----
                if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_GNs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "GÂY NGHIỆN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- HoaChat ----
                if ((_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0) || (_ExpMestMetyReq_HCs != null && _ExpMestMetyReq_HCs.Count > 0))
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO mps000346PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         _ExpMestMaterials,
         _ExpMestMetyReq_HCs,
         _ExpMestMatyReq_HCs,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
         _Medicines,
         _Materials,
         "HÓA CHẤT"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- TD ----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_TDs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "ĐỘC"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- PX ----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_PXs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "PHÓNG XẠ"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- CO ----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_COs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "CORTICOID"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- DT ----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_DTs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "DỊCH TRUYỀN"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- KS ----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_KSs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "KHÁNG SINH"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region --- LAO ----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000347.PDO.Mps000347PDO Mps000347PDO = new MPS.Processor.Mps000347.PDO.Mps000347PDO
        (
         chmsExpMest,
         _ExpMestMedicines,
         null,
         _ExpMestMetyReq_LAOs,
         null,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
         IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
         null,
         _Medicines,
         null,
         "LAO"
          );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000347PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
                #region ----- TC -----
                if (_ExpMestMetyReq_TC != null && _ExpMestMetyReq_TC.Count > 0)
                {
                    MPS.Processor.Mps000346.PDO.Mps000346PDO mps000346PDO = new MPS.Processor.Mps000346.PDO.Mps000346PDO
                    (
                        chmsExpMest,
                        _ExpMestMedicines,
                        null,
                        _ExpMestMetyReq_TC,
                        null,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        null,
                        _Medicines,
                        null,
                        "PHIẾU TRẢ CƠ SỐ THUỐC TIỀN CHẤT"
                        );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000346PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(chmsExpMest.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetConfig()
        {
            try
            {
                this.AllowExportBloodOverRequestCFG = HisConfigs.Get<string>("HIS.Desktop.Plugins.ConfirmPresBlood.AllowExportBloodOverRequest");

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
                //get data 2 grid bỏ bản ghi nào mà có medicine_type_id null or = 0,, material_type_id cũng thế
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                gridViewExpMestBlty.PostEditor();
                gridViewExpMestBlty.UpdateCurrentRow();

                this.listExpMestBlty = (List<VHisExpMestBltyADO>)gridControlExpMestBlty.DataSource;

                HisExpMestConfirmSDO data = new HisExpMestConfirmSDO();
                data.ExpMestId = this.expMestId;
                data.ReqRoomId = this.requestRoomId;
                data.ExpBltyReqs = new List<ExpMestBltyReqSDO>();

                #region//Lay thuoc
                #endregion
                #region //lấy máu
                List<string> bloodTypeCheck = new List<string>();
              
                if (listExpMestBlty != null && listExpMestBlty.Count > 0)
                {
                    var checkAmount = listExpMestBlty.Where(o => o.AmountReq <= 0);
                    if (checkAmount != null && checkAmount.Count() > 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập số lượng túi máu chốt", "Thông báo");
                        return;
                    }
                    var checkBloodType = listExpMestBlty.Where(o => o.BLOOD_TYPE_ID == 0);

                    if (listBlood != null && listBlood.Count > 0)
                    {
                        List<VHisExpMestBltyADO> checkBLoodType = new List<VHisExpMestBltyADO>();
                        foreach (var expMestBlty in listExpMestBlty)
                        {
                            var bloodByType = listBlood.Where(o => o.BLOOD_TYPE_ID == expMestBlty.BLOOD_TYPE_ID).ToList();
                            if (bloodByType != null && bloodByType.Count > 0)
                            {
                                expMestBlty.AvailableAmount = bloodByType.Count;
                                if (bloodByType.Count < expMestBlty.AmountReq)
                                {
                                    checkBLoodType.Add(expMestBlty);
                                    var blood = BackendDataWorker.Get<HIS_BLOOD_TYPE>().FirstOrDefault(o => o.ID == expMestBlty.BLOOD_TYPE_ID);
                                    string Mess = String.Format("Loại máu {0} Không đủ khả dụng. Chốt {1}, Khả dụng {2}", blood.BLOOD_TYPE_NAME, expMestBlty.AmountReq, expMestBlty.AvailableAmount);
                                    WaitingManager.Hide();
                                    DevExpress.XtraEditors.XtraMessageBox.Show(Mess, "Thông báo");
                                    return;
                                }
                            }
                        }
                    }

                    foreach (var ExpMestBlty in listExpMestBlty)
                    {
                        ExpMestBltyReqSDO sdo = new ExpMestBltyReqSDO();
                        sdo.BloodTypeId = ExpMestBlty.BLOOD_TYPE_ID;
                        sdo.ExpMestBltyReqId = ExpMestBlty.ID;
                        sdo.Amount = ExpMestBlty.AmountReq;
                        data.ExpBltyReqs.Add(sdo);
                    }
                }

                #endregion

                if (data.ExpBltyReqs.Count == 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn máu", "Thông báo");
                    return;
                }

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Info("Input api/HisExpMest/PresBloodConfirm: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                // duyệt từ danh sách bổ sung/ thu hồi cơ số

                rsSave = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST>(
                "api/HisExpMest/PresBloodConfirm", ApiConsumers.MosConsumer, data, param);

                if (rsSave != null)
                {
                    success = true;
                    btnPrint.Enabled = true;
                    if (rsSave != null)
                        this.delegateSelectData(rsSave);
                    FillDataToGridExpMestBlty();
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                string txt = "";
                if (listExpMestBlty != null)
                {
                    foreach (var item in listExpMestBlty)
                    {
                        int amount = (int)item.AMOUNT;
                        for (int i = 0; i < amount; i++)
                        {
                            GenText(rsSave, item, i + 1, ref txt);
                            txt += "\n";
                        }
                        GenText(rsSave, item, null, ref txt);
                    }
                }
                string resultPrint = new Bartender.PrintTemBlood.PrintTemBlood().StartPrintTemBlood(txt);

                if (!string.IsNullOrWhiteSpace(resultPrint))
                {
                    Inventec.Common.Logging.LogSystem.Warn(resultPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            } 
        }

        private void GenText(HIS_EXP_MEST expMest, VHisExpMestBltyADO mestBLTYReq, int? index, ref string txt)
        {
            txt += expMest.TDL_PATIENT_CODE;
            txt += "," + expMest.TDL_PATIENT_DOB;
            txt += "," + (expMest.TDL_PATIENT_DOB != null && expMest.TDL_PATIENT_DOB > 10000000000000 ? expMest.TDL_PATIENT_DOB.ToString().Substring(0, 4) : "");
            txt += "," + expMest.TDL_PATIENT_GENDER_NAME;
            txt += "," + expMest.TDL_PATIENT_NAME;
            txt += "," + mestBLTYReq.BLOOD_ABO_CODE;
            txt += "," + mestBLTYReq.BLOOD_RH_CODE;
            txt += "," + BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == expMest.REQ_DEPARTMENT_ID).DEPARTMENT_NAME;
            txt += "," + expMest.TDL_TREATMENT_CODE;
            txt += "," + mestBLTYReq.BLOOD_TYPE_NAME;
            txt += "," + mestBLTYReq.AMOUNT;
            if (index.HasValue)
            {
                txt += "," + String.Format("{0}/{1}", index, mestBLTYReq.AMOUNT);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                btnSave.Focus();
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
                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        #region button deleteCombo
        private void cboBloodType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //cboBloodType.Properties.Buttons[1].Visible = false;
                    cboBloodType.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpVolume_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //gridLookUpVolume.Properties.Buttons[1].Visible = false;
                    gridLookUpVolume.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodAboCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //gridLookUpBloodAboCode.Properties.Buttons[1].Visible = false;
                    gridLookUpBloodAboCode.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpBloodRhCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //gridLookUpBloodRhCode.Properties.Buttons[1].Visible = false;
                    gridLookUpBloodRhCode.EditValue = null;
                    fillDataGridViewBlood();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion



        private void cboPrint_Click(object sender, EventArgs e)
        {
        }

        internal enum PrintType
        {
            MPS000346,
            MPS000347,
            MPS000422
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                if (this.expMest.CHMS_TYPE_ID == 1)
                {
                    DXMenuItem itemXuatBuCoSoTuTruc = new DXMenuItem("Phiếu bổ sung cơ số", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuCoSoTuTruc.Tag = PrintType.MPS000347;
                    menu.Items.Add(itemXuatBuCoSoTuTruc);
                }
                else
                {
                    DXMenuItem itemXuatBuThuocLe = new DXMenuItem("Phiếuthu hồi cơ số", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuThuocLe.Tag = PrintType.MPS000346;
                    menu.Items.Add(itemXuatBuThuocLe);
                }

                //cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                //LoadSpecificExpMest();
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.MPS000347:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000347, DelegateRunPrinter);
                        break;

                    case PrintType.MPS000346:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000346, DelegateRunPrinter);
                        break;

                    case PrintType.MPS000422:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000422, DelegateRunPrinter422);
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private bool DelegateRunPrinter422(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000422:
                        InPhieuTemDonMau(printTypeCode, fileName, ref result);
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

        string printerName = "";
        private void ProcessPrint(String printTypeCode)
        {
            try
            {
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.rsSave != null ? this.rsSave.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();

        private void InPhieuTemDonMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.rsSave.ID;
                var lstExpMest = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestFilter, new CommonParam()).FirstOrDefault();

                HisExpMestBltyReqViewFilter expMestBltyReqfilter = new HisExpMestBltyReqViewFilter();
                expMestBltyReqfilter.EXP_MEST_ID = rsSave.ID;

                var lstExpBltyService = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, expMestBltyReqfilter, new CommonParam());

                WaitingManager.Hide();
                MPS.Processor.Mps000422.PDO.Mps000422PDO pdo = new MPS.Processor.Mps000422.PDO.Mps000422PDO(
                 lstExpMest,
                 lstExpBltyService
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                LoadDataByExpMest();
                if (
                    !((this._DataMetys != null && this._DataMetys.Count > 0)
                    || (this._DataMatys != null && this._DataMatys.Count > 0))
                    )
                    return false;

                #region TT Chung
                WaitingManager.Show();
                chmsExpMest = new V_HIS_EXP_MEST();
                _Medicines = new List<HIS_MEDICINE>();
                _Materials = new List<HIS_MATERIAL>();
                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestMetyReq_HCs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TC = new List<HIS_EXP_MEST_METY_REQ>();

                long _expMestId = this.expMest.ID;
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = _expMestId;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();

                long _EXP_MEST_STT_ID = chmsExpMest.EXP_MEST_STT_ID;


                if (_EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                    || _EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                    if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                    {
                        List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                        MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = _MedicineIds;
                        _Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                    }

                    MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                    matyFilter.EXP_MEST_ID = _expMestId;
                    _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                    if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                    {
                        List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = _MaterialIds;
                        _Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                    }
                }

                var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);

                foreach (var item in this._DataMetys)
                {
                    var dataType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (dataType != null)
                    {
                        if (dataType.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMetyReq_HCs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        {
                            _ExpMestMetyReq_GNs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                        {
                            _ExpMestMetyReq_HTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                        {
                            _ExpMestMetyReq_TDs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                        {
                            _ExpMestMetyReq_PXs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                        {
                            _ExpMestMetyReq_COs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                        {
                            _ExpMestMetyReq_DTs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                        {
                            _ExpMestMetyReq_KSs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                        {
                            _ExpMestMetyReq_LAOs.Add(item);
                        }
                        else if (dataType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                        {
                            _ExpMestMetyReq_TC.Add(item);
                        }
                        else
                        {
                            _ExpMestMetyReq_Ts.Add(item);
                        }
                    }
                }

                foreach (var item in this._DataMatys)
                {
                    var dataMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                    {
                        _ExpMestMatyReq_HCs.Add(item);
                    }
                    else
                        _ExpMestMatyReq_VTs.Add(item);
                }

                WaitingManager.Hide();
                #endregion

                switch (printTypeCode)
                {
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000346:
                        Mps000346(ref result, printTypeCode, fileName);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__MPS000347:
                        Mps000347(ref result, printTypeCode, fileName);
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.MPS000422);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
