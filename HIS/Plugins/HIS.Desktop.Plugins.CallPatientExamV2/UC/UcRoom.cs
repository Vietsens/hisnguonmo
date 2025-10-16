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
    public delegate void DelegateCallPatient(HIS_SERVICE_REQ PatientCall);
    public partial class UcRoom : UserControl
    {
        private ServiceReqGateADO state { get; set; }
        private RoomGateSDO serviceReq { get; set; }
        private HIS_SERVICE_REQ currentServiceReq { get; set; }
        private DelegateCallPatient dlg { get; set; }
        private bool IsFirstLoad { get; set; } = true;
        public UcRoom(ServiceReqGateADO ado, RoomGateSDO sdo, DelegateCallPatient dlg)
        {
            InitializeComponent();

            try
            {
                this.dlg = dlg;
                state = ado;
                serviceReq = sdo;
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
                lblRoom.Text = serviceReq != null ? serviceReq.ROOM_NAME : "";

                this.lblRoom.Font = new Font("Times New Roman", state.sizeTitle, FontStyle.Bold);
                this.lblRoom.ForeColor = ColorTranslator.FromHtml(state.colorTitle);
                this.lblRoom.BackColor = ColorTranslator.FromHtml(state.bgColorTitle);

                this.lblContentTitle.Font = new Font("Times New Roman", state.sizeDangKham, FontStyle.Bold);
                this.lblContentNumber.Font = new Font("Times New Roman", state.sizeContentNumber, FontStyle.Bold);

                this.lblContentEnd.ForeColor = ColorTranslator.FromHtml(state.colorEnd);
                this.lblNumberEnd.ForeColor = ColorTranslator.FromHtml(state.colorEnd);

                this.lblNumberEnd.Font = new Font("Times New Roman", state.sizeEndTitle, FontStyle.Bold);
                this.lblContentEnd.Font = new Font("Times New Roman", state.sizeChoKham, FontStyle.Bold);

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
                {
                    if (!IsFirstLoad && ((currentServiceReq != null && currentCall.CALL_TIME - currentServiceReq.CALL_TIME > 3) || currentServiceReq == null))
                    {
                        dlg(currentCall);
                    }
                    currentServiceReq = currentCall;
                }
                if(IsFirstLoad)
                    IsFirstLoad = false;
                var waitingList = ServiceReqs
                    .Where(o => (o.CALL_COUNT ?? -1) < 0)
                    .OrderBy(o => o.NUM_ORDER)
                    .ToList();

                string waitingText = "";
                if (waitingList != null && waitingList.Count > 0)
                {
                    waitingText = string.Join(", ", waitingList.Select(s => s.NUM_ORDER.ToString()));
                }
                this.lblNumberEnd.Text = waitingText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool IsCalling()
        {
            try
            {
                return !string.IsNullOrEmpty(lblContentNumber.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
    }
}
