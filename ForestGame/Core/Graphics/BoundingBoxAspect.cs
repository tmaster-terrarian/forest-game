using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class BoundingBoxAspect : Aspect
{
    private readonly List<(Collider Collider, float HighlightAmount, float Opacity, bool LinePath)> _toDraw = [];
    private int _index = 0;

    protected override void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect)
    {
        var baseColor = Color.Orange * 0.95f * 0.8f * _toDraw[_index].Opacity;
        var highlightedColor = Color.Yellow * 0.95f;
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
            camTransform.Position += -Vector3.UnitX * 0.2f;
            var camPos = camTransform.WorldPosition;
            camPos.Y -= 0.1f;

            var direction = bbox.Median() - camPos;

            GraphicsUtil.DrawVector(
                graphicsDevice,
                camPos,
                direction / 2 * MathF.Pow(MathHelper.Max(0, _toDraw[_index].Opacity - 0.3f), 2f),
                highlightedColor,
                Color.Lerp(baseColor, highlightedColor, _toDraw[_index].HighlightAmount)
            );
        }
        _index++;
    }

    public void Submit(Transform transform, Collider collider, float highlightAmount, float opacity, bool linePath)
    {
        Submit(transform);
        _toDraw.Add((collider, highlightAmount, opacity, linePath));
    }

    public override void Clear()
    {
        base.Clear();
        _toDraw.Clear();
        _index = 0;
    }
}
