using Inventec.Common.Logging;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisEquipmentSet
{
    public class HisEquipmentSetViewFilterQuery : HisEquipmentSetViewFilter
    {
        public HisEquipmentSetViewFilterQuery()
            : base()
        {

        }

        internal List<System.Linq.Expressions.Expression<Func<V_HIS_EQUIPMENT_SET, bool>>> listVHisEquipmentSetExpression = new List<System.Linq.Expressions.Expression<Func<V_HIS_EQUIPMENT_SET, bool>>>();

        

        internal HisEquipmentSetSO Query()
        {
            HisEquipmentSetSO search = new HisEquipmentSetSO();
            try
            {
                #region Abstract Base
                if (this.ID.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.ID == this.ID.Value);
                }
				if (this.IDs != null)
                {
                    listVHisEquipmentSetExpression.Add(o => this.IDs.Contains(o.ID));
                }
                if (this.IS_ACTIVE.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.IS_ACTIVE == this.IS_ACTIVE.Value);
                }
                if (this.CREATE_TIME_FROM.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.CREATE_TIME.Value >= this.CREATE_TIME_FROM.Value);
                }
                if (this.CREATE_TIME_TO.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.CREATE_TIME.Value <= this.CREATE_TIME_TO.Value);
                }
                if (this.MODIFY_TIME_FROM.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.MODIFY_TIME.Value >= this.MODIFY_TIME_FROM.Value);
                }
                if (this.MODIFY_TIME_TO.HasValue)
                {
                    listVHisEquipmentSetExpression.Add(o => o.MODIFY_TIME.Value <= this.MODIFY_TIME_TO.Value);
                }
                if (!String.IsNullOrEmpty(this.CREATOR))
                {
                    listVHisEquipmentSetExpression.Add(o => o.CREATOR == this.CREATOR);
                }
                if (!String.IsNullOrEmpty(this.MODIFIER))
                {
                    listVHisEquipmentSetExpression.Add(o => o.MODIFIER == this.MODIFIER);
                }
                if (!String.IsNullOrEmpty(this.GROUP_CODE))
                {
                    listVHisEquipmentSetExpression.Add(o => o.GROUP_CODE == this.GROUP_CODE);
                }
                if (this.IDs != null)
                {
                    listVHisEquipmentSetExpression.Add(o => this.IDs.Contains(o.ID));
                }
                #endregion
                
                search.listVHisEquipmentSetExpression.AddRange(listVHisEquipmentSetExpression);
                search.OrderField = ORDER_FIELD;
                search.OrderDirection = ORDER_DIRECTION;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                search.listVHisEquipmentSetExpression.Clear();
                search.listVHisEquipmentSetExpression.Add(o => o.ID == NEGATIVE_ID);
            }
            return search;
        }
    }
}
