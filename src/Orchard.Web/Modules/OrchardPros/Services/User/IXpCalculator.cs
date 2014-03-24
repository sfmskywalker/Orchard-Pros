using Orchard;

namespace OrchardPros.Services.User {
    public interface IXpCalculator : IDependency {
        int CalculateXp(int level);
        bool CanLevelUp(int currentLevel, int currentXp);
    }
}