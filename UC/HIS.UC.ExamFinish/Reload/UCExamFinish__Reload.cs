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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExamFinish.ADO;

namespace HIS.UC.ExamFinish.Run
{
    public partial class UCExamFinish : UserControl
    {
        public void Reload(ExamFinishInitADO examFinish)
        {
            try
            {
                if (examFinish != null)
                {
                    if (examFinish.FinishTime.HasValue)
                    {
                        dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examFinish.FinishTime.Value) ?? DateTime.Now;
                    }
                    else
                    {
                        dtFinishTime.DateTime = DateTime.Now;
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