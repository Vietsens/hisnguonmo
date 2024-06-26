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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionDeposit
{
    public partial class frmDepositReason : FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        HIS.Desktop.Common.DelegateSelectData _DelegateSelectData;
        List<HIS_DEPOSIT_REASON> DepositReasons = null;

        public frmDepositReason(Inventec.Desktop.Common.Modules.Module Module, HIS.Desktop.Common.DelegateSelectData _delegateSelectData)
            : base(Module)
        {
            InitializeComponent();
            
            try
            {
                this.currentModule = Module;
                this._DelegateSelectData = _delegateSelectData;
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
                //Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TransactionDeposit.Resources.Lang", typeof(frmDepositReason).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDepositReason.layoutControl1.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmDepositReason.gridColumn4.Caption", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmDepositReason.gridColumn1.Caption", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmDepositReason.gridColumn2.Caption", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmDepositReason.gridColumn3.Caption", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmDepositReason.txtSearch.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDepositReason.Text", Base.ResourceLangManager.LanguageFrmTransactionDeposit, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (HIS_DEPOSIT_REASON)gridView1.GetFocusedRow();
                if (row != null)
                {
                    this._DelegateSelectData(row.DEPOSIT_REASON_NAME);
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmDepositReason_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void FillDataToControl()
        {
            try
            {
                DepositReasons = new List<HIS_DEPOSIT_REASON>();
                CommonParam paramCommon = new CommonParam();
                HisDepositReasonFilter filter = new HisDepositReasonFilter();

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.KEY_WORD = txtSearch.Text.Trim();

                DepositReasons = new BackendAdapter(paramCommon).Get<List<HIS_DEPOSIT_REASON>>("/api/HisDepositReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                gridView1.BeginUpdate();

                gridView1.GridControl.DataSource = DepositReasons;

                gridView1.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }


    }
}
