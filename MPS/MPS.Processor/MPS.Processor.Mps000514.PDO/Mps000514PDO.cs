using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000514.PDO
{
    public class Mps000514PDO : RDOBase
    {
        public HIS_PATIENT Patient { get; set; }
        public HIS_PATIENT_PACKAGE PatientPackage { get; set; }
        public List<HIS_PATIENT_PACKAGE_DT> PatientPackageDetails { get; set; }

        public Mps000514PDO()
        {
        }

        public Mps000514PDO(
            HIS_PATIENT patient,
            HIS_PATIENT_PACKAGE patientPackage,
            List<HIS_PATIENT_PACKAGE_DT> patientPackageDetails)
        {
            try
            {
                this.Patient = patient;
                this.PatientPackage = patientPackage;
                this.PatientPackageDetails = patientPackageDetails;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
