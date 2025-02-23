using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class GltfMesh
{
    public VertexPositionColorNormalTexture[] Buffer { get; set; } = [];

    public Node Node { get; set; }
}
