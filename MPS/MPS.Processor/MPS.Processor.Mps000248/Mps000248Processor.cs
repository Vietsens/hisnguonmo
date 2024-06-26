/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000248.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000248
{
    public class Mps000248Processor : AbstractProcessor
    {
        Mps000248PDO rdo;

        public Mps000248Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000248PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodeTreatmentCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                barcodeTreatmentCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatmentCode.IncludeLabel = false;
                barcodeTreatmentCode.Width = 120;
                barcodeTreatmentCode.Height = 40;
                barcodeTreatmentCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatmentCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatmentCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatmentCode.IncludeLabel = true;

                dicImage.Add(Mps000248ExtendSingleKey.TREATMENT_CODE_BARCODE, barcodeTreatmentCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "MedicineTypeNNs", rdo._MedicineIsAdrs);
                objectTag.AddObjectData(store, "MedicineTypes", rdo._Medicines);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                if (rdo._ADR != null)
                {
                    if (rdo._ADR.SERIOUS_LEVEL.HasValue)
                    {
                        if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__DiTatThai)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__DiTatThai, "X"));
                        else if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__KhongNghiemTrong)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__KhongNghiemTrong, "X"));
                        else if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NGuyCoTuVong)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__NGuyCoTuVong, "X"));
                        else if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__NhapVienKeoDai)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__NhapVienKeoDai, "X"));
                        else if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TanTatVinhVien)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__TanTatVinhVien, "X"));
                        else if (rdo._ADR.SERIOUS_LEVEL == IMSys.DbConfig.HIS_RS.HIS_ADR.SERIOUS_LEVEL__TuVong)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.SERIOUS_LEVEL__TuVong, "X"));
                    }
                    if (rdo._ADR.ADR_RESULT_ID.HasValue)
                    {
                        if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__ChuaHoiPhuc)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__ChuaHoiPhuc, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__DangHoiPhuc)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__DangHoiPhuc, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucCoDiChung)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__HoiPhucCoDiChung, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__HoiPhucKhongCoDiChung)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__HoiPhucKhongCoDiChung, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__KhongRo)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__KhongRo, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongDoADR)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__TuVongDoADR, "X"));
                        else if (rdo._ADR.ADR_RESULT_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.ADR_RESULT_ID__TuVongKhongDoADR)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.ADR_RESULT_ID__TuVongKhongDoADR, "X"));
                    }
                    if (rdo._ADR.RELATIONSHIP_ID.HasValue)
                    {
                        if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoKhaNang)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__CoKhaNang, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__CoThe)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__CoThe, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChacChan)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__ChacChan, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__ChuaPhan)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__ChuaPhan, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__Khac)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__Khac, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongChacChan)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__KhongChacChan, "X"));
                        else if (rdo._ADR.RELATIONSHIP_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.RELATIONSHIP_ID__KhongThePhanLoai)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.RELATIONSHIP_ID__KhongThePhanLoai, "X"));
                    }
                    if (rdo._ADR.EXPERTISE_STANDER_ID.HasValue)
                    {
                        if (rdo._ADR.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__NARANJO)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.EXPERTISE_STANDER_ID__NARANJO, "X"));
                        else if (rdo._ADR.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__WHO)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.EXPERTISE_STANDER_ID__WHO, "X"));
                        else if (rdo._ADR.EXPERTISE_STANDER_ID == IMSys.DbConfig.HIS_RS.HIS_ADR.EXPERTISE_STANDER_ID__OTHER)
                            SetSingleKey(new KeyValue(Mps000248ExtendSingleKey.EXPERTISE_STANDER_ID__OTHER, "X"));
                    }
                }

                AddObjectKeyIntoListkey<V_HIS_ADR>(rdo._ADR, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment, true);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
