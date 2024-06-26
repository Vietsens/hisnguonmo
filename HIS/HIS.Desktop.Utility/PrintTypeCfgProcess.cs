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
using DevExpress.XtraLayout;
//using HIS.Desktop.LocalStorage.ConfigPrintType;
//using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Logging;
//using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    internal class PrintTypeCfgProcess
    {
        const string methodExecuteDefault = "OnClickPrintWithPrintTypeCfg";

        //private static Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT_TYPE> dicPrintType;
        //internal static Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT_TYPE> DicPrintType
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (dicPrintType == null || dicPrintType.Count == 0)
        //            {
        //                dicPrintType = MPS.ProcessorBase.PrintConfig.PrintTypes.ToDictionary(o => o.ID, o => o);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }

        //        return dicPrintType;
        //    }
        //    set
        //    {
        //        dicPrintType = value;
        //    }
        //}

        internal PrintTypeCfgProcess() { }
        string ModuleLink;
        List<string> excludePrintType;
        Control controlMain;
        Dictionary<string, string> dicOfNameOnClickPrintWithPrintTypeCfg;

        /// <summary>
        /// Generate động các nút in
        /// </summary>
        /// <param name="moduleLink">ModuleLink của chức năng</param>
        /// <param name="control">Truyền vào main uc/form</param>
        /// <param name="excludePrintType">Các mã in sẽ bị xóa khỏi danh sách hiển thị các nút</param>
        internal void Run(string moduleLink, Control control, List<string> excludePrintType, Dictionary<string, string> _dicOfNameOnClickPrintWithPrintTypeCfg)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("PrintTypeCfgProcess.1");
                this.ModuleLink = moduleLink;
                this.dicOfNameOnClickPrintWithPrintTypeCfg = _dicOfNameOnClickPrintWithPrintTypeCfg;
                this.excludePrintType = excludePrintType;
                this.controlMain = control;
                var prtButtons = ConfigPrintTypeWorker.GetByModule(this.ModuleLink);
                if (prtButtons != null && prtButtons.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ModuleLink), ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prtButtons), prtButtons));

                    ModuleControlProcess moduleControlProcess = new ModuleControlProcess(true);
                    var buttonHierarchys = moduleControlProcess.GetControls(control);
                    if (buttonHierarchys != null && buttonHierarchys.Count() > 0)
                    {
                        foreach (var pControl in buttonHierarchys)
                        {
                            var prtControls = pControl.mControl != null ? prtButtons.Where(o => o.CONTROL_PATH == pControl.ControlPath).ToList() : null;
                            if (prtControls != null && prtControls.Count > 0)
                            {
                                List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();
                                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();

                                foreach (var prtButton in prtControls)
                                {
                                    var prt = DicPrintType != null && DicPrintType.ContainsKey(prtButton.PRINT_TYPE_ID) ? DicPrintType[prtButton.PRINT_TYPE_ID] : null;
                                    if (prt != null)
                                    {
                                        bool isExclude = this.excludePrintType != null && this.excludePrintType.Contains(prt.PRINT_TYPE_CODE);
                                        if (isExclude)
                                        {
                                            Inventec.Common.Logging.LogSystem.Debug("Ma in nam trong danh sach cac ma in bi loai bo do nghiep vu xu ly o chuc nang: " + this.ModuleLink + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prt.PRINT_TYPE_CODE), prt.PRINT_TYPE_CODE) + "" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.excludePrintType), this.excludePrintType));
                                            continue;
                                        }
                                        HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                                        menuPrintADO.EventHandler = new EventHandler(EventButtonPrintClick);
                                        menuPrintADO.PrintTypeCode = prt.PRINT_TYPE_CODE;
                                        if (this.dicOfNameOnClickPrintWithPrintTypeCfg != null && this.dicOfNameOnClickPrintWithPrintTypeCfg.ContainsKey(pControl.ControlName))
                                        {
                                            menuPrintADO.Tag = String.Format("{0}__{1}", prt.PRINT_TYPE_CODE, this.dicOfNameOnClickPrintWithPrintTypeCfg[pControl.ControlName]);
                                        }
                                        else
                                        {
                                            menuPrintADO.Tag = prt.PRINT_TYPE_CODE;
                                        }
                                        menuPrintADO.Caption = !String.IsNullOrEmpty(prtButton.CAPTION) ? prtButton.CAPTION : prt.PRINT_TYPE_NAME;
                                        menuPrintADOs.Add(menuPrintADO);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay du lieu bieu in trong bang sar_print_type tuong ung voi print_type_id trong SAR_PRINT_TYPE_CFG____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prtControls), prtControls) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pControl.ControlPath), pControl.ControlPath));
                                    }
                                }

                                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, MPS.ProcessorBase.PrintConfig.PrintTypes);
                                if (pControl.mControl is DevExpress.XtraLayout.LayoutControl)
                                {
                                    menuPrintInitADO.MinSizeHeight = ((DevExpress.XtraLayout.LayoutControl)pControl.mControl).MinimumSize.Height;
                                    menuPrintInitADO.MaxSizeHeight = ((DevExpress.XtraLayout.LayoutControl)pControl.mControl).MaximumSize.Height;
                                }

                                menuPrintInitADO.ControlContainer = pControl.mControl;
                                var menuResultADO = menuPrintProcessor.Run(menuPrintInitADO) as MenuPrintResultADO;
                                if (menuResultADO == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                                }
                            }
                            else if (pControl.mControl != null && (pControl.mControl is DevExpress.XtraLayout.LayoutControl || pControl.mControl is Panel))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Khong tim thay du lieu cau hinh nut in trong bang SAR_PRINT_TYPE_CFG____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prtButtons), prtButtons) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pControl.ControlPath), pControl.ControlPath));
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("buttonHierarchys is null || buttonHierarchys.count = 0");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("PrintTypeCfgProcess.2");
                }
                Inventec.Common.Logging.LogSystem.Debug("PrintTypeCfgProcess.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EventButtonPrintClick(object sender, EventArgs e)
        {
            try
            {
                string methodExecute = methodExecuteDefault;
                Type classType = this.controlMain.GetType();
                if (sender != null && this.dicOfNameOnClickPrintWithPrintTypeCfg != null)
                {
                    string printTypeCodeWithOther = "";
                    if (sender is DXMenuItem)
                    {
                        var bbtnItem = sender as DXMenuItem;
                        printTypeCodeWithOther = (bbtnItem.Tag ?? "").ToString();
                    }
                    else if (sender is SimpleButton)
                    {
                        var bbtnItem = sender as SimpleButton;
                        printTypeCodeWithOther = (bbtnItem.Tag ?? "").ToString();
                    }

                    if (!String.IsNullOrEmpty(printTypeCodeWithOther) && printTypeCodeWithOther.Contains("__"))
                    {
                        var arPtype = printTypeCodeWithOther.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arPtype.Count() > 1)
                        {
                            string printTypeCode = arPtype[0];
                            methodExecute = arPtype[1];

                            if (sender is DXMenuItem)
                            {
                                var bbtnItem = sender as DXMenuItem;
                                bbtnItem.Tag = printTypeCode;
                                sender = bbtnItem;
                            }
                            else if (sender is SimpleButton)
                            {
                                var bbtnItem = sender as SimpleButton;
                                bbtnItem.Tag = printTypeCode;
                                sender = bbtnItem;
                            }
                        }
                    }
                }

                MethodInfo methodInfo = classType.GetMethod(methodExecute);
                methodInfo.Invoke(this.controlMain, new object[] { sender, e });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
