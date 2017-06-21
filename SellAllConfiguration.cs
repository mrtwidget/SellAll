using Rocket.API;

namespace NEXIS.SellAll
{
    public class SellAllConfiguration : IRocketPluginConfiguration
    {
        public bool SellAllEnabled;
        public bool SellPlayerClothing;
        public bool SellPlayerWeapons;

        public void LoadDefaults()
        {
            SellAllEnabled = true;
            SellPlayerClothing = true;
            SellPlayerWeapons = true;
        }
    }
}