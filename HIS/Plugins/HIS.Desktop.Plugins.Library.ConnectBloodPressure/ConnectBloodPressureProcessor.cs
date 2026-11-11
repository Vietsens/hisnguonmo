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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.ConnectBloodPressure
{
    public class ConnectBloodPressureProcessor
    {
        /// <summary>vCong 62522: lỗi gần nhất khi mở ứng dụng kết nối máy đo.</summary>
        private static Exception lastOpenError;

        private static Inventec.Common.IntegrationError.IntegrationPartner AppPartner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.LocalService("ứng dụng kết nối máy đo huyết áp (Blood.Pressure.Monitor)"); }
        }

        private static Inventec.Common.IntegrationError.IntegrationPartner DevicePartner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Device("máy đo huyết áp"); }
        }

        public static bool OpenAppBloodPressure()
        {
            lastOpenError = null;
            try
            {
                if (IsProcessOpen("Blood.Pressure.Monitor"))
                {
                    return true;
                }
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.StartupPath + @"\Integrate\BloodPressure\Blood.Pressure.Monitor.exe";
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                lastOpenError = ex;
            }
            return false;
        }

        private static bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool KillAppBloodPressure()
        {
            try
            {
                var processBloodPressure = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("Blood.Pressure.Monitor")).ToList();
                if (processBloodPressure != null && processBloodPressure.Count() > 0)
                {
                    for (int i = 0; i < processBloodPressure.Count(); i++)
                    {
                        try
                        {
                            processBloodPressure[i].Kill();
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        public static HIS_DHST GetData()
        {
            HIS_DHST result = null;
            try
            {
                if (!OpenAppBloodPressure())
                {
                    // vCong 62522: thieu file Integrate\BloodPressure\Blood.Pressure.Monitor.exe / khong co quyen chay -> may tram
                    MessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.WorkstationError(AppPartner,
                        "Lỗi mở ứng dụng kết nối máy đo (Integrate\\BloodPressure\\Blood.Pressure.Monitor.exe)",
                        lastOpenError != null ? Inventec.Common.IntegrationError.IntegrationErrorClassifier.GetDetail(lastOpenError) : null));
                    return result;
                }

                ConnectService.ServiceConnectClient client = new ConnectService.ServiceConnectClient();
                string data = client.GetData();
                if (!String.IsNullOrWhiteSpace(data))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    var receivedData = Newtonsoft.Json.JsonConvert.DeserializeObject<BloodPressureRessultADO>(data);
                    if (receivedData != null)
                    {
                        if (String.IsNullOrWhiteSpace(receivedData.Message))
                        {
                            result = new HIS_DHST();
                            if (!String.IsNullOrWhiteSpace(receivedData.Systolic))
                            {
                                result.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(receivedData.Systolic);
                            }

                            if (!String.IsNullOrWhiteSpace(receivedData.Diastolic))
                            {
                                result.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(receivedData.Diastolic);
                            }

                            if (!String.IsNullOrWhiteSpace(receivedData.Time))
                            {
                                result.EXECUTE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(receivedData.Time);
                            }

                            if (!String.IsNullOrWhiteSpace(receivedData.Pulse))
                            {
                                result.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(receivedData.Pulse);
                            }
                        }
                        else
                        {
                            // vCong 62522: ung dung ket noi bao loi thiet bi (cong COM, khong phan hoi...)
                            MessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.WorkstationError(DevicePartner, "Máy đo huyết áp/ứng dụng kết nối máy đo báo lỗi", receivedData.Message));
                        }
                    }
                    else
                    {
                        MessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(AppPartner, "lấy kết quả máy đo huyết áp"));
                    }
                }
                else
                {
                    MessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(AppPartner, "lấy kết quả máy đo huyết áp"));
                }
            }
            catch (Exception ex)
            {
                KillAppBloodPressure();
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
                // vCong 62522: truoc day chi ghi log (WCF chua san sang / ung dung tat giua chung...)
                MessageBox.Show(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, AppPartner, "lấy kết quả máy đo huyết áp"));
            }
            return result;
        }
    }
}
