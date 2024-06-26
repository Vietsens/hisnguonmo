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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AddExamInfor;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using HIS.Desktop.Plugins.TrackingCreate.Config;
using HIS.Desktop.Plugins.TrackingCreate.frmGhiChu;
using HIS.Desktop.Plugins.TrackingCreate.Resources;
using HIS.Desktop.Plugins.TrackingCreate.ValidationRuleControl;
using HIS.Desktop.Utility;
using HIS.UC.DHST;
using HIS.UC.DHST.ADO;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class LoaiChamSoc : Form
    {

        List<HIS_CARE_TYPE> HisCare;
        public HIS_CARE_TYPE HisCare_;
        HIS.Desktop.Common.DelegateSelectData dataSelect;
        public LoaiChamSoc(HIS.Desktop.Common.DelegateSelectData dataSelect_)
        {
            this.dataSelect = dataSelect_;
            InitializeComponent();
        }

        private void LoaiChamSoc_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                SetIconFrm();
                FillDataControl();
            }
            catch (System.Exception)
            {
                throw;
            }
            
        }

        private void FillDataControl() 
        {
            
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareTypeFilter HisCFilter = new MOS.Filter.HisCareTypeFilter();
                if (!string.IsNullOrEmpty(txtKeyWord.Text))
                {
                    HisCFilter.KEY_WORD = txtKeyWord.Text;
                }
                HisCFilter.ORDER_DIRECTION = "DESC";
                HisCFilter.IS_ACTIVE = 1;
                HisCare = new BackendAdapter(param).Get<List<HIS_CARE_TYPE>>("api/HisCareType/Get", ApiConsumers.MosConsumer, HisCFilter, param);
                grcChonLoaiChamSoc.BeginUpdate();
                grcChonLoaiChamSoc.DataSource = HisCare;
                grcChonLoaiChamSoc.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            } 
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter) 
                {
                    FillDataControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void grdChonLoaiChamSoc_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

                var row = (HIS_CARE_TYPE)grdChonLoaiChamSoc.GetFocusedRow();
                if (row != null)
                {
                    HisCare_ = new HIS_CARE_TYPE();
                    HisCare_ = row;
                    if (HisCare_ != null) 
                    {
                        string maten = HisCare_.CARE_TYPE_CODE + ": " + HisCare_.CARE_TYPE_NAME;
                        dataSelect(maten);
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện LoaiChamSoc
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(LoaiChamSoc).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("LoaiChamSoc.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("LoaiChamSoc.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("LoaiChamSoc.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("LoaiChamSoc.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("LoaiChamSoc.Text", Resources.ResourceLanguageManager.LanguageResource__LoaiChamSoc, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
