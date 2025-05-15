
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
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabToDieuTri.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabCDHA.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabCDHA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabXetNghiem.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabThuocVatTuMau.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabThuocVatTuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabSieuAmNoiSoi.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabSieuAmNoiSoi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPhauThuatThuThuat.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabPhauThuatThuThuat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabGiaiPhauBenh.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabGiaiPhauBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện Resources
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovaleDebate.Resources.Lang", typeof(Resources).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreeListService
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovaleDebate.Resources.Lang", typeof(UCTreeListService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreeListService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeSereServ.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCTreeListService.treeSereServ.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_SendTestServiceReq.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_SendTestServiceReq.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_Edit.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_Delete.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_MediUsed.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_MediUsed.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.isRation.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.isRation.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_Number.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_Number.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_NoteAdo.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_NoteAdo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_RequestDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_RequestDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tc_TdlMedicineConcentra.Caption = Inventec.Common.Resource.Get.Value("UCTreeListService.tc_TdlMedicineConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCTreeListTracking
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovaleDebate.Resources.Lang", typeof(UCTreeListTracking).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreeListTracking.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeSereServ.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCTreeListTracking.treeSereServ.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TRACKING_TIME.Caption = Inventec.Common.Resource.Get.Value("UCTreeListTracking.TRACKING_TIME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.USER_NAME.Caption = Inventec.Common.Resource.Get.Value("UCTreeListTracking.USER_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.SERVICE.Caption = Inventec.Common.Resource.Get.Value("UCTreeListTracking.SERVICE.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CONTENT.Caption = Inventec.Common.Resource.Get.Value("UCTreeListTracking.CONTENT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




