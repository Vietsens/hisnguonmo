using Inventec.Core;

namespace Inventec.Desktop.MenuButton
{
    public abstract class BussinessBase : EntityBase
    {
        protected CommonParam param { get; set; }

        public BussinessBase()
            : base()
        {
            param = new CommonParam();
        }

        public BussinessBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }
    }
}
