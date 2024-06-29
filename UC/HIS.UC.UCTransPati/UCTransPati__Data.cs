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
using HIS.UC.UCTransPati.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.Icd.ADO;
using HIS.UC.UCTransPati.Config;
using HIS.UC.SecondaryIcd.ADO;

namespace HIS.UC.UCTransPati
{
    public partial class UCTransPati : UserControl
    {
        public void SetValue(UCTransPatiADO transPatiADO)
        {
            try
            {
                if (transPatiADO == null) throw new ArgumentNullException("UCTransPatiADO is null");

                txtMaNoiChuyenDen.Text = transPatiADO.NOICHUYENDEN_CODE;
                cboNoiChuyenDen.EditValue = transPatiADO.NOICHUYENDEN_CODE;

                IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = transPatiADO.ICD_CODE;
                icd.ICD_NAME = (!String.IsNullOrEmpty(transPatiADO.ICD_TEXT) ? transPatiADO.ICD_TEXT : transPatiADO.ICD_NAME);
                if (ucIcd != null)
                {
                    this.icdProcessor.Reload(ucIcd, icd);
                }

                SecondaryIcdDataADO Subicd = new SecondaryIcdDataADO();
                Subicd.ICD_SUB_CODE = transPatiADO.ICD_SUB_CODE;
                Subicd.ICD_TEXT = transPatiADO.ICD_SUB_NAME;
                if (ucSubIcd != null)
                {
                    this.SubIcdProcessor.Reload(ucSubIcd, Subicd);
                }
                if (transPatiADO.TRANSFER_IN_CMKT.HasValue)
                {
                    cboChuyenTuyen.EditValue = transPatiADO.TRANSFER_IN_CMKT;
                }
                txtInCode.Text = transPatiADO.SOCHUYENVIEN;
                txtMaHinhThucChuyen.Text = GetTranPatiFormById(transPatiADO.HINHTHUCHUYEN_ID).TRAN_PATI_FORM_CODE;
                cboHinhThucChuyen.EditValue = transPatiADO.HINHTHUCHUYEN_ID;
                txtMaLyDoChuyen.Text = GetTranPatiReasonById(transPatiADO.LYDOCHUYEN_ID).TRAN_PATI_REASON_CODE;
                cboLyDoChuyen.EditValue = transPatiADO.LYDOCHUYEN_ID;
                if (!String.IsNullOrEmpty(transPatiADO.RIGHT_ROUTER_TYPE))
                {
                    if (transPatiADO.RIGHT_ROUTER_TYPE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {

                    }
                    else if (transPatiADO.RIGHT_ROUTER_TYPE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)
                    {

                    }
                    else if (transPatiADO.RIGHT_ROUTER_TYPE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT)
                    {

                    }
                    else if (transPatiADO.RIGHT_ROUTER_TYPE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                    {

                    }
                    else if (transPatiADO.RIGHT_ROUTER_TYPE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                    {

                    }
                }
                if (transPatiADO.TRANSFER_IN_TIME_FROM != null)
                {
                    dtFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(transPatiADO.TRANSFER_IN_TIME_FROM ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtFromTime.EditValue = null;
                }
                if (transPatiADO.TRANSFER_IN_TIME_TO != null)
                {
                    dtToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(transPatiADO.TRANSFER_IN_TIME_TO ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtToTime.EditValue = null;
                }

                if(transPatiADO.TRANSFER_IN_REVIEWS != null)
                {
                    cboReviews.SelectedIndex = transPatiADO.TRANSFER_IN_REVIEWS.Value - 1;
                }    

                //ProcessLevelOfMediOrg();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///                 Sửa chức năng "Tiếp đón" (tiếp đón 1 và tiếp đón 2):
        ///Khi nhập thông tin chuyển tuyến, căn cứ vào tuyến của viện mà người dùng đang làm việc (LEVEL_CODE của HIS_BRANCH mà người dùng chọn làm việc) với tuyến của viện mà người dùng nhập "Nơi chuyển đến" để tự động điền "Hình thức chuyển" (LEVEL_CODE của HIS_MEDI_ORG), theo công thức sau:

        ///                - Nếu L2 - L1 = 1 --> chọn "Hình thức chuyển" mã "01" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE)
        ///                - Nếu L2 - L1 > 1 --> chọn "Hình thức chuyển" mã "02" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE)
        ///                - Nếu L2 - L1 < 0 --> chọn "Hình thức chuyển" mã "03" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG)
        ///                - Nếu L2 - L1 = 0 --> chọn "Hình thức chuyển" mã "04" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN)

        ///                Trong đó:
        ///                - LEVEL_CODE của "Tuyến của viện mà người dùng đang làm việc" là L1
        ///                - LEVEL_CODE của "Nơi chuyển đến" là L2

        ///                Lưu ý:
        ///                Hệ thống cũ, dữ liệu LEVEL_CODE của HIS_MEDI_ORG đang lưu dưới dạng text (TW, T, H, X), để tránh việc update cache có thể ảnh hưởng đến hiệu năng, lúc xử lý cần "if-else" để xử lý được với dữ liệu cũ, cụ thể cần check LEVEL_CODE của HIS_MEDI_ORG, gán lại giá trị trước khi tính toán:
        ///                - Nếu LEVEL_CODE = TW --> LEVEL_CODE = 1
        ///                - Nếu LEVEL_CODE = T --> LEVEL_CODE = 2
        ///                - Nếu LEVEL_CODE = H --> LEVEL_CODE = 3
        ///                - Nếu LEVEL_CODE = X --> LEVEL_CODE = 4
        ///                - Khác: --> giữ nguyên giá trị
        /// </summary>
        private void ProcessLevelOfMediOrg()
        {
            try
            {
                string lvBranch = FixWrongLevelCode(BranchDataWorker.Branch.HEIN_LEVEL_CODE);

                if (!String.IsNullOrEmpty(txtMaNoiChuyenDen.Text) && cboNoiChuyenDen.EditValue != null)
                {
                    var mediTrans = DataStore.MediOrgs.Where(o => o.MEDI_ORG_CODE == txtMaNoiChuyenDen.Text).FirstOrDefault();
                    if (mediTrans != null)
                    {
                        string lvTrans = FixWrongLevelCode(mediTrans.LEVEL_CODE);

                        int iLvBranch = int.Parse(lvBranch);
                        int iLvTrans = int.Parse(lvTrans);
                        int iKq = iLvTrans - iLvBranch;
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM tranPatiDefault = null;
                        if (iKq == 1)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq > 1)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq < 0)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault();
                        }
                        else if (iKq == 0)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault();
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Branch.HEIN_LEVEL_CODE", BranchDataWorker.Branch.HEIN_LEVEL_CODE) + Inventec.Common.Logging.LogUtil.TraceData("mediTrans.LEVEL_CODE", mediTrans.LEVEL_CODE) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tranPatiDefault), tranPatiDefault));

                        cboHinhThucChuyen.EditValue = tranPatiDefault != null ? (long?)tranPatiDefault.ID : null;
                        txtMaHinhThucChuyen.Text = tranPatiDefault != null ? tranPatiDefault.TRAN_PATI_FORM_CODE : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FixWrongLevelCode(string code)
        {
            string rs = "";
            try
            {
                if (code == "TW")
                {
                    rs = "1";
                }
                else if (code == "T")
                {
                    rs = "2";
                }
                else if (code == "H")
                {
                    rs = "3";
                }
                else if (code == "X")
                {
                    rs = "4";
                }
                else
                    rs = code;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        public UCTransPatiADO GetValue()
        {
            UCTransPatiADO transPatiADO = new UCTransPatiADO();
            try
            {
                if (cboHinhThucChuyen.EditValue != null && cboHinhThucChuyen.EditValue != "")
                    transPatiADO.HINHTHUCHUYEN_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboHinhThucChuyen.EditValue.ToString());
                if (cboLyDoChuyen.EditValue != null)
                    transPatiADO.LYDOCHUYEN_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboLyDoChuyen.EditValue.ToString());

                var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                {
                    transPatiADO.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    transPatiADO.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                }

                var SubicdValue = this.SubIcdProcessor.GetValue(this.ucSubIcd);
                if (SubicdValue != null && SubicdValue is SecondaryIcdDataADO)
                {
                    transPatiADO.ICD_SUB_CODE = ((SecondaryIcdDataADO)SubicdValue).ICD_SUB_CODE;
                    transPatiADO.ICD_SUB_NAME = ((SecondaryIcdDataADO)SubicdValue).ICD_TEXT;
                }
                if (cboNoiChuyenDen.EditValue != null && cboNoiChuyenDen.EditValue != "")
                {
                    transPatiADO.NOICHUYENDEN_CODE = cboNoiChuyenDen.EditValue.ToString();
                    transPatiADO.NOICHUYENDEN_NAME = cboNoiChuyenDen.Text;
                }
                if (cboChuyenTuyen.EditValue != null && cboChuyenTuyen.EditValue != "")
                {
                    transPatiADO.TRANSFER_IN_CMKT = Inventec.Common.TypeConvert.Parse.ToInt64(cboChuyenTuyen.EditValue.ToString());
                }
                transPatiADO.SOCHUYENVIEN = txtInCode.Text;

                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    transPatiADO.TRANSFER_IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    transPatiADO.TRANSFER_IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if(cboReviews.SelectedIndex > -1)
                {
                    transPatiADO.TRANSFER_IN_REVIEWS = (short)(cboReviews.SelectedIndex + 1);
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return transPatiADO;
        }

        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM GetTranPatiFormById(long? id)
        {
            try
            {
                if (id.HasValue)
                    return BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM();
        }

        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON GetTranPatiReasonById(long? id)
        {
            try
            {
                if (id.HasValue)
                    return BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return new MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON();
        }

        private long? GetInCMKT()
        {
            long? result = null;
            try
            {
                if (cboChuyenTuyen.EditValue != null)
                {
                    result = Inventec.Common.TypeConvert.Parse.ToInt64(cboChuyenTuyen.EditValue.ToString());
                }
                else
                    result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void RefreshUserControl()
        {
            try
            {
                IcdInputADO icd = new IcdInputADO();
                this.icdProcessor.Reload(ucIcd, icd);
                SecondaryIcdDataADO subicd = new SecondaryIcdDataADO();
                this.SubIcdProcessor.Reload(ucSubIcd, subicd);
                this.txtInCode.Text = "";
                this.txtMaHinhThucChuyen.Text = "";
                this.txtMaLyDoChuyen.Text = "";
                this.txtMaNoiChuyenDen.Text = "";
                this.cboChuyenTuyen.EditValue = null;
                this.cboHinhThucChuyen.EditValue = null;
                this.cboLyDoChuyen.EditValue = null;
                this.cboNoiChuyenDen.EditValue = null;
                dtFromTime.EditValue = null;
                dtToTime.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
