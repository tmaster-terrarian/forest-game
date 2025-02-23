using System.Reflection;
using Assimp;
using Assimp.Unmanaged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace ForestGame.Core.Graphics;

public class GltfModel
{
    public Scene Scene { get; private set; }

    public Transform Transform = Transform.Identity;

    private List<Node> _nodes = [];

    private readonly List<GltfMesh> _meshes = [];

    public IReadOnlyCollection<GltfMesh> Meshes => _meshes.AsReadOnly();

    private bool _isDirty = true;

    private GltfModel() { }

    public static GltfModel Load(string filePath)
    {
        string path = filePath.EndsWith(".glb") ? filePath : filePath + ".glb";

        var targetDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;

        // attempt to load the library manually just in case its not found
        string libName = "libassimp.so";

        if(OperatingSystem.IsMacOS())
            libName = "libassimp.dylib";
        else if(OperatingSystem.IsWindows())
            libName = "assimp.dll";

        try
        {
            AssimpLibrary.Instance.LoadLibrary(
                Path.Combine(targetDir, libName),
                Path.Combine(targetDir, libName)
            );
        }
        catch { }

        GltfModel mdl = new();

        using var importer = new AssimpContext();

        importer.SetConfig(new Assimp.Configs.RemoveDegeneratePrimitivesConfig(true));

        var scene = importer.ImportFile(
            path,
            PostProcessSteps.FindDegenerates |
            PostProcessSteps.FindInvalidData |
            PostProcessSteps.FlipUVs |
            PostProcessSteps.JoinIdenticalVertices |
            PostProcessSteps.ImproveCacheLocality |
            PostProcessSteps.OptimizeMeshes |
            PostProcessSteps.Triangulate
        );

        // Dictionary<string, Matrix> deformationBones = [];
        // Node rootBone = null!;
        List<Node> bones = [];

        // void findSkeleton()
        // {
        //     deformationBones = FindDeformationBones(scene);
        //     if (deformationBones.Count == 0)
        //         return;

        //     var rootBones = new HashSet<Node>();
        //     foreach (var boneName in deformationBones.Keys)
        //         rootBones.Add(FindRootBone(scene, boneName));

        //     if (rootBones.Count > 1)
        //         throw new Exception("Multiple skeletons found. Please ensure that the model does not contain more that one skeleton.");

        //     rootBone = rootBones.First();

        //     GetSubtree(rootBone, bones);
        // }
        // findSkeleton();

        GetSubtree(scene.RootNode, bones);

        mdl.Scene = scene;
        mdl._nodes = [..bones];
        mdl._isDirty = true;
        mdl.Build();

        return mdl;
    }

    private void Build()
    {
        if(!_isDirty) return;

        _meshes.Clear();

        foreach(var node in _nodes)
        {
            if(node.HasMeshes)
            {
                GltfMesh m = new();
                _meshes.Add(m);

                foreach(var meshIndex in node.MeshIndices)
                {
                    var mesh = Scene.Meshes[meshIndex];

                    List<VertexPositionColorNormalTexture> buff = [];

                    foreach(var vert in mesh.GetIndices() ?? [])
                    {
                        Vector2? texcoord = null;
                        if(mesh.HasTextureCoords(0) && mesh.TextureCoordinateChannels[0].Count > 0)
                        {
                            texcoord = mesh.TextureCoordinateChannels[0][vert].Truncate().ToXna();
                        }

                        Color? color = null;
                        if(mesh.HasVertexColors(0) && mesh.VertexColorChannels[0].Count > 0)
                        {
                            color = mesh.VertexColorChannels[0][vert].ToXna();
                        }

                        buff.Add(new(
                            mesh.Vertices[vert].ToXna(),
                            color ?? Color.Black,
                            Vector3.Normalize(mesh.Normals[vert].ToXna()),
                            texcoord ?? Vector2.Zero
                        ));
                    }

                    m.Buffer.AddRange(buff);
                    m.Node = node;
                }
            }
        }

        _isDirty = false;
    }

    public void Draw(GraphicsDevice graphicsDevice, Matrix world, Effect effect)
    {
        Build();

        var wParam = effect.Parameters["WorldMatrix"];

        foreach(var mesh in _meshes)
        {
            foreach(var pass in effect.CurrentTechnique.Passes)
            {
                GetNodeTransform(mesh.Node).Decompose(out var scale, out var rot, out var pos);
                Transform transform = new() {
                    Scale = scale,
                    Rotation = rot,
                    Position = pos,
                };

                wParam?.SetValue(Transform.Matrix * GetNodeTransform(mesh.Node));
                pass.Apply();

                graphicsDevice.DrawUserPrimitives(
                    Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                    [..mesh.Buffer],
                    0,
                    mesh.Buffer.Count / 3,
                    VertexPositionColorNormalTexture.VertexDeclaration
                );
            }
        }
    }

    private static Matrix GetNodeTransform(Node node)
    {
        var matrix = node.Transform.ToXna();
        if(node.Parent is not null)
        {
            matrix *= GetNodeTransform(node.Parent);
        }
        return matrix;
    }

    public static Dictionary<string, Matrix> FindDeformationBones(Scene scene)
    {
        var offsetMatrices = new Dictionary<string, Matrix>();
        if (scene.HasMeshes)
            foreach (var mesh in scene.Meshes)
                if (mesh.HasBones)
                    foreach (var bone in mesh.Bones)
                        if (!offsetMatrices.ContainsKey(bone.Name))
                            offsetMatrices[bone.Name] = bone.OffsetMatrix.ToXna();

        return offsetMatrices;
    }

    public static Node FindRootBone(Scene scene, string boneName)
    {
        Node node = scene.RootNode.FindNode(boneName);

        Node rootBone = node;
        while (node != scene.RootNode && !node.HasMeshes)
        {
            rootBone = node;

            node = node.Parent;
        }

        return rootBone;
    }

    public static void GetSubtree(Node node, List<Node> list)
    {
        list.Add(node);
        foreach (var child in node.Children)
            GetSubtree(child, list);
    }
}
