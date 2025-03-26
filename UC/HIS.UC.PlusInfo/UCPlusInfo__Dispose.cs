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
using HIS.UC.PlusInfo.Config;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraLayout;
using HIS.UC.PlusInfo.ShareMethod;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.PlusInfo
{
    public partial class UCPlusInfo : UserControlBase
    {

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                //if (ucExamHistory != null)
                //    ucExamHistory.DisposeControl();
                ucExamHistory = null;
                //if (ucTaxCode != null)
                //    ucTaxCode.DisposeControl();
                ucTaxCode = null;
                //if (ucHrmKskCode != null)
                //    ucHrmKskCode.DisposeControl();
                ucHrmKskCode = null;
                //if (ucAddressOfBirth1 != null)
                //    ucAddressOfBirth1.DisposeControl();
                ucAddressOfBirth1 = null;
                //if (ucDistrictOfBirth1 != null)
                //    ucDistrictOfBirth1.DisposeControl();
                ucDistrictOfBirth1 = null;
                //if (ucCommuneOfBirth1 != null)
                //    ucCommuneOfBirth1.DisposeControl();
                ucCommuneOfBirth1 = null;
                //if (ucPatientStoreCode != null)
                //    ucPatientStoreCode.DisposeControl();
                ucPatientStoreCode = null;
                //if (ucMaHoNgheo1 != null)
                //    ucMaHoNgheo1.DisposeControl();
                ucMaHoNgheo1 = null;
                //if (ucProvinceOfBirth1 != null)
                //    ucProvinceOfBirth1.DisposeControl();
                ucProvinceOfBirth1 = null;
                //if (ucProvinceNow1 != null)
                //    ucProvinceNow1.DisposeControl();
                ucProvinceNow1 = null;
                //if (ucProgram1 != null)
                //    ucProgram1.DisposeControl();
                ucProgram1 = null;
                //if (ucPhone1 != null)
                //    ucPhone1.DisposeControl();
                ucPhone1 = null;
                //if (ucNational1 != null)
                //    ucNational1.DisposeControl();
                ucNational1 = null;
                //if (ucMother1 != null)
                //    ucMother1.DisposeControl();
                ucMother1 = null;
                //if (ucMilitaryRank1 != null)
                //    ucMilitaryRank1.DisposeControl();
                ucMilitaryRank1 = null;
                //if (ucHouseHoldRelative1 != null)
                //    ucHouseHoldRelative1.DisposeControl();
                ucHouseHoldRelative1 = null;
                //if (ucHohName1 != null)
                //    ucHohName1.DisposeControl();
                ucHohName1 = null;
                //if (ucHouseHoldNumber1 != null)
                //    ucHouseHoldNumber1.DisposeControl();
                ucHouseHoldNumber1 = null;
                //if (ucFather1 != null)
                //    ucFather1.DisposeControl();
                ucFather1 = null;
                //if (ucEthnic1 != null)
                //    ucEthnic1.DisposeControl();
                ucEthnic1 = null;
                //if (ucEmail1 != null)
                //    ucEmail1.DisposeControl();
                ucEmail1 = null;
                //if (ucDistrictNow1 != null)
                //    ucDistrictNow1.DisposeControl();
                ucDistrictNow1 = null;
                //if (ucCommuneNow1 != null)
                //    ucCommuneNow1.DisposeControl();
                ucCommuneNow1 = null;
                //if (ucCMNDPlace1 != null)
                //    ucCMNDPlace1.DisposeControl();
                ucCMNDPlace1 = null;
                //if (ucCmndNumber1 != null)
                //    ucCmndNumber1.DisposeControl();
                ucCmndNumber1 = null;
                //if (ucCMNDDate1 != null)
                //    ucCMNDDate1.DisposeControl();
                ucCMNDDate1 = null;
                //if (ucBloodRh1 != null)
                //    ucBloodRh1.DisposeControl();
                ucBloodRh1 = null;
                //if (ucBlood1 != null)
                //    ucBlood1.DisposeControl();
                ucBlood1 = null;
                //if (ucWorkPlace1 != null)
                //    ucWorkPlace1.DisposeControl();
                ucWorkPlace1 = null;
                //if (ucAddress1 != null)
                //    ucAddress1.DisposeControl();
                ucAddress1 = null;
                totalRowLimit = 0;
                totalModule = 0;
                userControlNameAdded = null;
                moduleField = null;
                listControlForFormExtend = null;
                listControl = null;
                indexUCCMNDNumber = 0;
                indexUCMaHoNgheo = 0;
                tagIndex = 0;
                indexOfControlEnd = 0;
                dlgFocusNextUserControl = null;
                dataWorkPlaceADO = null;
                patientExtendADO = null;
                patientInfoADO = null;
                ucExtend1 = null;
                _shareMethod = null;
                _isShowControlHrmKskCode = false;
                this.timer1.Tick -= new System.EventHandler(this.timer1_Tick);
                this.Load -= new System.EventHandler(this.UCPlusInfo_Load);
                timer1 = null;
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
