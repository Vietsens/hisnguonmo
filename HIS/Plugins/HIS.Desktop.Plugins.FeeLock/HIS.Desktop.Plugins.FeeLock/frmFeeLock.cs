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
using HIS.Desktop.Controls.Session;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FeeLock
{
    public partial class frmFeeLock : Form
    {
        internal MOS.EFMODEL.DataModels.V_HIS_TREATMENT resultLockTreatment = null;
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment = null; 
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment1 = null;
        public frmFeeLock()
        {
            try
            {                InitializeComponent();
                //long treatmentId = 14603;
                //Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                //MOS.Filter.HisTreatmentView1Filter filter = new MOS.Filter.HisTreatmentView1Filter()
                //{
                //    CREATE_TIME_FROM = 20170415000000,
                //    CREATE_TIME_TO = 20170415235959
                //};
                //List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> treatment2 = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                //treatment1 = treatment2.First();
            }
            catch (Exception ex)
            {
                
            }

  
        }
        public frmFeeLock(MOS.EFMODEL.DataModels.V_HIS_TREATMENT  _treatment)
        {
            try
            {
                bool success = false;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                treatment = _treatment;
                
                InitializeComponent();
                if (treatment != null)
                {
                    if (treatment.FEE_LOCK_TIME != null && treatment.FEE_LOCK_TIME != 0)
                    {
                       //treatment.FEE_LOCK_TIME = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dtFeeLockTime.EditValue ?? 0);
                    }
                    else
                    {
                        dtFeeLockTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now()) ?? 0);
                    }
                }
                else
                {
                    dtFeeLockTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now()) ?? 0);
                }
                //Inventec.Core.EntityBase.Logging("Log du lieu dau vao khi goi api khoa ho so vien phi. MOS.EFMODEL.DataModels.HIS_TREATMENT: " + Newtonsoft.Json.JsonConvert.SerializeObject(TreatmentDTO), LogType.Info);

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_CHANGE_LOCK, ApiConsumer.ApiConsumers.MosConsumer, _treatment, param);
                

                
                dtFeeLockTime.Update();

                #region Show message
                MessageManager.Show(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (success == true)
                {
                    this.Close();
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmFeeLock_Load(object sender, EventArgs e)
        {
            MinimizeBox = false;
            MaximizeBox = false;
            try
            {
                
                dtFeeLockTime.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                treatment = treatment1;

                InitializeComponent();
                if (treatment != null)
                {
                    if (treatment.FEE_LOCK_TIME != null && treatment.FEE_LOCK_TIME != 0)
                    {
                        //treatment.FEE_LOCK_TIME = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dtFeeLockTime.EditValue ?? 0);
                    }
                    else
                    {
                        dtFeeLockTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now()) ?? 0);
                    }
                }
                else
                {
                    dtFeeLockTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now()) ?? 0);
                }
                //Inventec.Core.EntityBase.Logging("Log du lieu dau vao khi goi api khoa ho so vien phi. MOS.EFMODEL.DataModels.HIS_TREATMENT: " + Newtonsoft.Json.JsonConvert.SerializeObject(TreatmentDTO), LogType.Info);

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_CHANGE_LOCK, ApiConsumer.ApiConsumers.MosConsumer, treatment1, param);



                dtFeeLockTime.Update();

                #region Show message
                MessageManager.Show(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (success == true)
                {
                    this.Close();
                } 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }       

    
    }
}
