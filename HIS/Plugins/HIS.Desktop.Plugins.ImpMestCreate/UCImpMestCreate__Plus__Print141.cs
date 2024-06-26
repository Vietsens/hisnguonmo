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
using HIS.UC.MedicineType;
using HIS.UC.MaterialType;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.ImpMestCreate.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.IO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        private void InitMenuToButtonPrint(ResultImpMestADO _result)
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemDeNghi = new DXMenuItem("Biên bản kiểm nhập", new EventHandler(OnClickIn));
                itemDeNghi.Tag = PrintType.IN__BIEN_BAN_KIEM_NHAP;
                menu.Items.Add(itemDeNghi);

                if (_result != null && _result.HisManuSDO != null) 
                {
                    if (_result.HisManuSDO.ImpMest.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT
                    && _result.ImpMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        DXMenuItem itemBienBanNghiemThu = new DXMenuItem("Phiếu nhập nhà cung cấp", new EventHandler(OnClickIn));
                        itemBienBanNghiemThu.Tag = PrintType.IN__PHIEU_NHAP_NHA_CUNG_CAP;
                        menu.Items.Add(itemBienBanNghiemThu);
                        
                    }
                }

                dropDownButton__Print.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickIn(object sender, EventArgs e)
        {
            try
            {
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
                switch (printType)
                {
                    case PrintType.IN__BIEN_BAN_KIEM_NHAP:
                        onClickBienBanKiemNhapTuNcc(null, null);
                        break;
                    case PrintType.IN__PHIEU_NHAP_NHA_CUNG_CAP:
                        inPhieuNhapNhaCungCap();
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

        public enum PrintType
        {
            IN__PHIEU_NHAP_NHA_CUNG_CAP,
            IN__BIEN_BAN_KIEM_NHAP
        }

        private void InPhieuNhapTuNhaCungCap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                if (this.resultADO == null)
                    return;
                if (this.resultADO.HisManuSDO == null || this.resultADO.ImpMestTypeId != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không phải loại nhập từ nhà cung cấp", "Thông báo");
                    return;
                }
                WaitingManager.Show();

                HisImpMestViewFilter manuImpMestFilter = new HisImpMestViewFilter();
                manuImpMestFilter.ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisManuImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, manuImpMestFilter, null);
                if (hisManuImpMests != null && hisManuImpMests.Count != 1)
                {
                    throw new NullReferenceException("Khong lay duoc ManuImpMest bang impMestId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultADO.HisManuSDO), resultADO.HisManuSDO));
                }
                var manuImpMest = hisManuImpMests.FirstOrDefault();
                HisImpMestMedicineViewFilter mediFilter = new HisImpMestMedicineViewFilter();
                mediFilter.IMP_MEST_ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisImpMestMedicines = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, null);
                List<HIS_MEDICINE> medicines = new List<HIS_MEDICINE>();
                if (hisImpMestMedicines != null && hisImpMestMedicines.Count > 0)
                {
                    HisMedicineFilter medicineFilter = new HisMedicineFilter();
                    medicineFilter.IDs = hisImpMestMedicines.Select(o => o.MEDICINE_ID).Distinct().ToList();
                    medicines = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, new CommonParam());
                }

                HisImpMestMaterialViewFilter mateFilter = new HisImpMestMaterialViewFilter();
                mateFilter.IMP_MEST_ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var hisImpMestMaterials = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, mateFilter, null);
                List<HIS_MATERIAL> materials = new List<HIS_MATERIAL>();
                if (hisImpMestMaterials != null && hisImpMestMaterials.Count > 0)
                {
                    HisMaterialFilter materialFilter = new HisMaterialFilter();
                    materialFilter.IDs = hisImpMestMaterials.Select(o => o.MATERIAL_ID).Distinct().ToList();
                    materials = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, new CommonParam());
                }


                MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                impMestViewFilter.ID = this.resultADO.HisManuSDO.ImpMest.ID;
                var impMests = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, new CommonParam()).FirstOrDefault();

                //thêm danh sách hợp đồng
                List<V_HIS_MEDICAL_CONTRACT> MedicalContract = new List<V_HIS_MEDICAL_CONTRACT>();
                List<long> MedicalContractIds = new List<long>();

                if (medicines != null && medicines.Count > 0)
                {
                    List<long> medicineContract = new List<long>();
                    foreach (var item in medicines)
                    {
                        if (item.MEDICAL_CONTRACT_ID.HasValue)
                        {
                            medicineContract.Add(item.MEDICAL_CONTRACT_ID.Value);
                        }
                    }
                    if (medicineContract != null && medicineContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(medicineContract);
                    }
                }

                if (materials != null && materials.Count > 0)
                {
                    List<long> materialContract = new List<long>();
                    foreach (var item in materials)
                    {
                        if (item.MEDICAL_CONTRACT_ID.HasValue)
                        {
                            materialContract.Add(item.MEDICAL_CONTRACT_ID.Value);
                        }
                    }

                    if (materialContract != null && materialContract.Count > 0)
                    {
                        MedicalContractIds.AddRange(materialContract);
                    }
                }

                if (MedicalContractIds != null && MedicalContractIds.Count > 0)
                {
                    HisMedicalContractViewFilter MedicalContractFilter = new HisMedicalContractViewFilter();
                    MedicalContractFilter.IDs = MedicalContractIds;
                    MedicalContract = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, MedicalContractFilter, new CommonParam());
                }

                List<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO> MedicalContractADO = new List<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>();

                if (MedicalContract != null && MedicalContract.Count > 0)
                {
                    foreach (var item in medicines)
                    {
                        MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO ado = new MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>(ado, Contract);
                        ado.MEDICINE_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }

                    foreach (var item in materials)
                    {
                        MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO ado = new MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO();

                        var Contract = MedicalContract.FirstOrDefault(o => o.ID == item.MEDICAL_CONTRACT_ID);
                        Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000141.PDO.Mps000141PDO.MedicalContractADO>(ado, Contract);
                        ado.MATERIAL_ID = item.ID;
                        MedicalContractADO.Add(ado);
                    }
                }


                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((impMests != null ? impMests.TDL_TREATMENT_CODE : ""), printTypeCode, roomId);

                MPS.Processor.Mps000141.PDO.Mps000141PDO pdo = new MPS.Processor.Mps000141.PDO.Mps000141PDO(
                 impMests,
                 hisImpMestMedicines,
                 hisImpMestMaterials,
                 medicines,
                 materials,
                 BackendDataWorker.Get<HIS_IMP_SOURCE>(),
                 MedicalContractADO
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

    }
}
