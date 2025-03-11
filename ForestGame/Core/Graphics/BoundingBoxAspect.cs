using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class BoundingBoxAspect : Aspect
{
    private readonly List<(Collider Collider, float HighlightAmount, float Opacity, bool LinePath, bool Solid)> _toDraw = [];
    private int _index = 0;

    protected override void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect)
    {
        var baseColor = Color.Orange * 0.95f * 0.8f * _toDraw[_index].Opacity;
        var highlightedColor = Color.Yellow * 0.95f;
        if(_toDraw[_index].Solid)
        {
            baseColor = Color.Red * 0.95f * 0.8f * _toDraw[_index].Opacity;
            highlightedColor = Color.OrangeRed * 0.95f;
        }
        var bbox = _toDraw[_index].Collider.BoundingBox(transform.Scale);

        GraphicsUtil.DrawBoundingBox(
            graphicsDevice,
            bbox,
            baseColor,
            highlightedColor,
            _toDraw[_index].HighlightAmount
        );
        if(_toDraw[_index].LinePath)
        {
            var camTransform = RenderPipeline.Camera.Transform with { };
            var camPos = camTransform.WorldPosition;
            camPos += RenderPipeline.Camera.Forward * 0.12f - RenderPipeline.Camera.Up * 0.05f;

            var direction = bbox.Median() - camPos;

            GraphicsUtil.DrawVector(
                graphicsDevice,
                camPos,
                direction / 2 * MathF.Pow(MathHelper.Max(0, _toDraw[_index].Opacity - 0.3f), 2f),
                highlightedColor * (0.05f + 0.95f * MathF.Pow(Math.Max(0, Vector3.Dot(RenderPipeline.Camera.Forward, Vector3.Normalize(direction))), 2f)),
                Color.Lerp(baseColor, highlightedColor, _toDraw[_index].HighlightAmount)
            );
        }
        _index++;
    }

    public void Submit(Transform transform, Collider collider, float highlightAmount, float opacity, bool linePath, bool solid)
    {
        Submit(transform);
        _toDraw.Add((collider, highlightAmount, opacity, linePath, solid));
    }

    public override void Clear()
    {
        base.Clear();
        _toDraw.Clear();
        _index = 0;
    }
}
