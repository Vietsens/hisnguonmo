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
using HIS.Desktop.Plugins.Bordereau.Base;
using HIS.Desktop.Plugins.Bordereau.Config;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Utility;
using HIS.UC.MenuPrint;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {

        private void FillDataToButtonPrint()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                BordereauInitData initData = new BordereauInitData();
                initData.PatientTypeAlter = currentHisPatientTypeAlters != null ? currentHisPatientTypeAlters.OrderByDescending(o => o.LOG_TIME).ThenByDescending(o => o.ID).FirstOrDefault() : null;
                initData.UserNameReturnResult = cboLoginName.Text;
                //initData.TreatmentFees = this.treatmentFees;
                ReloadMenuOption reloadMenu = new ReloadMenuOption();
                reloadMenu.ReloadMenu = ReloadMenu;
                reloadMenu.Type = ReloadMenuOption.MenuType.DYNAMIC;

                HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = null;
                if (AutoClosePrintAndForm)
                {
                    processor = new PrintBordereauProcessor(currentModule.RoomId, currentModule.RoomTypeId, currentTreatment.ID, currentTreatment.PATIENT_ID, initData, reloadMenu, ReturnAfterEventPrint);
                }
                else
                {
                    processor = new PrintBordereauProcessor(currentModule.RoomId, currentModule.RoomTypeId, currentTreatment.ID, currentTreatment.PATIENT_ID, initData, reloadMenu);
                }
                processor.PayOption = this.payOption;
                processor.InitMenuPrint();
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Inventec.Common.Logging.LogSystem.Debug("Init print: " + elapsedMs);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReturnAfterEventPrint()
        {
            this.Close();
        }

        public void ReloadMenu(object data)
        {
            if (data != null)
            {
                if (data is List<MenuPrintADO>)
                {

                    MenuPrintProcessor menuPrintProcessor = new MenuPrintProcessor();
                    HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(data as List<MenuPrintADO>, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR_PRINT_TYPE>(false, true));
                    menuPrintInitADO.ControlContainer = pnlPrint;
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
