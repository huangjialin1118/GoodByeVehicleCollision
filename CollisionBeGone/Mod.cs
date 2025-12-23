using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Simulation;

namespace CollisionBeGone
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(CollisionBeGone)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        // 公开静态引用，供 CollisionBeGoneSystem 访问
        public static Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(CollisionBeGone), m_Setting, new Setting(this));

            // 注册 CollisionBeGone 系统
            // 执行顺序：CarNavigationSystem → CollisionBeGoneSystem → CarMoveSystem
            // 使用两个类型参数：第一个是要注册的系统，第二个是要在其之前运行的目标系统
            updateSystem.UpdateBefore<CollisionBeGoneSystem, CarMoveSystem>(SystemUpdatePhase.GameSimulation);
            log.Info("CollisionBeGoneSystem registered before CarMoveSystem");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
