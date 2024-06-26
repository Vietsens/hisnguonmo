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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.VoiceCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class VoiceCommandProcess
    {
        static List<VVA.EFMODEL.DataModels.VVA_VOICE_COMMAND> VvaVoiceCommandData { get; set; }

        public static void ReloadVoiceCommand()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReloadVoiceCommand.1");
                CommonParam param = new CommonParam();
                VVA.Filter.VvaVoiceCommandFilter voiceCommandFilter = new VVA.Filter.VvaVoiceCommandFilter();
                voiceCommandFilter.IS_ACTIVE = IMSys.DbConfig.VVA_RS.COMMON.IS_ACTIVE__TRUE;
                voiceCommandFilter.APP_CODE = GlobalVariables.APPLICATION_CODE;
                var rs = HIS.Desktop.ApiConsumer.ApiConsumers.VvaConsumer.Get<Inventec.Core.ApiResultObject<List<VVA.EFMODEL.DataModels.VVA_VOICE_COMMAND>>>("/api/VvaVoiceCommand/Get", param, voiceCommandFilter);
                VvaVoiceCommandData = rs != null ? rs.Data : null;

                AddToDicAICommandActionControlConfig();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => VvaVoiceCommandData), VvaVoiceCommandData));
                Inventec.Common.Logging.LogSystem.Debug("ReloadVoiceCommand.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static List<string> GenerateListVoiceText(string voiceText)
        {
            List<string> result = new List<string>();
            try
            {
                if (!String.IsNullOrEmpty(voiceText))
                {
                    result = voiceText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        static void AddToDicAICommandActionControlConfig()
        {
            if (VvaVoiceCommandData != null && VvaVoiceCommandData.Count > 0 && VoiceCommandData.voiceCommandProcessor != null && VoiceCommandData.isUseVoiceCommand)
            {
                List<CommandTypeADO> dicAICommandActionControlConfig = new List<CommandTypeADO>();
                foreach (var vc in VvaVoiceCommandData)
                {
                    try
                    {
                        CommandTypeADO commandTypeADO = new CommandTypeADO() { ModuleLink = vc.MODULE_LINK, listText = GenerateListVoiceText(vc.VOICE_TEXT), commandType = (int)(vc.COMMAND_TYPE ?? 0), commandActionLink = vc.COMMAND_ACTION };
                        dicAICommandActionControlConfig.Add(commandTypeADO);
                    }
                    catch (Exception exx)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exx);
                    }
                }
                VoiceCommandData.voiceCommandProcessor.InitialDicAICommandActionControlConfig(dicAICommandActionControlConfig);
            }
        }

        public static void InitVoiceCommand()
        {
            try
            {
                if (VoiceCommandData.isUseVoiceCommand)
                {
                    return;
                }
                VoiceCommandData.isUseVoiceCommand = true;


                string vaisConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Config.VAIS.ASR");
                if (!String.IsNullOrEmpty(vaisConfig))
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => vaisConfig), vaisConfig));
                    try
                    {
                        //wss://nhaplieu.vais.vn|example_api|1|500|16000|general
                        var arr = vaisConfig.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr != null && arr.Length > 0)
                        {
                            if (arr.Length > 0)
                            {
                                CommandCFG.Vais__WssUrl = arr[0];
                            }
                            else if (arr.Length > 1)
                            {
                                CommandCFG.Vais__APIKEY = arr[1];
                            }
                            else if (arr.Length > 2)
                            {
                                CommandCFG.Vais__version = arr[2];
                            }
                            else if (arr.Length > 3)
                            {
                                CommandCFG.Vais__BufferMillisecond = Convert.ToInt32(arr[3]);
                            }
                            else if (arr.Length > 4)
                            {
                                CommandCFG.Vais__SampleRate = Convert.ToInt32(arr[4]);
                            }
                            else if (arr.Length > 5)
                            {
                                CommandCFG.Vais__ModelName = arr[5];
                            }
                        }
                    }
                    catch (Exception exx)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exx);
                    }
                }

                VoiceCommandData.voiceCommandProcessor = new VoiceCommandProcessor(ProcessResultCommand);

                if (VvaVoiceCommandData == null || VvaVoiceCommandData.Count == 0)
                    ReloadVoiceCommand();
                else
                    AddToDicAICommandActionControlConfig();

                VoiceCommandData.voiceCommandProcessor.SetCommandActionLink(GetCurrentCommandActionLink);
                VoiceCommandData.voiceCommandProcessor.SetCommandModuleLink(GetCurrentModuleLink);
                VoiceCommandData.voiceCommandProcessor.SetDelegateTimeout(ProcessWhileTimeout);
                //VoiceCommandData.voiceCommandProcessor.StartListening();

                VoiceCommandData.voiceCommandProcessor.RunAsync();


                //VoiceCommandData.timerVoiceCommand = new System.Windows.Forms.Timer();
                //VoiceCommandData.timerVoiceCommand.Interval = 100;
                //VoiceCommandData.timerVoiceCommand.Enabled = true;
                //VoiceCommandData.timerVoiceCommand.Tick += TimerVoiceCommand_Tick;
                //VoiceCommandData.timerVoiceCommand.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void DisconnectVoiceCommand()
        {
            try
            {
                if (VoiceCommandData.timerVoiceCommand != null)
                {
                    VoiceCommandData.timerVoiceCommand.Stop();
                }
                if (VoiceCommandData.voiceCommandProcessor != null)
                {
                    VoiceCommandData.voiceCommandProcessor.Stop();
                }
                VoiceCommandData.timerVoiceCommand = null;
                VoiceCommandData.voiceCommandProcessor = null;
                VoiceCommandData.isUseVoiceCommand = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static Control GetMainControl()
        {
            Form f = Form.ActiveForm;
            try
            {
                Form active = null;
                var a = Application.OpenForms.Cast<Form>().ToList();//.First(x => x.Focused);
                if (a != null && a.Count > 0)
                {
                    foreach (var item in a)
                    {
                        if (item.ActiveControl == null) continue;
                        if (!item.CanFocus) continue;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.Name), item.Name));

                        //if (item.Name == "frmMain")
                        //{
                        //    item.Activate();
                        //}

                        if (item.ContainsFocus)
                        {
                            active = item;
                            break;
                        }
                    }
                    if (active == null)
                    {
                        for (int i = (Application.OpenForms.Count - 1); i >= 0; i--)
                        {
                            if (Application.OpenForms[i].Name == "frmWaitForm" || String.IsNullOrEmpty(Application.OpenForms[i].Name)) continue;
                            if (Application.OpenForms[i] == f) continue;

                            active = Application.OpenForms[i];
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => active.Name), active.Name));
                            break;
                        }
                    }

                    if (active != null)
                    {
                        f = active;
                    }
                }

                if (f != null && f == HIS.Desktop.Controls.Session.SessionManager.GetFormMain())
                {
                    Inventec.Common.Logging.LogSystem.Info("Form main is active, f.name=" + f.Name);
                    return HIS.Desktop.Controls.Session.SessionManager.GetCurrentPage();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return f;
        }

        static List<ModuleControlADO> GetDataModuleControl()
        {
            List<ModuleControlADO> moduleControls = null;
            var form = GetMainControl();

            ModuleControlProcess controlProcess = new ModuleControlProcess(true);
            moduleControls = controlProcess.GetControls(form);
            return moduleControls;
        }

        static Control FindFocusedControl()
        {
            try
            {
                var containerControl = GetMainControl() as IContainerControl;
                if (containerControl != null)
                {
                    Control c = containerControl.ActiveControl;
                    while ((c is DevExpress.XtraLayout.LayoutControl || c is PanelControl || c is XtraUserControl || c is UserControl))
                    {
                        if (c is DevExpress.XtraLayout.LayoutControl)
                        {
                            if (!(((DevExpress.XtraLayout.LayoutControl)c).ActiveControl == null))
                            {
                                c = FindActiveControlByParentLayoutControl((DevExpress.XtraLayout.LayoutControl)c);
                            }
                        }
                        else if (c is PanelControl || c is Panel || c is XtraUserControl || c is UserControl)
                        {
                            c = FindFocusedControlByParentPanelOrUserControl(c);
                        }
                    }

                    if (c is DevExpress.XtraEditors.TextBoxMaskBox)
                    {
                        c = c.Parent;
                    }
                    return c;
                }
                else
                    return GetMainControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        static Control FindFocusedControlByParentPanelOrUserControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }

        static Control FindActiveControlByParentLayoutControl(DevExpress.XtraLayout.LayoutControl layoutcontrol)
        {
            Control c = null;
            if (!(layoutcontrol.ActiveControl == null))
            {
                c = layoutcontrol.ActiveControl;
            }
            if (c is DevExpress.XtraLayout.LayoutControl)
            {
                return FindActiveControlByParentLayoutControl((DevExpress.XtraLayout.LayoutControl)c);
            }
            return c;
        }

        static string GetCurrentModuleLink()
        {
            string link = "";
            try
            {
                var form = GetMainControl();
                if (form != null && form.Name != "frmMain" && form.Name != "frmWaitForm" && form is FormBase)
                {
                    link = (form as FormBase).GetModuleLink();
                }
                if (String.IsNullOrEmpty(link))
                    link = GlobalVariables.CurrentModuleSelected != null ? GlobalVariables.CurrentModuleSelected.ModuleLink : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return link;
        }


        static string GetCurrentCommandActionLink()
        {
            string link = "";
            try
            {
                var moduleControls = GetDataModuleControl();

                Control control = FindFocusedControl();
                if (control != null)
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));
                if (control != null && moduleControls != null && moduleControls.Count > 0)
                {
                    foreach (var ctrl in moduleControls)
                    {
                        if (ctrl.mControl != null && ctrl.mControl == control)
                        {
                            link = ctrl.ControlPath;
                            Inventec.Common.Logging.LogSystem.Debug("GetCurrentCommandActionLink:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => link), link) + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return link;
        }

        static void TimerVoiceCommand_Tick(object sender, EventArgs e)
        {
            try
            {
                if (VoiceCommandData.isUseVoiceCommand)
                {
                    Inventec.Common.Logging.LogSystem.Debug("TimerVoiceCommand_Tick.1");

                    string strCurrentCommandActionLink = GetCurrentCommandActionLink();
                    string strCurrentmodulellink = GetCurrentModuleLink();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strCurrentCommandActionLink), strCurrentCommandActionLink)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strCurrentmodulellink), strCurrentmodulellink));
                    VoiceCommandData.voiceCommandProcessor.RunAsync();
                    Inventec.Common.Logging.LogSystem.Debug("TimerVoiceCommand_Tick.2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static void ProcessWhileTimeout()
        {
            DisconnectVoiceCommand();
        }

        static void ProcessResultCommand(ResultCommandADO resultCommand)
        {
            if (resultCommand.commandType >= 0)
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessResultCommand=>" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommand.commandType), resultCommand.commandType)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommand.commandActionLink), resultCommand.commandActionLink));

                if (!String.IsNullOrEmpty(resultCommand.commandActionLink))
                {
                    if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__MENU)
                    {
                        List<object> listArgs = new List<object>();
                        VoiceCommandPluginInstanceBehavior.ShowModule(resultCommand.commandActionLink, (GlobalVariables.CurrentModuleSelected != null ? GlobalVariables.CurrentModuleSelected.RoomId : 0), (GlobalVariables.CurrentModuleSelected != null ? GlobalVariables.CurrentModuleSelected.RoomTypeId : 0), listArgs);
                        Inventec.Common.Logging.LogSystem.Info("Truong hop goi lenh mo menu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultCommand.commandActionLink), resultCommand.commandActionLink)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.CurrentModuleSelected.RoomId), GlobalVariables.CurrentModuleSelected.RoomId)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.CurrentModuleSelected.RoomTypeId), GlobalVariables.CurrentModuleSelected.RoomTypeId));
                        return;
                    }

                    var moduleControls = GetDataModuleControl();

                    var controls = moduleControls.Where(o => o.ControlPath == resultCommand.commandActionLink).ToList();
                    var mcontrol = (controls != null && controls.Count > 0) ? controls.FirstOrDefault() : null;
                    if (mcontrol != null)
                    {
                        var control = mcontrol.mControl;

                        Inventec.Common.Logging.LogSystem.Info("control is other, control name = " + control.Name + "__control type = " + control.GetType().ToString() + "commandType = " + resultCommand.commandType);

                        if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__ENTER)
                        {
                            SendKeys.Send("{ENTER}");
                        }
                        else if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CUSTOM && control.Tag != null)
                        {
                            Action<ResultCommandADO> actCustom = control.Tag as Action<ResultCommandADO>;
                            if (actCustom != null)
                            {
                                actCustom(resultCommand);
                            }
                        }
                        else if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CLEAR)
                        {
                            control.Focus();

                            if (control is TextBoxMaskBox
                                || control is TextEdit
                                || control is ButtonEdit
                                || control is RichEditControl
                                || control is Telerik.WinControls.UI.RadRichTextEditor
                                || control is MemoEdit
                                || control is RichTextBox
                                || control is TextBox)
                            {
                                control.Text = "";
                                Inventec.Common.Logging.LogSystem.Info("delete value Text");
                            }
                            else if (control is LookUpEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("delete value LookUpEdit");
                                var cbo = control as LookUpEdit;
                                cbo.EditValue = null;
                            }
                            else if (control is GridLookUpEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("delete value GridLookUpEdit");
                                var cbo = control as GridLookUpEdit;
                                cbo.EditValue = null;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is other, control name = " + control.Name + "__control type = " + control.GetType().ToString() + "__CanFocus = " + control.CanFocus);
                            }
                        }
                        else if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__FOCUS && control.CanFocus)
                        {
                            control.Focus();
                        }
                        else if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__CLICK)
                        {
                            if (control is Button)
                            {
                                var btn = control as Button;
                                btn.PerformClick();
                            }
                            else if (control is SimpleButton)
                            {
                                var btn = control as SimpleButton;
                                btn.PerformClick();
                            }
                            else if (control is ButtonEdit)
                            {
                                //var btn = control as ButtonEdit;
                                //btn.PerformClick();
                            }
                        }
                        else
                        {
                            if (control is TextBoxMaskBox)
                            {
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    control.Text += resultCommand.text + " ";
                                    TextBoxMaskBox ct = ((TextBoxMaskBox)control);
                                    ct.MaskBoxSelectionStart = control.Text.Length;
                                    ct.MaskBoxSelectionLength = 0;
                                }
                            }
                            else if (control is TextEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is TextEdit");
                                var txt = control as TextEdit;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    txt.Text += resultCommand.text + " ";
                                }
                                txt.SelectionStart = control.Text.Length;
                                txt.SelectionLength = 0;
                            }
                            else if (control is ButtonEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is ButtonEdit");
                                var txt = control as ButtonEdit;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    txt.Text += resultCommand.text + " ";
                                }
                                txt.SelectionStart = control.Text.Length;
                                txt.SelectionLength = 0;
                            }
                            else if (control is RichEditControl)
                            {
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    RichEditControl ct = ((RichEditControl)control);

                                    DevExpress.XtraRichEdit.API.Native.Document document = ct.Document;

                                    ct.Document.InsertHtmlText(document.Range.End, resultCommand.text + " ", DevExpress.XtraRichEdit.API.Native.InsertOptions.KeepSourceFormatting);

                                    DevExpress.XtraRichEdit.API.Native.DocumentPosition position = document.Range.End;
                                    ct.Document.CaretPosition = position;
                                    ct.ScrollToCaret(-200);
                                }
                            }
                            else if (control is Telerik.WinControls.UI.RadRichTextEditor)
                            {
                                var txt = control as Telerik.WinControls.UI.RadRichTextEditor;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    control.Text += resultCommand.text + " ";
                                }
                            }
                            else if (control is MemoEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is MemoEdit");
                                var txt = control as MemoEdit;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    txt.Text += resultCommand.text + " ";
                                }
                                txt.SelectionStart = control.Text.Length;
                                txt.SelectionLength = 0;
                            }
                            else if (control is RichTextBox)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is RichTextBox");
                                var txt = control as RichTextBox;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    txt.Text += resultCommand.text + " ";
                                }
                                txt.SelectionStart = control.Text.Length;
                                txt.SelectionLength = 0;
                            }
                            else if (control is TextBox)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is TextBox");
                                var txt = control as TextBox;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    txt.Text += resultCommand.text + " ";
                                }
                                txt.SelectionStart = control.Text.Length;
                                txt.SelectionLength = 0;
                            }
                            else if (control is LookUpEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is LookUpEdit");
                                var cbo = control as LookUpEdit;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    cbo.Text = resultCommand.text;
                                }
                                cbo.ShowPopup();
                            }
                            else if (control is GridLookUpEdit)
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is GridLookUpEdit");
                                var cbo = control as GridLookUpEdit;
                                if (resultCommand.commandType == IMSys.DbConfig.VVA_RS.VVA_COMMAND_TYPE.COMMAND_TYPE__INPUT)
                                {
                                    cbo.Text = resultCommand.text;
                                }
                                cbo.ShowPopup();
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("control is other, control name = " + control.Name + "__control type = " + control.GetType().ToString() + "__CanFocus = " + control.CanFocus);
                                //if (resultCommand.commandType == CommandTypeCFG.CommandType__Input)
                                //{
                                //    control.Text += resultCommand.text + " ";
                                //}
                                //control.Focus();
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    txtResult.Text += resultCommand.text + " ";
            //    txtResult.SelectionStart = txtResult.Text.Length;
            //    txtResult.SelectionLength = 0;
            //}
        }
    }
}
