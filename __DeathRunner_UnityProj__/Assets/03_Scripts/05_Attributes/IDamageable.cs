using GenericScriptableArchitecture;

using F32 = System.Single;

namespace DeathRunner.Attributes
{
    public interface IDamageable
    {
        public Reference<F32> InvincibilityFrameDuration { get; }
        
        public EventReference OnInvincibilityEnabled  { get; }
        public EventReference OnInvincibilityDisabled { get; }
    }
}