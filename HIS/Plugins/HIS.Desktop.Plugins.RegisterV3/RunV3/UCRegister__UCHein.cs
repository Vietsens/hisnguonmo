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
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.QrCodeBHYT;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCHeniInfo.Data;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCOtherServiceReqInfo.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private async Task<object> SearchByCode(string code)
        {
            try
            {
                //LogSystem.Debug("SearchByCode => 1");
                var data = new HisPatientSDO();
                if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code is null");
                if (code.Length > 10 && code.Contains("|"))
                {
                    return GetDataQrCodeHeinCard(code);
                }
                else
                {
                    //ex khi mã sai==> nhạp la mã bhyt
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    data = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param)).SingleOrDefault();
                }
                //LogSystem.Debug("SearchByCode => 2");
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }

        private async Task<HeinCardData> GetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private void FillDataAfterSaerchPatientInUCPatientRaw(HeinCardData heinCardData)
        {
            try
            {
                //Kiem tra cau hinh co tu dong fill du lieu dia chi ghi tren the vao o dia chi benh nhan, co thi fill du lieu, khong thi bo qua
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                {
                    dataAddressPatient.Address = Inventec.Common.String.Convert.HexToUTF8Fix(heinCardData.Address);
                    this.ucAddressCombo1.SetValue(dataAddressPatient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
