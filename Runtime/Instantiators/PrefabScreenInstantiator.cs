using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Yans.UI.UIScreens;

namespace Yans.UI
{
    public class PrefabScreenInstantiator : ScreenInstantiator
    {
        #region private fields

        [SerializeField]
        private List<OrientedScreenReference> _prefabs;

        private List<OrientedScreenReference> _instantiatedPrefabs = new();
        #endregion

        #region public methods

        public override async UniTask<T> InstantiateScreen<T>(Transform parent, ScreenOrientation screenOrientation)
        {
            var screen = (T)await InstantiateScreen(typeof(T), parent, screenOrientation);
            return screen;
        }

        public override UniTask<UIScreen> InstantiateScreen(Type type, Transform parent, ScreenOrientation screenOrientation)
        {
            ScreenOrientation currentOrientation = screenOrientation;
            OrientedScreenReference prefab = null;

            while (currentOrientation != ScreenOrientation.AutoRotation)
            {
                prefab = _prefabs.Find(x => type.IsInstanceOfType(x.screen) && x.orientation == currentOrientation);
                if (prefab != null)
                    break;

                currentOrientation = GetFallbackOrientation(currentOrientation);
            }

            var reference = _instantiatedPrefabs.Find(x => type.IsInstanceOfType(x.screen) && x.orientation == currentOrientation);
            if (reference != null) return UniTask.FromResult(reference.screen);

            prefab ??= _prefabs.Find(x => type.IsInstanceOfType(x.screen) && x.orientation == ScreenOrientation.AutoRotation);

            if (prefab == null)
                throw new ArgumentException($"Screen of type {type} not found for orientation {screenOrientation} or any fallback orientation.");

            var instance = CreateScreenInstance(prefab.screen, parent);

            _instantiatedPrefabs.Add(new OrientedScreenReference() { orientation = prefab.orientation, screen = instance });
            return UniTask.FromResult(instance);
        }

        public override void CleanUpScreen(UIScreen screen)
        {
            _instantiatedPrefabs.RemoveAll(e => e.screen == screen);
            Destroy(screen.gameObject);
        }

        #endregion

        #region protected methods

        protected virtual UIScreen CreateScreenInstance(UIScreen prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        #endregion

        [Serializable]
        public class OrientedScreenReference
        {
            #region public fields
            public ScreenOrientation orientation = ScreenOrientation.AutoRotation;
            public UIScreen screen;
            #endregion
        }
    }
}