using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class SimpleModel
{
    private int _faceCount;

    public Mesh[] Meshes { get; private set; } = [];

    private VertexPositionColorNormalTexture[] _buffer = [];

    private SimpleModel() { }

    public void Build()
    {
        _faceCount = 0;
        List<VertexPositionColorNormalTexture> buff = [];

        for(int g = 0; g < Meshes.Length; g++)
        {
            for(int f = 0; f < Meshes[g].Faces.Length; f++)
            {
                for(int v = 0; v < Meshes[g].Faces[f].Length; v++)
                {
                    // v1/vt1/vn1
                    var i = Meshes[g].Faces[f][v];
                    buff.Add(new(
                        Meshes[g].Vertices[i[0]],
                        Meshes[g].VertexColors[i[0]],
                        Meshes[g].Normals[i[2]],
                        i[1] != -1 ? Meshes[g].TexCoords[i[1]] : Vector2.Zero
                    ));
                }
                _faceCount++;
            }
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

    public static SimpleModel Load(string objPath)
    {
        string path = objPath.EndsWith(".obj") ? objPath : objPath + ".obj";

        using StreamReader reader = new(File.Open(path, FileMode.Open));

        List<Vector3> v = [];
        List<Vector3> vn = [];
        List<Vector2> vt = [];
        List<int[][]> f = [];
        List<Color> vc = [];
        List<Mesh> g = [];

        Mesh? currentMesh = null;

        while(!reader.EndOfStream)
        {
            string? line = reader.ReadLine();

            if(line is null)
                continue;

            if(line.StartsWith("# ") || line.StartsWith("mtllib ") || line.StartsWith("usemtl "))
                continue;

            if(line.StartsWith("o "))
            {
                if(currentMesh is not null)
                {
                    if(vc.Count == 0)
                    {
                        for(int i = 0; i < v.Count; i++)
                            vc.Add(Color.White);
                    }

                    currentMesh.Vertices = [..v]; v = [];
                    currentMesh.Normals = [..vn]; vn = [];
                    currentMesh.TexCoords = [..vt]; vt = [];
                    currentMesh.Faces = [..f]; f = [];
                    currentMesh.VertexColors = [..vc]; vc = [];
                }

                currentMesh = new();
                g.Add(currentMesh);
            }
            else if(line.StartsWith("v "))
            {
                var split = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                v.Add(new Vector3(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3])
                ));
            }
            else if(line.StartsWith("vn "))
            {
                var split = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
                vn.Add(new Vector3(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3])
                ));
            }
            else if(line.StartsWith("vc ")) // custom vertex color implementation
            {
                var split = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
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
                Console.WriteLine($"{line}, {split[1]}, {split[2]}");
                vt.Add(new Vector2(
                    float.Parse(split[1]),
                    float.Parse(split[2])
                ));
            }
            else if(line.StartsWith("f "))
            {
                var split = line.Split(' ', 4);
                Console.WriteLine($"{line}, {split[1]}, {split[2]}, {split[3]}");
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

        if(currentMesh != null)
        {
            if(vc.Count == 0)
            {
                for(int i = 0; i < v.Count; i++)
                    vc.Add(Color.White);
            }

            currentMesh.Vertices = [..v];
            currentMesh.Normals = [..vn];
            currentMesh.TexCoords = [..vt];
            currentMesh.Faces = [..f];
            currentMesh.VertexColors = [..vc];
        }

        SimpleModel mdl = new() {
            Meshes = [..g],
        };
        mdl.Build();

        return mdl;
    }

    public class Mesh
    {
        public Vector3[] Vertices { get; set; } = [];
        public Vector3[] Normals { get; set; } = [];
        public Vector2[] TexCoords { get; set; } = [];
        public int[][][] Faces { get; set; } = [];
        public Color[] VertexColors { get; set; } = [];
    }
}
