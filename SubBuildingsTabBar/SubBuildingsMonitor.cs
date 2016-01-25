using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;

namespace SubBuildingsTabBar
{

    public class SubBuildingsMonitor : MonoBehaviour
    {

        private FieldInfo baseSub;
        private SubBuildingsTabstrip Tabs;
        private ushort cachedBuildingId;
        private CityServiceWorldInfoPanel servicePanel;
        private ZonedBuildingWorldInfoPanel zonedPanel;
        private UIPanel servicePanelUi;
        private UIPanel zonedPanelUi;

        public void Awake()
        {
            zonedPanelUi = UIView.Find<UIPanel>("(Library) ZonedBuildingWorldInfoPanel");
            zonedPanelUi.eventVisibilityChanged += ZonedPanel_eventVisibilityChanged;

            servicePanelUi = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            servicePanelUi.eventVisibilityChanged += ServicePanel_eventVisibilityChanged;
            servicePanelUi.size = zonedPanelUi.size;
            servicePanel = servicePanelUi.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            zonedPanel = zonedPanelUi.gameObject.transform.GetComponentInChildren<ZonedBuildingWorldInfoPanel>();
        }

        public void Update()
        {
            if (servicePanel == null || zonedPanel == null)
            {
                return;
            }
            Tabs = UIView.Find<SubBuildingsTabstrip>("SubBuildingsTabstrip");
            ushort buildingId = 0;
            if (servicePanelUi.isVisible)
            {
                buildingId = GetServiceInstanceId().Building;
            }
            else if (zonedPanelUi.isVisible)
            {
                buildingId = GetZonedInstanceId().Building;
            }
            if (buildingId == cachedBuildingId)
            {
                return;
            }
            Tabs.UpdateInfoPanelTabs(buildingId);
            cachedBuildingId = buildingId;
        }

        private InstanceID GetServiceInstanceId()
        {
            if (baseSub == null)
            {
                baseSub = servicePanel.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

            }
            return (InstanceID)baseSub.GetValue(servicePanel);
        }

        private InstanceID GetZonedInstanceId()
        {
            if (baseSub == null)
            {
                baseSub = zonedPanel.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

            }
            return (InstanceID)baseSub.GetValue(zonedPanel);
        }


        public void ServicePanel_eventVisibilityChanged(UIComponent component, bool value)
        {
            if (!value)
            {
                return;
            }
            Tabs.AlignTo(servicePanelUi, UIAlignAnchor.TopLeft);
            Tabs.relativePosition = new Vector2(13, -25);
        }

        public void ZonedPanel_eventVisibilityChanged(UIComponent component, bool value)
        {
            if (!value)
            {
                return;
            }
            Tabs.AlignTo(zonedPanelUi, UIAlignAnchor.TopLeft);
            Tabs.relativePosition = new Vector2(13, -25);
        }
    }

}


