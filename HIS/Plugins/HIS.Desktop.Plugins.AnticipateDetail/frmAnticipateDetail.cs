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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AnticipateDetail
{
    public partial class frmAnticipateDetail : Form
    {
        private MOS.EFMODEL.DataModels.HIS_ANTICIPATE anticipate;
        //private List<MSS.SDO.HisAnticipateMetySdo> anticipateMetiePrints;
        //private List<MOS.EFMODEL.DataModels.HIS_ANTICIPATE_METY> anticipateMetieprints;
        private List<MSS.SDO.HisAnticipateMetySdo> anticipateMetiePrints;
        Dictionary<string, object> dicParam;
        public frmAnticipateDetail()
        {
            InitializeComponent();
        }
        public frmAnticipateDetail(MOS.EFMODEL.DataModels.HIS_ANTICIPATE Anticipate)		
        {
            try
            {
                InitializeComponent();
                this.anticipate = Anticipate;
                anticipateMetiePrints = new List<MSS.SDO.HisAnticipateMetySdo>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void frmAnticipateDetail_Load(object sender, EventArgs e)
        {
            loadDataToGridControlMety();
            //loadDataToGridControlMaty();
            //loadDataToGridControlBlty();
            //initButtonPrint();
        }
        private void loadDataToGridControlMety()
        {
            try
            {
                if (this.anticipate == null)
                {
                    return;
                }
                //MSS.MANAGER.AnticipateMety.HisAnticipateMetyLogic anticipateLogic = new MANAGER.AnticipateMety.HisAnticipateMetyLogic();
                //MOS.Filter.HisAnticipateMetyViewFilter filter = new MOS.Filter.HisAnticipateMetyViewFilter();
                MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY anticipateLogic = new MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY();
                MOS.Filter.HisAnticipateMetyViewFilter filter = new MOS.Filter.HisAnticipateMetyViewFilter();
                filter.ANTICIPATE_ID = this.anticipate.ID;
                //var anticipateMeties = anticipateLogic.GetView(filter);
                var anticipateMeties = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ANTICIPATE_METY>(filter);
                if (anticipateMeties != null && anticipateMeties.Count > 0)
                {
                    foreach (var item in anticipateMeties)
                    {
                        MSS.SDO.HisAnticipateMetySdo aAnticipateMety = new MSS.SDO.HisAnticipateMetySdo();
                       // MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY aAnticipateMety = new MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_ANTICIPATE_METY>(aAnticipateMety, item);
                        aAnticipateMety.TotalMoney = item.AMOUNT * (item.IMP_PRICE ?? 0);
                        anticipateMetiePrints.Add(aAnticipateMety);
                    }
                }

                //gridControlAnticipateMety.DataSource = anticipateMeties;
                gridControl1.DataSource = anticipateMeties;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
