using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.UIToolkitHelpers
{
    public class StatGraph : VisualElement
    {
        private readonly VisualElement _container;
        private StatData[] _statData;

        private List<StatGroup> _statGroups;
        private Vector3 _graphCenter;

        private readonly float _lineBuffer;
        private readonly float _radialOffset;
        private readonly float _labelOffset;
        private readonly bool _isSmooth;
        private readonly Color _graphColor, _outlineColor;
        private readonly int _maxValue;
        private readonly bool _showStatLines;

        public StatGraph(Color graphColor, Color outlineColor, int maxValue, float labelOffset = 0.2f, float radialOffset = -90f, float lineBuffer = 0.5f, bool showStatLines = true)
        {
            _graphColor = graphColor;
            _outlineColor = outlineColor;
            _maxValue = maxValue;
            _showStatLines = showStatLines;

            _lineBuffer = lineBuffer;
            _radialOffset = radialOffset;
            _labelOffset = labelOffset;
            _isSmooth = false;

            var box = UIHelpers.Create("stat-box");

            _container = UIHelpers.Create("stat-container");

            box.Add(_container);

            Add(box);

            _container.generateVisualContent += GenerateVisualContent;
        }

        public void SetStats(StatData[] statData)
        {
            _statData = statData;
            SetupContainer();
        }

        private void SetupContainer()
        {
            _container.Clear();

            var shortest = parent != null ? Mathf.Min(parent.resolvedStyle.width, parent.resolvedStyle.height) : 300f;
            var graphSize = shortest - 50;

            _container.style.width = graphSize;
            _container.style.height = graphSize;

            _graphCenter = Vector3.one * (graphSize / 2);
            _graphCenter.z = 0;

            var statCount = _statData.Length;
            var lineLength = graphSize * 0.4f;

            var angleDelta = 360f / statCount;
            var extent = Vector3.right * lineLength;

            _statGroups = new List<StatGroup>();

            for (var i = 0; i < statCount; i++)
            {
                var group = new StatGroup()
                {
                    Data = _statData[i],
                    End = _graphCenter + GetEndPoint(i),
                    NextEnd = _graphCenter + GetEndPoint(i + 1),
                };

                group.Point = Vector3.Lerp(_graphCenter, group.End, (group.Data.Value + _lineBuffer) / _maxValue);

                const float LABEL_CONTAINER_SIZE = 20f;
                var labelPosition = Vector3.LerpUnclamped(_graphCenter, group.End, 1 + _labelOffset) - new Vector3(LABEL_CONTAINER_SIZE, LABEL_CONTAINER_SIZE, 0) * 0.5f;

                var labelContainer = UIHelpers.Create("label-container");
                labelContainer.style.width = LABEL_CONTAINER_SIZE;
                labelContainer.style.height = LABEL_CONTAINER_SIZE;
                labelContainer.style.left = labelPosition.x;
                labelContainer.style.top = labelPosition.y;
                labelContainer.Add(new Label(group.Data.ShortName));

                _container.Add(labelContainer);

                _statGroups.Add(group);
            }

            Vector3 GetEndPoint(int index) => Quaternion.AngleAxis(angleDelta * index + _radialOffset, Vector3.forward) * extent;

            // Refresh the painter
            _container.MarkDirtyRepaint();
        }

        private void GenerateVisualContent(MeshGenerationContext ctx)
        {
            var painter = ctx.painter2D;

            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Round;
            painter.lineWidth = 2;

            // Draw grid lines
            for (var i = 0; i < _statGroups.Count(); i++)
            {
                var group = _statGroups[i];

                if (_showStatLines)
                {
                    var lineColor = _outlineColor;
                    lineColor.a = 0.5f;

                    painter.strokeColor = lineColor;
                    painter.BeginPath();
                    painter.MoveTo(_graphCenter);
                    painter.LineTo(group.End);
                    painter.Stroke();
                }

                painter.strokeColor = _outlineColor;
                painter.BeginPath();
                painter.MoveTo(group.End);
                painter.LineTo(group.NextEnd);
                painter.Stroke();
            }

            painter.lineWidth = 3;
            painter.fillColor = _graphColor;

            painter.BeginPath();
            painter.lineWidth = 3;

            for (var i = 0; i < _statGroups.Count; i++)
            {
                if (_isSmooth)
                {
                    // Work this out at some stage
                    painter.LineTo(_statGroups[(i + 1) % _statGroups.Count].Point);
                }
                else
                {
                    painter.LineTo(_statGroups[(i + 1) % _statGroups.Count].Point);
                }
            }

            if (!_isSmooth) painter.ClosePath();
            painter.Fill();
        }
        
        private struct StatGroup
        {
            public StatData Data;
            public Vector3 End;
            public Vector3 NextEnd;
            public Vector3 Point;
        }
    }
    
    public class StatData
    {
        public StatData(string name, int value)
        {
            Name = name;
            Value = value;
        }

        private readonly string Name;
        public readonly int Value;

        public string ShortName => (Name.Length > 3 ? Name[..3] : Name).ToUpper();
    }
}