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
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private ADO.PatientADO GetPatientById(long patientId)
        {
            //Inventec.Common.Logging.LogSystem.Info("Begin get GetPatientById");
            ADO.PatientADO currentPatientADO = new ADO.PatientADO();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = new V_HIS_PATIENT();
            try
            {
                MOS.Filter.HisPatientViewFilter patientFilter = new MOS.Filter.HisPatientViewFilter();
                patientFilter.ID = patientId;
                CommonParam param = new CommonParam();
                var patients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                    currentPatientADO = new ADO.PatientADO(patient);
                }
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //Inventec.Common.Logging.LogSystem.Info("End get GetPatientById");
            return currentPatientADO;
        }

        private List<HIS_EKIP_USER> ProcessEkipUser(HIS_SERE_SERV sereServ)
        {
            List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
            try
            {
                if (this.ekipUserAdos != null && ekipUserAdos.Count > 0)
                {
                    foreach (var item in ekipUserAdos)
                    {
                        if (item.EXECUTE_ROLE_ID == 0 || String.IsNullOrEmpty(item.LOGINNAME))
                        {
                            continue;
                        }

                        AutoMapper.Mapper.CreateMap<HisEkipUserADO, HIS_EKIP_USER>();
                        HIS_EKIP_USER ekipUser = AutoMapper.Mapper.Map<HisEkipUserADO, HIS_EKIP_USER>(item);

                        var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == ekipUser.LOGINNAME);
                        if (acsUser != null)
                        {
                            if (String.IsNullOrEmpty(acsUser.USERNAME))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThieuThongTinKip, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                throw new EkipNameException();
                            }
                            else
                                ekipUser.USERNAME = acsUser.USERNAME;
                        }

                        if (sereServ.EKIP_ID.HasValue)
                        {
                            ekipUser.EKIP_ID = sereServ.EKIP_ID.Value;
                        }
                        else
                        {
                            ekipUser.EKIP_ID = 0;
                        }

                        ekipUsers.Add(ekipUser);
                    }
                }

                if (ekipUsers != null && ekipUsers.Count == 0)
                {
                    ekipUsers = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                ekipUsers = null;
                if (ex.GetType() == typeof(EkipNameException))
                {
                    throw new Exception();
                }
            }
            return ekipUsers;
        }

        private void ProcessSereServPtttInfo(ref HisSereServExtSDO sereServExtSDO)
        {
            try
            {
                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT, HIS_SERE_SERV_PTTT>();
                sereServExtSDO.HisSereServPttt = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT, HIS_SERE_SERV_PTTT>(this.sereServPTTT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private DevExpress.XtraRichEdit.RichEditControl GettxtDescription()
        {
            DevExpress.XtraRichEdit.RichEditControl result = null;
            try
            {
                if (this.isWordFull && this.wordFullDocument != null)
                {
                    result = this.wordFullDocument.txtDescription;
                }
                else
                {
                    result = this.wordDocument.txtDescription;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
