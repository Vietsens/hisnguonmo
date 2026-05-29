using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000514.PDO;

namespace MPS.Processor.Mps000514
{
    class Mps000514Processor : AbstractProcessor
    {
        Mps000514PDO rdo;

        public Mps000514Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000514PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag =
                    new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag =
                    new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag =
                    new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                SetSingleKey();
                SetBarcodeKey();

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "PackageDetails", rdo.PatientPackageDetails);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                // Tự động map TẤT CẢ properties bệnh nhân → template keys
                AddObjectKeyIntoListkey<HIS_PATIENT>(rdo.Patient, false);

                // Map property của gói với prefix "PKG_" để tránh trùng tên cột audit (ID, CREATE_TIME...)
                // => key trong template: {PKG_PACKAGE_NAME}, {PKG_STATUS_CODE}, {PKG_TOTAL_PAID}...
                AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT_PACKAGE>(rdo.PatientPackage, "PKG_", false);

                if (rdo.PatientPackage != null)
                {
                    SetSingleKey(new KeyValue(
                        Mps000514ExtendSingleKey.REGISTER_DATE_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatientPackage.REGISTER_DATE)));

                    SetSingleKey(new KeyValue(
                        Mps000514ExtendSingleKey.LOCKED_DATE_STR,
                        rdo.PatientPackage.LOCKED_DATE.HasValue
                            ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatientPackage.LOCKED_DATE.Value)
                            : ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBarcodeKey()
        {
            try
            {
                if (dicImage == null)
                {
                    dicImage = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                }

                if (rdo.Patient != null && !String.IsNullOrEmpty(rdo.Patient.PATIENT_CODE))
                {
                    Inventec.Common.BarcodeLib.Barcode barcodePatientCode =
                        new Inventec.Common.BarcodeLib.Barcode(rdo.Patient.PATIENT_CODE);
                    barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                    barcodePatientCode.IncludeLabel = true;
                    barcodePatientCode.Width = 120;
                    barcodePatientCode.Height = 40;
                    barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                    barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                    dicImage.Add(Mps000514ExtendSingleKey.BARCODE_PATIENT_CODE, barcodePatientCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
