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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000490.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000490
{
    public class Mps000490Processor : AbstractProcessor
    {
        Mps000490PDO rdo;
        List<HIS_ASSESSMENT_MEMBER> listMembers = new List<HIS_ASSESSMENT_MEMBER>();
        List<HIS_ASSESSMENT_MEMBER> listAbsents = new List<HIS_ASSESSMENT_MEMBER>();
        List<HIS_ASSESSMENT_MEMBER> listGuests = new List<HIS_ASSESSMENT_MEMBER>();
        List<HIS_ASSESSMENT_MEMBER> listDisagreeds = new List<HIS_ASSESSMENT_MEMBER>();
        public Mps000490Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000490PDO)
            {
                rdo = (Mps000490PDO)rdoBase;
            }
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                SetSingleKey();
                objectTag.AddObjectData(store, "Members", this.listMembers);
                objectTag.AddObjectData(store, "Absents", this.listAbsents);
                objectTag.AddObjectData(store, "Guests", this.listGuests);
                objectTag.AddObjectData(store, "Disagreeds", this.listDisagreeds);
                this.SetSignatureKeyImageByCFG();
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
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
                if (rdo.MedicalAssessment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_MEDICAL_ASSESSMENT>(rdo.MedicalAssessment, false);
                    if (rdo.MedicalAssessment.ASSESSMENT_TYPE_ID != null)
                    {
                        switch (rdo.MedicalAssessment.ASSESSMENT_TYPE_ID)
                        {
                            case 1:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám giám định lần đầu"));
                                break;
                            case 2:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám giám định lại"));
                                break;
                            case 3:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám giám định tái phát"));
                                break;
                            case 4:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám phúc quyết"));
                                break;
                            case 5:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám phúc quyết lần cuối"));
                                break;
                            case 6:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám bổ sung"));
                                break;
                            case 7:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Khám vết thương còn sót"));
                                break;
                            case 8:
                                SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ASSESSMENT_TYPE_NAME, "Giám định tổng hợp"));
                                break;
                        }

                    }
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.INJURY_RATE_100, (rdo.MedicalAssessment.INJURY_RATE ?? 0) * 100));
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.PREVIOUS_INJURY_RATE_100, (rdo.MedicalAssessment.PREVIOUS_INJURY_RATE ?? 0) * 100));
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.INJURY_RATE_TOTAL_100, (rdo.MedicalAssessment.INJURY_RATE_TOTAL ?? 0) * 100));

                }
                if (rdo.ListAssessmentMember != null && rdo.ListAssessmentMember.Count > 0)
                {
                    HIS_ASSESSMENT_MEMBER president = rdo.ListAssessmentMember.FirstOrDefault(o => o.IS_PRESIDENT == 1);
                    if (president != null)
                    {
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.PRESIDENT_USERNAME, president.USERNAME));
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.PRESIDENT_LOGINNAME, president.LOGINNAME));
                    }
                    HIS_ASSESSMENT_MEMBER secretary = rdo.ListAssessmentMember.FirstOrDefault(o => o.IS_SECRETARY == 1);
                    if (secretary != null)
                    {
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.SECRETARY_USERNAME, secretary.USERNAME));
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.SECRETARY_LOGINNAME, secretary.LOGINNAME));
                    }
                    var liMembers = rdo.ListAssessmentMember.Where(o => o.IS_ABSENT != 1 && o.IS_GUEST != 1 && o.IS_PRESIDENT != 1 && o.IS_SECRETARY != 1);
                    if (liMembers != null)
                        listMembers = liMembers.ToList();

                    listGuests = rdo.ListAssessmentMember.Where(o => o.IS_GUEST == 1) != null ? rdo.ListAssessmentMember.Where(o => o.IS_GUEST == 1).ToList() : new List<HIS_ASSESSMENT_MEMBER>();
                    listDisagreeds = rdo.ListAssessmentMember.Where(o => o.IS_DISAGREED == 1) != null ? rdo.ListAssessmentMember.Where(o => o.IS_DISAGREED == 1).ToList() : new List<HIS_ASSESSMENT_MEMBER>();
                    listAbsents = rdo.ListAssessmentMember.Where(o => o.IS_ABSENT == 1) != null ? rdo.ListAssessmentMember.Where(o => o.IS_ABSENT == 1).ToList() : new List<HIS_ASSESSMENT_MEMBER>();

                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.ABSENT_MEMBERS, String.Join(", ", listAbsents.Select(o => o.USERNAME))));
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.GUEST_MEMBERS, String.Join(", ", listGuests.Select(o => o.USERNAME))));
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.DISAGREED_MEMBERS, String.Join(", ", listDisagreeds.Select(o => o.USERNAME))));
                    var listAgrees = rdo.ListAssessmentMember.Where(o => o.IS_ABSENT != 1 && o.IS_GUEST != 1 && o.IS_DISAGREED != 1);
                    SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.AGREED_NUMBER, listAgrees != null ? listAgrees.Count() : 0));
                    var signer = rdo.ListAssessmentMember.FirstOrDefault(o => o.ON_BEHALF_TO_SIGN == 1);
                    if (signer != null)
                    {
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.HAS_ON_BEHALF_TO_SIGN, 1));
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.SIGNED_USERNAME, signer.USERNAME));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.HAS_ON_BEHALF_TO_SIGN, 0));
                        SetSingleKey(new KeyValue(Mps000490ExtendSingleKey.SIGNED_USERNAME, president.USERNAME));
                    }



                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
