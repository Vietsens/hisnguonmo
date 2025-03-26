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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCPatientRaw.ClassUCPatientRaw;
using DevExpress.Utils.Menu;
using HIS.UC.UCPatientRaw.ADO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.DelegateRegister;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        private async void ProcessGetDataHrm(string valueSearch)
        {
            try
            {
                MOS.SDO.HisPatientSDO _PatientSDOByHrm = new MOS.SDO.HisPatientSDO();
                var _addressConnectHrm = (AddressConnectHrm)Newtonsoft.Json.JsonConvert.DeserializeObject<AddressConnectHrm>(HisConfigCFG.AddressConnectHrm);
                HIS.Desktop.Plugins.Library.RegisterConnectHrm.RegisterConnectHrmProcessor _RegisterConnectHrmProcessor = new HIS.Desktop.Plugins.Library.RegisterConnectHrm.RegisterConnectHrmProcessor();
                _PatientSDOByHrm = await _RegisterConnectHrmProcessor.GetDataHrm1(_addressConnectHrm.Address, _addressConnectHrm.Loginname, _addressConnectHrm.Password, _addressConnectHrm.GrantType, _addressConnectHrm.ClientId, _addressConnectHrm.ClientSecret, valueSearch);//"000017"
                this.Invoke(new MethodInvoker(delegate()
                {
                    this.ProcessPatientCodeKeydown(_PatientSDOByHrm);
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidationByChildrenUnder6Years(bool isTreSoSinh, bool isHasReset)
        {
            //try
            //{
            //    if (isTreSoSinh && HisConfigCFG.MustHaveNCSInfoForChild)
            //    {
            //        if (this.ucRelativeInfo1..Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //            this.lcitxtHomePerson.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Black;
            //        //if (this.lcitxtCorrelated.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //    this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Maroon;

            //        //if (this.lcitxtRelativeAddress.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //{
            //        //    this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        //    //this.SetRelativeAddress(false);
            //        //}

            //        //if (this.lciRelativeCMNDNumber.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //    this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
            //    }
            //    else
            //    {
            //        this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lcitxtHomePerson.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Black;
            //        if (isHasReset)
            //            this.txtRelativeAddress.Text = "";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
    }
}
