using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {


        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Optometrist.Resources.Lang", typeof(HIS.Desktop.Plugins.Optometrist.UC.UCOptometrist).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.VISION_TEST_ROOM_NAME.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOptometrist.VISION_TEST_ROOM_NAME.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_GLASS_SCRATCHED.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_GLASS_SCRATCHED.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_CONTACT_LENSE.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_CONTACT_LENSE.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_POLYCARBONATE_GLASS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_POLYCARBONATE_GLASS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_READING_GLASS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_READING_GLASS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_PROGRESSIVE_GLASS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_PROGRESSIVE_GLASS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_BIFOCAL_GLASS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_BIFOCAL_GLASS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_PHOTOCHROMIC_GLASS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.IS_PHOTOCHROMIC_GLASS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label63.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label63.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Phong.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_Phong.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.Gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label1.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label49.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label49.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label50.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label50.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label51.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label51.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label52.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label52.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label53.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label53.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label54.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label54.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label62.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label62.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label55.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label55.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label56.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label56.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label57.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label57.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label58.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label59.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label59.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label60.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label60.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label61.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label61.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label20.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label21.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label22.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label23.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label24.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label25.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label8.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label9.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label10.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label11.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label12.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label31.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label13.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label4.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label5.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label6.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label7.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label3.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label37.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label2.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label38.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label39.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label14.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label15.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label16.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label17.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label18.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label41.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label19.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label40.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label26.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label27.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label29.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label30.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label43.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label44.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label45.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label46.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label32.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label33.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label35.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label36.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label42.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label47.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label48.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.label48.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.VISION_TEST_USERNAME.Properties.NullText = Inventec.Common.Resource.Get.Value("UCOptometrist.VISION_TEST_USERNAME.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOptometristPrintKham.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkOptometristPrintKham.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOptometristPrintDon.Properties.Caption = Inventec.Common.Resource.Get.Value("UCOptometrist.chkOptometristPrintDon.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintPhieuKham.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnPrintPhieuKham.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCOptometrist.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}