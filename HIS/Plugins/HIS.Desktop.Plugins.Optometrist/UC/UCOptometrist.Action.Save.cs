using HIS.Desktop.Plugins.Optometrist.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        public void OptometristSave()
        {
            btnSave_Click(null, null);
        }
        internal void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;

                if (!dxValidationProvider1.Validate()) return;

                CommonParam param = new CommonParam();
                bool success = false;

                HIS_SERE_SERV_VIEX sdo = new HIS_SERE_SERV_VIEX();
                BuildVisionExamSdo(ref sdo);
                var requestUri = ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_UPDATE;
                if (sdo.ID == 0)
                {
                    requestUri = ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_VIEX_CREATE;
                }
                var result = new BackendAdapter(param).Post<HIS_SERE_SERV_VIEX>(requestUri, ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                if (result != null)
                {
                    var selected = GetSelectedSereServ();
                    if (selected != null)
                    {
                        if (selected.HIS_SERE_SERV_VIEX == null)
                        {
                            selected.HIS_SERE_SERV_VIEX = new List<HIS_SERE_SERV_VIEX>();
                        }
                        else
                        {
                            selected.HIS_SERE_SERV_VIEX.Clear();
                        }
                        selected.HIS_SERE_SERV_VIEX.Add(result);
                        selected.VISION_TEST_TIME = result.VISION_TEST_TIME;
                        selected.VISION_TEST_ROOM_NAME = result.VISION_TEST_ROOM_NAME;
                        selected.VISION_TEST_NUM = result.VISION_TEST_NUM;
                        gridViewSereServ.UpdateCurrentRow();
                    }
                    success = true;

                    if (chkOptometristPrintKham.Checked)
                    {
                        btnPrintPhieuKham_Click(null, null);
                    }

                    if (chkOptometristPrintDon.Checked)
                    {
                        btnPrint_Click(null, null);
                    }

                    btnSave.Enabled = false;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildVisionExamSdo(ref HIS_SERE_SERV_VIEX sdo)
        {
            try
            {
                if (sdo == null) sdo = new HIS_SERE_SERV_VIEX();
                sdo.SERE_SERV_ID = currentsereServ.ID;
                sdo.TDL_TREATMENT_ID = currentsereServ.TDL_TREATMENT_ID;
                var ss6 = gridViewSereServ.GetFocusedRow() as SereServOptometristADO;

                if (ss6.HIS_SERE_SERV_VIEX.Count > 0)
                {
                    sdo.ID = ss6.HIS_SERE_SERV_VIEX.FirstOrDefault().ID;
                }

                // Thông tin chung
                if (VISION_TEST_TIME.EditValue != null)
                {
                    sdo.VISION_TEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(VISION_TEST_TIME.DateTime);
                }
                sdo.VISION_TEST_NUM = VISION_TEST_NUM.Text != "" ? Convert.ToInt64(VISION_TEST_NUM.EditValue) : (long?)null;
                sdo.VISION_TEST_LOGINNAME = VISION_TEST_USERNAME.EditValue != null ? VISION_TEST_USERNAME.EditValue.ToString() : null;
                sdo.VISION_TEST_USERNAME = VISION_TEST_USERNAME.Text;
                sdo.VISION_TEST_ROOM_CODE = VISION_TEST_ROOM_NAME.EditValue != null ? VISION_TEST_ROOM_NAME.EditValue.ToString() : null;
                sdo.VISION_TEST_ROOM_NAME = VISION_TEST_ROOM_NAME.Text;

                // I. Kết quả khám khúc xạ
                // Khúc xạ máy (MACHINE)
                sdo.MACHINE_RIGHT_SPH = MACHINE_RIGHT_SPH.Text;
                sdo.MACHINE_RIGHT_CYL = MACHINE_RIGHT_CYL.Text;
                sdo.MACHINE_RIGHT_AXIS = MACHINE_RIGHT_AXIS.Text;
                sdo.MACHINE_LEFT_SPH = MACHINE_LEFT_SPH.Text;
                sdo.MACHINE_LEFT_CYL = MACHINE_LEFT_CYL.Text;
                sdo.MACHINE_LEFT_AXIS = MACHINE_LEFT_AXIS.Text;

                // Kính cũ (OLD)
                sdo.OLD_RIGHT_SPH = OLD_RIGHT_SPH.Text;
                sdo.OLD_RIGHT_CYL = OLD_RIGHT_CYL.Text;
                sdo.OLD_RIGHT_AXIS = OLD_RIGHT_AXIS.Text;
                sdo.OLD_RIGHT_GLASS = OLD_RIGHT_GLASS.Text;
                sdo.OLD_LEFT_SPH = OLD_LEFT_SPH.Text;
                sdo.OLD_LEFT_CYL = OLD_LEFT_CYL.Text;
                sdo.OLD_LEFT_AXIS = OLD_LEFT_AXIS.Text;
                sdo.OLD_LEFT_GLASS = OLD_LEFT_GLASS.Text;

                // Khúc xạ hiện tại (NOW)
                sdo.NOW_RIGHT_EYE = NOW_RIGHT_EYE.Text;
                sdo.NOW_RIGHT_HOLE = NOW_RIGHT_HOLE.Text;
                sdo.NOW_RIGHT_SPH = NOW_RIGHT_SPH.Text;
                sdo.NOW_RIGHT_CYL = NOW_RIGHT_CYL.Text;
                sdo.NOW_RIGHT_AXIS = NOW_RIGHT_AXIS.Text;
                sdo.NOW_RIGHT_GLASS = NOW_RIGHT_GLASS.Text;
                sdo.NOW_LEFT_EYE = NOW_LEFT_EYE.Text;
                sdo.NOW_LEFT_HOLE = NOW_LEFT_HOLE.Text;
                sdo.NOW_LEFT_SPH = NOW_LEFT_SPH.Text;
                sdo.NOW_LEFT_CYL = NOW_LEFT_CYL.Text;
                sdo.NOW_LEFT_AXIS = NOW_LEFT_AXIS.Text;
                sdo.NOW_LEFT_GLASS = NOW_LEFT_GLASS.Text;

                // Khúc xạ sau liệt điều tiết (REAL)
                sdo.REAL_RIGHT_EYE = REAL_RIGHT_EYE.Text;
                sdo.REAL_RIGHT_SPH = REAL_RIGHT_SPH.Text;
                sdo.REAL_RIGHT_CYL = REAL_RIGHT_CYL.Text;
                sdo.REAL_RIGHT_AXIS = REAL_RIGHT_AXIS.Text;
                sdo.REAL_RIGHT_GLASS = REAL_RIGHT_GLASS.Text;
                sdo.REAL_LEFT_EYE = REAL_LEFT_EYE.Text;
                sdo.REAL_LEFT_SPH = REAL_LEFT_SPH.Text;
                sdo.REAL_LEFT_CYL = REAL_LEFT_CYL.Text;
                sdo.REAL_LEFT_AXIS = REAL_LEFT_AXIS.Text;
                sdo.REAL_LEFT_GLASS = REAL_LEFT_GLASS.Text;

                // Skiascopy, Nhãn áp, Bề dày giác mạc, Đường kính đồng tử, K1, K2
                sdo.RIGHT_EYE_BEFORE = RIGHT_EYE_BEFORE.Text;
                sdo.LEFT_EYE_BEFORE = LEFT_EYE_BEFORE.Text;
                sdo.RIGHT_EYE_AFTER = RIGHT_EYE_AFTER.Text;
                sdo.LEFT_EYE_AFTER = LEFT_EYE_AFTER.Text;
                sdo.RIGHT_EYE_PRESSURE = RIGHT_EYE_PRESSURE.Text;
                sdo.LEFT_EYE_PRESSURE = LEFT_EYE_PRESSURE.Text;
                sdo.RIGHT_EYE_THICKNESS = RIGHT_EYE_THICKNESS.Text;
                sdo.LEFT_EYE_THICKNESS = LEFT_EYE_THICKNESS.Text;
                //sdo.RIGHT_EYE_PUPIL = RIGHT_EYE_PUPIL.Text;
                //sdo.LEFT_EYE_PUPIL = LEFT_EYE_PUPIL.Text;
                sdo.RIGHT_EYE_K1 = RIGHT_EYE_K1.Text;
                sdo.LEFT_EYE_K1 = LEFT_EYE_K1.Text;
                sdo.RIGHT_EYE_K2 = RIGHT_EYE_K2.Text;
                sdo.LEFT_EYE_K2 = LEFT_EYE_K2.Text;

                // Vận nhãn, Sắc giác, Thị giác lập thể, Độ lác, Trục nhãn cầu
                sdo.RIGHT_EYE_MOVE = RIGHT_EYE_MOVE.Text;
                sdo.LEFT_EYE_MOVE = LEFT_EYE_MOVE.Text;
                sdo.RIGHT_EYE_SENSE = RIGHT_EYE_SENSE.Text;
                sdo.LEFT_EYE_SENSE = LEFT_EYE_SENSE.Text;
                sdo.RIGHT_EYE_3D = RIGHT_EYE_3D.Text;
                sdo.LEFT_EYE_3D = LEFT_EYE_3D.Text;
                sdo.RIGHT_EYE_DEVIATION = RIGHT_EYE_DEVIATION.Text;
                sdo.LEFT_EYE_DEVIATION = LEFT_EYE_DEVIATION.Text;
                sdo.RIGHT_EYE_BALL = RIGHT_EYE_BALL.Text;
                sdo.LEFT_EYE_BALL = LEFT_EYE_BALL.Text;

                // II. Đơn kính
                // Nhìn xa (FAR)
                sdo.FAR_RIGHT_SPH = FAR_RIGHT_SPH.Text;
                sdo.FAR_RIGHT_CYL = FAR_RIGHT_CYL.Text;
                sdo.FAR_RIGHT_AXIS = FAR_RIGHT_AXIS.Text;
                sdo.FAR_RIGHT_VISION = FAR_RIGHT_VISION.Text;
                sdo.FAR_LEFT_SPH = FAR_LEFT_SPH.Text;
                sdo.FAR_LEFT_CYL = FAR_LEFT_CYL.Text;
                sdo.FAR_LEFT_AXIS = FAR_LEFT_AXIS.Text;
                sdo.FAR_LEFT_VISION = FAR_LEFT_VISION.Text;
                sdo.FAR_ADD = FAR_ADD.Text;

                // Nhìn gần (NEAR)
                sdo.NEAR_RIGHT_SPH = NEAR_RIGHT_SPH.Text;
                sdo.NEAR_RIGHT_CYL = NEAR_RIGHT_CYL.Text;
                sdo.NEAR_RIGHT_AXIS = NEAR_RIGHT_AXIS.Text;
                sdo.NEAR_RIGHT_VISION = NEAR_RIGHT_VISION.Text;
                sdo.NEAR_LEFT_SPH = NEAR_LEFT_SPH.Text;
                sdo.NEAR_LEFT_CYL = NEAR_LEFT_CYL.Text;
                sdo.NEAR_LEFT_AXIS = NEAR_LEFT_AXIS.Text;
                sdo.NEAR_LEFT_VISION = NEAR_LEFT_VISION.Text;

                // Khoảng cách đồng tử (BP)
                sdo.FAR_BP = FAR_BP.Text;
                sdo.NEAR_BP = NEAR_BP.Text;

                // Ghi chú kính
                sdo.IS_BIFOCAL_GLASS = IS_BIFOCAL_GLASS.Checked ? (short)1 : (short?)null;
                sdo.IS_PROGRESSIVE_GLASS = IS_PROGRESSIVE_GLASS.Checked ? (short)1 : (short?)null;
                sdo.IS_READING_GLASS = IS_READING_GLASS.Checked ? (short)1 : (short?)null;
                sdo.IS_PHOTOCHROMIC_GLASS = IS_PHOTOCHROMIC_GLASS.Checked ? (short)1 : (short?)null;
                sdo.IS_POLYCARBONATE_GLASS = IS_POLYCARBONATE_GLASS.Checked ? (short)1 : (short?)null;
                sdo.IS_CONTACT_LENSE = IS_CONTACT_LENSE.Checked ? (short)1 : (short?)null;

                // Ghi chú, Trạng thái kính
                sdo.VISION_EXAM_NOTE = VISION_EXAM_NOTE.Text;
                sdo.GLASS_STATUS = GLASS_STATUS.Text;
                sdo.GLASS_STATUS = GLASS_STATUS.Text;

                // Thời gian sử dụng, Số lượng, Kính xước
                if (GLASS_USE_TIME.EditValue != null)
                {
                    sdo.GLASS_USE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(GLASS_USE_TIME.DateTime);
                }
                sdo.GLASS_AMOUNT = GLASS_AMOUNT.Text != "" ? Convert.ToInt64(GLASS_AMOUNT.EditValue) : (long?)null;
                sdo.IS_GLASS_SCRATCHED = IS_GLASS_SCRATCHED.Checked ? (short)1 : (short?)null;
                // Thời gian tra thuốc
                if (MEDI_USE_TIME.EditValue != null)
                {
                    sdo.MEDI_USE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(MEDI_USE_TIME.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}