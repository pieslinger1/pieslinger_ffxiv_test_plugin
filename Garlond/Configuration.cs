﻿using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace Garlond
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;
        public int TimeZoneUTC { get; set; } = (int) TimeZoneInfo.Local.BaseUtcOffset.Hours;
        public string TextProperty { get; set; } = "default value";

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
