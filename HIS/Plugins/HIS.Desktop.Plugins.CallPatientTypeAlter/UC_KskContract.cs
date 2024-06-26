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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class UC_KskContract : UserControl
    {
        internal List<V_HIS_KSK_CONTRACT> listKskContract;
        public UC_KskContract()
        {
            InitializeComponent();
        }

        private void UC_KskContract_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboContract();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UC_KskContract
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(UC_KskContract).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_KskContract.cboContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UC_KskContract.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboContract()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                listKskContract = new BackendAdapter(param).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã hợp đồng", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboContract, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboContract.Enabled = false;
                cboContract.Enabled = true;
                if (cboContract.EditValue != null)
                {
                    var contract = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                    if (contract != null)
                    {
                        lblNgayHetHan.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EXPIRY_DATE ?? 0);
                        lblNgayHieuLuc.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(contract.EFFECT_DATE ?? 0);
                        lblTenCongTy.Text = contract.WORK_PLACE_NAME;
                        lblTyLeThanhToan.Text = Convert.ToInt64(contract.PAYMENT_RATIO*100).ToString() + "%";
                    }
                    else
                    {
                        lblNgayHetHan.Text = "";
                        lblNgayHieuLuc.Text = "";
                        lblTenCongTy.Text = "";
                        lblTyLeThanhToan.Text = "";
                    }
                }
                else
                {
                    lblNgayHetHan.Text = "";
                    lblNgayHieuLuc.Text = "";
                    lblTenCongTy.Text = "";
                    lblTyLeThanhToan.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboContract_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string text = "";
                if (cboContract.EditValue != null && listKskContract != null && listKskContract.Count > 0)
                {
                    var ksk = listKskContract.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString()));
                    if (ksk != null)
                    {
                        text = ksk.KSK_CONTRACT_CODE + " - " + ksk.WORK_PLACE_NAME;
                    }
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public void DisposeControl()
        {
            try
            {
                listKskContract = null;
                this.cboContract.EditValueChanged -= new System.EventHandler(this.cboContract_EditValueChanged);
                this.cboContract.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboContract_CustomDisplayText);
                this.Load -= new System.EventHandler(this.UC_KskContract_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboContract.Properties.DataSource = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                layoutControlItem1 = null;
                gridLookUpEdit1View = null;
                cboContract = null;
                lblTenCongTy = null;
                lblTyLeThanhToan = null;
                lblNgayHieuLuc = null;
                lblNgayHetHan = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
