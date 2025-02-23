using Microsoft.Xna.Framework;

namespace ForestGame.Core.Graphics;

public class Mesh
{
    public Vector3[] Vertices { get; set; } = [];
    public Vector3[] Normals { get; set; } = [];
    public Vector2[] TexCoords { get; set; } = [];
    public int[][][] Faces { get; set; } = [];
    public Color[] VertexColors { get; set; } = [];

    public Matrix Matrix { get; set; }
}
