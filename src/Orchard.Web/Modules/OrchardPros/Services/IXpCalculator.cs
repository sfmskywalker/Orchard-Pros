using Orchard;

namespace OrchardPros.Services {
    public interface IXpCalculator : IDependency {
        int CalculateXp(int level);
        bool CanLevelUp(int currentLevel, int currentXp);
    }
}