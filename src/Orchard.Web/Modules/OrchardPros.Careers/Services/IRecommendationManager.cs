using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.ViewModels;

namespace OrchardPros.Careers.Services {
    public interface IRecommendationManager : IDependency {
        IEnumerable<Recommendation> Fetch(int profileId);
        IEnumerable<RecommendationEx> FetchEx(int profileId);
        Recommendation Create(int profileId, Action<Recommendation> initialize = null);
        Recommendation Get(int id);
        void Delete(Recommendation recommendation);
        void Approve(Recommendation recommendation);
    }
}