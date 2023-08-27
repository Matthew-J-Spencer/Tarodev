using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.UIToolkitHelpers
{
    public static class UIHelpers
    {
        public static VisualElement Create(params string[] classNames)
        {
            return Create<VisualElement>(classNames);
        }

        public static T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            var ele = new T();
            foreach (var className in classNames)
            {
                ele.AddToClassList(className);
            }
            return ele;
        }

        // Animations
        public static void Fade(this VisualElement element, bool on, Action onComplete = null, float duration = 0.2f)
        {
            var tween = element.experimental.animation.Start(on ? 0 : 1, on ? 1 : 0, (int)(duration * 1000), (e, v) => e.style.opacity = new StyleFloat(v));
            if (onComplete != null) tween.OnCompleted(onComplete);
        }

        public static void AnimateIn(this VisualElement newElement, Vector2 dir, float duration = 1f)
        {
            var targetInWorldSpace = newElement.parent.LocalToWorld(newElement.layout.position);
            var goalPosition = newElement.parent.parent.WorldToLocal(targetInWorldSpace);

            newElement.experimental.animation.Position(goalPosition, (int)(duration * 1000)).from = goalPosition + dir.normalized * 1000;
        }
    }
}