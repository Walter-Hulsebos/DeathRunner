using Leopotam.Ecs;

namespace Asset_Cleaner {
    internal class InSceneResult : IEcsAutoReset {
        public string ScenePath;

        public void Reset() {
            ScenePath = default;
        }
    }
}