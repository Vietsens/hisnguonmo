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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisService.frmServiceRati;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.HisService.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisRefectory.frmServiceRati
{
    public partial class frmServiceRati : DevExpress.XtraEditors.XtraForm
    {
        long ServiceID = 0;
        List<RationTimeADO> lstSerRatiADO;
        List<HIS_RATION_TIME> DataRationTime;
        List<HIS_SERVICE_RATI> DataServiceRation;

        public frmServiceRati(long ID)
        {
            this.ServiceID = ID;
            InitializeComponent();
        }

        private void frmServiceRati_Load(object sender, EventArgs e)
        {
            LoadRationTime();
            LoadServiceRati();

            FillDataToGrid();
            SetCaptionByLanguageKey();
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmServiceRati
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(frmServiceRati).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServiceRati.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmServiceRati.layoutControl2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmServiceRati.btnSave.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCheck.Caption = Inventec.Common.Resource.Get.Value("frmServiceRati.grclCheck.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCode.Caption = Inventec.Common.Resource.Get.Value("frmServiceRati.grclCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclName.Caption = Inventec.Common.Resource.Get.Value("frmServiceRati.grclName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmServiceRati.bar2.Text",ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmServiceRati.barButtonItem1.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmServiceRati.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRationTime()
        {
            CommonParam param = new CommonParam();
            MOS.Filter.HisRationTimeFilter filter = new MOS.Filter.HisRationTimeFilter();
            DataRationTime = new BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            lstSerRatiADO = new List<RationTimeADO>();
            foreach (var item in DataRationTime)
            {
                RationTimeADO Ado = new RationTimeADO();
                Ado.ID = item.ID;
                Ado.RATION_TIME_CODE = item.RATION_TIME_CODE;
                Ado.RATION_TIME_NAME = item.RATION_TIME_NAME;
                Ado.check = false;
                lstSerRatiADO.Add(Ado);
            }
        }

        private void LoadServiceRati()
        {
            CommonParam param = new CommonParam();
            MOS.Filter.HisServiceRatiFilter filter = new MOS.Filter.HisServiceRatiFilter();
            filter.SERVICE_ID = this.ServiceID;
            DataServiceRation = new BackendAdapter(param).Get<List<HIS_SERVICE_RATI>>("api/HisServiceRati/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            if (DataServiceRation != null && DataServiceRation.Count > 0)
            {
                foreach (var serrati in DataServiceRation)
                {
                    foreach (var ado in lstSerRatiADO)
                    {
                        if (ado.ID == serrati.RATION_TIME_ID)
                        {
                            ado.check = true;
                        }
                    }
                }
            }
        }

        private void FillDataToGrid()
        {
            if (lstSerRatiADO != null && lstSerRatiADO.Count > 0)
            {
                this.gridControl1.DataSource = lstSerRatiADO;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Boolean success = false;
                CommonParam param = new CommonParam();
                HisServiceRatiSDO data = new HisServiceRatiSDO();
                data.ServiceId = this.ServiceID;
                List<RationTimeADO> lst = lstSerRatiADO.Where(o => o.check == true).ToList();
                data.RationTimeIds = new List<long>();
                foreach (var item in lst)
                {
                    data.RationTimeIds.Add(item.ID);
                }
                var resultData = new BackendAdapter(param).Post<List<HIS_SERVICE_RATI>>("api/HisServiceRati/Create", ApiConsumers.MosConsumer, data, param);
                if (resultData !=null)
                {
                    success = true;
                    this.Hide();
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
            }
        }
    }
}
