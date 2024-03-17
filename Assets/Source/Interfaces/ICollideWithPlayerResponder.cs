using Source.Actors;

namespace Source.Interfaces
{
    /// <summary>
    /// Requires the CollideWithPlayer component.  Allows other component to respond to
    /// player collision.
    /// </summary>
    public interface ICollideWithPlayerResponder
    {
        void CollideWithPlayer(Player playerComponent);
    }
}