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
    public const string ActorSolidCollisionSystem = "actor_solid_collision_system";
    public const string ColliderSyncSystem = "collider_sync_system";

    internal static void Initialize()
    {
        Registry.Register<IComponentSystem>(ColliderSyncSystem, new ColliderSyncSystem());
        Registry.Register<IComponentSystem>(Player, new PlayerSystem());
        Registry.Register<IComponentSystem>(MotorMovement, new MotorMovementSystem());
        Registry.Register<IComponentSystem>(ActorSolidCollisionSystem, new ActorSolidCollisionSystem());
        Registry.Register<IComponentSystem>(ActorDeCollision, new ActorDeCollisionSystem());
        Registry.Register<IComponentSystem>(Actor, new ActorSystem());
        Registry.Register<IComponentSystem>(ModelGraphics, new ModelGraphicsSystem());
    }
}
