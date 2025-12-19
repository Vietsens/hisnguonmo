using HIS.Desktop.Plugins.MchTreatmentExamService.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress
{
    public partial class UCAddress : HIS.Desktop.Utility.UserControlBase
    {
        public UCAddressADO GetValue()
        {
            UCAddressADO getData = new UCAddressADO();
            try
            {
                if (!IsChangeStrucAdreess)
                {
                    getData.Commune_Code = (string)(cboCommune.EditValue ?? "");
                    getData.Commune_Name = cboCommune.Text;
                    getData.District_Code = (string)(cboDistrict.EditValue ?? "");
                    getData.District_Name = cboDistrict.Text;
                }
                else
                {
                    getData.Commune_Code = (string)(cboDistrict.EditValue ?? "");
                    getData.Commune_Name = cboDistrict.Text;
                }
                getData.Province_Code = (string)(cboProvince.EditValue ?? "");
                getData.Province_Name = cboProvince.Text;
                getData.Address = this.txtAddress.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                getData = null;
            }
            return getData;
        }
    }
}
