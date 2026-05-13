using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Battle
{
    public struct TargetedComponent : IEcsAutoReset<TargetedComponent>
    {
        public Stack<EcsPackedEntity?> TargetedDices;
        
        public void AutoReset(ref TargetedComponent c)
        {
            c.TargetedDices = new Stack<EcsPackedEntity?>();
        }
    }
}