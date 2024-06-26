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
using HIS.Desktop.Plugins.Bordereau.ADO;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau
{
    internal class PriceEditType
    {
        internal const int EditTypeSurgPrice = 1;
        internal const int EditTypePackagePrice = 2;
    }

    public delegate decimal? GetPriceBypackageDelegate(SereServADO sereServADOOld);
    public delegate decimal? GetPriceBySurgDelegate(SereServADO sereServADOOld);

    public partial class frmPriceEdit : Form
    {
        SereServADO sereServADOOld;
        Action<SereServADO> actEditedHandler;
        GetPriceBypackageDelegate getPriceBypackageDelegate;
        GetPriceBySurgDelegate getPriceBySurgDelegate;
        int ActionType;

        public frmPriceEdit()
        {
            InitializeComponent();
        }

        public frmPriceEdit(SereServADO data, Action<SereServADO> actEdited, int actionType, GetPriceBypackageDelegate getPriceBypackageDelegate, GetPriceBySurgDelegate getPriceBySurgDelegate)
        {
            InitializeComponent();
            this.sereServADOOld = data;
            this.actEditedHandler = actEdited;
            this.ActionType = actionType;
            this.getPriceBypackageDelegate = getPriceBypackageDelegate;
            this.getPriceBySurgDelegate = getPriceBySurgDelegate;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actEditedHandler != null)
                {
                    switch (this.ActionType)
                    {
                        case PriceEditType.EditTypeSurgPrice:
                            sereServADOOld.AssignSurgPriceEdit = this.spinPrice.Value > 0 ? (decimal?)this.spinPrice.Value : null;
                            break;
                        case PriceEditType.EditTypePackagePrice:
                            sereServADOOld.AssignPackagePriceEdit = this.spinPrice.Value > 0 ? (decimal?)this.spinPrice.Value : null;
                            break;
                        default:
                            break;
                    }
                    this.actEditedHandler(sereServADOOld);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPriceEdit_Load(object sender, EventArgs e)
        {
            try
            {
                switch (this.ActionType)
                {
                    case PriceEditType.EditTypeSurgPrice:
                        InitPriceBySurg();
                        break;
                    case PriceEditType.EditTypePackagePrice:
                        InitPriceByPackage();
                        break;
                    default:
                        break;
                }
                this.spinPrice.Focus();
                this.spinPrice.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPriceByPackage()
        {
            if (this.sereServADOOld.AssignPackagePriceEdit > 0)
            {
                this.spinPrice.EditValue = this.sereServADOOld.AssignPackagePriceEdit;
            }
            else
                this.spinPrice.EditValue = this.getPriceBypackageDelegate(this.sereServADOOld);
        }

        private void InitPriceBySurg()
        {
            if (this.sereServADOOld.AssignSurgPriceEdit > 0)
            {
                this.spinPrice.EditValue = this.sereServADOOld.AssignSurgPriceEdit;
            }
            else if (this.getPriceBySurgDelegate != null)
            {
                this.spinPrice.EditValue = this.getPriceBySurgDelegate(this.sereServADOOld);
            }
        }

        private void frmPriceEdit_Closed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ActionType = 0;
                getPriceBySurgDelegate = null;
                getPriceBypackageDelegate = null;
                actEditedHandler = null;
                sereServADOOld = null;
                this.btnOK.Click -= new System.EventHandler(this.btnOK_Click);
                this.Load -= new System.EventHandler(this.frmPriceEdit_Load);
                emptySpaceItem2 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                btnOK = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                spinPrice = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
