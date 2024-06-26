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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.MpsBehavior.Mps000297;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.MpsBehavior.Mps000298;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.MpsBehavior.Mps000389;
using HIS.UC.MenuPrint.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt
{
    public partial class PrintTreatmentEndTypeExtProcessor
    {
        public enum OPTION
        {
            PRINT,
            INIT_MENU,
            PRINT__INIT_MENU
        }

        public long treatmentId { get; set; }
        public DelegateSelectData refeshMenu { get; set; }
        public CreateMenu.TYPE createMenuType { get; set; }
        private bool PrintNow { get; set; }
        private long? roomId { get; set; }

        public PrintTreatmentEndTypeExtProcessor(long _treatmentId, CreateMenu.TYPE _createMenuType, long? roomId)
        {
            try
            {
                this.treatmentId = _treatmentId;
                this.createMenuType = _createMenuType;
                this.roomId = roomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public PrintTreatmentEndTypeExtProcessor(long _treatmentId, DelegateSelectData _refeshMenu, CreateMenu.TYPE _createMenuType, long? roomId)
        {
            try
            {
                this.treatmentId = _treatmentId;
                this.refeshMenu = _refeshMenu;
                this.createMenuType = _createMenuType;
                this.roomId = roomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Print(PrintTreatmentEndTypeExPrintType.TYPE printType, OPTION option)
        {
            Print(printType, option, false);
        }

        public void Print(PrintTreatmentEndTypeExPrintType.TYPE printType, OPTION option, bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                if (option == OPTION.INIT_MENU || option == OPTION.PRINT__INIT_MENU)
                {
                    this.InitMenu(printType);
                }

                if (option == OPTION.PRINT__INIT_MENU || option == OPTION.PRINT)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    switch (printType)
                    {
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM:
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_HUONG_BHXH, DelegateRunPrinter);
                            break;
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI:
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_DUONG_THAI, DelegateRunPrinter);

                            break;

                        case PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO:
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE___HEN_MO, DelegateRunPrinter);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitMenu(PrintTreatmentEndTypeExPrintType.TYPE printType)
        {
            try
            {
                if (this.createMenuType == PrintTreatmentEndTypeExt.CreateMenu.TYPE.NORMAL)
                {
                    switch (printType)
                    {
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM:
                            this.CreateMenu("Giấy nghỉ hưởng BHXH", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_HUONG_BHXH);
                            break;
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI:
                            this.CreateMenu("Giấy nghỉ dưỡng thai", PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_DUONG_THAI);
                            break;
                        case PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO:
                            this.CreateMenu("Giấy hẹn mổ", PrintTypeCodeWorker.PRINT_TYPE_CODE___HEN_MO);
                            break;
                        default:
                            break;
                    }
                }
                else if (this.createMenuType == PrintTreatmentEndTypeExt.CreateMenu.TYPE.DYNAMIC)
                {
                    switch (printType)
                    {
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM:
                            this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_HUONG_BHXH);
                            break;
                        case PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI:
                            this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_DUONG_THAI);
                            break;
                        case PrintTreatmentEndTypeExPrintType.TYPE.HEN_MO:
                            this.CreateMenuDynamic(PrintTypeCodeWorker.PRINT_TYPE_CODE___HEN_MO);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MenuPrintADO CreateMenuDynamic(string PrintTypeCode)
        {
            MenuPrintADO menuPrintADO = null;
            try
            {
                menuPrintADO = new MenuPrintADO();
                menuPrintADO.EventHandler = new EventHandler(this.PrintEventFromMenu);
                menuPrintADO.PrintTypeCode = PrintTypeCode;
                menuPrintADO.Tag = PrintTypeCode;
                if (this.refeshMenu != null)
                {
                    this.refeshMenu(menuPrintADO);
                }
            }
            catch (Exception ex)
            {
                menuPrintADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return menuPrintADO;
        }

        private void RunPrintMpsCode(string mpsCode)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(mpsCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateMenu(string namePrint, string printTypeCode)
        {
            try
            {
                DXMenuItem itemMenu = new DXMenuItem(namePrint,
                                new EventHandler(this.PrintEventFromMenu));
                itemMenu.Tag = printTypeCode;
                if (this.refeshMenu != null)
                {
                    this.refeshMenu(itemMenu);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintEventFromMenu(object sender, EventArgs e)
        {
            try
            {
                string mpsCode = null;
                if (sender is DXMenuItem)
                {
                    var btn = sender as DXMenuItem;
                    mpsCode = (string)(btn.Tag);
                }
                this.RunPrintMpsCode(mpsCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                ILoad loadMps = null;
                switch (printCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_OM:
                        loadMps = new Mps000298Behavior(treatmentId, roomId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_DUONG_THAI:
                        loadMps = new Mps000297Behavior(treatmentId, roomId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___NGHI_HUONG_BHXH:
                        loadMps = new Mps000298Behavior(treatmentId, roomId);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE___HEN_MO:
                        loadMps = new Mps000389Behavior(treatmentId, roomId);
                        break;
                    default:
                        break;
                }
                result = loadMps != null ? loadMps.Run(printCode, fileName, PrintNow) : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
