using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.Pharmacology
{
    public class ActiveIngredientADO : HIS_ACTIVE_INGREDIENT
    {
        public ActiveIngredientADO()
        {
        }

        /// <summary>
        /// Copy day du cac truong cua hoat chat.
        /// Phai copy het vi api UpdateList ghi de ca dong, thieu truong nao la mat du lieu truong do
        /// </summary>
        public ActiveIngredientADO(HIS_ACTIVE_INGREDIENT data)
        {
            if (data != null)
            {
                this.ID = data.ID;
                this.ACTIVE_INGREDIENT_CODE = data.ACTIVE_INGREDIENT_CODE;
                this.ACTIVE_INGREDIENT_NAME = data.ACTIVE_INGREDIENT_NAME;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
                this.IS_CONSULTATION_REQUIRED = data.IS_CONSULTATION_REQUIRED;
                this.IS_APPROVAL_REQUIRED = data.IS_APPROVAL_REQUIRED;
                this.NOTE = data.NOTE;
                this.MIMS_GUID = data.MIMS_GUID;
                this.MIMS_NAME = data.MIMS_NAME;
                this.MIMS_TYPE = data.MIMS_TYPE;
                this.MIMS_LAST_SYNC_TIME = data.MIMS_LAST_SYNC_TIME;
                this.IS_MIMS_MAPPED = data.IS_MIMS_MAPPED;
                this.MIMS_MAPPING_STATUS = data.MIMS_MAPPING_STATUS;
                this.MIMS_MAPPING_NOTE = data.MIMS_MAPPING_NOTE;
                this.PHARMACOLOGY_ID = data.PHARMACOLOGY_ID;
            }
        }

        public bool check2 { get; set; }

        /// <summary>
        /// Tra ve entity goc de gui len api, khong mang theo cot check cua luoi
        /// </summary>
        public HIS_ACTIVE_INGREDIENT ToActiveIngredient()
        {
            HIS_ACTIVE_INGREDIENT data = new HIS_ACTIVE_INGREDIENT();
            data.ID = this.ID;
            data.ACTIVE_INGREDIENT_CODE = this.ACTIVE_INGREDIENT_CODE;
            data.ACTIVE_INGREDIENT_NAME = this.ACTIVE_INGREDIENT_NAME;
            data.APP_CREATOR = this.APP_CREATOR;
            data.APP_MODIFIER = this.APP_MODIFIER;
            data.CREATE_TIME = this.CREATE_TIME;
            data.CREATOR = this.CREATOR;
            data.GROUP_CODE = this.GROUP_CODE;
            data.IS_ACTIVE = this.IS_ACTIVE;
            data.IS_DELETE = this.IS_DELETE;
            data.MODIFIER = this.MODIFIER;
            data.MODIFY_TIME = this.MODIFY_TIME;
            data.IS_CONSULTATION_REQUIRED = this.IS_CONSULTATION_REQUIRED;
            data.IS_APPROVAL_REQUIRED = this.IS_APPROVAL_REQUIRED;
            data.NOTE = this.NOTE;
            data.MIMS_GUID = this.MIMS_GUID;
            data.MIMS_NAME = this.MIMS_NAME;
            data.MIMS_TYPE = this.MIMS_TYPE;
            data.MIMS_LAST_SYNC_TIME = this.MIMS_LAST_SYNC_TIME;
            data.IS_MIMS_MAPPED = this.IS_MIMS_MAPPED;
            data.MIMS_MAPPING_STATUS = this.MIMS_MAPPING_STATUS;
            data.MIMS_MAPPING_NOTE = this.MIMS_MAPPING_NOTE;
            data.PHARMACOLOGY_ID = this.PHARMACOLOGY_ID;
            return data;
        }
    }
}
