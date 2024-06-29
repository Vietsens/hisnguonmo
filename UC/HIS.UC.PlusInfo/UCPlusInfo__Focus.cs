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
using HIS.Desktop.Utility;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.WorkPlace;
using HIS.Desktop.DelegateRegister;
using HIS.UC.PlusInfo.ShareMethod;
using DevExpress.XtraLayout;

namespace HIS.UC.PlusInfo
{
    public partial class UCPlusInfo : UserControlBase
    {
        public void FocusUserControl()
        {
            try
            {
                if (this.listControl != null && this.listControl.Count > 0)
                {
                    FocusControl(this.listControl[0]);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Focus den uc UCPlusInfo that bai: \n" + ex);
            }
        }

        public void FocusNextUserControl(DelegateFocusNextUserControl _dlgFocusNextUserControl)
        {
            try
            {
                if (_dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl = _dlgFocusNextUserControl;
                    //this.ucWorkPlace.FocusNextUserControl(this.dlgFocusNextUserControl);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusNextUserControl(List<UserControl> listControl)
        {
            try
            {
                for (int i = 0; i < listControl.Count; i++)
                {
                    switch (listControl[i + 1].Name)
                    {
                        #region --------- Kiem tra control và get data -------------

                        case ChoiceControl.ucAddress:
                            break;
                        case ChoiceControl.ucWorkPlace:

                            break;
                        case ChoiceControl.ucBlood:

                            break;
                        case ChoiceControl.ucBloodRh:

                            break;
                        case ChoiceControl.ucCmndDate:

                            break;
                        case ChoiceControl.ucCmndNumber:

                            break;
                        case ChoiceControl.ucCmndPlace:

                            break;
                        case ChoiceControl.ucCommuneNow:

                            break;
                        case ChoiceControl.ucDistrictNow:

                            break;
                        case ChoiceControl.ucEmai:

                            break;
                        case ChoiceControl.ucPatientStoreCode:

                            break;
                        case ChoiceControl.ucEthnic:

                            break;
                        case ChoiceControl.ucFatherName:

                            break;
                        case ChoiceControl.ucHouseHold:

                            break;
                        case ChoiceControl.ucHouseHoldRelative:

                            break;
                        case ChoiceControl.ucMilitaryRank:

                            break;
                        case ChoiceControl.ucMotherName:

                            break;
                        case ChoiceControl.ucNational:

                            break;
                        case ChoiceControl.ucPhoneNumber:

                            break;
                        case ChoiceControl.ucProgram:

                            break;
                        case ChoiceControl.ucProvinceNow:

                            break;
                        case ChoiceControl.ucProvinceOfBirth:

                            break;
                        case ChoiceControl.ucMaHoNgheo:

                            break;
                        case ChoiceControl.ucTaxCode:

                            break;
                        default:
                            break;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetCurrentModuleAgain(Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                this.ucWorkPlace1.SetCurrentModuleAgain(module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (this.ucAddress1 != null)
                {
                    valid = valid && this.ucAddress1.ValidateRequiredField();
                }
                if (this.ucPhone1 != null)
                {
                    valid = valid && this.ucPhone1.ValidateRequiredField();
                }
                if (this.ucCMNDPlace1 != null)
                {
                    valid = valid && this.ucCMNDPlace1.ValidateRequiredField();
                }
                if (this.ucEmail1 != null)
                {
                    valid = valid && this.ucEmail1.ValidateRequiredField();
                }
                if (this.ucFather1 != null)
                {
                    valid = valid && this.ucFather1.ValidateRequiredField();
                }
                if (this.ucMother1 != null)
                {
                    valid = valid && this.ucMother1.ValidateRequiredField();
                }
                if (this.ucPatientStoreCode != null)
                {
                    valid = valid && this.ucPatientStoreCode.ValidateRequiredField();
                }
                if (this.ucHohName1 != null)
                {
                    valid = valid && this.ucHohName1.ValidateRequiredField();
                }
                if (this.ucHouseHoldNumber1 != null)
                {
                    valid = valid && this.ucHouseHoldNumber1.ValidateRequiredField();
                }
                if (this.ucProvinceOfBirth1 != null)
                {
                    valid = valid && this.ucProvinceOfBirth1.ValidateRequiredField();
                }
                if (this.ucCmndNumber1 != null)
                {
                    valid = valid && this.ucCmndNumber1.ValidateRequiredField();
                }
                if (this.ucMaHoNgheo1 != null)
                {
                    valid = valid && this.ucMaHoNgheo1.ValidateRequiredField();
                }
                if (this.ucHrmKskCode != null && this._isShowControlHrmKskCode)
                {
                    valid = valid && this.ucHrmKskCode.ValidateRequiredField();
                }
                if (this.ucEthnic1 != null)
                {
                    valid = valid && this.ucEthnic1.ValidateRequiredField();
                }
                if (this.ucTaxCode != null)
                {
                    valid = valid && this.ucTaxCode.ValidateRequiredField();
                }
                if (this.ucCMNDDate1 != null)
                {
                    valid = valid && this.ucCMNDDate1.ValidateRequiredField();
                }
                if (this.ucNational1 != null)
                {
                    valid = valid && this.ucNational1.ValidateRequiredField();
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        public void ResetRequiredField()
        {
            try
            {
                if (this.ucAddress1 != null)
                {
                    this.ucAddress1.ResetRequiredField();
                }
                if (this.ucEmail1 != null)
                {
                    this.ucEmail1.ResetRequiredField();
                }
                if (this.ucFather1 != null)
                {
                    this.ucFather1.ResetRequiredField();
                }
                if (this.ucMother1 != null)
                {
                    this.ucMother1.ResetRequiredField();
                }
                if (this.ucPatientStoreCode != null)
                {
                    this.ucPatientStoreCode.ResetRequiredField();
                }
                if (this.ucHohName1 != null)
                {
                    this.ucHohName1.ResetRequiredField();
                }
                if (this.ucHouseHoldNumber1 != null)
                {
                    this.ucHouseHoldNumber1.ResetRequiredField();
                }
                if (this.ucHrmKskCode != null)
                {
                    this.ucHrmKskCode.ResetRequiredField();
                }
                if (this.ucEthnic1 != null)
                {
                    this.ucEthnic1.ResetRequiredField();
                }
                if (this.ucPhone1 != null)
                {
                    this.ucPhone1.ResetRequiredField();
                }
                if (this.ucTaxCode != null)
                {
                    this.ucTaxCode.ResetRequiredField();
                }
                if (this.ucCMNDDate1 != null)
                {
                    this.ucCMNDDate1.ResetRequiredField();
                }
                if (this.ucNational1 != null)
                {
                    this.ucNational1.ResetRequiredField();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusNextUserControl(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if ((int)sender < 18)
                {
                    foreach (LayoutControlItem col in layoutControlGroup1.Items)
                    {
                        if (col != null && col.Control != null && col.Control.TabIndex == (int)sender + 1)
                        {
                            UserControl uc = null;
                            foreach (UserControl item in listControl)
                            {
                                if (item == col.Control)
                                {
                                    uc = item;
                                    break;
                                }
                            }
                            if (uc != null)
                            {
                                FocusControl(uc);
                            }
                        }
                    }
                }
                else
                {
                    UserControl uc = null;
                    foreach (UserControl item in listControl)
                    {
                        if (item.TabIndex == (int)sender + 1)
                        {
                            uc = item;
                            break;
                        }
                    }
                    if (uc != null)
                    {
                        FocusControl(uc);
                    }
                    //else if (ucExtend1 != null)//Focus xg button them
                    //{
                    //    foreach (var item in ucExtend1.Controls[0].Controls)
                    //    {
                    //        if (item.GetType() == typeof(DevExpress.XtraEditors.SimpleButton))
                    //        {
                    //            Control _Button = ((Control)item);
                    //            _Button.Focus();
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusControl(UserControl uc)
        {
            try
            {
                if (uc != null)
                {
                    Control text = null;
                    Control lookup = null;
                    Control gridLookup = null;
                    Control buttonEdit = null;
                    foreach (var item in uc.Controls[0].Controls)
                    {
                        if (item.GetType() == typeof(DevExpress.XtraEditors.TextEdit))
                        {
                            text = ((Control)item);
                        }
                        else if (item.GetType() == typeof(DevExpress.XtraEditors.LookUpEdit))
                        {
                            lookup = ((Control)item);
                        }
                        else if (item.GetType() == typeof(DevExpress.XtraEditors.GridLookUpEdit))
                        {
                            gridLookup = ((Control)item);
                        }
                        else if (item.GetType() == typeof(System.Windows.Forms.Panel))
                        {
                            var panel = (System.Windows.Forms.Panel)item;
                            foreach (var pn in panel.Controls)
                            {
                                if (pn.GetType() == typeof(DevExpress.XtraEditors.ButtonEdit))
                                    buttonEdit = (Control)pn;
                            }
                        }

                        if (item.GetType() == typeof(DevExpress.XtraEditors.PanelControl))
                        {
                            var panel = (DevExpress.XtraEditors.PanelControl)item;
                            foreach (var pn in panel.Controls)
                            {
                                var c = (UserControl)pn;
                                foreach (var d in c.Controls[0].Controls)
                                {
                                    if (d.GetType() == typeof(DevExpress.XtraEditors.TextEdit))
                                    {
                                        ((Control)d).Focus();
                                        break;
                                    }
                                }
                                break;
                            }
                            break;
                        }
                    }

                    if (text != null || lookup != null || gridLookup != null || buttonEdit != null)
                    {
                        if (text != null)
                        {
                            text.Focus();
                        }
                        else if (lookup != null)
                        {
                            lookup.Focus();
                        }
                        else if (gridLookup != null)
                        {
                            gridLookup.Focus();
                        }
                        else if (buttonEdit != null)
                        {
                            buttonEdit.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusUser(List<UserControl> listControl, int indexControlCallMethod)
        {
            foreach (var item in listControl)
            {
                if (item.TabIndex == indexControlCallMethod + 1)
                {
                    listControl[item.TabIndex].Focus();
                }
            }
        }
    }
}
