using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class GraphicsUtil
{
    private static BasicEffect _basicEffect;
    private static Effect _vertSnapEffect;
    private static VertexPositionColor[] _gridVertices;
    private static bool _isInitialized = false;

    static GraphicsUtil()
    {
        _basicEffect = new(RenderPipeline.GraphicsDevice)
        {
            VertexColorEnabled = true
        };

        _vertSnapEffect = ContentLoader.Load<Effect>("fx/vertsnap")!;
        Vector2 vertexSnapRes = Vector2.Floor(RenderPipeline.Window.ClientBounds.Size.ToVector2() / RenderPipeline.ResolutionScale / 2);

        _vertSnapEffect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
        _vertSnapEffect.Parameters["MainTex"]?.SetValue(RenderPipeline.WhiteTexture);
    }

    public static void DrawGrid(GraphicsDevice graphicsDevice, int gridSize, float cellSize, Color color, Matrix world)
    {
        if (!_isInitialized)
        {
            // Color color = new Color(1f, 1f, 1f, 0.1f) * 0.1f;

            // Create grid vertices
            _gridVertices = new VertexPositionColor[(gridSize + 1) * 4];
            for (int i = 0; i <= gridSize; i++)
            {
                float position = i * cellSize;

                // Horizontal lines
                _gridVertices[i * 4] = new VertexPositionColor(new Vector3(0, position, 0), color);
                _gridVertices[i * 4 + 1] = new VertexPositionColor(new Vector3(gridSize * cellSize, position, 0), color);

                // Vertical lines
                _gridVertices[i * 4 + 2] = new VertexPositionColor(new Vector3(position, 0, 0), color);
                _gridVertices[i * 4 + 3] = new VertexPositionColor(new Vector3(position, gridSize * cellSize, 0), color);
            }

            _isInitialized = true;
        }

        _basicEffect.World = world;
        _basicEffect.View = RenderPipeline.ViewMatrix;
        _basicEffect.Projection = RenderPipeline.ProjectionMatrix;

        graphicsDevice.BlendState = BlendState.AlphaBlend;

        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _gridVertices, 0, _gridVertices.Length / 2);
        }

        graphicsDevice.BlendState = BlendState.Opaque;
    }

    public static void DrawQuad(GraphicsDevice graphicsDevice, Texture2D texture, Color color, Matrix worldMatrix, Effect effect, float width, float height, Vector2 pixelTopLeft, Vector2 pixelBottomRight)
    {
        Vector2 uvTopLeft = pixelTopLeft / texture.Bounds.Size.ToVector2();
        Vector2 uvBottomRight = pixelBottomRight / texture.Bounds.Size.ToVector2();

        Vector3 normal = Vector3.Forward;

        // Define the vertices of the quad
        VertexPositionColorNormalTexture[] vertices = [
            new(new Vector3(-width / 2, -height / 2, 0), color, normal, uvTopLeft),
            new(new Vector3(-width / 2, height / 2, 0), color, normal, new Vector2(uvTopLeft.X, uvBottomRight.Y)),
            new(new Vector3(width / 2, -height / 2, 0), color, normal, new Vector2(uvBottomRight.X, uvTopLeft.Y)),
            new(new Vector3(width / 2, height / 2, 0), color, normal, uvBottomRight)
        ];

        // Set up basic effect parameters
        // BasicEffect basicEffect = new(graphicsDevice)
        // {
        //     TextureEnabled = true,
        //     Texture = texture,
        //     VertexColorEnabled = true,
        //     World = worldMatrix,
        //     View = RenderPipeline.ViewMatrix,
        //     Projection = RenderPipeline.ProjectionMatrix
        // };

        effect.Parameters["WorldMatrix"]?.SetValue(worldMatrix);

        // Apply the basic effect
        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            // Draw the quad
            graphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleStrip,
                vertices,
                0, 2,
                VertexPositionColorNormalTexture.VertexDeclaration
            );
        }
    }

    public static void DrawVector(GraphicsDevice graphicsDevice, Vector3 origin, Vector3 direction, Color colorA, Color colorB)
    {
        // Draw the arrow body (line)
        VertexPositionColor[] vertices = [
            new VertexPositionColor(origin, colorA),
            new VertexPositionColor(origin + direction, colorB)
        ];

        graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
    }

    public static void DrawVector(GraphicsDevice graphicsDevice, Vector3 origin, Vector3 direction, Color color)
    {
        DrawVector(graphicsDevice, origin, direction, color, color);
    }

    public static void DrawVector(GraphicsDevice graphicsDevice, Vector3 origin, Vector3 direction, Color colorA, Color colorB, float length)
    {
        direction.Normalize();
        DrawVector(graphicsDevice, origin, direction * length, colorA, colorB);
    }

    public static void DrawVector(GraphicsDevice graphicsDevice, Vector3 origin, Vector3 direction, Color color, float length)
    {
        DrawVector(graphicsDevice, origin, direction, color, length);
    }

    public static void DrawGizmo(GraphicsDevice graphicsDevice, Vector3 origin, Matrix world)
    {
        _basicEffect.World = Matrix.CreateTranslation(origin) * world;
        _basicEffect.View = RenderPipeline.ViewMatrix;
        _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(30),
            graphicsDevice.Viewport.AspectRatio,
            0.01f, 300.0f
        );

        // Start rendering with the BasicEffect
        _basicEffect.CurrentTechnique.Passes[0].Apply();

        // Draw the arrows
        DrawVector(graphicsDevice, Vector3.Forward * 0.5f, Vector3.Forward, Color.Blue, 0.5f);   // forward arrow
        DrawVector(graphicsDevice, Vector3.Zero, Vector3.UnitX, Color.Red);    // X-axis arrow
        DrawVector(graphicsDevice, Vector3.Zero, Vector3.UnitY, Color.Lime);   // Y-axis arrow
        DrawVector(graphicsDevice, Vector3.Zero, Vector3.UnitZ, Color.Blue);   // Z-axis arrow
    }

    public static void DrawBoundingBox(GraphicsDevice graphicsDevice, BoundingBox boundingBox, Color baseColor, Color highlightedColor, float highlighted)
    {
        var min = boundingBox.Min;
        var max = boundingBox.Max;

        var rMin = min + new Vector3(MathF.Sin(Time.Elapsed * 3f) * 0.01f, MathF.Cos(Time.Elapsed * 3f) * 0.01f, MathF.Sin(Time.Elapsed * 3f) * 0.01f);
        var rMax = max - new Vector3(MathF.Sin(Time.Elapsed * 3f) * 0.01f, MathF.Cos(Time.Elapsed * 3f) * 0.01f, MathF.Sin(Time.Elapsed * 3f) * 0.01f);

        Color color = Color.Lerp(baseColor, highlightedColor, highlighted);

        var cornerColor = color * 1.2f;

        VertexPositionColor[] vertices = [
            new VertexPositionColor(rMin, cornerColor),
            new VertexPositionColor(rMin with { X = rMax.X } + MathUtil.RandomVector3(-0.01f, 0.01f), color),

            new VertexPositionColor(rMin, cornerColor),
            new VertexPositionColor(rMin with { Y = rMax.Y } + MathUtil.RandomVector3(-0.01f, 0.01f), color),

            new VertexPositionColor(rMin, cornerColor),
            new VertexPositionColor(rMin with { Z = rMax.Z } + MathUtil.RandomVector3(-0.01f, 0.01f), color),

            new VertexPositionColor(rMax, cornerColor),
            new VertexPositionColor(rMax with { X = rMin.X } + MathUtil.RandomVector3(-0.01f, 0.01f), color),

            new VertexPositionColor(rMax, cornerColor),
            new VertexPositionColor(rMax with { Y = rMin.Y } + MathUtil.RandomVector3(-0.01f, 0.01f), color),

            new VertexPositionColor(rMax, cornerColor),
            new VertexPositionColor(rMax with { Z = rMin.Z } + MathUtil.RandomVector3(-0.01f, 0.01f), color),
        ];

        _basicEffect.World = Matrix.Identity;
        _basicEffect.View = RenderPipeline.ViewMatrix;
        _basicEffect.Projection = RenderPipeline.ProjectionMatrix;

        graphicsDevice.BlendState = BlendState.Additive;

        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 6);
        }

        graphicsDevice.BlendState = BlendState.Opaque;
    }
}
