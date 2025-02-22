using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class ObjModel
{
    private int _faceCount;

    public Mesh Mesh { get; private set; }

    public Transform Transform = Transform.Identity;

    private VertexPositionColorNormalTexture[] _buffer = [];

    private ObjModel() { }

    public void Build()
    {
        _faceCount = 0;
        List<VertexPositionColorNormalTexture> buff = [];

        for(int f = 0; f < Mesh.Faces.Length; f++)
        {
            for(int v = 0; v < Mesh.Faces[f].Length; v++)
            {
                // v1/vt1/vn1
                var i = Mesh.Faces[f][v];
                buff.Add(new(
                    Mesh.Vertices[i[0]],
                    Mesh.VertexColors[i[0]],
                    Mesh.Normals[i[2]],
                    i[1] != -1 ? Mesh.TexCoords[i[1]] : Vector2.Zero
                ));
            }
            _faceCount++;
        }
        _buffer = [..buff];
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserPrimitives(
            PrimitiveType.TriangleList,
            _buffer,
            0,
            _faceCount,
            VertexPositionColorNormalTexture.VertexDeclaration
        );
    }

    public static ObjModel Load(string objPath)
    {
        string path = objPath.EndsWith(".obj") ? objPath : objPath + ".obj";

        using StreamReader reader = new(File.Open(path, FileMode.Open));

        List<Vector3> v = [];
        List<Vector3> vn = [];
        List<Vector2> vt = [];
        List<int[][]> f = [];
        List<Color> vc = [];
        List<Mesh> g = [];

        Mesh m = new();

        while(!reader.EndOfStream)
        {
            string? line = reader.ReadLine();

            if(line is null)
                continue;

            if(line.StartsWith("# ") || line.StartsWith("mtllib ") || line.StartsWith("usemtl "))
                continue;

            if(line.StartsWith("v "))
            {
                var split = line.Split(' ', 8, StringSplitOptions.RemoveEmptyEntries);
                // Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                v.Add(new Vector3(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3])
                ));

                if(split.Length > 4)
                {
                    vc.Add(new Color(
                        float.Parse(split[4]),
                        float.Parse(split[5]),
                        float.Parse(split[6]),
                        split.Length > 7
                            ? float.Parse(split[7])
                            : 1f
                    ));
                }
            }
            else if(line.StartsWith("vn "))
            {
                var split = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                // Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                vn.Add(new Vector3(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3])
                ));
            }
            else if(line.StartsWith("vc ")) // custom vertex color implementation
            {
                var split = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                // Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                vc.Add(new Color(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3]),
                    1f
                ));
            }
            else if(line.StartsWith("vt "))
            {
                var split = line.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                // Console.WriteLine($"{line}, {split[1]}, {split[2]}");
                vt.Add(new Vector2(
                    float.Parse(split[1]),
                    float.Parse(split[2])
                ));
            }
            else if(line.StartsWith("f "))
            {
                var split = line.Split(' ', 4);
                // Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                List<int[]> ind = [];
                for(int i = 1; i < 4; i++)
                {
                    var split2 = split[i].Split('/', 3);
                    ind.Add([
                        int.Parse(split2[0]) - 1,
                        split2[1].Length > 0
                            ? int.Parse(split2[1]) - 1
                            : -1,
                        int.Parse(split2[2]) - 1,
                    ]);
                }
                f.Add([..ind]);
            }
        }

        if(vc.Count < v.Count)
        {
            while(vc.Count < v.Count)
                vc.Add(Color.White);
        }

        m.Vertices = [..v];
        m.Normals = [..vn];
        m.TexCoords = [..vt];
        m.Faces = [..f];
        m.VertexColors = [..vc];

        ObjModel mdl = new() {
            Mesh = m,
        };
        mdl.Build();

        return mdl;
    }
}

public class Mesh
{
    public Vector3[] Vertices { get; set; } = [];
    public Vector3[] Normals { get; set; } = [];
    public Vector2[] TexCoords { get; set; } = [];
    public int[][][] Faces { get; set; } = [];
    public Color[] VertexColors { get; set; } = [];
}
