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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.ClassGlobal;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.Location;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraLayout;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.Desktop.DelegateRegister;
using DevExpress.XtraLayout.Utils;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.PlusInfo.Design
{
    public partial class frmPatientExtend : Form
    {
        #region Reclare
        public delegate void PatientInfoResult(object data);
        UCPatientExtendADO patientExtendADO { get; set; }
        PatientInfoResult delegatePatientInfoResult;
        DelegateFocusMoveout dlgFocusNextUserControl;

        UCPlusInfoADO dataPlusInfo = new UCPlusInfoADO();

        List<UserControl> listControl = null;
        int indexUCMaHoNgheo = 0;
        int indexCMNDNumber = 0;
        bool isShowControlHrmKskCode;
        #endregion

        #region Construct

        public frmPatientExtend()
        {
            InitializeComponent();
        }

        public frmPatientExtend(UCPatientExtendADO _patientExtendADO, PatientInfoResult delegatePatientInfoResult)
        {
            InitializeComponent();
            this.patientExtendADO = _patientExtendADO;
            this.delegatePatientInfoResult = delegatePatientInfoResult;
        }

        public frmPatientExtend(List<UserControl> _listControl, bool _isShowControlHrmKskCode)
        {
            try
            {
                InitializeComponent();
                if (this.listControl == null)
                    this.listControl = new List<UserControl>();
                this.listControl = _listControl;
                this.isShowControlHrmKskCode = _isShowControlHrmKskCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Load

        private void frmPatientExtend_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.LoadControlForForm(this.listControl);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        } 
        /// <summary>
           ///Hàm xét ngôn ngữ cho giao diện frmPatientExtend
           /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(frmPatientExtend).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl2.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.simpleButton1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.bar1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPatientExtend.barButtonItem1.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmPatientExtend.barButtonItem2.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadControlForForm(List<UserControl> listUC)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listUC.count", (listUC != null ? listUC.Count : 0) + ""));

                int setAgainIndex = 17;
                layoutControl1.Root.LayoutMode = LayoutMode.Table;

                RowDefinition[] rowDefinitions = new RowDefinition[] { };
                List<RowDefinition> listRow = new List<RowDefinition>();
                for (int i = 0; i < listUC.Count - 2; i++)
                {
                    RowDefinition row = new RowDefinition(layoutControl1.Root, 50, SizeType.Percent);
                    listRow.Add(row);
                }
                rowDefinitions = listRow.ToArray();
                layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(rowDefinitions);
                int rowCout = 0;
                if (listUC.Count % 2 == 0)
                    rowCout = listUC.Count / 2;
                else
                    rowCout = listUC.Count / 2 + 1;
                for (int j = 0; j < 2; j++)
                {
                    int temp = j;
                    for (int i = (temp == 0 ? 0 : rowCout); i < (temp == 0 ? rowCout : listUC.Count); i++)
                    {
                        setAgainIndex += 1;
                        listUC[i].TabIndex = setAgainIndex;
                        LayoutControlItem item1 = layoutControl1.Root.AddItem(listUC[i].Name, listUC[i]);
                        if (this.isShowControlHrmKskCode && listUC[i].Name == ChoiceControl.ucHrmKskCode)
                        {

                        }

                        item1.OptionsTableLayoutItem.RowIndex = i % rowCout;
                        item1.OptionsTableLayoutItem.ColumnIndex = temp;
                        item1.TextVisible = false;
                        if (listUC[i].Name == ChoiceControl.ucMaHoNgheo)
                            this.indexUCMaHoNgheo = i;
                        if (listUC[i].Name == ChoiceControl.ucCmndNumber)
                            this.indexCMNDNumber = i;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                bool valid = true;
                if (this.indexUCMaHoNgheo > 0)
                    valid = valid && ((Design.UCMaHoNgheo)listControl[indexUCMaHoNgheo]).val;
                if (this.indexCMNDNumber > 0)
                    valid = valid && ((Design.UCCMND)listControl[indexCMNDNumber]).val;
                if (!valid)
                    return;
                this.Close();
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
                foreach (LayoutControlItem col in layoutControlGroup1.Items)
                {
                    if (col != null && col.Control.TabIndex == (int)sender + 1)
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusControl(UserControl uc)
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
    }
}
