using ForestGame.Core;
using ForestGame.ComponentSystems;

namespace ForestGame.Registries;

public static class ComponentSystems
{
    public const string Actor = "actor";
    public const string Player = "player";
    public const string Physics = "physics";
    public const string ModelGraphics = "model_graphics";
    public const string Bouncing = "bouncing";

    internal static void Initialize()
    {
        Registry.Register<IComponentSystem>(Actor, new ActorSystem());
        Registry.Register<IComponentSystem>(Player, new PlayerSystem());
        Registry.Register<IComponentSystem>(Physics, new PhysicsSystem());
        Registry.Register<IComponentSystem>(ModelGraphics, new ModelGraphicsSystem());
        Registry.Register<IComponentSystem>(Bouncing, new BouncingSystem());
    }
}
