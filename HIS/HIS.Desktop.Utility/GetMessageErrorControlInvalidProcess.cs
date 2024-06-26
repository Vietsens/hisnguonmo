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
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.ConfigHideControl;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class GetMessageErrorControlInvalidProcess
    {
        public GetMessageErrorControlInvalidProcess() { }

        public void Run(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderControl, List<ModuleControlADO> ModuleControls, CommonParam paramCommon)
        {
            try
            {
                var invalidControls = dxValidationProviderControl.GetInvalidControls();
                if (invalidControls != null && invalidControls.Count > 0)
                {
                    List<string> paramMessageErrorOther = new List<string>();
                    List<string> paramMessageErrorEmpty = new List<string>();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("invalidControls.Count", invalidControls.Count));
                    foreach (System.Windows.Forms.Control c in invalidControls)
                    {
                        string errorC = "", lciEditorText = "";
                        if (ModuleControls == null || ModuleControls.Count == 0)
                        {
                            ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                            ModuleControls = controlProcess.GetControls(control);
                        }

                        ModuleControlADO lci = ModuleControls.Where(o => o.ControlType == "DevExpress.XtraLayout.LayoutControlItem" && o.lControl != null && ((DevExpress.XtraLayout.LayoutControlItem)o.lControl).Control == c).FirstOrDefault();
                        if (lci == null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Control " + c.Name + " - " + c.GetType().ToString() + " khong tim thay LayoutControlItem nao thoa man co Control == c");
                            lci = ModuleControls.Where(o => o.ControlType == "DevExpress.XtraLayout.LayoutControlItem" && o.lControl != null && ((DevExpress.XtraLayout.LayoutControlItem)o.lControl).Control == c.Parent).FirstOrDefault();
                        }

                        if (lci != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lci.ControlName), lci.ControlName));
                            var lciEditor = (lci.lControl as DevExpress.XtraLayout.LayoutControlItem);
                            if (lciEditor != null && lciEditor.TextVisible == true)
                            {
                                lciEditorText = lciEditor.Text;
                                errorC = lciEditor.Text.Replace(":", "");
                                Inventec.Common.Logging.LogSystem.Debug("Tim thay layoutcontrol thoa man: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lciEditor.Text), lciEditor.Text));
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("lci is null");
                        }

                        string errorT = dxValidationProviderControl.GetValidationRule(c).ErrorText;
                        if (errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc)
                            || errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc))
                        {
                            paramMessageErrorEmpty.Add(errorC);
                        }
                        else
                        {
                            errorC = String.Format("{0}:{1}", errorC, errorT);
                            paramMessageErrorOther.Add(errorC);
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorC), errorC)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorT), errorT)
                            + Inventec.Common.Logging.LogUtil.TraceData("lciEditorText", lciEditorText)
                            + Inventec.Common.Logging.LogUtil.TraceData("c.Name", c.Name));
                    }

                    if (paramMessageErrorOther.Count > 0)
                    {
                        paramCommon.Messages.AddRange(paramMessageErrorOther);
                    }
                    if (paramMessageErrorEmpty.Count > 0)
                    {
                        paramCommon.Messages.Add(String.Join(", ", paramMessageErrorEmpty) + " " + Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                    }

                    if (paramCommon.Messages.Count > 0)
                    {
                        paramCommon.Messages = paramCommon.Messages.Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Run(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderControl, List<ModuleControlADO> ModuleControls, List<string> paramMessageErrorEmpty, List<string> paramMessageErrorOther)
        {
            try
            {
                var invalidControls = dxValidationProviderControl.GetInvalidControls();
                if (invalidControls != null && invalidControls.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("invalidControls.Count", invalidControls.Count));
                    foreach (System.Windows.Forms.Control c in invalidControls)
                    {
                        string errorC = "", lciEditorText = "";
                        if (ModuleControls == null || ModuleControls.Count == 0)
                        {
                            ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                            ModuleControls = controlProcess.GetControls(control);
                        }

                        var lci = ModuleControls.Where(o => o.ControlType == "DevExpress.XtraLayout.LayoutControlItem" && o.lControl != null && (((DevExpress.XtraLayout.LayoutControlItem)o.lControl).Control == c || ((DevExpress.XtraLayout.LayoutControlItem)o.lControl).Control == c.Parent)).FirstOrDefault();
                        if (lci != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lci.ControlName), lci.ControlName));
                            var lciEditor = (lci.lControl as DevExpress.XtraLayout.LayoutControlItem);
                            if (lciEditor != null && lciEditor.TextVisible == true)
                            {
                                lciEditorText = lciEditor.Text;
                                errorC = lciEditor.Text.Replace(":", "");
                                Inventec.Common.Logging.LogSystem.Debug("Tim thay layoutcontrol thoa man: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lciEditor.Text), lciEditor.Text));
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("lci is null");
                        }

                        string errorT = dxValidationProviderControl.GetValidationRule(c).ErrorText;
                        if (errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc)
                            || errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc))
                        {
                            paramMessageErrorEmpty.Add(errorC);
                        }
                        else
                        {
                            errorC = String.Format("{0}:{1}", errorC, errorT);
                            paramMessageErrorOther.Add(errorC);
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorC), errorC)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => errorT), errorT)
                            + Inventec.Common.Logging.LogUtil.TraceData("lciEditorText", lciEditorText)
                            + Inventec.Common.Logging.LogUtil.TraceData("c.Name", c.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
