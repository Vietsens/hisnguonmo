using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InviteConsultation.ADO
{
 
    public class HisSpecialistExamCreateAutoTrackingFeSDO : HIS_SPECIALIST_EXAM
    {
        public short IsAutoCreateTracking { get; set; }

        public string MedicalInstruction { get; set; }
    }
}
