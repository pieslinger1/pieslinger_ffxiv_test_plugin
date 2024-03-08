using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game.Text;
using Garlond.Windows;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Garlond
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Garlond";
        private const string CommandName = "/garlond";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private IClientState ClientState {get; init; }
        private IChatGui ChatGui {get; init; }

        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Garlond");

        private FormWindow FormWindow { get; init; }
        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IClientState clientState,
            [RequiredVersion("1.0")] IChatGui chatGui
            )
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ClientState = clientState;
            this.ChatGui = chatGui;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            FormWindow = new FormWindow(this);
            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, goatImage);
            

            WindowSystem.AddWindow(FormWindow);
            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            FormWindow.Dispose();
            ConfigWindow.Dispose();
            MainWindow.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
        public void DrawFormUI()
        {
            FormWindow.IsOpen = true;
        }
        public uint get_my_health(){
            var char_instance = new CharacterData();
            return char_instance.Health;
        }
        public string get_my_name(){
            var name = this.ClientState.LocalPlayer?.Name.ToString();
            if(name is null){
                return "NULL NAME";
            }
            else{
                return name;
            }
        }
        public string get_my_world(){
            var player_home_world = this.ClientState.LocalPlayer?.HomeWorld;
            if(player_home_world is null){
                return "NULL HOME WORLD";
            }
            var home_world_data = player_home_world.GameData;
            if(home_world_data is null){
                return "NULL HOME WORLD DATA";
            }
            return home_world_data.Name;
        }
        public void send_chat_message(string message){
            //Services.XivCommon.Functions.Chat.SendMessage($"/tell {name}@{homeworldName}");
            //Services.XivCommon.Functions.Chat.SendMessage("/tell Satoru Gojou@Coeurl Hello");

            //ChatGui.Print(message);
            ChatGui.Print(new XivChatEntry(){
                Type = XivChatType.TellIncoming,
                Message = message
            });
            ChatGui.Print(new XivChatEntry(){
                Type = XivChatType.TellOutgoing,
                Message = message
            });
            ChatGui.Print(new XivChatEntry(){
                Type = XivChatType.Echo,
                Message = message
            });
        }
        public void send_json(){
            Task.Run(() => SendJsonAsync()).Wait();
        }

        public static async Task SendJsonAsync(){
            var httpClient = new HttpClient();
            var json = "{\"key\": \"value\"}"; // Example JSON data
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Replace "http://localhost:5000/submit" with the actual URL of your Python REST API
                var response = await httpClient.PostAsync("https://tataru-taru-rest-api-pieslinger.replit.app/submit", content);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        public void get_json(){
            string url = "https://tataru-taru-rest-api-pieslinger.replit.app/get_json";
            string result = GetDataWithRetry(url, TimeSpan.FromSeconds(1), 3, TimeSpan.FromSeconds(3));
            Console.WriteLine(result);
            send_chat_message(result.ToString());
        }

        public static string GetDataWithRetry(string url, TimeSpan timeout, int maxRetries, TimeSpan delay)
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                using (var cts = new CancellationTokenSource())
                {
                    Task<string> task = GetDataAsync(url, cts.Token);
                    bool isCompletedSuccessfully = Task.WaitAny(new[] { task }, timeout) == 0;

                    if (isCompletedSuccessfully && task.Status == TaskStatus.RanToCompletion)
                    {
                        return task.Result; // Successfully completed within timeout
                    }
                    else
                    {
                        // Operation did not complete within timeout, cancel and retry
                        cts.Cancel();
                        Console.WriteLine($"Attempt {attempt + 1} failed. Retrying after delay...");
                        Thread.Sleep(delay); // Wait before retrying
                    }
                }
            }
            throw new TimeoutException("Operation failed to complete within the allotted retries and timeout.");
        }

        public static async Task<string> GetDataAsync(string url, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
