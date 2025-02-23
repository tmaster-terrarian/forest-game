using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class GltfMesh
{
    public List<VertexPositionColorNormalTexture> Buffer { get; } = [];

    public Node Node { get; set; }
}
