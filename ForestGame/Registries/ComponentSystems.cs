using ForestGame.Core;
using ForestGame.ComponentSystems;

namespace ForestGame.Registries;

public static class ComponentSystems
{
    public const string Actor = "actor";
    public const string Player = "player";
    public const string ModelGraphics = "model_graphics";
    public const string MotorMovement = "motor_movement";
    public const string ActorDeCollision = "actor_decollision";

    internal static void Initialize()
    {
        Registry.Register<IComponentSystem>(Player, new PlayerSystem());
        Registry.Register<IComponentSystem>(MotorMovement, new MotorMovementSystem());
        Registry.Register<IComponentSystem>(Actor, new ActorSystem());
        Registry.Register<IComponentSystem>(ModelGraphics, new ModelGraphicsSystem());
        Registry.Register<IComponentSystem>(ActorDeCollision, new ActorDeCollisionSystem());
    }
}
