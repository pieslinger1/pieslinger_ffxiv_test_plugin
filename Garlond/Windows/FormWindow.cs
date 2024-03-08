using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Threading;

namespace Garlond.Windows;

class FormFields{
    public string Title;
    public string Body;
    public FormFields(){
        this.Title = "";
        this.Body = "";
    }
}

public class FormWindow : Window, IDisposable
{
    private Configuration Configuration;
    private FormFields FormFields;
    private bool isSubmitting;
    private DateTime lastSubmissionTime;

    public FormWindow(Plugin plugin) : base(
        "A Wonderful Window for form",
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
        this.FormFields = new FormFields();
    }

    public void Dispose() { }

    public override void Draw()
    {
        if(isSubmitting){
            // check the time since the last submission
            if(this.lastSubmissionTime.AddSeconds(2).CompareTo(DateTime.UtcNow) < 0){
                ImGui.Text("Closing");
                isSubmitting = false;
                this.FormFields = new FormFields();
                this.IsOpen = false;
            }
            else{
                ImGui.Text("In the 2 second confirmation time");
                // display the information that got submitted and a confirmation saying this window will close in 3 seconds
            }
        }
        else{
            var shortFormTextValue = this.FormFields.Title;
            var longFormTextValue = this.FormFields.Body;


            float fullWidth = ImGui.GetContentRegionAvail().X;
            ImGui.SetNextItemWidth(fullWidth);
            ImGui.InputText("##shortFormTextValue", ref shortFormTextValue, 50);

            // Set width for the multi-line input
            ImGui.SetNextItemWidth(fullWidth);
            ImGui.InputTextMultiline("##inputLabel", ref longFormTextValue, 256, new Vector2(fullWidth, 100), ImGuiInputTextFlags.None, null);

            if(shortFormTextValue != this.FormFields.Title){
                this.FormFields.Title = shortFormTextValue;
            }
            if(longFormTextValue != this.FormFields.Body){
                this.FormFields.Body = longFormTextValue;
            }

            if(ImGui.Button("Submit")){
                // Send the form to the rest API
                this.isSubmitting = true;
                this.lastSubmissionTime = DateTime.UtcNow;
            }
        }
    }
}
