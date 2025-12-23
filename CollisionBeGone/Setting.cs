using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace CollisionBeGone
{
    [FileLocation(nameof(CollisionBeGone))]
    [SettingsUIGroupOrder(kToggleGroup)]
    [SettingsUIShowGroupName(kToggleGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kToggleGroup = "Toggle";

        public Setting(IMod mod) : base(mod)
        {
        }

        /// <summary>
        /// 启用/禁用 CollisionBeGone 功能
        /// </summary>
        [SettingsUISection(kSection, kToggleGroup)]
        public bool Enabled { get; set; } = true;

        public override void SetDefaults()
        {
            Enabled = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Collision Be Gone" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Settings" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Enabled)), "Enable Collision Be Gone" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Enabled)), "When enabled, vehicles will ignore other vehicles and drive through them. Traffic signals, pedestrians, and trains are still respected." },
            };
        }

        public void Unload()
        {
        }
    }
}
