using Source.Actors;

namespace Source.Interfaces
{
    /// <summary>
    /// Used for any component that will do damage to the player
    /// </summary>
    public interface IEnemyProjectile
    {
        void HitPlayer(Player playerComponent);
    }
}