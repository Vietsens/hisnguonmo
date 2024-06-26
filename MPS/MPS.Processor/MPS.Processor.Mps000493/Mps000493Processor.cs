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
using MPS.Processor.Mps000493.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000493
{
    public class Mps000493Processor : AbstractProcessor
    {
        Mps000493PDO rdo;
        List<HIS_STENT_CONCLUDE> listStentConclude = new List<HIS_STENT_CONCLUDE>();
        List<FileImageADO> listFile = new List<FileImageADO>();
        public Mps000493Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            if (rdoBase != null && rdoBase is Mps000493PDO)
            {
                rdo = (Mps000493PDO)rdoBase;
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
                objectTag.AddObjectData(store, "StentConcludes", rdo.HisStentConcludes);
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
                if (rdo.VHisEkipUsers != null)
                {
                    List<string> listEkipUserNames = rdo.VHisEkipUsers.Select(o => o.USERNAME).ToList();
                    SetSingleKey(new KeyValue(Mps000493ExtendSingleKey.EKIP_USERS, String.Join(" - ", listEkipUserNames)));
                }
                AddObjectKeyIntoListkey<HIS_PATIENT>(rdo.HisPatient, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_5>(rdo.VHisSereServ5, false);
                AddObjectKeyIntoListkey<V_HIS_SERE_SERV_PTTT>(rdo.VHisSereServPttt, false);
                AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.VHisServiceReq, false);
                AddObjectKeyIntoListkey<HIS_SERE_SERV_EXT>(rdo.HisSereServExt, false);
                if (rdo.HisSereServExt != null)
                {
                    SetSingleKey(new KeyValue(Mps000493ExtendSingleKey.DESCRIPTION, rdo.HisSereServExt.DESCRIPTION));
                    SetSingleKey(new KeyValue(Mps000493ExtendSingleKey.NOTE, rdo.HisSereServExt.NOTE));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000493ExtendSingleKey.DESCRIPTION, ""));
                    SetSingleKey(new KeyValue(Mps000493ExtendSingleKey.NOTE, ""));
                }
                if (rdo.HisSereServFiles != null && rdo.HisSereServFiles.Count > 0)
                {
                    var lstFile = rdo.HisSereServFiles.OrderBy(o => o.ID).ToList();
                    int count = 1;
                    foreach (var item in lstFile)
                    {
                        if (!String.IsNullOrWhiteSpace(item.URL))
                        {
                            FileImageADO ado = new FileImageADO();
                            ado.FILE_NAME = item.SERE_SERV_FILE_NAME;
                            SetSingleImage("IMAGE_" + count, item.URL, ref ado);
                            count++;
                            listFile.Add(ado);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetSingleImage(string key, string imageUrl, ref FileImageADO ado)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                    stream.Position = 0;
                    ado.IMAGE_DATA = stream.ToArray();
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
