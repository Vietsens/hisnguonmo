using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientExamV2
{
    public partial class UcRoom : UserControl
    {
        private ServiceReqGateADO state { get; set; }
        public UcRoom(ServiceReqGateADO ado)
        {
            InitializeComponent();

            try
            {
                state = ado;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void UcRoom_Load(object sender, EventArgs e)
        {
            SetColorForm();
        }
        private void SetColorForm()
        {
            try
            {
                this.lblRoom.Font = new Font("Arial", state.sizeTitle);
                this.lblRoom.ForeColor = ColorTranslator.FromHtml(state.colorTitle);
                this.lblRoom.BackColor = ColorTranslator.FromHtml(state.bgColorTitle);

                this.lblContentTitle.Font = new Font("Arial", state.sizeDangKham);
                this.lblContentNumber.Font = new Font("Arial", state.sizeContentNumber);

                this.lblContentEnd.ForeColor = ColorTranslator.FromHtml(state.colorEnd);
                this.lblNumberEnd.ForeColor = ColorTranslator.FromHtml(state.colorEnd);

                this.lblNumberEnd.Font = new Font("Arial", state.sizeEndTitle);
                this.lblContentEnd.Font = new Font("Arial", state.sizeChoKham);

                this.lblContentEnd.BackColor = ColorTranslator.FromHtml(state.bgColorEnd);
                this.lblNumberEnd.BackColor = ColorTranslator.FromHtml(state.bgColorEnd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public async Task ReloadData(List<HIS_SERVICE_REQ> ServiceReqs)
        {
            try
            {
                var currentCall = ServiceReqs
                    .Where(o => o.CALL_COUNT > 0)
                    .OrderByDescending(o => o.CALL_TIME)
                    .FirstOrDefault();
                this.lblContentNumber.Text = currentCall != null ? currentCall.NUM_ORDER.ToString() : "";
                if (currentCall != null)
                    ChooseRoomForWaitingScreenProcess.StackServiceReqCall.Add(
                          new ServiceReqCallADO { ServiceReq = currentCall, IsCalling = true }
                      );
                var waitingList = ServiceReqs
                    .Where(o => o.CALL_COUNT == 0)
                    .OrderBy(o => o.NUM_ORDER)
                    .ToList();

                string waitingText = "";
                int roomCount = state != null && state.roomGateSDOs != null ? state.roomGateSDOs.Count : 0;
                if (roomCount > 5 && waitingList.Count > 8)
                {
                    var firstFive = waitingList.Take(5).Select(s => s.NUM_ORDER.ToString());
                    waitingText = string.Join(", ", firstFive) + ", ...";
                }
                else
                {
                    waitingText = waitingList != null && waitingList.Count > 0
                        ? string.Join(", ", waitingList.Select(s => s.NUM_ORDER.ToString()))
                        : "";
                }
                this.lblNumberEnd.Text = waitingText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
      
    }
}
