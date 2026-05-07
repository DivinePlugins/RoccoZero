using Divine.Core.Entities;

namespace Divine.Core.Managers.TargetSelector.Selector
{
    internal interface ISelector
    {
        CHero GetTarget();
    }
}