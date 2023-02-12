using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Math2D;

namespace GameEngine.Physics;

public class ColliderPrimitives
{
    public static ColliderAsset Square { get; private set; } = MakeSquare();
    public static ColliderAsset Circle { get; private set; } = MakeCircle();

    private static ColliderAsset MakeSquare()
    {
        ColliderAsset asset = new ColliderAsset();

        asset.Data.SetVertices(new()
        {
            new( 0.5f,  0.5f, 1),
            new( 0.5f, -0.5f, 1),
            new(-0.5f, -0.5f, 1),
            new(-0.5f,  0.5f, 1)
        });

        return asset;
    }

    private static ColliderAsset MakeCircle()
    {
        ColliderAsset asset = new ColliderAsset();

        asset.Data.SetCircle(new Vector2(0, 0), 1);

        return asset;
    }
}
