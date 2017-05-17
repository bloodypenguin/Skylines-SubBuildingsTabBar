using System.Runtime.CompilerServices;
using SubBuildingsTabBar.Redirection.Attributes;

namespace SubBuildingsTabBar.Detour
{
    [TargetType(typeof(TransportStationAI))]
    public class TransportStationAIDetour
    {
        [RedirectMethod]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetPassengerCount(ushort buildingID, ref Building data)
        {
            return (int)data.m_customBuffer1;
        }
    }
}