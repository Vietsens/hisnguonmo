using DevExpress.XtraEditors;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Copy Baby Data to Child Tab

        private void CopyBabyDataToChildTab(HIS_BABY baby)
        {
            try
            {
                if (baby == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Baby data is null");
                    return;
                }
                _child = new MCH_CHILD();

                _child.CHILD_NAME = baby.BABY_NAME;
                if (baby.GENDER_ID.HasValue)
                {
                    _child.CHILD_GENDER = baby.GENDER_ID.Value.ToString();
                }

                if (baby.WEIGHT.HasValue)
                {
                    _child.WEIGHT = baby.WEIGHT.Value.ToString();
                }

                if (baby.HEIGHT.HasValue)
                {
                    _child.HEIGHT = baby.HEIGHT.Value.ToString();
                }

                if (baby.HEAD.HasValue)
                {
                    _child.HEAD_CIRCUM = baby.HEAD.Value.ToString();
                }

                _child.DELIVERY_ASSISTANT = baby.MIDWIFE;
                _child.ETHNIC_CODE = baby.ETHNIC_CODE;
                _child.ETHNIC_NAME = baby.ETHNIC_NAME;
                _child.TEMPORARY_HEIN_CARD_NUMBER = baby.HEIN_CARD_NUMBER_TMP;

                if (baby.BIRTH_CERT_NUM.HasValue)
                {
                    _child.BIRTH_CERTIFICATE_CODE = baby.BIRTH_CERT_NUM.Value.ToString();
                }

                _child.CHILD_BIRTH_DATE = baby.BORN_TIME;

                if (baby.BORN_RESULT_ID == 1)
                {
                    
                    _child.LIVE_BIRTH = "0";
                    _child.IS_DEATH = 0;
                }
                else if (baby.BORN_RESULT_ID == 2)
                {
                    _child.LIVE_BIRTH = "1";
                    _child.IS_DEATH = 1;
                }

                if (baby.IS_INJECT_K1.HasValue && baby.IS_INJECT_K1.Value == 1)
                {
                    _child.VITAMIN_K1 = "1";
                }

                if (baby.IS_INJECT_B.HasValue && baby.IS_INJECT_B.Value == 1)
                {
                    _child.HEPB_VACCINE = "1";
                }

                _child.BIRTH_PROVINCE_CODE = baby.BIRTH_PROVINCE_CODE;
                _child.BIRTH_PROVINCE_NAME = baby.BIRTH_PROVINCE_NAME;
                _child.BIRTH_DISTRICT_CODE = baby.BIRTH_DISTRICT_CODE;
                _child.BIRTH_DISTRICT_NAME = baby.BIRTH_DISTRICT_NAME;
                _child.BIRTH_COMMUNE_CODE = baby.BIRTH_COMMUNE_CODE;
                _child.BIRTH_COMMUNE_NAME = baby.BIRTH_COMMUNE_NAME;
                _child.BIRTH_ADDRESS = baby.BIRTHPLACE;

                if (baby.BIRTH_CERT_NUM.HasValue && baby.BIRTH_CERT_NUM.Value > 0)
                {
                    _child.HAS_BIRTH_CERTIFICATE = "1";
                    _child.BIRTH_CERTIFICATE_DATE = baby.ISSUED_DATE;

                    if (baby.IS_REISSUED.HasValue && baby.IS_REISSUED.Value == 1)
                    {
                        _child.BIRTH_CERTIFICATE_ROUND = "1";
                    }
                    else
                    {
                        _child.BIRTH_CERTIFICATE_ROUND = "0";
                    }
                }

                if (baby.POSTPARTUM_CARE.HasValue)
                {
                    if (baby.POSTPARTUM_CARE.Value == 1)
                    {
                        _child.CARE_WEEK_1 = "1";
                    }
                    else if (baby.POSTPARTUM_CARE.Value == 2)
                    {
                        _child.CARE_WEEK_2_TO_6 = "1";
                    }
                }

                if (xtraTabControl1 != null && xtraTabControl1.TabPages.Count > 2)
                {
                    xtraTabControl1.SelectedTabPageIndex = 2;
                    xtraTabControl2.SelectedTabPageIndex = 1;
                }

                FillDataToTab3Child();

                XtraMessageBox.Show(
                    "Đã copy thông tin trẻ sơ sinh vào tab Sinh đẻ - Con.\nVui lòng kiểm tra và bổ sung các thông tin cần thiết.",
                    "Thông báo",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show(
                    "Có lỗi xảy ra khi copy dữ liệu: " + ex.Message,
                    "Lỗi",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
