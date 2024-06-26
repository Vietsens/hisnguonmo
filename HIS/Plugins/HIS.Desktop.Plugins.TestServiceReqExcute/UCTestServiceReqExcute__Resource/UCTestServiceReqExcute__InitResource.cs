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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TestServiceReqExcute.Resources.Lang", typeof(HIS.Desktop.Plugins.TestServiceReqExcute.UCTestServiceReqExcute__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollSTT.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdCollSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexCode.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexName.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdCollDonvitinh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVallue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColVallue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColValueNormal.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColValueNormal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMinValue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColMinValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxValue.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColMaxValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLevel.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColLevel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsParent.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColIsParent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNote.Caption = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.grdColNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKeDonThuoc.Text = Inventec.Common.Resource.Get.Value("UCTestServiceReqExcute.btnKeDonThuoc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
