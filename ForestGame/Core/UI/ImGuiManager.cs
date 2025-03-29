using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;

namespace ForestGame.Core.UI;

public static class ImGuiManager
{
    private static ImGuiRenderer _renderer;

    public class Panel(string name, Action<Panel> layoutBuilder)
    {
        public string Name => name;
        public void DoLayout() => layoutBuilder(this);

        public bool visible;
    }

    public static class Layout
    {
        public static bool Visible => Global.Editor;

        public static class Panels
        {
            public static Panel EcsDebug { get; } = new("arch debug", panel =>
            {
                if(ImGui.Begin("arch debug view", ref panel.visible))
                {
                    if(ImGui.BeginTabBar("ecs", ImGuiTabBarFlags.None))
                    {
                        if(ImGui.BeginTabItem("actors"))
                        {
                            List<EntityReference> entitiesList = [];
                            EcsManager.world.Query(QueryDescription.Null, (Entity entity) => {
                                entitiesList.Add(entity.Reference());
                            });

                            if(ImGui.BeginTable("ecs_scene_table", 3, ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable))
                            {
                                ImGui.TableSetupColumn("id");
                                ImGui.TableSetupColumn("prototype");
                                ImGui.TableSetupColumn("components");
                                ImGui.TableHeadersRow();

                                foreach(var entity in entitiesList.Select(e => e.Entity))
                                {
                                    ImGui.TableNextRow();

                                    ImGui.TableSetColumnIndex(0);
                                    ImGui.TextUnformatted(entity.Id.ToString());

                                    if(entity.TryGet<PrototypeIdentity>(out var prototype))
                                    {
                                        ImGui.TableSetColumnIndex(1);
                                        ImGui.TextUnformatted(prototype.Id.ToString());
                                    }

                                    ImGui.TableSetColumnIndex(2);
                                    ImGui.TextUnformatted(string.Join(", ", entity.GetComponentTypes().Select(t => t.Type.Name)));
                                }

                                ImGui.EndTable();
                            }

                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Broccoli"))
                        {
                            ImGui.Text("This is the Broccoli tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Cucumber"))
                        {
                            ImGui.Text("This is the Cucumber tab!\nblah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        ImGui.EndTabBar();
                    }

                    ImGui.End();
                }
            });
        }

        public static IEnumerable<Panel> GetPanels()
        {
            yield return Panels.EcsDebug;
        }
    }

    internal static void Initialize(Game1 game)
    {
        _renderer = new(game);
        _renderer.RebuildFontAtlas();
    }

    internal static void Draw()
    {
        if(!Layout.Visible)
            return;

        _renderer.BeginLayout(Time._gameTime);

        if(ImGui.BeginMainMenuBar())
        {
            foreach(var panel in Layout.GetPanels())
            {
                if(ImGui.MenuItem(panel.Name))
                    panel.visible = !panel.visible;

                if(panel.visible)
                    panel.DoLayout();
            }

            // ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 7);

            // if(ImGui.Button("x"))
            // {
            //     Layout.Visible = false;
            //     Global.LockMouse = true;
            // }

            ImGui.EndMainMenuBar();
        }

        _renderer.EndLayout();
    }
}
