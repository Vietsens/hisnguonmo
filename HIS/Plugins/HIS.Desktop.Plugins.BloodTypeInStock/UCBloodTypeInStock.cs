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
using HIS.UC.HisBloodTypeInStock;
using Inventec.Desktop.Plugins.BloodTypeInStock.Base;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.BloodTypeInStock
{
    public partial class UCBloodTypeInStock : UserControl
    {
        long mediStockId;
        HisBloodTypeInStockProcessor hisBloodProcessor;

        UserControl ucBloodInfo;

        public UCBloodTypeInStock()
        {
            InitializeComponent();
            ResourceLangManager.InitResourceLanguageManager();
            InitHisBloodTree();
        }

        public UCBloodTypeInStock(long mediStockId)
        {
            InitializeComponent();
            ResourceLangManager.InitResourceLanguageManager();
            InitHisBloodTree();
            this.mediStockId = mediStockId;
        }

        public void MeShow(long mediStockId)
        {
            this.mediStockId = mediStockId;
            if (this.mediStockId != null)
            {
                fillDataLableControl();
                ShowUcControl();
            }
        }

        private void UCBloodTypeInStock_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BloodTypeInStock.Resources.Lang", typeof(HIS.Desktop.Plugins.BloodTypeInStock.UCBloodTypeInStock).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCBloodTypeInStock.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataLableControl()
        {
            try
            {
                var currentMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().SingleOrDefault(p => p.ID == this.mediStockId);
                if (currentMediStock != null)
                {
                    lblMediStockCode.Text = currentMediStock.MEDI_STOCK_CODE;
                    lblMediStockName.Text = currentMediStock.MEDI_STOCK_NAME;
                    lblDepartmentName.Text = currentMediStock.DEPARTMENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowUcControl()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (this.ucBloodInfo != null)
                {
                    this.panelControl1.Controls.Add(this.ucBloodInfo);
                    this.ucBloodInfo.Dock = DockStyle.Fill;

                    MOS.Filter.HisBloodTypeStockViewFilter filter = new MOS.Filter.HisBloodTypeStockViewFilter();
                    filter.MEDI_STOCK_ID = this.mediStockId;
                    var lstBlood = new BackendAdapter(param).Get<List<HisBloodTypeInStockSDO>>(HisRequestUriStore.HIS_BLOOD_TYPE_GETVIEW_BY_IN_STOCK, ApiConsumers.MosConsumer, filter, param);
                    hisBloodProcessor.Reload(ucBloodInfo, lstBlood);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitHisBloodTree()
        {
            try
            {
                hisBloodProcessor = new UC.HisBloodTypeInStock.HisBloodTypeInStockProcessor();
                HisBloodTypeInStockInitADO ado = new HisBloodTypeInStockInitADO();
                ado.HisBloodTypeInStockColumns = new List<HisBloodTypeInStockColumn>();
                ado.IsShowSearchPanel = true;

                //Column mã
                HisBloodTypeInStockColumn mediTypeCodeNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodTypeCode", 200, false);
                mediTypeCodeNameCol.VisibleIndex = 0;
                ado.HisBloodTypeInStockColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisBloodTypeInStockColumn mediTypeNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodTypeName", 300, false);
                mediTypeNameCol.VisibleIndex = 1;
                ado.HisBloodTypeInStockColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisBloodTypeInStockColumn serviceUnitNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__BLOOD_IN_STOCK__COLUMN_VOLUME", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "Volume", 100, false);
                serviceUnitNameCol.VisibleIndex = 2;
                ado.HisBloodTypeInStockColumns.Add(serviceUnitNameCol);
                //Column số lượng tồn
                HisBloodTypeInStockColumn totalAmountCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "Amount", 150, false);
                totalAmountCol.VisibleIndex = 3;
                ado.HisBloodTypeInStockColumns.Add(totalAmountCol);
                //Column ABO
                HisBloodTypeInStockColumn availableAmoutCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ABO", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodAboCode", 100, false);
                availableAmoutCol.VisibleIndex = 4;
                //ado.HisBloodTypeInStockColumns.Add(availableAmoutCol);
                //Column RH
                HisBloodTypeInStockColumn rhAmoutCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_RH", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodRhCode", 100, false);
                availableAmoutCol.VisibleIndex = 5;
                //ado.HisBloodTypeInStockColumns.Add(availableAmoutCol);
                //Column mã nhóm máu
                HisBloodTypeInStockColumn bloodGroupCodeCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__BLOOD_IN_STOCK__COLUMN_BLOOD_GROUP_CODE", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodGroupCode", 200, false);
                bloodGroupCodeCol.VisibleIndex = 6;
                //ado.HisBloodTypeInStockColumns.Add(bloodGroupCodeCol);
                //Column nhóm máu
                HisBloodTypeInStockColumn bloodGroupNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__BLOOD_IN_STOCK__COLUMN_BLOOD_GROUP_NAME", ResourceLangManager.LanguageUCBloodTypeInStock, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodGroupName", 400, false);
                bloodGroupNameCol.VisibleIndex = 7;
                //ado.HisBloodTypeInStockColumns.Add(bloodGroupNameCol);

                this.ucBloodInfo = (UserControl)hisBloodProcessor.Run(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ShowUcControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RefeshData()
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
