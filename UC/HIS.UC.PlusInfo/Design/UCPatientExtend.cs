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
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.UC.WorkPlace;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.DelegateRegister;
using DevExpress.XtraPrinting.Export;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCPatientExtend : UserControl
    {
        List<UserControl> listUC = new List<UserControl>();
        bool isShowControlHrmKskCode;
        #region Constructor

        public UCPatientExtend()
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
        }

        public UCPatientExtend(bool _isShowControlHrmKskCode)
        {
            InitializeComponent();
            this.isShowControlHrmKskCode = _isShowControlHrmKskCode;
            SetCaptionByLanguageKey();
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCPatientExtend
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCPatientExtend).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPatientExtend.layoutControl1.Text",ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPatientExtend.Text = Inventec.Common.Resource.Get.Value("UCPatientExtend.btnPatientExtend.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Method

        private void btnPatientExtend_Click(object sender, EventArgs e)
        {
            try
            {
                frmPatientExtend frm = new frmPatientExtend(listUC, this.isShowControlHrmKskCode);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        public void SetControlForFormExtend(List<UserControl> controls)
        {
            try
            {

                if (controls != null)
                    listUC = controls;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetDelegateForOpenForm()
        {
            try
            {
                this.btnPatientExtend_Click(null,null);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        internal void DisposeControl()
        {
            try
            {
                isShowControlHrmKskCode = false;
                listUC = null;
                this.btnPatientExtend.Click -= new System.EventHandler(this.btnPatientExtend_Click);
                emptySpaceItem1 = null;
                layoutControlItem1 = null;
                btnPatientExtend = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
