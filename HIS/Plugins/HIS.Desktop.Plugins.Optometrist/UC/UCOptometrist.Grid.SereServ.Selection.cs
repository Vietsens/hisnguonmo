using ACS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Optometrist.ADO;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        private SereServOptometristADO GetSelectedSereServ()
        {
            try
            {
                return gridViewSereServ.GetFocusedRow() as SereServOptometristADO;
            }
            catch
            {
                return null;
            }
        }

        private void UpdateEditModeBySelectedSereServ(SereServOptometristADO selected)
        {
            try
            {
                // Chỉ cho phép lưu khi đang chọn đúng sereServ hiện tại (lần hiện tại)
                bool isCurrent = selected != null && currentsereServ != null && selected.ID == currentsereServ.ID;

                // Nếu form có flow khác để enable/disable thì ưu tiên khóa theo yêu cầu mới
                btnSave.Enabled = isCurrent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnSereServFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                var data = gridViewSereServ.GetRow(e.FocusedRowHandle) as SereServOptometristADO;
                BindSelectedSereServ(data);
                UpdateEditModeBySelectedSereServ(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindSelectedSereServ(SereServOptometristADO hisSereServ = null)
        {
            try
            {

                hisSereServ = hisSereServ ?? GetSelectedSereServ();
                if (hisSereServ == null) return;
                bool isNew = (hisSereServ.ID == this.currentsereServ.ID && (hisSereServ.HIS_SERE_SERV_VIEX == null || hisSereServ.HIS_SERE_SERV_VIEX.Count == 0));
                var hisSereServViex = hisSereServ.HIS_SERE_SERV_VIEX?
                    .OrderByDescending(o => o.ID)
                    .FirstOrDefault();
                if (hisSereServViex == null)
                {
                    hisSereServViex = new HIS_SERE_SERV_VIEX();
                }
                {
                    if (hisSereServViex.VISION_TEST_TIME.HasValue)
                    {
                        VISION_TEST_TIME.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisSereServViex.VISION_TEST_TIME.Value).Value;
                    }
                    else
                    {
                        if (isNew)
                        {
                            VISION_TEST_TIME.DateTime = DateTime.Now;
                        }
                        else
                        {
                            VISION_TEST_TIME.EditValue = null;
                        }
                    }
                    VISION_TEST_NUM.EditValue = hisSereServViex.VISION_TEST_NUM;
                    VISION_TEST_USERNAME.EditValue = hisSereServViex.VISION_TEST_LOGINNAME;
                    VISION_TEST_ROOM_NAME.EditValue = hisSereServViex.VISION_TEST_ROOM_CODE;

                    MACHINE_RIGHT_SPH.Text = hisSereServViex.MACHINE_RIGHT_SPH;
                    MACHINE_RIGHT_CYL.Text = hisSereServViex.MACHINE_RIGHT_CYL;
                    MACHINE_RIGHT_AXIS.Text = hisSereServViex.MACHINE_RIGHT_AXIS;
                    MACHINE_LEFT_SPH.Text = hisSereServViex.MACHINE_LEFT_SPH;
                    MACHINE_LEFT_CYL.Text = hisSereServViex.MACHINE_LEFT_CYL;
                    MACHINE_LEFT_AXIS.Text = hisSereServViex.MACHINE_LEFT_AXIS;

                    OLD_RIGHT_SPH.Text = hisSereServViex.OLD_RIGHT_SPH;
                    OLD_RIGHT_CYL.Text = hisSereServViex.OLD_RIGHT_CYL;
                    OLD_RIGHT_AXIS.Text = hisSereServViex.OLD_RIGHT_AXIS;
                    OLD_RIGHT_GLASS.Text = hisSereServViex.OLD_RIGHT_GLASS;
                    OLD_LEFT_SPH.Text = hisSereServViex.OLD_LEFT_SPH;
                    OLD_LEFT_CYL.Text = hisSereServViex.OLD_LEFT_CYL;
                    OLD_LEFT_AXIS.Text = hisSereServViex.OLD_LEFT_AXIS;
                    OLD_LEFT_GLASS.Text = hisSereServViex.OLD_LEFT_GLASS;

                    NOW_RIGHT_EYE.Text = hisSereServViex.NOW_RIGHT_EYE;
                    NOW_RIGHT_HOLE.Text = hisSereServViex.NOW_RIGHT_HOLE;
                    NOW_RIGHT_SPH.Text = hisSereServViex.NOW_RIGHT_SPH;
                    NOW_RIGHT_CYL.Text = hisSereServViex.NOW_RIGHT_CYL;
                    NOW_RIGHT_AXIS.Text = hisSereServViex.NOW_RIGHT_AXIS;
                    NOW_RIGHT_GLASS.Text = hisSereServViex.NOW_RIGHT_GLASS;
                    NOW_LEFT_EYE.Text = hisSereServViex.NOW_LEFT_EYE;
                    NOW_LEFT_HOLE.Text = hisSereServViex.NOW_LEFT_HOLE;
                    NOW_LEFT_SPH.Text = hisSereServViex.NOW_LEFT_SPH;
                    NOW_LEFT_CYL.Text = hisSereServViex.NOW_LEFT_CYL;
                    NOW_LEFT_AXIS.Text = hisSereServViex.NOW_LEFT_AXIS;
                    NOW_LEFT_GLASS.Text = hisSereServViex.NOW_LEFT_GLASS;

                    REAL_RIGHT_EYE.Text = hisSereServViex.REAL_RIGHT_EYE;
                    REAL_RIGHT_SPH.Text = hisSereServViex.REAL_RIGHT_SPH;
                    REAL_RIGHT_CYL.Text = hisSereServViex.REAL_RIGHT_CYL;
                    REAL_RIGHT_AXIS.Text = hisSereServViex.REAL_RIGHT_AXIS;
                    REAL_RIGHT_GLASS.Text = hisSereServViex.REAL_RIGHT_GLASS;
                    REAL_LEFT_EYE.Text = hisSereServViex.REAL_LEFT_EYE;
                    REAL_LEFT_SPH.Text = hisSereServViex.REAL_LEFT_SPH;
                    REAL_LEFT_CYL.Text = hisSereServViex.REAL_LEFT_CYL;
                    REAL_LEFT_AXIS.Text = hisSereServViex.REAL_LEFT_AXIS;
                    REAL_LEFT_GLASS.Text = hisSereServViex.REAL_LEFT_GLASS;

                    RIGHT_EYE_BEFORE.Text = hisSereServViex.RIGHT_EYE_BEFORE;
                    LEFT_EYE_BEFORE.Text = hisSereServViex.LEFT_EYE_BEFORE;
                    RIGHT_EYE_AFTER.Text = hisSereServViex.RIGHT_EYE_AFTER;
                    LEFT_EYE_AFTER.Text = hisSereServViex.LEFT_EYE_AFTER;
                    RIGHT_EYE_PRESSURE.Text = hisSereServViex.RIGHT_EYE_PRESSURE;
                    LEFT_EYE_PRESSURE.Text = hisSereServViex.LEFT_EYE_PRESSURE;
                    RIGHT_EYE_THICKNESS.Text = hisSereServViex.RIGHT_EYE_THICKNESS;
                    LEFT_EYE_THICKNESS.Text = hisSereServViex.LEFT_EYE_THICKNESS;
                    RIGHT_EYE_K1.Text = hisSereServViex.RIGHT_EYE_K1;
                    LEFT_EYE_K1.Text = hisSereServViex.LEFT_EYE_K1;
                    RIGHT_EYE_K2.Text = hisSereServViex.RIGHT_EYE_K2;
                    LEFT_EYE_K2.Text = hisSereServViex.LEFT_EYE_K2;

                    RIGHT_EYE_MOVE.Text = hisSereServViex.RIGHT_EYE_MOVE;
                    LEFT_EYE_MOVE.Text = hisSereServViex.LEFT_EYE_MOVE;
                    RIGHT_EYE_SENSE.Text = hisSereServViex.RIGHT_EYE_SENSE;
                    LEFT_EYE_SENSE.Text = hisSereServViex.LEFT_EYE_SENSE;
                    RIGHT_EYE_3D.Text = hisSereServViex.RIGHT_EYE_3D;
                    LEFT_EYE_3D.Text = hisSereServViex.LEFT_EYE_3D;
                    RIGHT_EYE_DEVIATION.Text = hisSereServViex.RIGHT_EYE_DEVIATION;
                    LEFT_EYE_DEVIATION.Text = hisSereServViex.LEFT_EYE_DEVIATION;
                    RIGHT_EYE_BALL.Text = hisSereServViex.RIGHT_EYE_BALL;
                    LEFT_EYE_BALL.Text = hisSereServViex.LEFT_EYE_BALL;

                    FAR_RIGHT_SPH.Text = hisSereServViex.FAR_RIGHT_SPH;
                    FAR_RIGHT_CYL.Text = hisSereServViex.FAR_RIGHT_CYL;
                    FAR_RIGHT_AXIS.Text = hisSereServViex.FAR_RIGHT_AXIS;
                    FAR_RIGHT_VISION.Text = hisSereServViex.FAR_RIGHT_VISION;
                    FAR_LEFT_SPH.Text = hisSereServViex.FAR_LEFT_SPH;
                    FAR_LEFT_CYL.Text = hisSereServViex.FAR_LEFT_CYL;
                    FAR_LEFT_AXIS.Text = hisSereServViex.FAR_LEFT_AXIS;
                    FAR_LEFT_VISION.Text = hisSereServViex.FAR_LEFT_VISION;
                    FAR_ADD.Text = hisSereServViex.FAR_ADD;

                    NEAR_RIGHT_SPH.Text = hisSereServViex.NEAR_RIGHT_SPH;
                    NEAR_RIGHT_CYL.Text = hisSereServViex.NEAR_RIGHT_CYL;
                    NEAR_RIGHT_AXIS.Text = hisSereServViex.NEAR_RIGHT_AXIS;
                    NEAR_RIGHT_VISION.Text = hisSereServViex.NEAR_RIGHT_VISION;
                    NEAR_LEFT_SPH.Text = hisSereServViex.NEAR_LEFT_SPH;
                    NEAR_LEFT_CYL.Text = hisSereServViex.NEAR_LEFT_CYL;
                    NEAR_LEFT_AXIS.Text = hisSereServViex.NEAR_LEFT_AXIS;
                    NEAR_LEFT_VISION.Text = hisSereServViex.NEAR_LEFT_VISION;

                    FAR_BP.Text = hisSereServViex.FAR_BP;
                    NEAR_BP.Text = hisSereServViex.NEAR_BP;

                    IS_BIFOCAL_GLASS.Checked = hisSereServViex.IS_BIFOCAL_GLASS == 1;
                    IS_PROGRESSIVE_GLASS.Checked = hisSereServViex.IS_PROGRESSIVE_GLASS == 1;
                    IS_READING_GLASS.Checked = hisSereServViex.IS_READING_GLASS == 1;
                    IS_PHOTOCHROMIC_GLASS.Checked = hisSereServViex.IS_PHOTOCHROMIC_GLASS == 1;
                    IS_POLYCARBONATE_GLASS.Checked = hisSereServViex.IS_POLYCARBONATE_GLASS == 1;
                    IS_CONTACT_LENSE.Checked = hisSereServViex.IS_CONTACT_LENSE == 1;

                    VISION_EXAM_NOTE.Text = hisSereServViex.VISION_EXAM_NOTE;
                    GLASS_STATUS.Text = hisSereServViex.GLASS_STATUS;

                    if (hisSereServViex.GLASS_USE_TIME.HasValue)
                    {
                        GLASS_USE_TIME.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisSereServViex.GLASS_USE_TIME.Value).Value;
                    }
                    else
                    {
                        if (isNew)
                        {
                            GLASS_USE_TIME.DateTime = DateTime.Now;
                        }
                        else
                        {
                            GLASS_USE_TIME.EditValue = null;
                        }
                    }
                    GLASS_AMOUNT.EditValue = hisSereServViex.GLASS_AMOUNT;
                    IS_GLASS_SCRATCHED.Checked = hisSereServViex.IS_GLASS_SCRATCHED == 1;

                    if (hisSereServViex.MEDI_USE_TIME.HasValue)
                    {
                        MEDI_USE_TIME.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisSereServViex.MEDI_USE_TIME.Value).Value;
                    }
                    else
                    {
                        if (isNew)
                        {
                            MEDI_USE_TIME.DateTime = DateTime.Now;
                        }
                        else
                        {
                            MEDI_USE_TIME.EditValue = null;
                        }
                    }
                }
                if (VISION_TEST_USERNAME.EditValue == null && isNew)
                {
                    string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == loginname);
                    if (user != null)
                    {
                        VISION_TEST_USERNAME.EditValue = user.LOGINNAME;
                    }
                }

                // cập nhật trạng thái cho phép sửa/lưu theo lần khám đang chọn
                UpdateEditModeBySelectedSereServ(hisSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}