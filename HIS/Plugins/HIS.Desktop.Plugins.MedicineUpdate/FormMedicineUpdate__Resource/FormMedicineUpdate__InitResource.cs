        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicineUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineUpdate.FormMedicineUpdate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonSave.Caption = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.barButtonSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMedicineIsStarMark.Properties.Caption = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.chkMedicineIsStarMark.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciImpVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackgeNumber.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciPackgeNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineBytNumOrder.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciMedicineBytNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineIsStarMark.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciMedicineIsStarMark.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineRegisterNumOrder.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciMedicineRegisterNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMedicineTcyNumOrder.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciMedicineTcyNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSTTBid.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciSTTBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGroupBid.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciGroupBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackBid.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.lciPackBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormMedicineUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
