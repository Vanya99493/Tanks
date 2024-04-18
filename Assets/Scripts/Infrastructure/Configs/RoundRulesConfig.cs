using System;

namespace Infrastructure.Configs
{
    [Serializable]
    public class RoundRulesConfig
    {
        public int RoundsToWinNumber;
        public float StartDelay;
        public float EndDelay;
    }
}