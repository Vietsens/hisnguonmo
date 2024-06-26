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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ManuImpMestUpdate.ADO;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Config;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Resources;
using HIS.Desktop.Plugins.ManuImpMestUpdate.Validation;
using HIS.Desktop.Utility;
using HIS.UC.MaterialType;
using HIS.UC.MedicineType;
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate
{
    public partial class frmManuImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        private void OnClickBienBanKiemNhapTuNcc(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.resultADO == null)
                    return;

                if (this.resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    store.RunPrintTemplate("Mps000085", InBienBanNhap);
                }
                else
                {
                    store.RunPrintTemplate("Mps000199", InBienBanNhap);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool InBienBanNhap(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                if (this.resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    HisImpMestViewFilter hisImpMestViewFilter = new HisImpMestViewFilter();
                    hisImpMestViewFilter.ID = this.impMestId;
                    var hisImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>(Base.GlobalStore.HIS_IMP_MEST_GETVIEW, ApiConsumers.MosConsumer, hisImpMestViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (hisImpMests != null && hisImpMests.Count != 1)
                    {
                        throw new NullReferenceException("Khong lay duoc manuImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.hisManuImpMestSDO), resultADO.hisManuImpMestSDO));
                    }
                    var hisImpMest = hisImpMests.FirstOrDefault();

                    HIS_SUPPLIER supplier = new HIS_SUPPLIER();
                    if (hisImpMest != null && hisImpMest.SUPPLIER_ID != null)
                    {
                        MOS.Filter.HisSupplierFilter supplierFilter = new HisSupplierFilter();
                        supplierFilter.ID = hisImpMest.SUPPLIER_ID;
                        supplier = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SUPPLIER>>("api/HisSupplier/Get", ApiConsumer.ApiConsumers.MosConsumer, supplierFilter, new CommonParam()).FirstOrDefault();
                    }

                    HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                    mediFilter.IMP_MEST_ID = this.resultADO.hisManuImpMestSDO.ImpMest.ID;
                    var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                    if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                    {
                        HisMedicineFilter medicineFilter = new HisMedicineFilter();
                        medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                        medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                    }

                    HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                    mateFilter.IMP_MEST_ID = this.resultADO.hisManuImpMestSDO.ImpMest.ID;
                    var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                    if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                    {
                        HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                        materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                    }


                    MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                    userFilter.IMP_MEST_ID = this.resultADO.hisManuImpMestSDO.ImpMest.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        rs = rs.OrderBy(p => p.ID).ToList();
                    }

                    MPS.Processor.Mps000085.PDO.Mps000085PDO Mps000170RDO = new MPS.Processor.Mps000085.PDO.Mps000085PDO(
                        hisImpMest,
                        hisImpMestMedicines,
                        hisImpMestMaterials,
                        rs,
                        medicines,
                        materials,
                        supplier
                        );

                    PrintDataS(printTypeCode, fileName, Mps000170RDO, ref result);
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK && resultADO.hisInitImpMestSDO != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                        userFilter.IMP_MEST_ID = this.resultADO.hisInitImpMestSDO.ImpMest.ID;
                        var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                        _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                        HisImpMestViewFilter initImpMestFilter = new HisImpMestViewFilter();
                        initImpMestFilter.ID = this.resultADO.hisInitImpMestSDO.ImpMest.ID;
                        var hisInitImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, initImpMestFilter, null);
                        if (hisInitImpMests != null && hisInitImpMests.Count != 1)
                        {
                            throw new NullReferenceException("Khong lay duoc InitImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.hisInitImpMestSDO), resultADO.hisInitImpMestSDO));
                        }
                        var initImpMest = hisInitImpMests.FirstOrDefault();

                        HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                        mediFilter.IMP_MEST_ID = this.resultADO.hisInitImpMestSDO.ImpMest.ID;
                        var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                        HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                        mateFilter.IMP_MEST_ID = this.resultADO.hisInitImpMestSDO.ImpMest.ID;
                        var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                        // MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(initImpMest, hisImpMestMedicines, hisImpMestMaterials);
                        MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                           initImpMest,
                           hisImpMestMedicines,
                           hisImpMestMaterials,
                           null,
                           _ImpMestUser);

                        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                    }
                    else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK && resultADO.hisInveImpMestSDO != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                        userFilter.IMP_MEST_ID = this.resultADO.hisInveImpMestSDO.ImpMest.ID;
                        var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                        _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();

                        HisImpMestViewFilter InveImpMestFilter = new HisImpMestViewFilter();
                        InveImpMestFilter.ID = this.resultADO.hisInveImpMestSDO.ImpMest.ID;
                        var hisInveImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, InveImpMestFilter, null);
                        if (hisInveImpMests != null && hisInveImpMests.Count != 1)
                        {
                            throw new NullReferenceException("Khong lay duoc inveImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.hisInveImpMestSDO), resultADO.hisInveImpMestSDO));
                        }
                        var inveImpMest = hisInveImpMests.FirstOrDefault();

                        HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                        mediFilter.IMP_MEST_ID = this.resultADO.hisInveImpMestSDO.ImpMest.ID;
                        var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                        HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                        mateFilter.IMP_MEST_ID = this.resultADO.hisInveImpMestSDO.ImpMest.ID;
                        var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);

                        // MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(inveImpMest, hisImpMestMedicines, hisImpMestMaterials);
                        MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000170RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                            inveImpMest,
                            hisImpMestMedicines,
                            hisImpMestMaterials,
                            null,
                            _ImpMestUser);

                        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000170RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                    }
                    else if (resultADO.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC && resultADO.hisOtherImpMestSDO != null)
                    {
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisImpMestUserViewFilter userFilter = new MOS.Filter.HisImpMestUserViewFilter();
                        userFilter.IMP_MEST_ID = this.resultADO.hisOtherImpMestSDO.ImpMest.ID;
                        var _ImpMestUser = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_USER>>("/api/HisImpMestUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                        _ImpMestUser = _ImpMestUser.OrderBy(p => p.ID).ToList();


                        HisImpMestViewFilter otherImpMestFilter = new HisImpMestViewFilter();
                        otherImpMestFilter.ID = this.resultADO.hisOtherImpMestSDO.ImpMest.ID;
                        var hisOtherImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, otherImpMestFilter, null);
                        if (hisOtherImpMests != null && hisOtherImpMests.Count != 1)
                        {
                            throw new NullReferenceException("Khong lay duoc otherImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.hisOtherImpMestSDO), resultADO.hisOtherImpMestSDO));
                        }
                        var otherImpMest = hisOtherImpMests.FirstOrDefault();

                        HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                        mediFilter.IMP_MEST_ID = this.resultADO.hisOtherImpMestSDO.ImpMest.ID;
                        var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);

                        HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                        mateFilter.IMP_MEST_ID = this.resultADO.hisOtherImpMestSDO.ImpMest.ID;
                        var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                        //MPS.Processor.Mps000170.PDO.Mps000170PDO Mps000170RDO = new MPS.Processor.Mps000170.PDO.Mps000170PDO(otherImpMest, hisImpMestMedicines, hisImpMestMaterials);
                        MPS.Processor.Mps000199.PDO.Mps000199PDO Mps000199RDO = new MPS.Processor.Mps000199.PDO.Mps000199PDO(
                            otherImpMest,
                            hisImpMestMedicines,
                            hisImpMestMaterials,
                            null,
                            _ImpMestUser);
                        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000199RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                    }
                    WaitingManager.Hide();
                    if (PrintData != null)
                    {
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    else
                        throw new Exception("lỗi in mps000199");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void PrintDataS(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
