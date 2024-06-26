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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7V2;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using MOS.SDO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ExecuteRoom.ReqChangeService
{


    public partial class FormKeThuoc : Form
    {
        long MediStockId;
        int select;
        long roomID;
        List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO> hisreq = new List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO>();
        public FormKeThuoc(long? MediStockId, int select_, long roomID_, List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO> hisreq_)
        {
            InitializeComponent();
            this.MediStockId = MediStockId ?? 0;
            this.select = select_;
            this.roomID = roomID_;
            this.hisreq = hisreq_;
        }
        MOS.SDO.SubclinicalPresResultSDO Subclinical;
        private void FormKeThuoc_Load(object sender, EventArgs e)
        {
            SetIconFrm();
            btnDongY.Focus();

            btnDongY.Enabled = false;
            // GetDataExpPresCreateByConfig();
        }
        private void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormKeThuoc
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Lang", typeof(FormKeThuoc).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDongY.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.btnDongY.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormKeThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnDongY_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void GetDataExpPresCreateByConfig()
        {
            try
            {
                int i = 0;
                WaitingManager.Show();
                List<string> tb = new List<string>();
                foreach (var item in this.hisreq)
                {
                    //CommonParam param_K = new CommonParam();
                    //HisSereServFilter filter = new HisSereServFilter();
                    //filter.SERVICE_REQ_ID = item.ID;
                    //var rs = new BackendAdapter(param_K).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param_K);


                    //HisSereServExtFilter filter_ = new HisSereServExtFilter();
                    //filter_.TDL_SERVICE_REQ_ID = item.ID;
                    //var sereservExt = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter_, null);


                    CommonParam param_ = new CommonParam();
                    Subclinical = new SubclinicalPresResultSDO();
                    bool success = false;
                    ExpendPresSDO sdo = new ExpendPresSDO();
                    sdo.MediStockId = this.MediStockId;
                    sdo.RequestRoomId = this.roomID;
                    sdo.ServiceReqId = item.ID;
                    //if (rs != null && rs.Count()> 0)
                    //{
                    //    sdo.SereServId = rs.FirstOrDefault().ID;
                    //}
                    Subclinical = new BackendAdapter(param_).Post<SubclinicalPresResultSDO>("api/HisServiceReq/ExpPresCreateByConfig", ApiConsumers.MosConsumer, sdo, param_);
                    i += 1;
                    WaitingManager.Hide();
                    if (Subclinical != null)
                    {
                        lblDangke.Text = Subclinical.ServiceReqs.FirstOrDefault().SERVICE_REQ_CODE + " - " + Subclinical.ServiceReqs.FirstOrDefault().TDL_PATIENT_NAME;
                        lblDaKe.Text = i.ToString() + "/" + select.ToString();
                    }
                    else
                    {
                        lblDangke.Text = item.SERVICE_REQ_CODE + " - " + item.TDL_PATIENT_NAME;
                        lblDaKe.Text = i.ToString() + "/" + select.ToString();
                        string tb_ = item.SERVICE_REQ_CODE + ": Thất bại. " + param_.GetMessage().ToString();
                        tb.Add(tb_);
                        if (tb != null && tb.Count > 0)
                        {
                            txtLoi.Text = string.Join("\r\n", tb);
                        }
                    }
                }
                if (i == select)
                {
                    btnDongY.Enabled = true;
                    btnDongY.Focus();
                }
                else
                {
                    btnDongY.Focus();
                    btnDongY.Enabled = false;
                }
             
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
