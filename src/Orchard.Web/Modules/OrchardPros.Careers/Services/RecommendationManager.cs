using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Data;
using Orchard.Services;
using Orchard.Users.Models;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.ViewModels;

namespace OrchardPros.Careers.Services {
    public class RecommendationManager : IRecommendationManager {
        private readonly IRepository<Recommendation> _recommendationRepository;
        private readonly IRepository<UserPartRecord> _userRepository;
        private readonly IClock _clock;

        public RecommendationManager(IRepository<Recommendation> recommendationRepository, IRepository<UserPartRecord> userRepository, IClock clock) {
            _recommendationRepository = recommendationRepository;
            _userRepository = userRepository;
            _clock = clock;
        }

        public IEnumerable<Recommendation> Fetch(int profileId) {
            return _recommendationRepository.Fetch(x => x.UserId == profileId);
        }

        public IEnumerable<RecommendationEx> FetchEx(int profileId) {
            return from recommendation in _recommendationRepository.Table
                where recommendation.UserId == profileId
                from user in _userRepository.Table
                where user.Id == profileId
                   select new RecommendationEx {
                    Approved = recommendation.Approved,
                    CreatedUtc = recommendation.CreatedUtc,
                    UserId = recommendation.UserId,
                    Id = recommendation.Id,
                    RecommendingUserId = recommendation.RecommendingUserId,
                    RecommendingUser = user,
                    Text = recommendation.Text
                };
        }

        public Recommendation Create(int profileId, Action<Recommendation> initialize = null) {
            var recommendation = new Recommendation {
                UserId = profileId,
                CreatedUtc = _clock.UtcNow
            };
            if (initialize != null)
                initialize(recommendation);
            _recommendationRepository.Create(recommendation);
            return recommendation;
        }

        public Recommendation Get(int id) {
            return _recommendationRepository.Get(id);
        }

        public void Delete(Recommendation recommendation) {
            _recommendationRepository.Delete(recommendation);
        }

        public void Approve(Recommendation recommendation) {
            recommendation.Approved = true;
        }
    }
}