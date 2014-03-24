using System;

namespace OrchardPros.Services.User {
    public class XpCalculator : IXpCalculator {
        private const int BaseXp = 100;
        private const double Factor = 1.4;

        public int CalculateXp(int level) {
            return (int)(BaseXp * (Math.Pow(level, Factor)));
        }

        public bool CanLevelUp(int currentLevel, int currentXp) {
            var nextLevel = currentLevel + 1;
            var nextLevelXp = CalculateXp(nextLevel);

            return currentXp >= nextLevelXp;
        }
    }
}