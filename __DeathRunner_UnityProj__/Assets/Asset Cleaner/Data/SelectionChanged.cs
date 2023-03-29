using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asset_Cleaner {
    internal class SelectionChanged : IEcsAutoReset {
        public Object Target;
        public Scene Scene;
        public FindModeEnum From;

        public void Reset() {
            Target = default;
            Scene = default;
            From = default;
        }
    }
}