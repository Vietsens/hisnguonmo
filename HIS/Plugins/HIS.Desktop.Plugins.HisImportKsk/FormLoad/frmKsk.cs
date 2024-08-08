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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportKsk.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportKsk.FormLoad
{
    public partial class frmKsk : HIS.Desktop.Utility.FormBase
    {
        List<KskImportADO> kskAdos;
        List<KskImportADO> currentAdos;
        List<HIS_KSK> listKsk;
        RefeshReference delegateRefresh;
        DelegateSelectData delegateSelectData;
        List<V_HIS_KSK_CONTRACT> listKskContract;
        List<ACS_USER> listAcsUser;
        int positionHandleControl = -1;
        bool checkClick;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmKsk(Inventec.Desktop.Common.Modules.Module module, RefeshReference _delegate)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateRefresh = _delegate;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmKsk(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData _delegate)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.delegateSelectData = _delegate;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmKsk(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<KskImportADO> dataSource)
        {
            try
            {
                if (kskAdos != null && kskAdos.Count > 0)
                {
                    var checkError = kskAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<KskImportADO> dataSource)
        {
            try
            {
                gridControlKsk.BeginUpdate();
                gridControlKsk.DataSource = null;
                gridControlKsk.DataSource = dataSource;
                gridControlKsk.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void convertDateStringToTimeNumber(string date, ref long? dateTime, ref string check)
        {
            try
            {
                if (date.Length > 14)
                {
                    check = Message.MessageImport.Maxlength;
                    return;
                }

                if (date.Length < 14)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }

                if (date.Length > 0)
                {
                    if (!Inventec.Common.DateTime.Check.IsValidTime(date))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    dateTime = Inventec.Common.TypeConvert.Parse.ToInt64(date);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addKskToProcessList(List<KskImportADO> _ksk, ref List<KskImportADO> _kskRef)
        {
            try
            {
                _kskRef = new List<KskImportADO>();
                long i = 0;
                foreach (var item in _ksk)
                {
                    i++;
                    string error = "";
                    var kskAdo = new KskImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<KskImportADO>(kskAdo, item);

                    if (!string.IsNullOrEmpty(item.KSK_CODE))
                    {
                        if (listKsk != null && listKsk.Count > 0)
                        {
                            var kskGet = listKsk.FirstOrDefault(o => o.KSK_CODE == item.KSK_CODE.Trim());
                            if (kskGet != null)
                            {
                                kskAdo.KSK_ID = kskGet.ID;
                                kskAdo.KSK_NAME = kskGet.KSK_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Mã nhóm dịch vụ");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã nhóm dịch vụ");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã nhóm dịch vụ");
                    }

                    if (!string.IsNullOrEmpty(item.VIR_PATIENT_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.VIR_PATIENT_NAME, 200))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Họ tên");
                        }

                        try
                        {
                            int idx = item.VIR_PATIENT_NAME.Trim().LastIndexOf(" ");
                            if (idx > -1)
                            {
                                kskAdo.FIRST_NAME = item.VIR_PATIENT_NAME.Trim().Substring(idx).Trim();
                                kskAdo.LAST_NAME = item.VIR_PATIENT_NAME.Trim().Substring(0, idx).Trim();
                            }
                            else
                            {
                                kskAdo.FIRST_NAME = item.VIR_PATIENT_NAME.Trim();
                                kskAdo.LAST_NAME = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Họ tên");
                    }

                    if (!string.IsNullOrEmpty(item.GENDER_NAME))
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>();
                        if (gender != null && gender.Count > 0)
                        {
                            var genderGet = gender.FirstOrDefault(o => o.GENDER_NAME == item.GENDER_NAME.Trim());
                            if (genderGet != null)
                            {
                                kskAdo.GENDER_ID = genderGet.ID;
                                kskAdo.GENDER_CODE = genderGet.GENDER_CODE;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
                            }
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Giới tính");
                    }

                    if (!string.IsNullOrEmpty(item.DOB_STR))
                    {
                        try
                        {
                            if (item.DOB_STR.Trim().Length < 10)
                            {
                                throw new Exception();
                            }
                            var dob = Convert.ToDateTime(item.DOB_STR.Trim());
                            kskAdo.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dob) ?? 0;
                        }
                        catch (Exception)
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Ngày sinh");
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.YEAR_DOB))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.YEAR_DOB.Trim()))
                        {
                            kskAdo.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(item.YEAR_DOB.Trim() + "0101000000");
                            kskAdo.IS_HAS_NOT_DAY_DOB = 1;
                            if (kskAdo.DOB.ToString().Trim().Length > 14 || kskAdo.DOB < 0)
                            {
                                error += string.Format(Message.MessageImport.KhongHopLe, "Năm sinh");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Năm sinh");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Ngày sinh hoặc năm sinh");
                    }

                    if (!string.IsNullOrEmpty(item.PATIENT_CODE))
                    {
                        string code = item.PATIENT_CODE.Trim();
                        if (!checkDigit(code))
                        {
                            if (code.Length < 12)
                                kskAdo.PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã bệnh nhân");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ADDRESS))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ADDRESS, 200))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Địa chỉ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.PHONE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PHONE, 12))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Số điện thoại");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EMAIL))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.EMAIL, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Địa chỉ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.WORK_PLACE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.WORK_PLACE, 100))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Nơi làm việc");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.CMND_CCCD))
                    {
                        if (Encoding.UTF8.GetBytes(item.CMND_CCCD.Trim()).Count() == 9)
                        {
                            kskAdo.CMND_NUMBER = item.CMND_CCCD.Trim();
                        }
                        else if (Encoding.UTF8.GetBytes(item.CMND_CCCD.Trim()).Count() == 12)
                        {
                            kskAdo.CCCD_NUMBER = item.CMND_CCCD.Trim();
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "CMND/CCCD");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.KSK_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.KSK_ORDER_STR))
                        {
                            kskAdo.KSK_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.KSK_ORDER_STR);
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "STT hồ sơ KSK");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.BARCODE))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.BARCODE))
                        {
                            if (Encoding.UTF8.GetBytes(item.BARCODE.Trim()).Count() > 9)
                            {
                                error += string.Format(Message.MessageImport.Maxlength, "Barcode xét nghiệm");
                            }
                            else
                                kskAdo.BARCODE = item.BARCODE.Trim();
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Barcode xét nghiệm");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.INTRUCTION_TIME_STR))
                    {
                        long? dateTime = null;
                        string check = "";
                        convertDateStringToTimeNumber(item.INTRUCTION_TIME_STR, ref dateTime, ref check);
                        if (dateTime != null && string.IsNullOrEmpty(check))
                        {
                            kskAdo.INTRUCTION_TIME = dateTime ?? 0;
                        }
                        else
                        {
                            error += string.Format(check, "Ngày khám");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Ngày khám");
                    }


                    if (!string.IsNullOrEmpty(item.AdditionKskId_STR))
                    {
                        var lstAdditionKskId = BackendDataWorker.Get<HIS_KSK>().Where(o => o.IS_ACTIVE == 1 && o.KSK_CODE == item.AdditionKskId_STR).ToList();

                        if (lstAdditionKskId != null && lstAdditionKskId.Count > 0)
                        {
                            kskAdo.AdditionKskId = lstAdditionKskId.FirstOrDefault().ID;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongTonTai, "Mã nhóm dịch vụ bổ sung");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AdditionServiceIds_STR))
                    {
                        string error1 = "", error2 = "";
                        List<long> ServiceID = new List<long>();
                        List<long> ServiceID2 = new List<long>();
                        List<string> ServiceCode = new List<string>();
                        var lstSTR = item.AdditionServiceIds_STR.Split(',');

                        if (lstSTR != null && lstSTR.Count() > 0)
                        {
                            for (int a = 0; a < lstSTR.Count(); a++)
                            {
                                HisServiceFilter filter1 = new HisServiceFilter();
                                filter1.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                filter1.SERVICE_CODE__EXACT = lstSTR[a];
                                var lstAdditionServiceIds1 = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter1, new CommonParam());

                                if (lstAdditionServiceIds1 != null && lstAdditionServiceIds1.Count > 0)
                                {
                                    ServiceID.Add(lstAdditionServiceIds1.FirstOrDefault().ID);
                                    ServiceCode.Add(lstAdditionServiceIds1.FirstOrDefault().SERVICE_CODE);
                                }
                                else
                                {
                                    error1 += lstSTR[a] + ',';
                                }
                            }

                            if (ServiceID != null && ServiceID.Count > 0)
                            {
                                if (kskAdo.AdditionKskId != null)
                                {
                                    int dem = -1;
                                    foreach (var Sid in ServiceID)
                                    {
                                        dem++;
                                        HisKskServiceFilter filter2 = new HisKskServiceFilter();
                                        filter2.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                                        filter2.SERVICE_ID = Sid;
                                        filter2.KSK_ID = kskAdo.AdditionKskId;

                                        var lstAdditionServiceIds2 = new BackendAdapter(new CommonParam()).Get<List<HIS_KSK_SERVICE>>("api/HisKskService/Get", ApiConsumers.MosConsumer, filter2, new CommonParam());
                                        if (lstAdditionServiceIds2 != null && lstAdditionServiceIds2.Count > 0)
                                        {
                                            ServiceID2.Add(Sid);
                                        }
                                        else
                                        {
                                            error2 += ServiceCode[dem] + ",";
                                        }
                                    }
                                    if (ServiceID2 != null && ServiceID2.Count > 0)
                                    {
                                        kskAdo.AdditionServiceIds = ServiceID2;
                                    }
                                }
                                else
                                {
                                    error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã nhóm dịch vụ bổ sung");
                                }
                            }

                            error1 = error1.EndsWith(",") ? error1.Substring(0, error1.Length - 1) : error1;
                            if (!string.IsNullOrEmpty(error1))
                            {
                                error += string.Format(Message.MessageImport.KhongTonTai, "Mã dịch vụ bổ sung " + error1);
                            }

                            error2 = error2.EndsWith(",") ? error2.Substring(0, error2.Length - 1) : error2;
                            if (!string.IsNullOrEmpty(error2))
                            {
                                error += string.Format(Message.MessageImport.DichVuBoSungKhongtrongNhom, error2);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.MILITARY_RANK_CODE_STR))
                    {
                        var lstMilitaryRank = BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.IS_ACTIVE == 1 && o.MILITARY_RANK_CODE == item.MILITARY_RANK_CODE_STR).ToList();

                        if (lstMilitaryRank != null && lstMilitaryRank.Count > 0)
                        {
                            kskAdo.MILITARY_RANK_ID = lstMilitaryRank.FirstOrDefault().ID;
                            kskAdo.militaryRankId = lstMilitaryRank.FirstOrDefault().ID;
                            kskAdo.MILITARY_RANK_NAME_STR = lstMilitaryRank.FirstOrDefault().MILITARY_RANK_NAME;
                        }
                        else
                        {
                            kskAdo.MILITARY_RANK_NAME_STR = item.MILITARY_RANK_CODE_STR;
                            error += string.Format(Message.MessageImport.KhongTonTai, "Mã quân hàm");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.POSITION_CODE_STR))
                    {
                        var lstPosition = BackendDataWorker.Get<HIS_POSITION>().Where(o => o.IS_ACTIVE == 1 && o.POSITION_CODE.ToLower() == item.POSITION_CODE_STR.ToLower()).ToList();

                        if (lstPosition != null && lstPosition.Count > 0)
                        {
                            kskAdo.POSITION_ID = lstPosition.FirstOrDefault().ID;
                            kskAdo.positionId = lstPosition.FirstOrDefault().ID;
                            kskAdo.POSITION_NAME_STR = lstPosition.FirstOrDefault().POSITION_NAME;
                        }
                        else
                        {
                            kskAdo.POSITION_NAME_STR = item.POSITION_CODE_STR;
                            error += string.Format(Message.MessageImport.KhongTonTai, "Mã chức vụ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.PATIENT_CLASSIFY_CODE_STR))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.PATIENT_CLASSIFY_CODE_STR) <= 10)
                        {
                            var lstPatientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == 1 && o.PATIENT_CLASSIFY_CODE.ToLower() == item.PATIENT_CLASSIFY_CODE_STR.ToLower()).ToList();

                            if (lstPatientClassify != null && lstPatientClassify.Count > 0)
                            {
                                kskAdo.PATIENT_CLASSIFY_ID = lstPatientClassify.FirstOrDefault().ID;
                                kskAdo.patientClassifyId = lstPatientClassify.FirstOrDefault().ID;
                                kskAdo.PATIENT_CLASSIFY_NAME_STR = lstPatientClassify.FirstOrDefault().PATIENT_CLASSIFY_NAME;
                            }
                            else
                            {
                                kskAdo.PATIENT_CLASSIFY_NAME_STR = item.PATIENT_CLASSIFY_CODE_STR;
                                error += string.Format(Message.MessageImport.KhongTonTai, "Mã đối tượng chi tiết");
                            }
                        }
                        else
                        {
                            kskAdo.PATIENT_CLASSIFY_NAME_STR = item.PATIENT_CLASSIFY_CODE_STR;
                            error += "Mã đối tượng chi tiết phải dưới 10 ký tự|";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.CAREER_CODE_STR))
                    {
                        var lstCareer = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == 1 && o.CAREER_CODE.ToLower() == item.CAREER_CODE_STR.ToLower()).ToList();

                        if (lstCareer != null && lstCareer.Count > 0)
                        {
                            kskAdo.CAREER_ID = lstCareer.FirstOrDefault().ID;
                            kskAdo.careerId = lstCareer.FirstOrDefault().ID;
                            kskAdo.CAREER_NAME = lstCareer.FirstOrDefault().CAREER_NAME;
                            kskAdo.CAREER_NAME_STR = lstCareer.FirstOrDefault().CAREER_NAME;
                            kskAdo.CAREER_CODE = lstCareer.FirstOrDefault().CAREER_CODE;
                            kskAdo.CAREER_CODE_STR = lstCareer.FirstOrDefault().CAREER_CODE;
                        }
                        else
                        {
                            kskAdo.CAREER_NAME_STR = item.CAREER_CODE_STR;
                            error += string.Format("Mã nghề nghiệp không chính xác|");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.HT_ADDRESS_STR))
                    {
                        kskAdo.HT_ADDRESS = item.HT_ADDRESS_STR;
                        kskAdo.HT_ADDRESS_STR = item.HT_ADDRESS_STR;
                    }

                    if (!string.IsNullOrEmpty(item.HRM_KSK_CODE_STR) && string.IsNullOrEmpty(item.HRM_EMPLOYEE_CODE_STR))
                        error += "Bắt buộc phải có thông tin Mã nhân viên|";

                    if (!string.IsNullOrEmpty(item.ICD_CODE_STR))
                    {
                        var icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => o.ICD_CODE.ToLower() == item.ICD_CODE_STR.ToLower());
                        if (icd != null)
                        {
                            kskAdo.ICD_CODE = icd.ICD_CODE;
                        }
                        else
                        {
                            error += string.Format("Mã {0} không có trong danh mục ICD.|", item.ICD_CODE_STR);
                        }

                    }
                    if (!string.IsNullOrEmpty(item.ICD_NAME) && string.IsNullOrEmpty(item.ICD_CODE_STR))
                    {
                        error += "Không được để trống Mã bệnh chính khi có Tên bệnh chính|";
                    }
                    if (!string.IsNullOrEmpty(item.ICD_SUB_CODE_STR))
                    {
                        List<string> attachIcdSubCodes = new List<string>();
                        List<string> attachIcdSubErrors = new List<string>();
                        List<string> attachIcdSubCodeAvaiables = new List<string>();
                        if (item.ICD_SUB_CODE_STR.Contains(";"))
                        {
                            attachIcdSubCodes = item.ICD_SUB_CODE_STR.Split(';').ToList();
                        }
                        else
                        {
                            attachIcdSubCodes = new List<string> { item.ICD_SUB_CODE };
                        }
                        if (attachIcdSubCodes.Count() > 0)
                        {
                            var icds = BackendDataWorker.Get<HIS_ICD>();
                            foreach (var icdCode in attachIcdSubCodes)
                            {
                                var icd = icds.FirstOrDefault(o => o.ICD_CODE == icdCode);
                                if (icd != null)
                                {
                                    attachIcdSubCodeAvaiables.Add(icdCode);
                                }
                                else
                                {
                                    attachIcdSubErrors.Add(icdCode);
                                }
                            }
                        }
                        if (attachIcdSubCodeAvaiables.Count() > 0)
                        {
                            kskAdo.ICD_SUB_CODE = string.Join(";", attachIcdSubCodeAvaiables);
                        }
                        if (attachIcdSubErrors.Count() > 0)
                        {
                            error += string.Format("ICD {0} không hợp lệ|", string.Join(";", attachIcdSubErrors));
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ICD_TEXT) && string.IsNullOrEmpty(item.ICD_SUB_CODE_STR))
                    {
                        error += "Không được để trống Mã bệnh phụ khi có Tên bệnh phụ|";
                    }
                    if (!string.IsNullOrEmpty(item.NATIONAL_CODE_STR))
                    {
                        var national = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == 1 && o.NATIONAL_CODE.ToLower() == item.NATIONAL_CODE_STR.ToLower()).ToList();

                        if (national != null && national.Count > 0)
                        {
                            kskAdo.nationalId = national.FirstOrDefault().ID;
                            kskAdo.NATIONAL_CODE = national.FirstOrDefault().NATIONAL_CODE;
                            kskAdo.NATIONAL_NAME = national.FirstOrDefault().NATIONAL_NAME;
                            kskAdo.MPS_NATIONAL_CODE = national.FirstOrDefault().MPS_NATIONAL_CODE;
                        }
                        else
                        {
                            kskAdo.NATIONAL_CODE_STR = item.NATIONAL_CODE_STR;
                            error += string.Format("Mã quốc gia {0} không có trong danh mục|", item.NATIONAL_CODE_STR);
                        }
                    }
                    else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.HisImportKsk.National_Required") == "1")
                    {
                        error += "Bắt buộc nhập mã quốc gia|";
                    }

                    if (!string.IsNullOrEmpty(item.PROVINCE_CODE_STR))
                    {
                        var lstSdaProvince = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.IS_ACTIVE == 1 && o.PROVINCE_CODE.ToLower() == item.PROVINCE_CODE_STR.ToLower()).ToList();

                        if (lstSdaProvince != null && lstSdaProvince.Count > 0)
                        {
                            if (kskAdo.nationalId.HasValue)
                            {
                                kskAdo.provinceId = lstSdaProvince.FirstOrDefault().ID;
                                var lstSdaProvinceTmp = lstSdaProvince.Where(o => o.NATIONAL_ID == kskAdo.nationalId.Value).ToList();
                                if (lstSdaProvinceTmp != null && lstSdaProvinceTmp.Count > 0)
                                {
                                    kskAdo.provinceId = lstSdaProvinceTmp.FirstOrDefault().ID;
                                    kskAdo.PROVINCE_CODE = lstSdaProvinceTmp.FirstOrDefault().PROVINCE_CODE;
                                    kskAdo.PROVINCE_NAME = lstSdaProvinceTmp.FirstOrDefault().PROVINCE_NAME;
                                }
                                else
                                {
                                    error += string.Format("Tỉnh {0} có mã {1} không thuộc quốc gia {2}|", lstSdaProvince.FirstOrDefault().PROVINCE_NAME, lstSdaProvince.FirstOrDefault().PROVINCE_CODE, kskAdo.NATIONAL_NAME);
                                }
                            }
                            else
                            {
                                kskAdo.provinceId = lstSdaProvince.FirstOrDefault().ID;
                                kskAdo.PROVINCE_CODE = lstSdaProvince.FirstOrDefault().PROVINCE_CODE;
                                kskAdo.PROVINCE_NAME = lstSdaProvince.FirstOrDefault().PROVINCE_NAME;
                                var national = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == 1 && o.ID == lstSdaProvince.FirstOrDefault().NATIONAL_ID).ToList();
                                if (national != null && national.Count > 0)
                                {
                                    kskAdo.nationalId = national.FirstOrDefault().ID;
                                    kskAdo.NATIONAL_CODE = national.FirstOrDefault().NATIONAL_CODE;
                                    kskAdo.NATIONAL_NAME = national.FirstOrDefault().NATIONAL_NAME;
                                    kskAdo.MPS_NATIONAL_CODE = national.FirstOrDefault().MPS_NATIONAL_CODE;
                                }
                            }
                        }
                        else
                        {
                            kskAdo.PROVINCE_CODE_STR = item.PROVINCE_CODE_STR;
                            error += string.Format("Mã tỉnh {0} không có trong danh mục|", item.PROVINCE_CODE_STR);
                        }
                    }
                    else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.HisImportKsk.National_Required") == "1")
                    {
                        error += "Bắt buộc nhập mã tỉnh|";
                    }
                    if (!string.IsNullOrEmpty(item.DISTRICT_CODE_STR))
                    {
                        var lstSdaDistrict = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.IS_ACTIVE == 1 && o.DISTRICT_CODE.ToLower() == item.DISTRICT_CODE_STR.ToLower()).ToList();

                        if (lstSdaDistrict != null && lstSdaDistrict.Count > 0)
                        {
                            if (kskAdo.provinceId.HasValue)
                            {
                                kskAdo.districtId = lstSdaDistrict.FirstOrDefault().ID;
                                var sdaDistrict = lstSdaDistrict.Where(o => o.PROVINCE_ID == kskAdo.provinceId.Value).ToList();
                                if (sdaDistrict != null && sdaDistrict.Count > 0)
                                {
                                    kskAdo.districtId = sdaDistrict.FirstOrDefault().ID;
                                    kskAdo.DISTRICT_CODE = sdaDistrict.FirstOrDefault().DISTRICT_CODE;
                                    kskAdo.DISTRICT_NAME = sdaDistrict.FirstOrDefault().DISTRICT_NAME;
                                }
                                else
                                {
                                    error += string.Format("Huyện {0} có mã {1} không thuộc tỉnh {2}|", lstSdaDistrict.FirstOrDefault().DISTRICT_NAME, lstSdaDistrict.FirstOrDefault().DISTRICT_CODE, kskAdo.PROVINCE_NAME);
                                }
                            }
                            else
                            {
                                error += "Nhập mã huyện nhưng chưa nhập mã tỉnh|";
                                kskAdo.districtId = lstSdaDistrict.FirstOrDefault().ID;
                                kskAdo.DISTRICT_CODE = lstSdaDistrict.FirstOrDefault().DISTRICT_CODE;
                                kskAdo.DISTRICT_NAME = lstSdaDistrict.FirstOrDefault().DISTRICT_NAME;
                            }
                        }
                        else
                        {
                            kskAdo.DISTRICT_CODE_STR = item.DISTRICT_CODE_STR;
                            error += string.Format("Mã huyện {0} không có trong danh mục|", item.DISTRICT_CODE_STR);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.COMMUNE_CODE_STR))
                    {
                        var lstSdaCommune = BackendDataWorker.Get<SDA_COMMUNE>().Where(o => o.IS_ACTIVE == 1 && o.COMMUNE_CODE.ToLower() == item.COMMUNE_CODE_STR.ToLower()).ToList();

                        if (lstSdaCommune != null && lstSdaCommune.Count > 0)
                        {
                            if (kskAdo.districtId.HasValue)
                            {
                                var sdaCommune = lstSdaCommune.Where(o => o.DISTRICT_ID == kskAdo.districtId.Value).ToList();
                                if (sdaCommune != null && sdaCommune.Count > 0)
                                {
                                    kskAdo.COMMUNE_CODE = sdaCommune.FirstOrDefault().COMMUNE_CODE;
                                    kskAdo.COMMUNE_NAME = sdaCommune.FirstOrDefault().COMMUNE_NAME;
                                }
                                else
                                {
                                    error += string.Format("Xã {0} có mã {1} không thuộc huyện {2}|", lstSdaCommune.FirstOrDefault().COMMUNE_NAME, lstSdaCommune.FirstOrDefault().COMMUNE_CODE, kskAdo.DISTRICT_NAME);
                                }
                            }
                            else
                            {
                                error += "Nhập mã xã nhưng chưa nhập mã huyện|";
                                kskAdo.COMMUNE_CODE = lstSdaCommune.FirstOrDefault().COMMUNE_CODE;
                                kskAdo.COMMUNE_NAME = lstSdaCommune.FirstOrDefault().COMMUNE_NAME;
                            }
                        }
                        else
                        {
                            kskAdo.COMMUNE_CODE_STR = item.COMMUNE_CODE_STR;
                            error += string.Format("Mã xã {0} không có trong danh mục|", item.COMMUNE_CODE_STR);
                        }
                    }

                    kskAdo.ERROR = error;
                    kskAdo.ID = i;

                    _kskRef.Add(kskAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckHour(string input, ref string check)
        {
            try
            {
                if (input.Length > 4)
                {
                    check = Message.MessageImport.KhongHopLe;
                    return;
                }
                else
                {
                    if (checkHour(input))
                    {
                        check = Message.MessageImport.KhongHopLe;
                        return;
                    }
                    else
                    {
                        var gio = input.Substring(0, 2);
                        var phut = input.Substring(2, 2);

                        if (Inventec.Common.TypeConvert.Parse.ToInt32(gio) > 24 || Inventec.Common.TypeConvert.Parse.ToInt32(gio) < 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                        if (Inventec.Common.TypeConvert.Parse.ToInt32(phut) > 60 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) < 0 || Inventec.Common.TypeConvert.Parse.ToInt32(phut) % 5 != 0)
                        {
                            check = Message.MessageImport.KhongHopLe;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkNumber(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]) && s[i] != ',')
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private bool checkHour(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsNumber(s[i]))
                        return result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool checkSuccess = false;
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (cboContract.EditValue != null)
                {
                    var contract = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                    if (contract != null && contract.EXPIRY_DATE != null && ((contract.EXPIRY_DATE ?? 0) + 235959) < (Inventec.Common.DateTime.Get.Now() ?? 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Hợp đồng khám sức khỏe đã hết hạn hiệu lực");
                        return;
                    }
                }

                WaitingManager.Show();

                HisKskContractSDO kskContractSdo = new HisKskContractSDO();
                kskContractSdo.KskContractId = Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString());
                kskContractSdo.RequestRoomId = this.currentModule.RoomId;
                kskContractSdo.Loginname = cboLogin.EditValue.ToString();
                kskContractSdo.Username = listAcsUser.FirstOrDefault(o => o.LOGINNAME == cboLogin.EditValue.ToString()).USERNAME;
                //kskContractSdo. = item.HRM_EMPLOYEE_CODE_STR;
                //kskContractSdo.HrmEmployeeCode = 

                List<MOS.SDO.HisKskPatientSDO> kskPatientSDOs = new List<MOS.SDO.HisKskPatientSDO>();

                //AutoMapper.Mapper.CreateMap<KskImportADO, HIS_EXECUTE_ROOM>();
                //var data = AutoMapper.Mapper.Map<List<HIS_EXECUTE_ROOM>>(kskAdos);
                if (kskAdos != null && kskAdos.Count > 0)
                {
                    foreach (var item in kskAdos)
                    {
                        item.ID = 0;
                        MOS.SDO.HisKskPatientSDO sdo = new MOS.SDO.HisKskPatientSDO();
                        HIS_PATIENT patient = new HIS_PATIENT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(patient, item);

                        sdo.KskId = item.KSK_ID;
                        sdo.IntructionTime = item.INTRUCTION_TIME;
                        sdo.Patient = patient;
                        sdo.KskOrder = item.KSK_ORDER ?? 0;
                        sdo.Barcode = item.BARCODE;
                        sdo.AdditionKskId = item.AdditionKskId;
                        sdo.AdditionServiceIds = item.AdditionServiceIds;
                        sdo.HrmKskCode = item.HRM_KSK_CODE_STR;
                        sdo.HrmEmployeeCode = item.HRM_EMPLOYEE_CODE_STR;
                        sdo.IcdCode = item.ICD_CODE;
                        sdo.IcdName = item.ICD_NAME;
                        sdo.IcdSubCode = item.ICD_SUB_CODE;
                        sdo.IcdText = item.ICD_TEXT;
                        kskPatientSDOs.Add(sdo);
                    }
                }
                kskContractSdo.KskPatients = kskPatientSDOs;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("input____SDO", kskContractSdo));

                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param)
                        .PostRO<MOS.SDO.HisKskContractSDO>("api/HisKskContract/Import", ApiConsumers.MosConsumer, kskContractSdo, param);
                if (rs != null)
                {
                    if (!rs.Success)
                    {
                        if (rs.Data != null)
                        {
                            var error = rs.Data.KskPatients.Where(o => o.Descriptions != null && o.Descriptions.Count > 0).ToList();
                            if (error != null && error.Count > 0)
                            {
                                checkSuccess = true;
                                string message = "Có " + error.Count.ToString() + " hồ sơ lỗi. Bạn cần xuất dữ liệu lỗi để kiếm tra";
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo");

                                kskAdos = new List<KskImportADO>();
                                foreach (var item in rs.Data.KskPatients)
                                {
                                    var ado = new KskImportADO(item, listKsk);
                                    kskAdos.Add(ado);
                                }

                                SetDataSource(kskAdos);
                            }
                        }
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        GetData();
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                        if (this.delegateSelectData != null)
                        {
                            this.delegateSelectData(Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                        }
                    }

                    WaitingManager.Hide();

                    if (!checkSuccess)
                    {
                        #region Hien thi message thong bao
                        MessageManager.Show(this.ParentForm, param, rs.Success);
                    }
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Show_Error_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (KskImportADO)gridViewKsk.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (KskImportADO)gridViewKsk.GetFocusedRow();
                if (row != null)
                {
                    if (kskAdos != null && kskAdos.Count > 0)
                    {
                        kskAdos.Remove(row);
                        gridControlKsk.DataSource = null;
                        gridControlKsk.DataSource = kskAdos;
                        CheckErrorLine(null);
                        if (checkClick)
                        {
                            if (btnShowLineError.Text == "Dòng lỗi")
                            {
                                btnShowLineError.Text = "Dòng không lỗi";
                            }
                            else
                            {
                                btnShowLineError.Text = "Dòng lỗi";
                            }
                            btnShowLineError_Click(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                checkClick = true;
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = kskAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = kskAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewKsk_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    KskImportADO data = (KskImportADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(data.ERROR))
                        {
                            e.RepositoryItem = Btn_ErrorLine;
                        }
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = Btn_Delete;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewKsk_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    KskImportADO pData = (KskImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                listKsk = new List<HIS_KSK>();
                CommonParam param = new CommonParam();

                HisKskFilter kskFilter = new HisKskFilter();
                kskFilter.IS_ACTIVE = 1;
                listKsk = new BackendAdapter(param).Get<List<HIS_KSK>>("api/HisKsk/Get", ApiConsumers.MosConsumer, kskFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmKsk_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ValidateForm();
                InitComboContract();
                InitComboLogin();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                btnSave.Enabled = false;
                btnShowLineError.Enabled = false;
                GetData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                listKskContract = new BackendAdapter(param).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboContract, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLogin()
        {
            try
            {
                listAcsUser = BackendDataWorker.Get<ACS_USER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tài khoản", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Tên tài khoản", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 350);
                ControlEditorLoader.Load(cboLogin, listAcsUser, controlEditorADO);
                cboLogin.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                txtLogin.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "\\Tmp\\Imp", "IMPORT_KSK.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_KSK";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisKskImport = import.GetWithCheck<KskImportADO>(0);
                        if (hisKskImport != null && hisKskImport.Count > 0)
                        {
                            List<KskImportADO> listAfterRemove = new List<KskImportADO>();
                            foreach (var item in hisKskImport)
                            {
                                listAfterRemove.Add(item);
                            }

                            foreach (var item in hisKskImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.PATIENT_CODE)
                                    && string.IsNullOrEmpty(item.VIR_PATIENT_NAME)
                                    && string.IsNullOrEmpty(item.GENDER_NAME)
                                    && string.IsNullOrEmpty(item.DOB_STR)
                                    && string.IsNullOrEmpty(item.YEAR_DOB)
                                    && string.IsNullOrEmpty(item.PHONE)
                                    && string.IsNullOrEmpty(item.CMND_CCCD)
                                    && string.IsNullOrEmpty(item.ADDRESS)
                                    && string.IsNullOrEmpty(item.EMAIL)
                                    && string.IsNullOrEmpty(item.KSK_CODE)
                                    && string.IsNullOrEmpty(item.AdditionKskId_STR)
                                    && string.IsNullOrEmpty(item.AdditionServiceIds_STR)
                                    && string.IsNullOrEmpty(item.INTRUCTION_TIME_STR)
                                    && string.IsNullOrEmpty(item.KSK_ORDER_STR)
                                    && string.IsNullOrEmpty(item.BARCODE)
                                    && string.IsNullOrEmpty(item.WORK_PLACE)
                                    && string.IsNullOrEmpty(item.MILITARY_RANK_CODE_STR)
                                    && string.IsNullOrEmpty(item.POSITION_CODE_STR)
                                    && string.IsNullOrEmpty(item.PATIENT_CLASSIFY_CODE_STR)
                                    && string.IsNullOrEmpty(item.CAREER_CODE_STR)
                                    && string.IsNullOrEmpty(item.HT_ADDRESS_STR);

                                if (checkNull)
                                {
                                    listAfterRemove.Remove(item);
                                }
                            }

                            WaitingManager.Hide();

                            this.currentAdos = listAfterRemove;

                            if (this.currentAdos != null && this.currentAdos.Count > 0)
                            {
                                checkClick = false;
                                btnSave.Enabled = true;
                                btnShowLineError.Enabled = true;
                                kskAdos = new List<KskImportADO>();
                                addKskToProcessList(currentAdos, ref kskAdos);
                                SetDataSource(kskAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidateLookupWithTextEdit(cboContract, txtContract);
                ValidateLookupWithTextEdit(cboLogin, txtLogin);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtContract_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtContract.Text))
                    {
                        cboContract.EditValue = null;
                        cboContract.Focus();
                        cboContract.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_KSK_CONTRACT> searchs = null;
                        var listData1 = this.listKskContract.Where(o => o.KSK_CONTRACT_CODE.ToUpper().Contains(txtContract.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.KSK_CONTRACT_CODE.ToUpper() == txtContract.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtContract.Text = searchs[0].KSK_CONTRACT_CODE;
                            cboContract.EditValue = searchs[0].ID;
                        }
                        else
                        {
                            cboContract.Focus();
                            cboContract.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboContract_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboContract.EditValue != null)
                    {
                        var data = this.listKskContract.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboContract.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtContract.Text = data.KSK_CONTRACT_CODE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLogin_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtLogin.Text))
                    {
                        cboLogin.EditValue = null;
                        cboLogin.Focus();
                        cboLogin.ShowPopup();
                    }
                    else
                    {
                        List<ACS_USER> searchs = null;
                        var listData1 = this.listAcsUser.Where(o => o.LOGINNAME.ToUpper().Contains(txtLogin.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.LOGINNAME.ToUpper() == txtLogin.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtLogin.Text = searchs[0].LOGINNAME;
                            cboLogin.EditValue = searchs[0].LOGINNAME;
                        }
                        else
                        {
                            cboLogin.Focus();
                            cboLogin.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboLogin_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLogin.EditValue != null)
                    {
                        var data = this.listAcsUser.SingleOrDefault(o => o.LOGINNAME == cboLogin.EditValue.ToString());
                        if (data != null)
                        {
                            txtLogin.Text = data.LOGINNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CreateExport));
                thread.Priority = ThreadPriority.Normal;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateExport()
        {
            try
            {
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "EXPORT_KSK.xlsx");

                //chọn đường dẫn
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    List<KskImportADO> export = new List<KskImportADO>();

                    if (this.kskAdos != null && this.kskAdos.Count > 0)
                    {
                        var errorList = this.kskAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                        if (errorList != null && errorList.Count > 0)
                        {
                            export = errorList;
                        }
                    }

                    ProcessData(export, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<KskImportADO> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ErrorList", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }
    }
}
