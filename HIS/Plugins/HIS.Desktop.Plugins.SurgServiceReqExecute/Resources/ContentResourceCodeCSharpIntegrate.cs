
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmEkipTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(frmEkipTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDepartment.ToolTip = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkDepartment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublicDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkPublicDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(Resources).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormEkipUser
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormEkipUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEkipTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("FormEkipUser.cboEkipTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLU_User.NullText = Inventec.Common.Resource.Get.Value("FormEkipUser.GridLU_User.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("FormEkipUser.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormEkipUser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormPtttMethod
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormPtttMethod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSearch.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.barBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_icon.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_icon.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_MethodCode.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_MethodCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_MethodName.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_MethodName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PTTT.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_PTTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PTTT.ToolTip = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_PTTT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_amount.Caption = Inventec.Common.Resource.Get.Value("FormPtttMethod.gc_amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit_PTTT.NullText = Inventec.Common.Resource.Get.Value("FormPtttMethod.repositoryItemGridLookUpEdit_PTTT.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLU_PTTT.NullText = Inventec.Common.Resource.Get.Value("FormPtttMethod.GridLU_PTTT.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.btnChoose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormPtttMethod.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormPtttMethod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormPtttTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormPtttTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublicDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublicDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPtttTempCode.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPtttTempName.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormImageTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormImageTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormImageTemp.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSearch.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.barBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormViewImage
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(FormViewImage).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormViewImage.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormViewImage.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileControl.Text = Inventec.Common.Resource.Get.Value("FormViewImage.tileControl.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.PictureEdit.Properties.NullText = Inventec.Common.Resource.Get.Value("FormViewImage.PictureEdit.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormViewImage.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmInputDetail
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(frmInputDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTraMatMo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTraMatMo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit10.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit10.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTraMatDD.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTraMatDD.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTiemMatCNC.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatCNC.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTiemMatDuoiKM.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatDuoiKM.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTiemMatTienPhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatTienPhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTiemMatCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTiemMatKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTiemMatKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPhucHoiVetMoKhauVat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhucHoiVetMoKhauVat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPhucHoiVetMoBomPhu.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhucHoiVetMoBomPhu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCMMNVCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCMMNVCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCBSCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCMMNVKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCMMNVKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCBSMayCatDK.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSMayCatDK.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCBSKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRBSCBSCatBangKeo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRBSCBSCatBangKeo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRachBaoSauCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRachBaoSauCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raRachBaoSauKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raRachBaoSauKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDatIOLBangSung.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLBangSung.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDatIOLCung.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLCung.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDatIOLCoDinhCM.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLCoDinhCM.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDatIOLBangPince.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLBangPince.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDatIOLMem.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDatIOLMem.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raHutChatT3IA.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raHutChatT3IA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raHutChatT3Kim2Nong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raHutChatT3Kim2Nong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDayNhanBangChatNhay.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanBangChatNhay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDayNhanBangNuoc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanBangNuoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDayNhanCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raXoayNhanKhoKhan.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXoayNhanKhoKhan.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDayNhanKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDayNhanKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit6.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit6.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raXoayNhanDeDang.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXoayNhanDeDang.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raCatBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raCatBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raXMoBaoHinhTemThu.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXMoBaoHinhTemThu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raXeBaoTruocT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raXeBaoTruocT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raNhuomBaoCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNhuomBaoCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raNhuomBaoKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNhuomBaoKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit12.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit12.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMVTPCungMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPCungMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMVTPRia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPRia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMVTPGiacMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMVTPGiacMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMoKMRiaCo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMoKMRiaCo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMoKMRiaKhong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMoKMRiaKhong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raLechTTT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raLechTTT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDoIII.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoIII.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDoV.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoV.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDoIV.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoIV.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDoII.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoII.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raChiCoTruc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raChiCoTruc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raVanhMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raVanhMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTeTaiMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTeTaiMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTrongBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTrongBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPhaco.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPhaco.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTreoCungMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTreoCungMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raNgoaiBao.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raNgoaiBao.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raMP.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raMP.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDucT3BenhLy.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3BenhLy.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDucT3BamSinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3BamSinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raKhongCoT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raKhongCoT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raLechT3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raLechT3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDucT3Gia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDucT3Gia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raDoI.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raDoI.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem52.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem52.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem59.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem64.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem64.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem69.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem69.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem51.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem51.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem53.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem53.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem72.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem72.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem74.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem74.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem79.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem73.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem73.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem75.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem67.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem67.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem80.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem80.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem82.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem82.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem84.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem84.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem81.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem81.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem89.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem89.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem92.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem92.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem93.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem93.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTTMongMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTTMongMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPTMong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTMong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPagePTDucTTT.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPagePTDucTTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPageGlocom.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPageGlocom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl33.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit1.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit2.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit2.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit7.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit7.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit8.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit8.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit9.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit9.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl32.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit4.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit4.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit5.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit5.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit11.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit11.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit13.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit13.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl11.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit15.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit15.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit18.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit18.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit20.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit20.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit21.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit21.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit22.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit22.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit23.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit23.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit24.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit24.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit25.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit25.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit26.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit26.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit27.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit27.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit28.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit28.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit29.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit29.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit30.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit30.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit31.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit31.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit32.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit32.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit33.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit33.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit34.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit34.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit35.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit35.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit36.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit36.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit37.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit37.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit38.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit38.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit39.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit39.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl15.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl16.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit40.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit40.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit41.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit41.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit42.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit42.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit43.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit43.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit44.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit44.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauDayVungRia.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauDayVungRia.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauDayCungDo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauDayCungDo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonGay.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonGay.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamCNC.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamCNC.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauChiGiacMac.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauChiGiacMac.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonSo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonSo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamViTriBaoTenon.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamViTriBaoTenon.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauCoTruc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauCoTruc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raCoDinhNhanCauVanhMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raCoDinhNhanCauVanhMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl17.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamTeTaiMat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamTeTaiMat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raPPVoCamGayMe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raPPVoCamGayMe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhGanTuyet.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhGanTuyet.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTienTrien.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTienTrien.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhSoPhat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhSoPhat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTuyetDoi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTuyetDoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTranTrong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTranTrong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGiaDoanBenhTiemTang.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGiaDoanBenhTiemTang.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGloocomGocMo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomGocMo.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGloocomThuPhat.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomThuPhat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGloocomBamSinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomBamSinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raGloocomGocDong.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raGloocomGocDong.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Glocom__raTinhTrangBaoTenonBT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.Glocom__raTinhTrangBaoTenonBT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl18.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem104.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem104.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem109.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem109.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem115.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem115.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem117.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem117.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem119.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem119.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem122.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem122.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem128.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem128.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem132.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem132.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem135.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem135.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem136.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem136.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem143.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem143.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem144.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem144.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem145.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem145.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem153.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem153.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem169.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem169.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem187.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem187.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem121.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem121.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem130.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem130.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem141.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem141.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem157.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem157.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem138.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem138.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem152.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem152.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem158.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem158.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem146.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem146.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem168.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem168.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem170.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem170.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem162.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem162.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem174.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem174.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem175.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem175.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem182.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem182.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem185.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem185.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem281.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem281.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem96.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem96.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem285.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem285.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPagePTMong.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.tabPagePTMong.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit72.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit72.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit108.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit108.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit71.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit71.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit70.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit70.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit69.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit69.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit3.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit93.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit93.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit96.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit96.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit97.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit97.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit100.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit100.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit101.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit101.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl26.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit103.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit103.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit104.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit104.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit106.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit106.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit107.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit107.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit109.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit109.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit110.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit110.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit112.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit112.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit113.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit113.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit115.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit115.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit64.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit64.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit68.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit68.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem184.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem184.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem209.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem209.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem106.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem106.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem230.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem230.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem228.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem228.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem202.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem202.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem159.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem159.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem176.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem176.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem225.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem225.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem147.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem147.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem148.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem148.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem233.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem233.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem177.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem177.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem226.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem226.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem199.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem199.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem173.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem173.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem212.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem212.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem244.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem244.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem245.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem245.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem248.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem248.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPagePTSupMi.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPagePTSupMi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl53.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl53.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl19.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl21.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit80.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit80.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit83.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit83.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit84.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit84.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit85.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit85.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit86.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit86.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit87.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit87.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit88.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit88.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit89.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit89.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit90.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit90.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit91.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit91.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit92.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit92.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit95.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit95.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit98.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit98.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit99.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit99.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit102.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit102.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit105.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit105.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit111.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit111.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl22.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl23.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl24.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit114.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit114.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit117.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit117.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit118.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit118.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit119.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit119.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit120.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit120.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit122.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit122.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit123.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit123.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit124.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit124.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit125.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit125.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit126.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit126.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit127.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit127.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit128.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit128.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl25.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit129.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit129.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit130.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit130.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit131.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit131.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit132.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit132.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit133.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit133.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit135.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit135.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit136.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit136.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit137.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit137.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit138.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit138.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit139.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit139.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit140.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit140.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit141.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit141.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit142.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit142.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl27.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl52.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl52.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem180.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem180.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem201.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem201.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem211.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem211.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem214.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem214.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem218.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem218.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem219.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem219.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem237.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem237.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem250.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem250.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem254.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem254.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem262.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem262.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem573.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem573.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem216.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem216.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem235.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem235.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem242.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem242.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem247.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem247.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem260.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem260.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem261.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem261.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem270.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem270.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem253.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem253.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem271.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem271.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem275.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem275.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem287.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem287.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageTaiTaoLeQuan.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTaiTaoLeQuan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl16.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit144.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit144.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit171.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit171.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit173.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit173.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit174.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit174.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit175.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit175.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit176.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit176.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit177.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit177.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit178.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit178.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit179.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit179.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit180.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit180.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit181.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit181.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit182.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit182.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit183.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit183.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit184.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit184.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit185.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit185.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit186.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit186.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit187.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit187.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit188.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit188.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit189.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit189.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit190.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit190.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit191.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit191.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit192.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit192.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl34.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit193.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit193.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit194.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit194.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit197.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit197.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit200.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit200.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit201.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit201.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit204.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit204.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit205.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit205.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit206.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit206.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem306.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem306.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem311.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem311.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem317.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem317.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem321.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem321.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem323.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem323.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem325.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem325.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem332.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem332.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem319.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem319.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem340.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem340.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem330.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem330.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem334.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem334.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem331.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem331.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem335.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem335.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem342.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem342.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem327.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem327.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem337.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem337.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem343.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem343.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem354.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem354.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem324.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem324.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem355.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem355.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem364.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem364.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem359.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem359.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem387.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem387.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl17.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageTTMongMat.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTTMongMat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl18.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl19.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl28.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl37.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl38.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl39.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl40.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl41.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit255.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit255.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit256.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit256.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl42.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit257.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit257.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit258.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit258.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit261.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit261.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit264.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit264.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit266.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit266.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit267.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit267.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit268.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit268.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit269.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit269.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem395.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem395.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem400.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem400.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem406.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem406.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem410.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem410.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem421.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem421.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem426.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem426.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl20.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TabPageTTTLaser.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.TabPageTTTLaser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl21.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl22.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl31.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl29.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl30.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit319.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit319.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit320.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit320.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl50.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl50.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit321.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit321.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit322.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit322.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit325.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit325.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit328.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit328.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit329.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit329.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit333.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.checkEdit333.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem484.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem484.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem489.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem489.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem495.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem495.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem499.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem499.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem518.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem518.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl23.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPTDucThuyTInhThe.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTDucThuyTInhThe.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raTTLaser.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raTTLaser.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPTGlocom.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTGlocom.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPTSupMi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTSupMi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.raPTTaiTaoLeQuan.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.raPTTaiTaoLeQuan.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabEye.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabEye.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabSkin.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl25.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl44.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl43.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDiscloseSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDiscloseSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCloseSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkCloseSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkShapingSkinOther.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkShapingSkinOther.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkShapingSkin.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkShapingSkin.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDamageTypePart.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDamageTypePart.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDamageTypeAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkDamageTypeAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl36.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl35.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPostureTummy.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureTummy.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPostureSide.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureSide.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPostureUp.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInputDetail.chkPostureUp.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPosture.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciPosture.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSkinDamage.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamage.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSkinDamagePosition.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamagePosition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSkinDamageAmount.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciSkinDamageAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDamageType.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciDamageType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciShapingSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciShapingSkin.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCloseSkin.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciCloseSkin.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatSkinOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.lciTreatSkinOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton2.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.simpleButton2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.xtraTabOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl_TabOther.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControl_TabOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem286.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem286.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem289.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem289.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem291.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem291.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem293.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem293.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem295.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem295.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem295.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.layoutControlItem295.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInputDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện SurgServiceReqExecuteControl
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(SurgServiceReqExecuteControl).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.tileViewColumn2.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileViewColumnName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileViewColumnSTTImage.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumnSTTImage.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlLeft.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCoppyInfo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCoppyInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCoppyInfo.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCoppyInfo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkServiceCode.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkServiceCode.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSavePtttTemp.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSavePtttTemp.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPtttTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPtttTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dropDownButtonGPBL.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dropDownButtonGPBL.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.dropDownButtonGPBL.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnTuTruc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssignBlood.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAssignBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceCode_InEkip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColServiceCode_InEkip.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColServiceName_InEKip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColServiceName_InEKip.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColAmount.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColUnit_InEkip.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColUnit_InEkip.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSereServAttachServiceCode.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSereServAttachServiceName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSereServAttachAmount.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSereServAttachUnit.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColSereServAttachUnit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSTT.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceCode.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceName.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColUnit.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColUnit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumber.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColObjectPay.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.grdColObjectPay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMaty.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumnMaty.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnAssignPre.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddbPhatSinh.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.ddbPhatSinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddbPhatSinh.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.ddbPhatSinh.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlGroup2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPtttTemp.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPtttTemp.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPtttTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPtttTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem55.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem55.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOther.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlRight.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKetThuc.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkKetThuc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoosePtttMethods.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnChoosePtttMethods.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImagePublic.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnImagePublic.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateImageLuuDo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnCreateImageLuuDo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMoTa.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageMoTa.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageGhiChu.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageGhiChu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageLuocDo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.xtraTabPageLuocDo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tileViewItemElement1.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tileViewItemElement2.Text = Inventec.Common.Resource.Get.Value("tileViewItemElement2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileViewIsChecked.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewIsChecked.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tileViewColumn4.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.tileViewColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Checked.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.Checked.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImage.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumnImage.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdCmName.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcdCmName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIcdCm.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcdCm.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSaveGroup.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkSaveGroup.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChonMauPTCHuyenKhoaMat.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnChonMauPTCHuyenKhoaMat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcd3.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd3.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIcd3.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd3.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcd2.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd2.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIcd2.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd2.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcd1.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboIcd1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIcd1.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.chkIcd1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPhuongPhapThucTe.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPhuongPhapThucTe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBanMo.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboBanMo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMachine.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMachine.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMoKTCao.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMoKTCao.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKQVoCam.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboKQVoCam.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPhuongPhap2.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboPhuongPhap2.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoaiPT.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboLoaiPT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKTDT.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnKTDT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveEkipTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSaveEkipTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEkipTemp.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboEkipTemp.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSwapService.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSwapService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdExtraCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.txtIcdExtraCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepartmentTran.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnDepartmentTran.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFinish.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.btnFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDeathSurg.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboDeathSurg.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCatastrophe.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboCatastrophe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCondition.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cboCondition.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbBloodRh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbEmotionlessMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbEmotionlessMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbPtttGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.cbbPtttGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKetLuan.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKetLuan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTinhTrang.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTinhTrang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCachThuc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciCachThuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChuanDoanPhu.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChuanDoanPhu.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChuanDoanPhu.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChuanDoanPhu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEkipTemp.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciEkipTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhanLoai.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPhanLoai.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem21.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhuongPhap.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciPhuongPhap.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVoCam.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciVoCam.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBanMo.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciBanMo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem17.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMachine.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciMachine.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMachine.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciMachine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTaiBien.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTaiBien.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTuVongTrong.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciTuVongTrong.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd1.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcd3.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcd3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThoiGianKetThuc.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciThoiGianKetThuc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkGroup.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciChkGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdCmCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdCmCode.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdCmSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmSubCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdCmSubCode.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciIcdCmSubCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem42.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartment.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNhomMau.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciNhomMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRh.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciRh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKTC.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKTC.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKTC.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.lciKTC.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem56.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem56.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem58.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("SurgServiceReqExecuteControl.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }





        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCEkipUser
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgServiceReqExecute.Resources.Lang", typeof(UCEkipUser).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPosition.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.cboPosition.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLookupEdit_UserName.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookupEdit_UserName.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLookUpEdit_Department.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.GridLookUpEdit_Department.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCEkipUser.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditUsername.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemGridLookUpEditUsername.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemSearchLookUpEdit1.NullText = Inventec.Common.Resource.Get.Value("UCEkipUser.repositoryItemSearchLookUpEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInformationSurg.Text = Inventec.Common.Resource.Get.Value("UCEkipUser.lciInformationSurg.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




