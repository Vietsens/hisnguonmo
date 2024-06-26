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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Core
{
    public class TranslateWorker
    {
        /// <summary>
        ///Duyệt đa ngôn ngữ
        ///Kiểm tra ngôn ngữ đang chọn có phải là ngôn ngữ cơ sơ không
        ///Nếu là cơ sở: 
        ///------- Bỏ qua
        ///------- Nếu không phải là cơ sở: 
        ///-------------- duyệt trong bảng SdaTranslate theo key -> lấy giá trị của của dữ liệu tương ứng với ngôn ngữ được chọn
        ///-------------- update vào ram giá trị của dữ liệu đó vào Ram
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRaw"></param>
        public static void TranslateData<T>(List<T> dataRaw)
        {
            try
            {
                if (((typeof(T).ToString() == "ACS.EFMODEL.DataModels.ACS_MODULE")
                    || (typeof(T).ToString() == "ACS.EFMODEL.DataModels.ACS_MODULE_GROUP")
                    //|| (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_ICD")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_ROOM_TYPE")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.V_HIS_ROOM")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_TRANSACTION_TYPE")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_DEPARTMENT")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_GENDER")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_IMP_MEST_STT")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_PAY_FORM")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE")
                    || (typeof(T).ToString() == "MOS.EFMODEL.DataModels.HIS_MILITARY_RANK")
                    ) && TranslateDataWorker.Language != null && TranslateDataWorker.Language.IS_BASE != 1)
                {
                    List<SDA.EFMODEL.DataModels.SDA_TRANSLATE> translatesAll;
                    List<SDA.EFMODEL.DataModels.SDA_TRANSLATE> translates;

                    translatesAll = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_TRANSLATE>();

                    if (translatesAll != null && translatesAll.Count > 0)
                    {
                        string typeName = typeof(T).ToString();
                        var arrName = typeName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (arrName != null && arrName.Count >= 3)
                        {                           
                            translates = translatesAll
                                            .Where(o => o.LANGUAGE_ID == TranslateDataWorker.Language.ID
                                                //&& o.SCHEMA == GetSchemaName(arrName[0])
                                                    && o.TABLE_NAME == arrName[3]).ToList();
                            Inventec.Common.Logging.LogSystem.Info("TranslateData.translates.count=" + (translates != null ? translates.Count : 0) + "____" + "LANGUAGE_ID=" + (TranslateDataWorker.Language.ID) + "____" + "TABLE_NAME=" + (arrName[3]));
                            if (translates != null && translates.Count > 0)
                            {
                                dataRaw.ForEach(o => UpdateLanguage(o, translates));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static void UpdateLanguage<T>(T t, List<SDA.EFMODEL.DataModels.SDA_TRANSLATE> translates)
        {
            try
            {
                Type type = t.GetType();
                var propers = type.GetProperties();

                foreach (var tra in translates)
                {
                    var ppName1 = (!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_ONE) ? propers.FirstOrDefault(o => o.Name == tra.FIND_COLUMN_NAME_ONE && (o.GetValue(t, null) ?? "").ToString() == tra.FIND_DATA_CODE_ONE) : null);
                    var ppName2 = (!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_TWO) ? propers.FirstOrDefault(o => o.Name == tra.FIND_COLUMN_NAME_TWO && (o.GetValue(t, null) ?? "").ToString() == tra.FIND_DATA_CODE_TWO) : null);
                                      
                    var ppNameUpdate = propers.FirstOrDefault(o => tra.UPDATE_COLUMN_NAME == o.Name);
                    if (((!String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_ONE)
                        && !String.IsNullOrEmpty(tra.FIND_COLUMN_NAME_TWO)
                        && ppName1 != null
                        && ppName2 != null)
                        || (ppName1 != null || ppName2 != null)) && ppNameUpdate != null)
                    {
                        ppNameUpdate.SetValue(t, tra.UPDATE_DATA_VALUE);
                    }                   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
