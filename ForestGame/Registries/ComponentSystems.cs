using ForestGame.Core;
using ForestGame.ComponentSystems;

namespace ForestGame.Registries;

public static class ComponentSystems
{
    public const string Player = "player";
    public const string MotorMovement = "motor_movement";
    public const string Actor = "actor";
    public const string ModelGraphics = "model_graphics";
    public const string ActorDeCollision = "actor_decollision";
    public const string ManipulatorRender = "manipulator_render";
    public const string ManipulatorTarget = "manipulator_target";
    public const string ActorSolidCollision = "actor_solid_collision";
    public const string ColliderSync = "collider_sync";
    public const string WorldLooping = "world_looping";

    internal static void Initialize()
    {
        Registry.Register<IComponentSystem>(WorldLooping, new WorldLoopingSystem());
        Registry.Register<IComponentSystem>(ColliderSync, new ColliderSyncSystem());
        Registry.Register<IComponentSystem>(Player, new PlayerSystem());
        Registry.Register<IComponentSystem>(MotorMovement, new MotorMovementSystem());
        Registry.Register<IComponentSystem>(ActorSolidCollision, new ActorSolidCollisionSystem());
        Registry.Register<IComponentSystem>(ActorDeCollision, new ActorDeCollisionSystem());
        Registry.Register<IComponentSystem>(Actor, new ActorSystem());
        Registry.Register<IComponentSystem>(ModelGraphics, new ModelGraphicsSystem());
        Registry.Register<IComponentSystem>(ManipulatorTarget, new ManipulatorTargetSystem());
        Registry.Register<IComponentSystem>(ManipulatorRender, new ManipulatorRenderSystem());
    }
}
