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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class FormDeathInfo : Form
    {
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment { get; set; }
        private int positionHandle = -1;
        internal FormTreatmentFinish Form;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;

        private UserControl uc;
        private HIS.UC.Death.DeathProcessor processor;

        public FormDeathInfo()
        {
            InitializeComponent();
        }

        public FormDeathInfo(MOS.EFMODEL.DataModels.HIS_TREATMENT _hisTreatment)
            : this()
        {
            try
            {
                this.hisTreatment = _hisTreatment;
                initUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void initUC()
        {
            try
            {
                processor = new UC.Death.DeathProcessor();
                HIS.UC.Death.ADO.DeathInitADO ado = new UC.Death.ADO.DeathInitADO();
                //ado.IsTextHolder = true;
                uc = (UserControl)processor.Run(ado);

                if (uc != null)
                {
                    this.panelControl.Controls.Add(uc);
                    uc.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormDeathInfo_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                LoadKeysFromlanguage();
                SetDefaultValueControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__TEXT");
                this.btnSave.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__CLOSE_TREATMENT_SAVE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                HIS.UC.Death.ADO.DeathDataSourcesADO ado = new UC.Death.ADO.DeathDataSourcesADO();
                ado.CurrentHisTreatment = hisTreatment;
                ado.HisDeathCauses = Base.GlobalStore.HisDeathCauses;
                ado.HisDeathWithins = Base.GlobalStore.HisDeathWithins;
                processor.Reload(uc, ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var data = processor.GetValue(uc);
                if (data is MOS.SDO.HisTreatmentFinishSDO)
                {
                    MyGetData((MOS.SDO.HisTreatmentFinishSDO)data);
                    this.Close();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
