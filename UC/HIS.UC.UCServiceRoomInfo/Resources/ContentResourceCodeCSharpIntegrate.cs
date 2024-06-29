
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCServiceRoomInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCServiceRoomInfo.Resources.Lang", typeof(UCServiceRoomInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCServiceRoomInfo.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lcUCServiceRoomInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboPatientTypePrimary.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.CboPatientTypePrimary.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCServiceRoomInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCServiceRoomInfo.Resources.Lang", typeof(UCServiceRoomInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCServiceRoomInfo.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lcUCServiceRoomInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboPatientTypePrimary.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.CboPatientTypePrimary.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientType.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboPatientTypePhuThu.Text = Inventec.Common.Resource.Get.Value("UCServiceRoomInfo.lciCboPatientTypePhuThu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




