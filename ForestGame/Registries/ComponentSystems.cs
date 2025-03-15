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
    public const string Chaser = "chaser";

    internal static void Initialize()
    {
        Registry.Register<ISystem>(WorldLooping, new WorldLoopingSystem());
        Registry.Register<ISystem>(ColliderSync, new ColliderSyncSystem());
        Registry.Register<ISystem>(Player, new PlayerSystem());
        Registry.Register<ISystem>(Chaser, new ChaserSystem());
        Registry.Register<ISystem>(MotorMovement, new MotorMovementSystem());
        Registry.Register<ISystem>(ActorSolidCollision, new ActorSolidCollisionSystem());
        Registry.Register<ISystem>(ActorDeCollision, new ActorDeCollisionSystem());
        Registry.Register<ISystem>(Actor, new ActorSystem());
        Registry.Register<ISystem>(ModelGraphics, new ModelGraphicsSystem());
        Registry.Register<ISystem>(ManipulatorTarget, new ManipulatorTargetSystem());
        Registry.Register<ISystem>(ManipulatorRender, new ManipulatorRenderSystem());
    }
}
