using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using UnityEngine;

namespace SubBuildingsTabBar
{

    public class SubBuildingsTabstrip : UITabstrip
    {
        private List<ushort> _idList;
        private UITextureAtlas ingame;
        private UITextureAtlas thumbnails;

        public override void Start()
        {
            base.Start();
            this.size = new Vector2(432, 25);
            this.relativePosition = new Vector2(13, -25);
            this.name = "SubBuildingsTabstrip";
            this.startSelectedIndex = 0;
            this.padding = new RectOffset(0, 3, 0, 0);
            ingame = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Ingame");
            thumbnails = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Thumbnails");
        }

        public void UpdateInfoPanelTabs(ushort buildingId)
        {
            if (buildingId == 0)
            {
                _idList = null;
                RemoveAllTabs();
                return;
            }
            if (_idList != null && _idList.Contains(buildingId))
            {
                return;
            }
            RemoveAllTabs();
            var selectedBuilding = BuildingManager.instance.m_buildings.m_buffer[buildingId];
            if ((selectedBuilding.m_parentBuilding == 0) && (selectedBuilding.m_subBuilding == 0))
            {
                return;
            }
            var mainBuildingId = buildingId;
            while (BuildingManager.instance.m_buildings.m_buffer[mainBuildingId].m_parentBuilding != 0)
            {
                mainBuildingId = BuildingManager.instance.m_buildings.m_buffer[mainBuildingId].m_parentBuilding;
            }
            _idList = new List<ushort> { mainBuildingId };
            var subBuildingId = BuildingManager.instance.m_buildings.m_buffer[mainBuildingId].m_subBuilding;
            while (subBuildingId != 0)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[subBuildingId];
                if (IsDummy(ref building))
                {
                    subBuildingId = BuildingManager.instance.m_buildings.m_buffer[subBuildingId].m_subBuilding;
                    continue;
                }
                _idList.Add(subBuildingId);
                subBuildingId = BuildingManager.instance.m_buildings.m_buffer[subBuildingId].m_subBuilding;
            }
            if (_idList.Count == 1)
            {
                RemoveAllTabs();
                return;
            }

            var counter = 0;
            foreach (var id in _idList)
            {
                var info = BuildingManager.instance.m_buildings.m_buffer[id].Info;
                var tabButton = this.AddUIComponent<UIButton>();
                tabButton.size = new Vector2(58, 25);
                tabButton.normalBgSprite = "SubBarButtonBase";
                tabButton.disabledBgSprite = "SubBarButtonBaseDisabled";
                tabButton.pressedBgSprite = "SubBarButtonBasePressed";
                tabButton.hoveredBgSprite = "SubBarButtonBaseHovered";
                tabButton.focusedBgSprite = "SubBarButtonBaseFocused";
                tabButton.name = name;
                var sprite = tabButton.AddUIComponent<UISprite>();
                sprite.size = new Vector2(35, 25);
                sprite.isInteractive = false;
                sprite.relativePosition = new Vector2(12, 0);

                var itemClass = info.m_class;
                if (itemClass == null || itemClass.GetZone() == ItemClass.Zone.None)
                {
                    var service = info.GetService();
                    if (service != ItemClass.Service.None)
                    {
                        var subService = info.GetSubService();
                        if (subService == ItemClass.SubService.None)
                        {
                            var nameByValue = Utils.GetNameByValue(service, "Game");
                            sprite.spriteName = "ToolbarIcon" + nameByValue;
                            sprite.atlas = ingame;
                        }
                        else
                        {
                            sprite.spriteName = "SubBar" + subService;
                            sprite.atlas = ingame;
                        }
                    }
                }
                else
                {
                    var zone = itemClass.GetZone();
                    sprite.spriteName = "Zoning" + zone + "Focused";
                    sprite.atlas = thumbnails;
                }
                tabButton.eventClicked += SelectSub;
                tabButton.isVisible = true;

                if (_idList[counter] == buildingId)
                {
                    selectedIndex = counter;
                }
                counter++;
            }
        }

        private void RemoveAllTabs()
        {
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }
            foreach (var child in tabs.Clone().Where(child => child != null))
            {
                child.Hide();
                this.RemoveUIComponent(child);
                DestroyImmediate(child);
            }
        }

        private static bool IsDummy(ref Building building)
        {
            if (building.Info == null)
            {
                return true;
            }
            var buildingAi = building.Info.m_buildingAI;
            return buildingAi is DummyBuildingAI;
        }

        private void SelectSub(UIComponent component, UIMouseEventParameter param)
        {
            if (_idList == null)
            {
                return;
            }
            var buildingId = _idList[selectedIndex];
            var building = BuildingManager.instance.m_buildings.m_buffer[buildingId];
            if (building.Info != null)
            {
                var localeField = typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance);
                var locale = (Locale)localeField.GetValue(SingletonLite<LocaleManager>.instance);
                var key = new Locale.Key { m_Identifier = "BUILDING_TITLE", m_Key = building.Info.name };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, building.Info.name);
                }
                key = new Locale.Key { m_Identifier = "BUILDING_DESC", m_Key = building.Info.name };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, string.Empty);
                }
                key = new Locale.Key { m_Identifier = "BUILDING_SHORT_DESC", m_Key = building.Info.name };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, string.Empty);
                }
            }
            DefaultTool.OpenWorldInfoPanel(new InstanceID { Building = buildingId }, new Vector2(0, 0));
        }
    }
}
