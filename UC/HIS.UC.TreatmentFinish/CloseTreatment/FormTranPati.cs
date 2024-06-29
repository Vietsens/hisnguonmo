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
using HIS.Desktop.LocalStorage.BackendData;
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
    public partial class FormTranPati : Form
    {
        #region Declare
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment { get; set; }
        //private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        internal FormTreatmentFinish Form;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;

        UserControl uc;
        HIS.UC.TranPati.TranPatiProcessor processor;
        #endregion

        #region ctor
        public FormTranPati()
        {
            InitializeComponent();
        }

        public FormTranPati(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
            : this()
        {
            try
            {
                if (treatment != null)
                {
                    this.hisTreatment = treatment;
                }
                initUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void initUC()
        {
            try
            {
                processor = new UC.TranPati.TranPatiProcessor();
                HIS.UC.TranPati.ADO.TranPatiInitADO ado = new UC.TranPati.ADO.TranPatiInitADO();
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

        private void FormTranPati_Load(object sender, EventArgs e)
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
                this.Text = Form.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TRANSFER__TEXT");
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
                HIS.UC.TranPati.ADO.TranPatiDataSourcesADO ado = new UC.TranPati.ADO.TranPatiDataSourcesADO();
                ado.CurrentHisTreatment = hisTreatment;
                ado.HisMediOrgs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                ado.HisTranPatiForms = Base.GlobalStore.HisTranPatiForms;
                ado.HisTranPatiReasons = Base.GlobalStore.HisTranPatiReasons;
                processor.Reload(uc, ado);
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

    }
}
