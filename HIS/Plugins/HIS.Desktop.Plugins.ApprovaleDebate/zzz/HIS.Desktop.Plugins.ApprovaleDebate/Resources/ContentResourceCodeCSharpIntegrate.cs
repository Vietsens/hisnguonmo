
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmApprovaleDebate
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovaleDebate.Resources.Lang", typeof(frmApprovaleDebate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabToDieuTri.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabCDHA.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabCDHA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBacSiHoiChan.Properties.NullText = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.cboBacSiHoiChan.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabXetNghiem.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabThuocVatTuMau.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabThuocVatTuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabSieuAmNoiSoi.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabSieuAmNoiSoi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPhauThuatThuThuat.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabPhauThuatThuThuat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabGiaiPhauBenh.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabGiaiPhauBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




