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
using HIS.Desktop.Utility;
using MOS.SDO;
using HIS.UC.UCImageInfo.ADO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.IO;
using System.Drawing.Imaging;
using HIS.UC.UCPatientRaw.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.PlusInfo.ADO;
using SDA.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCImage(HisPatientSDO hisPatientSDO)
        {
            try
            {
                UCImageInfoADO dataImage = new UCImageInfoADO();
                dataImage.ListImageData = new List<ImageInfoADO>();
                if (!String.IsNullOrEmpty(hisPatientSDO.AVATAR_URL))
                {
                    dataImage.ListImageData.Add(new ImageInfoADO() { Url = hisPatientSDO.AVATAR_URL, Type = UC.UCImageInfo.Base.ImageType.CHAN_DUNG });
                }

                if (!String.IsNullOrEmpty(hisPatientSDO.BHYT_URL))
                {
                    dataImage.ListImageData.Add(new ImageInfoADO() { Url = hisPatientSDO.BHYT_URL, Type = UC.UCImageInfo.Base.ImageType.THE_BHYT });
                }

                if (!String.IsNullOrEmpty(hisPatientSDO.CMND_BEFORE_URL))
                {
                    dataImage.ListImageData.Add(new ImageInfoADO() { Url = hisPatientSDO.CMND_BEFORE_URL, Type = UC.UCImageInfo.Base.ImageType.CMND_CCCD_TRUOC });
                }

                if (!String.IsNullOrEmpty(hisPatientSDO.CMND_AFTER_URL))
                {
                    dataImage.ListImageData.Add(new ImageInfoADO() { Url = hisPatientSDO.CMND_AFTER_URL, Type = UC.UCImageInfo.Base.ImageType.CMND_CCCD_SAU });
                }

                this.ucImageInfo1.SetValue(dataImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataByCmndBefore(object obj)
        {
            try
            {
                if (obj != null )
                {
                    Image data = (Image)obj;

                    WaitingManager.Show();
                    string base64 = ConvertBase64Image(data);
                    if (!String.IsNullOrEmpty(base64))
                    {
                        CommonParam param = new CommonParam();
                        RecognitionSDO updateDTO = new RecognitionSDO();
                        updateDTO.ImageBase64 = base64;

                        var resultData = new BackendAdapter(param).Post<RecognitionResultSDO>("api/HisPatient/Recognition", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            SetDataToControlBefore(resultData);
                        }

                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToControlBefore(RecognitionResultSDO data)
        {
            try
            {
                if (data != null)
                {
                    UCPatientRawADO patientRawInfoValue = ucPatientRaw1.GetValue();

                    HIS_GENDER gender = null;
                    string dob = "";

                    if (!String.IsNullOrEmpty(data.sex) && data.sex != "N/A")
                    {
                        gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_NAME.ToUpper() == data.sex.ToUpper());
                    }

                    if (!String.IsNullOrEmpty(data.birthday) && data.birthday != "N/A")
                    {
                        string[] dobStr = data.birthday != null ? data.birthday.Split('-') : null;
                        if (dobStr != null && dobStr.Length == 3)
                        {
                            string dd = string.Format("{0:00}", Convert.ToInt64(dobStr[0]));
                            string mm = string.Format("{0:00}", Convert.ToInt64(dobStr[1]));
                            string yyyy = string.Format("{0:0000}", Convert.ToInt64(dobStr[2]));
                            dob = string.Format("{0}{1}{2}000000", yyyy, mm, dd);
                        }
                    }

                    if (patientRawInfoValue != null && !String.IsNullOrEmpty(patientRawInfoValue.PATIENT_NAME))
                    {
                        if (MessageBox.Show(ResourceMessage.BanCoMuonThayDoiThongTinBenhNhanTheoCmndCccdHayKhong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }

                    patientRawInfoValue = new UCPatientRawADO();
                    UCAddressADO addressInfoValue = ucAddressCombo1.GetValue();
                    UCPlusInfoADO patientPlusInformationInfoValue = ucPlusInfo1.GetValue();
                    if (addressInfoValue == null)
                        addressInfoValue = new UCAddressADO();

                    if (patientPlusInformationInfoValue == null)
                        patientPlusInformationInfoValue = new UCPlusInfoADO();

                    addressInfoValue.Address = data.address;
                    patientRawInfoValue.PATIENT_NAME = data.name != "N/A" ? data.name : "";
                    patientRawInfoValue.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dob);
                    patientPlusInformationInfoValue.CMND_NUMBER = data.id != "N/A" ? data.id : "";

                    if (gender != null)
                    {
                        patientRawInfoValue.GENDER_ID = gender.ID;
                    }

                    if (!String.IsNullOrWhiteSpace(data.province) && data.province.ToUpper() != "N/A")
                    {
                        var province = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE.ToUpper() == data.province.ToUpper());
                        if (province != null)
                        {
                            addressInfoValue.Province_Code = province.PROVINCE_CODE;
                            addressInfoValue.Province_Name = province.PROVINCE_CODE;
                        }

                        if (province != null && !String.IsNullOrWhiteSpace(data.district) && data.district.ToUpper() != "N/A")
                        {
                            var disTrict = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.PROVINCE_CODE == province.PROVINCE_CODE
                                && o.DISTRICT_CODE.ToUpper() == data.district.ToUpper());
                            if (disTrict != null)
                            {
                                addressInfoValue.District_Code = disTrict.DISTRICT_CODE;
                                addressInfoValue.District_Name = disTrict.DISTRICT_CODE;
                            }

                            if (disTrict != null && !String.IsNullOrWhiteSpace(data.precinct) && data.precinct.ToUpper() != "N/A")
                            {
                                var commune = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.DISTRICT_CODE.ToUpper() == disTrict.DISTRICT_CODE.ToUpper() && o.COMMUNE_CODE.ToUpper() == data.precinct.ToUpper());
                                if (commune != null)
                                {
                                    addressInfoValue.Commune_Code = commune.COMMUNE_CODE;
                                    addressInfoValue.Commune_Name = commune.COMMUNE_CODE;
                                }
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Error("SetDataToControlBefore");
                    ucPatientRaw1.SetValue(patientRawInfoValue);
                    ucAddressCombo1.SetValue(addressInfoValue);
                    ucPlusInfo1.SetValue(patientPlusInformationInfoValue);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ConvertBase64Image(Image data)
        {
            string result = "";
            Bitmap bmp = new Bitmap(data);
            using (MemoryStream m = new MemoryStream())
            {
                bmp.Save(m, ImageFormat.Jpeg);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);

                if (base64String.Length % 4 == 1)
                {
                    result = base64String + "===";
                }
                else if (base64String.Length % 4 == 2)
                {
                    result = base64String + "==";
                }
                else if (base64String.Length % 4 == 3)
                {
                    result = base64String + "=";
                }
                else
                {
                    result = base64String;
                }
            }
            return result;
        }

        private void ReloadDataByCmndAfter(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Image data = (Image)obj;

                    WaitingManager.Show();
                    string base64 = ConvertBase64Image(data);
                    if (!String.IsNullOrEmpty(base64))
                    {
                        CommonParam param = new CommonParam();
                        RecognitionSDO updateDTO = new RecognitionSDO();
                        updateDTO.ImageBase64 = base64;

                        var resultData = new BackendAdapter(param).Post<RecognitionResultSDO>("api/HisPatient/Recognition", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            SetDataToControlAfter(resultData);
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToControlAfter(RecognitionResultSDO data)
        {
            try
            {
                if (data != null)
                {
                    UCPlusInfoADO patientPlusInformationInfoValue = ucPlusInfo1.GetValue();
                    string issueDate = "";
                    if (!String.IsNullOrEmpty(data.issue_date) && data.issue_date != "N/A")
                    {
                        string[] issueDateStr = data.issue_date != null ? data.issue_date.Split('-') : null;

                        if (issueDateStr != null && issueDateStr.Length == 3)
                        {
                            string dd = string.Format("{0:00}", Convert.ToInt64(issueDateStr[0]));
                            string mm = string.Format("{0:00}", Convert.ToInt64(issueDateStr[1]));
                            string yyyy = string.Format("{0:0000}", Convert.ToInt64(issueDateStr[2]));
                            issueDate = string.Format("{0}{1}{2}000000", yyyy, mm, dd);
                        }
                    }

                    if (patientPlusInformationInfoValue == null)
                        patientPlusInformationInfoValue = new UCPlusInfoADO();

                    if ((patientPlusInformationInfoValue.CMND_DATE.HasValue && !String.IsNullOrEmpty(issueDate) && Convert.ToInt64(issueDate) != patientPlusInformationInfoValue.CMND_DATE.Value)
                        || (!String.IsNullOrEmpty(patientPlusInformationInfoValue.CMND_PLACE) && data.issue_by.ToUpper() != patientPlusInformationInfoValue.CMND_PLACE.Trim().ToUpper()))
                    {
                        if (MessageBox.Show(ResourceMessage.BanCoMuonThayDoiThongTinNgayCapNoiCapTheoCmndCccdHayKhong, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    patientPlusInformationInfoValue.CMND_DATE = Convert.ToInt64(issueDate);
                    patientPlusInformationInfoValue.CMND_PLACE = data.issue_by != "N/A" ? data.issue_by : "";
                    ucPlusInfo1.SetValue(patientPlusInformationInfoValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadDataByCmnd(object obj1, object obj2)
        {
            try
            {
                
                if (obj1 != null)
                {
                    Image data = (Image)obj1;

                    WaitingManager.Show();
                    string base64 = ConvertBase64Image(data);
                    if (!String.IsNullOrEmpty(base64))
                    {
                        CommonParam param = new CommonParam();
                        RecognitionSDO updateDTO = new RecognitionSDO();
                        updateDTO.ImageBase64 = base64;

                        var resultData = new BackendAdapter(param).Post<RecognitionResultSDO>("api/HisPatient/Recognition", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            SetDataToControlAfter(resultData);
                        }
                    }
                    WaitingManager.Hide();
                }
                if (obj2 != null)
                {
                    Image data = (Image)obj2;

                    WaitingManager.Show();
                    string base64 = ConvertBase64Image(data);
                    if (!String.IsNullOrEmpty(base64))
                    {
                        CommonParam param = new CommonParam();
                        RecognitionSDO updateDTO = new RecognitionSDO();
                        updateDTO.ImageBase64 = base64;

                        var resultData = new BackendAdapter(param).Post<RecognitionResultSDO>("api/HisPatient/Recognition", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            SetDataToControlBefore(resultData);
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
