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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCRelativeInfo.ADO;

namespace HIS.UC.UCRelativeInfo
{
    public class RelativeProcessor
    {

        DelegateFocusNextUserControl dlgFocusNextUserControl;
        public UCRelativeInfo ControlWorker { get; set; }

        public RelativeProcessor()
        {
            this.Init();
        }

        void Init()
        {
            try
            {
                this.ControlWorker = new UCRelativeInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCRelativeADO GetValue()
        {
            UCRelativeADO dataGetFromForm = new UCRelativeADO();
            try
            {
                dataGetFromForm = this.ControlWorker.GetValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                dataGetFromForm = null;
            }
            return dataGetFromForm;
        }

        public void SetValue(UCRelativeADO ado)
        {
            try
            {
                if (ado != null)
                {
                    this.ControlWorker.SetValue(ado);
                    this.dlgFocusNextUserControl = ado._FocusNextUserControl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusUserControl()
        {
            try
            {
                this.ControlWorker.FocusUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusNextUserControl()
        {
            try
            {
                if (this.dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public void ShowFormPopup(UserControl _userControl, string _nameForm)
        //{
        //    try
        //    {
        //        if (_userControl != null)
        //        {
        //            var form = new Form();
        //            form.Controls.Add(_userControl);
        //            form.MaximizeBox = false;
        //            form.Text = _nameForm;
        //            form.MaximumSize = new System.Drawing.Size(_userControl.Width, _userControl.Height);
        //            form.MinimumSize = new System.Drawing.Size(_userControl.Width, _userControl.Height);
        //            form.AutoSize = true;
        //            form.StartPosition = FormStartPosition.CenterScreen;
        //            form.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        
    }
}
