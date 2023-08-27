using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.UIToolkitHelpers
{
    public class DashedBox : VisualElement
    {
        private readonly VisualElement _box;
        private readonly Color _color;
        private readonly float _dashWidth;
        private readonly float _dashLength;
        private readonly float _resolution;
        private readonly float _dashHalfLength;

        public DashedBox(VisualElement contentElement, Color color, float dashWidth = 3, float dashLength = 5, int resolution = 3)
        {
            _color = color;
            _dashWidth = dashWidth;
            _dashLength = dashLength;
            _resolution = resolution;
            _dashHalfLength = dashLength / 2;

            _box = new VisualElement();
            _box.AddToClassList("dashed-box");
            _box.Add(contentElement);

            Add(_box);

            _box.generateVisualContent += GenerateVisualContent;
        }

        private void GenerateVisualContent(MeshGenerationContext ctx)
        {
            var offset = new Vector2(5, 0);
            var width = _box.resolvedStyle.width - 10;
            var height = _box.resolvedStyle.height;

            var painter = ctx.painter2D;

            painter.strokeColor = _color;
            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Round;
            painter.lineWidth = _dashWidth;

            var topLeft = Vector2.zero + offset;
            var topRight = new Vector2(width, 0) + offset;
            var bottomRight = new Vector2(width, height) + offset;
            var bottomLeft = new Vector2(0, height) + offset;

            DrawDashed(topLeft, topRight);
            DrawDashed(topRight, bottomRight);
            DrawDashed(bottomRight, bottomLeft);
            DrawDashed(bottomLeft, topLeft);

            DrawCorner(topLeft, Vector2.down, Vector2.right);
            DrawCorner(topRight, Vector2.down, Vector2.left);
            DrawCorner(bottomRight, Vector2.up, Vector2.left);
            DrawCorner(bottomLeft, Vector2.up, Vector2.right);

            void DrawCorner(Vector2 point, Vector2 dir1, Vector2 dir2)
            {
                painter.BeginPath();
                painter.MoveTo(point - dir1 * _dashLength);
                painter.BezierCurveTo(point - dir1 * _dashLength, point, point + dir2 * _dashLength);
                painter.Stroke();
            }

            void DrawDashed(Vector2 start, Vector2 end)
            {
                var dir = (end - start).normalized;
                var dashOffset = dir * _dashHalfLength;

                start += dashOffset;
                end -= dashOffset;

                var length = Vector2.Distance(start, end);
                var dashAmount = (int)(length / _dashLength / _resolution);

                for (var i = 1f; i < dashAmount; i++)
                {
                    var dashPoint = Vector2.Lerp(start, end, i / dashAmount);

                    painter.BeginPath();
                    painter.MoveTo(dashPoint - dashOffset);
                    painter.LineTo(dashPoint + dashOffset);
                    painter.Stroke();
                }
            }
        }
    }
}