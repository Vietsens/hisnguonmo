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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.TransactionDebtCollect.Base;
using HIS.Desktop.Plugins.TransactionDebtCollect.Config;
using HIS.Desktop.Utilities;
using HIS.UC.MenuPrint;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt.HeinTreatmentType;
using MPS.ADO.Bordereau;
using Newtonsoft.Json;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionDebtCollect
{
    public partial class frmTransactionDebtCollect : HIS.Desktop.Utility.FormBase
    {
        private void FillDataToButtonPrint()
        {
            try
            {
                WaitingManager.Show();

                ReloadMenuOption reloadMenuBordereau = new ReloadMenuOption();
                reloadMenuBordereau.ReloadMenu = ReloadMenu;
                reloadMenuBordereau.Type = ReloadMenuOption.MenuType.DYNAMIC;
                BordereauInitData bordereauInitData = new BordereauInitData();

                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT>();
                bordereauInitData.Treatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT>(this.currentTreatment);
                AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_5, HIS_SERE_SERV>();
                bordereauInitData.PatientTypeAlter = resultPatientType;
                HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.currentModule != null ? this.currentModule.RoomId : 0, this.currentModule != null ? this.currentModule.RoomTypeId : 0, currentTreatment.ID, currentTreatment.PATIENT_ID, bordereauInitData, reloadMenuBordereau);
                processor.InitMenuPrint();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadMenu(object data)
        {
            if (data != null)
            {
                if (data is List<MenuPrintADO>)
                {

                    MenuPrintProcessor menuPrintProcessor = new MenuPrintProcessor();
                    HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(data as List<MenuPrintADO>, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR_PRINT_TYPE>());
                    //menuPrintInitADO.ControlContainer = panelPrintBordereau;
                    var uc = menuPrintProcessor.Run(menuPrintInitADO);
                    if (uc == null)
                    {
                        LogSystem.Warn("Khoi tao uc print that bai trong chuc nang bang ke. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => uc), uc));
                    }
                }
            }
        }
    }
}
