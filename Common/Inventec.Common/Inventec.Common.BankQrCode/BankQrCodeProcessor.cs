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
using Inventec.Common.BankQrCode.ADO;
using Inventec.Common.BankQrCode.Provider.BIDV;
using Inventec.Common.BankQrCode.Provider.LPBANK;
using Inventec.Common.BankQrCode.Provider.PVCB;
using Inventec.Common.BankQrCode.Provider.VIETTINBANK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode
{
    public class BankQrCodeProcessor
    {
        BankQrCodeInputADO InputData;
        public BankQrCodeProcessor(BankQrCodeInputADO inputData)
        {
            this.InputData = inputData;
        }

        public ResultQrCode GetQrCode(ProvinceType type)
        {
            if (this.InputData == null || string.IsNullOrWhiteSpace(this.InputData.SystemConfig) || string.IsNullOrWhiteSpace(this.InputData.TransactionCode) || this.InputData.Amount <= 0)
            {
                ResultQrCode error = new ResultQrCode();
                error.Message = "Dữ liệu đầu vào không hợp lệ";
                return error;
            }

            IRun iRunQrData = null;
            switch (type)
            {
                case ProvinceType.BIDV:
                    iRunQrData = new BIDVProcessor(this.InputData);
                    break;
                case ProvinceType.VIETINBANK:
                    iRunQrData = new VietinBankProcessor(this.InputData);
                    break;
                case ProvinceType.PVCB:
                    iRunQrData = new PvcomBankProcessor(this.InputData);
                    break;
                case ProvinceType.LPBANK:
                    iRunQrData = new LPBankProcessor(this.InputData);
                    break;
                default:
                    break;
            }

            return iRunQrData.Run();
        }

        public ResultQrCode GetConsumerQrCode(ProvinceType type)
        {
            if (this.InputData == null || this.InputData.ConsumerInfo == null)
            {
                ResultQrCode error = new ResultQrCode();
                error.Message = "Dữ liệu đầu vào không hợp lệ";
                return error;
            }

            IRun iRunQrData = null;
            switch (type)
            {
                case ProvinceType.BIDV:
                    iRunQrData = new BIDVProcessor(this.InputData);
                    break;
                case ProvinceType.VIETINBANK:
                    iRunQrData = new VietinBankProcessor(this.InputData);
                    break;
                case ProvinceType.PVCB:
                    iRunQrData = new PvcomBankProcessor(this.InputData);
                    break;
                case ProvinceType.LPBANK:
                    iRunQrData = new LPBankProcessor(this.InputData);
                    break;
                default:
                    break;
            }

            return iRunQrData.RunConsumer();
        }
    }
}
