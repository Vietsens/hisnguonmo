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
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.ProviderBehavior;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.OtherTreatmentHistory
{
    public class OtherTreatmentHistoryProcessor
    {
        private InitDataADO InitData;

        public OtherTreatmentHistoryProcessor(InitDataADO data)
        {
            this.InitData = data;
        }

        public void Run(Enum type)
        {
            try
            {
                if (CheckData())
                {
                    IRun iRun = null;
                    switch (this.InitData.ProviderType)
                    {
                        case ProviderType.Medisoft:
                            iRun = new MedisoftBehavior(this.InitData);
                            break;
                        default:
                            break;
                    }

                    if (iRun != null)
                        iRun.Run(type);
                    else
                        Inventec.Common.Logging.LogSystem.Error("OtherTreatmentHistoryProcessor IRun is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckData()
        {
            bool result = true;
            try
            {
                if (this.InitData == null)
                {
                    MessageBox.Show("Lỗi dữ liệu");
                    Inventec.Common.Logging.LogSystem.Error("InitData is null");
                    result = false;
                }
                else if (String.IsNullOrWhiteSpace(this.InitData.ProviderType))
                {
                    MessageBox.Show("Không xác định được loại hệ thống");
                    Inventec.Common.Logging.LogSystem.Error("ProviderType is null");
                    result = false;
                }
                else if (this.InitData.PatientId == 0 && this.InitData.Patient == null)
                {
                    MessageBox.Show("Không xác định được bệnh nhân");
                    Inventec.Common.Logging.LogSystem.Error("PatientId, Patient is null");
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
