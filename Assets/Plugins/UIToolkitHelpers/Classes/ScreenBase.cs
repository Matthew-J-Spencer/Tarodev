using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tarodev.UIToolkitHelpers
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ScreenBase : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet _styleSheet;
        [SerializeField] private bool _fadeIn = true;

        protected VisualElement Root { get; private set; }

        protected virtual void OnEnable() => StartCoroutine(SetupAndGenerate());

        protected virtual void OnValidate()
        {
            if (Application.isPlaying || !gameObject.activeSelf) return;
            StartCoroutine(SetupAndGenerate());
        }

        private IEnumerator SetupAndGenerate()
        {
            if (!_document) yield break;
            yield return null;
            Root = _document.rootVisualElement;
            Root.Clear();

            if (_fadeIn && Application.isPlaying) Root.Fade(true);

            if (_styleSheet) Root.styleSheets.Add(_styleSheet);
            Root.AddToClassList("root");

            yield return null;
            GenerateInterface();
        }

        /// <summary>
        /// Set as a coroutine to allow frame waits. UI Toolkit can be a bit funny sometimes
        /// </summary>
        protected abstract void GenerateInterface();

        public void DisableScreen()
        {
            if (!_fadeIn) gameObject.SetActive(false);
            else Root.Fade(false, () => gameObject.SetActive(false));
        }
    }
}