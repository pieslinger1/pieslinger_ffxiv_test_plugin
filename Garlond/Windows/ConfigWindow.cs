using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Garlond.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "A Wonderful Configuration Window",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        /*
        this.Size = new Vector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;
        */
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = this.Configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Configs Bool", ref configValue))
        {
            this.Configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }
        var textConfigValue = this.Configuration.TextProperty;
        ImGui.InputText("##textConfigValue", ref textConfigValue, 50);
        if(this.Configuration.TextProperty != textConfigValue){
            this.Configuration.TextProperty = textConfigValue;
            this.Configuration.Save();
        }


        ImGui.Text($"Select the time zone that makes the displayed day and time below match your local day and time.\n{DateTime.UtcNow.AddHours(this.Configuration.TimeZoneUTC)}");
        /*

        if(ImGui.RadioButton("UTC -2", this.Configuration.TimeZoneUTC == -2)){
            this.Configuration.TimeZoneUTC = -2;
            this.Configuration.Save();
        }
        if(ImGui.RadioButton("UTC -3", this.Configuration.TimeZoneUTC == -3)){
            this.Configuration.TimeZoneUTC = -3;
            this.Configuration.Save();
        }
        if(ImGui.RadioButton("UTC -4", this.Configuration.TimeZoneUTC == -4)){
            this.Configuration.TimeZoneUTC = -4;
            this.Configuration.Save();
        }
        if(ImGui.RadioButton("UTC -5", this.Configuration.TimeZoneUTC == -5)){
            this.Configuration.TimeZoneUTC = -5;
            this.Configuration.Save();
        }
        */

        int temporaryTimeZoneUTC = this.Configuration.TimeZoneUTC; // TODO change this to be based on system time to default to the correct timezone, or even just dont provide the option to change timezone off of system time

        ImGui.RadioButton("UTC -2", ref temporaryTimeZoneUTC, -2);
        ImGui.RadioButton("UTC -3", ref temporaryTimeZoneUTC, -3);
        ImGui.RadioButton("UTC -4", ref temporaryTimeZoneUTC, -4);
        ImGui.RadioButton("UTC -5", ref temporaryTimeZoneUTC, -5);

        if(this.Configuration.TimeZoneUTC != temporaryTimeZoneUTC){
            this.Configuration.TimeZoneUTC = temporaryTimeZoneUTC;
            this.Configuration.Save();
        }

        float temp_time_zone_float = this.Configuration.TimeZoneUTC;
        ImGui.SliderFloat("Time Zone", ref temp_time_zone_float, -14, 14, "%.0f");
        if(temp_time_zone_float != this.Configuration.TimeZoneUTC){
            this.Configuration.TimeZoneUTC = (int) temp_time_zone_float;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("another Config Bool", ref configValue))
        {
            this.Configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

    }
}
