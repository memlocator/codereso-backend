using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics;

public class Collider : ECS.Component
{
    public ColliderData Data;
    public ColliderAsset? Asset;
    public override bool ShouldBeReplicated { get => false; }

    public void CollidedWith(Collider other, PhysicsResults results)
    {
        if (ParentObject?.Transform == null) return;

        ParentObject.Transform.Position += results.MinimumTranslation;
    }

    public override void Update(float delta)
    {
        if (ParentObject?.Transform == null) return;
        if (!ParentObject.Transform.IsStale) return;
        if (Asset == null) return;

        Data.TransformFrom(ref Asset.Data, ParentObject.Transform.Matrix);
    }
}
