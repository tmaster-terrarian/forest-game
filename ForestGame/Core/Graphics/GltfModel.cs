using System.Reflection;
using Assimp;
using Assimp.Unmanaged;
using Microsoft.Xna.Framework;

namespace ForestGame.Core.Graphics;

public class GltfModel
{
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

        using var importer = new AssimpContext();

        importer.SetConfig(new Assimp.Configs.RemoveDegeneratePrimitivesConfig(true));

        var scene = importer.ImportFile(path,
            PostProcessSteps.FindDegenerates |
            PostProcessSteps.FindInvalidData |
            PostProcessSteps.FlipUVs |
            PostProcessSteps.FlipWindingOrder |
            PostProcessSteps.JoinIdenticalVertices |
            PostProcessSteps.ImproveCacheLocality |
            PostProcessSteps.OptimizeMeshes |
            PostProcessSteps.Triangulate
        );

        Dictionary<string, Matrix> deformationBones = [];
        Node rootBone = null;
        List<Node> bones = [];

        void findSkeleton()
        {
            deformationBones = FindDeformationBones(scene);
            if (deformationBones.Count == 0)
                return;

            var rootBones = new HashSet<Node>();
            foreach (var boneName in deformationBones.Keys)
                rootBones.Add(FindRootBone(scene, boneName));

            if (rootBones.Count > 1)
                throw new Exception("Multiple skeletons found. Please ensure that the model does not contain more that one skeleton.");

            rootBone = rootBones.First();

            GetSubtree(rootBone, bones);
        }
        findSkeleton();

        return null;
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
