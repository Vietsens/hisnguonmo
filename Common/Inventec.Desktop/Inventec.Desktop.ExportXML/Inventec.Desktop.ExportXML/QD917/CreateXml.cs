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
using System;
using Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO;
using Inventec.Desktop.ExportXML.QD917.Common;
using Inventec.Desktop.ExportXML.QD917.ProcessData;

namespace Inventec.Desktop.ExportXML.QD917
{
    public class CreateXml
    {
        public ResultReturn SaveXml(InputData inputData)
        {
            ResultReturn result;
            try
            {
                var giamDinh = new HoSoBenhNhan_Process().GiamDinhHs(inputData);
                if (giamDinh.Succeeded == false || giamDinh.Obj == null)
                    return giamDinh;

                var fileName = string.Format("{0}___{1}", DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss"), inputData.V_HIS_TREATMENT.TREATMENT_CODE);
                var path = string.Format("{0}/{1}.xml", GlobalStore.PathSaveXml, fileName);
                result = new CommonQD917().CreatedXmlFile(giamDinh.Obj as HS_GIAMDINHHS_ADO, false, true, true, path);
            }
            catch (Exception ex)
            {
                result = new ResultReturn { Obj = null, Succeeded = false, Result = ex.Message };
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
