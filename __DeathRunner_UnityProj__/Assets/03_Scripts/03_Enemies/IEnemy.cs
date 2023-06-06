using Unity.Mathematics;

namespace DeathRunner.Enemies
{
    public interface IEnemy
    {
        void SetTargetPosition(float3 pos);
    }
}