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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {

        private bool CheckListServiceReqV2(List<ADO.ServiceReqADO> listServiceReq, CommonParam param)
        {
            bool result = false;
            try
            {
                    param.Messages = new List<string>();
                    Dictionary<long, List<ADO.ServiceReqADO>> dicServiceReqByTreatment = new Dictionary<long, List<ADO.ServiceReqADO>>();

                    //Dictionary<long, List<V_HIS_SERVICE_REQ_2>> dicServiceReqByTime = new Dictionary<long, List<V_HIS_SERVICE_REQ_2>>();
                   // string serviceCode = "";

                    foreach (var item in listServiceReq)
                    {
                        //if (item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        //    item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM ||
                        //    item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        //    item.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                        //{
                        //    serviceCode += String.Format("{0}, ", item.SERVICE_REQ_CODE);
                        //}
                        if (!dicServiceReqByTreatment.ContainsKey(item.TREATMENT_ID))
                            dicServiceReqByTreatment[item.TREATMENT_ID] = new List<ADO.ServiceReqADO>();
                        dicServiceReqByTreatment[item.TREATMENT_ID].Add(item);

                    }

                    //string isNotCheck = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Base.ConfigKey.MpsTotalToBordereau);

                    //if (!String.IsNullOrEmpty(serviceCode) && !(isNotCheck == "1"))
                    //{
                    //    param.Messages.Add(String.Format(Resources.ResourceMessage.DichVuLaThuoc, serviceCode));
                    //}
                    //else
                        if (dicServiceReqByTreatment.Count > 1)
                    {
                        param.Messages.Add(Resources.ResourceMessage.DichVuKhongCungHoSoDieuTri);
                    }

                    if (param.Messages.Count > 0)
                    {
                        result = true;
                    }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //InPhieu
        private void ExecuteBefPrint(ServiceReqADO _ServiceReq)
        {
            try
            {
                this.currentServiceReqPrint = new ServiceReqADO();
                if (_ServiceReq != null)
                {
                    this.currentServiceReqPrint = _ServiceReq;
                    this.serviceReqPrintRaw = GetServiceReqForPrint(_ServiceReq.ID);
                    WaitingManager.Hide();
                    if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                    {
                        this.prescriptionPrint = null;
                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = currentServiceReqPrint.ID;
                        var expMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                        if (expMest != null && expMest.Count > 0)
                        {
                            this.prescriptionPrint = expMest.FirstOrDefault();
                            if (this.prescriptionPrint.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                InDonThuocVatTu();
                            }
                        }
                        else
                        {
                            InDonThuocVatTu();
                        }
                    }
                    else if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                    {
                        //Đơn máu
                        PrintBlood();
                    }
                    else
                        ProcessingPrintV2();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessingPrintV2()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)//Khám
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                //Siêu âm
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)//Xét nghiệm
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS)
                //Nội soi
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                //Thăm dò chức năng
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)
                //Thủ thuật
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                //Phẫu thuật
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                //Chẩn đoán hình ảnh
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                }
                //else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisServiceReqTypeCFG.SERVICE_REQ_TYPE_ID__PRES)//Thuốc vật tư
                //{
                //    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118, DelegateRunPrinter);
                //}
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN)
                //Phục hồi chức năng
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC)
                //Khác
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G)
                //Giường
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)
                //giải phẫu bệnh lý
                {
                    InPhieuYeuCauDichVu(MPS000167);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

