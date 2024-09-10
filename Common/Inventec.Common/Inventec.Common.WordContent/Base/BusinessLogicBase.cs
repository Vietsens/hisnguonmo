using Inventec.Core;
namespace Inventec.Common.WordContent.Base
{
    /// <summary>
    /// Internal & khong cho phep truyen theo param --> bat buoc su dung CopyCommonParamInfo.
    /// </summary>
    public abstract class BusinessLogicBase : LogicBase
    {
        public BusinessLogicBase(CommonParam param)
            : base(param)
        {

        }

        public BusinessLogicBase()
            : base()
        {

        }
    }
}
