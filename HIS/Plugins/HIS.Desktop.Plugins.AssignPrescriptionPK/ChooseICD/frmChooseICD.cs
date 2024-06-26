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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD
{
    public partial class frmChooseICD : Form
    {

        private List<HIS_ICD> icds { get; set; }
        private List<IcdADO> icdADOs { get; set; }
        private DelegateSelectData refeshDataChooseICD { get; set; }

        public frmChooseICD(List<HIS_ICD> _icds, DelegateSelectData _refeshDataChooseICD)
        {
            InitializeComponent();
            try
            {
                this.icds = _icds;
                this.refeshDataChooseICD = _refeshDataChooseICD;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseICD_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
                LoadIcdToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmChooseICD
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmChooseICD).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseICD.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmChooseICD.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmChooseICD.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmChooseICD.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseICD.Text", Resources.ResourceLanguageManager.LanguageResourcefrmChooseICD, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadIcdToGrid()
        {
            try
            {
                icdADOs = new List<IcdADO>();
                if (icds != null && icds.Count > 0)
                {
                    foreach (var item in icds)
                    {
                        IcdADO icdADO = new IcdADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<IcdADO>(icdADO, item);
                        icdADOs.Add(icdADO);
                    }
                }
                gridControlICD.DataSource = icdADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditChooseService_Click(object sender, EventArgs e)
        {
            try
            {
                var icd = (IcdADO)gridViewICD.GetFocusedRow();
                foreach (var item in icdADOs)
                {
                    if (icd.ID == item.ID)
                    {

                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                }
                gridControlICD.RefreshDataSource();
                gridViewICD.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                List<IcdADO> sereServADOs = gridViewICD.DataSource as List<IcdADO>;
                if (icdADOs == null || icdADOs.Count == 0)
                    return;
                IcdADO icdCheck = null;
                foreach (var item in icdADOs)
                {
                    if (item.IsChecked)
                    {
                        icdCheck = item;
                        break;
                    }
                }

                if (icdCheck == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 icd!", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (refeshDataChooseICD != null)
                        refeshDataChooseICD(icdCheck);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
