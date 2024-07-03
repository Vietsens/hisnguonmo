        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientTypeAlter.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientTypeAlter.frmSwapPatientTypeAlter__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblUpdatePatientType.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.lblUpdatePatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUpdatePatientType.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.btnUpdatePatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNote.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.lblNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChooSereServ.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.btnChooSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.gridColPatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit.NullText = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.repositoryItemGridLookUpEdit.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSwapPatientTypeAlter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
