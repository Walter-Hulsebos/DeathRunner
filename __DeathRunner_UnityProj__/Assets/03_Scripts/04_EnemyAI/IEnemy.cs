using Unity.Mathematics;

namespace DeathRunner.EnemyAI
{
    public interface IEnemy
    {
        void SetTargetPosition(float3 pos);
    }
}