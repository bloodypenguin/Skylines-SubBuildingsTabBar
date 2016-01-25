using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace SubBuildingsTabBar
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private const string GO_NAME = "SubBuildingsMonitor";
        private static SubBuildingsTabstrip tabs;


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            if (tabs == null)
            {
                tabs = (SubBuildingsTabstrip)UIView.GetAView().AddUIComponent(typeof(SubBuildingsTabstrip));
            }
            var subBuildingsMonitor = new GameObject(GO_NAME);
            subBuildingsMonitor.AddComponent<SubBuildingsMonitor>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            var go = GameObject.Find(GO_NAME);
            if (go != null)
            {
                Object.Destroy(go);
            }
            if (tabs != null)
            {
                Object.Destroy(tabs);
                tabs = null;
            }
        }
    }
}