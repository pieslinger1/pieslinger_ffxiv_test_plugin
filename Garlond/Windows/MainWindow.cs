using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Garlond.Windows;

public class MainWindow : Window, IDisposable
{
    private IDalamudTextureWrap GoatImage;
    private Plugin Plugin;

    public MainWindow(Plugin plugin, IDalamudTextureWrap goatImage) : base(
        "My Amazing Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.GoatImage = goatImage;
        this.Plugin = plugin;
    }

    public void Dispose()
    {
        this.GoatImage.Dispose();
    }

    public override void Draw()
    {

        ImGui.BeginTabBar("Tab Bar Name");

        if (ImGui.BeginTabItem("Tab 1"))
        {
            DrawTab1();
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Tab 2"))
        {
            DrawTab2();
            ImGui.EndTabItem();
        }


        ImGui.EndTabBar();


        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        ImGui.Text($"The text config is {this.Plugin.Configuration.TextProperty}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }
        if (ImGui.Button("Open Form"))
        {
            this.Plugin.DrawFormUI();
        }


        ImGui.Spacing();

        ImGui.TextUnformatted("Have a big goat:");
        ImGui.Indent(55);
        ImGui.Image(this.GoatImage.ImGuiHandle, new Vector2(this.GoatImage.Width, this.GoatImage.Height));
        ImGui.Unindent(55);
        ImGui.TextUnformatted("Here's more text");

        ImGui.TextUnformatted("I see your health as " + Plugin.get_my_health().ToString());
        ImGui.TextUnformatted("I see your name as " + Plugin.get_my_name().ToString());
        ImGui.TextUnformatted("I see your world as " + Plugin.get_my_world());
        ImGui.TextUnformatted($"UTC timezone is set to {this.Plugin.Configuration.TimeZoneUTC}");

        if(ImGui.Button("Send a prewritten message")){
            Plugin.send_chat_message("/tell Satoru Gojou@Coeurl We've been trying to reach you regarding your car's insurance.");
        }

        if(ImGui.Button("Copy some stuff to clipboard")){
            ImGui.SetClipboardText("/tell Satoru Gojou@Coeurl We've been trying to reach you regarding your car's extended warranty.");
        }

        if(ImGui.Button("Send JSON to the API")){
            this.Plugin.send_json();
        }
        if(ImGui.Button("Get JSON from the API")){
            this.Plugin.get_json();
        }
        

    }

    private void DrawTab1(){
        ImGui.Spacing();
        ImGui.SetWindowFontScale(1.5f);
        ImGui.TextUnformatted("Tab1");
    }

    private void DrawTab2(){
        ImGui.Spacing();
        ImGui.SetWindowFontScale(1.5f);
        ImGui.TextUnformatted("Tab2");
    }
}
