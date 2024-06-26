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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AggrExpMestDetail.ADO;
using HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail.AggregateExpMestPrintFilter;
using HIS.Desktop.Plugins.AggrExpMestDetail.Base;
using HIS.Desktop.Plugins.Library.PrintPrescription;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HISHIS.Desktop.Plugins.AggrExpMestDetail.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail
{
    public partial class frmAggrExpMestDetail : HIS.Desktop.Utility.FormBase
    {
        internal void PrintAggregateExpMest()
        {
            try
            {
                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this;
                }

                ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.AggExpMest, ExpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void clickItemInGopDonThuoc(V_HIS_EXP_MEST aggExpMest)
        {
            try
            {
                if (aggExpMest == null)
                {
                    throw new ArgumentNullException("aggExpMest is null");
                }
                CommonParam param = new CommonParam();

                //Load expmest
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.AGGR_EXP_MEST_ID = aggExpMest.ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                if (expMests == null || expMests.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("expMests is null");
                    return;
                }

                //Load đơn phòng khám
                var serviceReqIds = expMests.Where(p => p.SERVICE_REQ_ID != null).Select(o => o.SERVICE_REQ_ID ?? 0);
                if (serviceReqIds == null || serviceReqIds.Count() == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("serviceReqIds is null");
                    return;
                }

                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.IDs = serviceReqIds.Distinct().ToList();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                   .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                if (serviceReqs == null || serviceReqs.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("serviceReqs is null");
                    return;
                }

                //Laays thuoc va tu trong kho

                HisExpMestMedicineFilter expMestMedicineFilter = new HisExpMestMedicineFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialFilter expMestMaterialFilter = new HisExpMestMaterialFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, expMestMaterialFilter, param);


                List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                if ((expMestMedicines != null && expMestMedicines.Count > 0)
                            || (expMestMaterials != null && expMestMaterials.Count > 0))
                {
                    OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();
                    outPatientPresResultSDO.ExpMests = expMests;
                    outPatientPresResultSDO.ServiceReqs = serviceReqs;
                    outPatientPresResultSDO.Medicines = expMestMedicines;
                    outPatientPresResultSDO.Materials = expMestMaterials;
                    OutPatientPresResultSDOForPrints.Add(outPatientPresResultSDO);
                }
                HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST, HIS_EXP_MEST>();
                expMest = AutoMapper.Mapper.Map<HIS_EXP_MEST>(aggExpMest);
                PrintPrescriptionProcessor printPrescriptionProcessor = new PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, expMest, this.moduleData);

                printPrescriptionProcessor.Print(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_GOP_DON_THUOC__MPS000234, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void PrintAggregateExpMest(V_HIS_EXP_MEST currentAggExpMest)
        {
            try
            {

                if (barManager1 == null)
                {
                    barManager1 = new DevExpress.XtraBars.BarManager();
                    barManager1.Form = this;
                }

                ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.AggExpMest, ExpMestAggregateMouseRightClick, barManager1);
                processor.InitMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExpMestAggregateMouseRightClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Tag is ExpMestAggregateListPopupMenuProcessor.PrintType)
                {
                    //frmAggregateExpMestPrintFilter formPrintFilter;
                    var moduleType = (ExpMestAggregateListPopupMenuProcessor.PrintType)e.Item.Tag;
                    switch (moduleType)
                    {
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InTraDoiThuoc:
                            ShowFormFilter(1);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuTongHop:

                            ShowFormFilter(2);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuoc:

                            ShowFormFilter(3);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuLinhThuocTheoBenhNhan:

                            ShowFormFilter(4);
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuCongKhaiThuocBenhNhan:
                            OnClickInPhieuCongKhaiTheoBenhNhan();
                            break;
                        case ExpMestAggregateListPopupMenuProcessor.PrintType.InPhieuHuyThuocVatTu_434:
                            OnClickInPhieuHuyThuocVatTu();
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
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.AggExpMest);
                    listArgs.Add(printType);
                    int[] selectRows = gridViewExpMestChild.GetSelectedRows();
                    
                        if (chkPrint.Checked)
                        {
                            HIS.Desktop.ADO.AggrExpMestPrintSDO sdo = new HIS.Desktop.ADO.AggrExpMestPrintSDO();
                            sdo.PrintNow = true;
                            listArgs.Add(sdo);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("_________________"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listArgs), listArgs));          
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OnClickInPhieuCongKhaiTheoBenhNhan()
        {
            try
            {
                InPhieuCongKhaiTheoBN();
                //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //richEditorMain.RunPrintTemplate("Mps000262", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //private bool DelegateRunPrinter(string printTypeCode, string fileName)
        //{
        //    bool result = false;
        //    try
        //    {
        //        switch (printTypeCode)
        //        {
        //            case "Mps000262":
        //                InPhieuCongKhaiTheoBN(printTypeCode, fileName, ref result);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return result;
        //}

        private void InPhieuCongKhaiTheoBN()
        {
            try
            {
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();

                List<ExpMestSDO> expMestCheckeds = new List<ExpMestSDO>();
                int[] selectRows = gridViewExpMestChild.GetSelectedRows();

                if (selectRows == null || selectRows.Count() == 0)
                {
                    return;
                }
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        expMestCheckeds.Add((ExpMestSDO)gridViewExpMestChild.GetRow(selectRows[i]));
                    }
                }

                List<long> expMestIds = expMestCheckeds.Select(o => o.ID).ToList();
                // nếu là load lên mặc định (check all các phiếu xuất)

                if (this.expMestMedicines != null && this.expMestMedicines.Count > 0)
                {
                    expMestMedicineTemps = this.expMestMedicines.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                }

                if (this.expMestMaterials != null && this.expMestMaterials.Count > 0)
                {
                    expMestMaterialTemps = this.expMestMaterials.Where(o => expMestIds.Contains(o.EXP_MEST_ID ?? 0)).ToList();
                }

                String message = "";

                foreach (var item in expMestCheckeds)
                {
                    if (item.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                    {
                        message += item.EXP_MEST_CODE + "; ";
                    }
                }

                if (expMestCheckeds == null || expMestCheckeds.Count() == 0)
                {
                    return;
                }

                if (!String.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Không cho phép chọn phiếu bù lẻ để in phiếu công khai [mã phiếu xuất: " + message + "]");
                }

                WaitingManager.Show();

                var groupPatient = expMestCheckeds.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL).ToList();

                List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                foreach (var item in groupPatient)
                {
                    V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMest, item);
                    lstExpMest.Add(expMest);
                }

                var mps262 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(lstExpMest, expMestMedicineTemps, expMestMaterialTemps);
                if (mps262 != null)
                {
                    mps262.Print("Mps000262");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPhieuCongKhaiThuoc_Click(object sender, EventArgs e)
        {
            try
            {
                OnClickInPhieuCongKhaiTheoBenhNhan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuHuyThuocVatTu()
        {
            try
            {
                List<V_HIS_EXP_MEST> lstExpMest = new List<V_HIS_EXP_MEST>();
                foreach (var item in ExpMestChildFromAggs)
                {
                    V_HIS_EXP_MEST expMest = new V_HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(expMest, item);
                    lstExpMest.Add(expMest);
                }

                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineTemps = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialTemps = new List<V_HIS_EXP_MEST_MATERIAL>();

                if (expMestMedicines != null && expMestMedicines.Count > 0)
                {
                    expMestMedicineTemps = expMestMedicines.Where(o => o.IS_NOT_PRES == 1).ToList();
                }

                if (expMestMaterials != null && expMestMaterials.Count > 0)
                {
                    expMestMaterialTemps = expMestMaterials.Where(o => o.IS_NOT_PRES == 1).ToList();
                }

                if (expMestMedicineTemps.Count <= 0 && expMestMaterialTemps.Count <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("Chi tiet khong co thuoc khong phải don. ko co IS_NOT_PRES = 1");
                    return;
                }

                var mps000434 = new HIS.Desktop.Plugins.Library.PrintAggrExpMest.PrintAggrExpMestProcessor(lstExpMest, expMestMedicineTemps, expMestMaterialTemps);
                mps000434.RoomId = this.moduleData.RoomId;
                if (mps000434 != null)
                {
                    mps000434.Print("Mps000434", false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
