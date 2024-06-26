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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Bordereau.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        public void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControl1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmBordereau.btnPrint.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridViewBordereau.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmBordereau.gridViewBordereau.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnIsOutKtcFee.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnIsOutKtcFee.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn1.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn2.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn10.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn3.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn4.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn5.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn5.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnPatientTypeName.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnPatientTypeName.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnPatientTypeName.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn11.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn11.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnDvDinhKem.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnDvDinhKem.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnDvDinhKem.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnDvDinhKem.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.grcExpendTypeId.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.grcExpendTypeId.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.grcExpendTypeId.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.grcExpendTypeId.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn7.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn7.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn8.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn9.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());

                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn18.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                

                this.gridColumnDescription.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnDescription.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnIsNoExecute.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnIsNoExecute.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.repositoryItemLookUpEdit_PatientType.NullText = Inventec.Common.Resource.Get.Value("frmBordereau.repositoryItemLookUpEdit_PatientType.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmBordereau.repositoryItemPictureEdit1.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit_PatientType.NullText = Inventec.Common.Resource.Get.Value("frmBordereau.repositoryItemGridLookUpEdit_PatientType.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem3.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem4.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem5.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem6.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.lciPayType.Text = Inventec.Common.Resource.Get.Value("frmBordereau.lciPayType.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmBordereau.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());

                //minhnq
                this.chkAmount.Text = Inventec.Common.Resource.Get.Value("frmBordereau.chkAmount.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                //this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem18.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem8.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                //this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem10.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                //this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem11.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem12.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem19.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem16.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem15.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem21.OptionsToolTip.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem21.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                //this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmBordereau.layoutControlItem20.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmBordereau.txtKeyword.Properties.NullValuePrompt", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmBordereau.btnFind.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.lciReturnResult.Text = Inventec.Common.Resource.Get.Value("frmBordereau.lciReturnResult.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn18.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnDoiTuongPT.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnDoiTuongPT.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnDoiTuongPT.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnDoiTuongPT.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn13.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnIsNotUseBHYT.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnIsNotUseBHYT.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnEquipment.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnEquipment.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnOtherPaySource.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnOtherPaySource.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn17.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn17.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn17.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnStentOrder.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnStentOrder.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnStentOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnStentOrder.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumnShareCount.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumnShareCount.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridCol_Package.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridCol_Package.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn6.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn16.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn15.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn15.ToolTip", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmBordereau.gridColumn12.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
