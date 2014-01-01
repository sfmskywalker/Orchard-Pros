using System;
using System.Collections.Generic;
using Orchard.Data;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class ExperienceManager : IExperienceManager {
        private readonly IRepository<Experience> _experienceRepository;

        public ExperienceManager(IRepository<Experience> experienceRepository) {
            _experienceRepository = experienceRepository;
        }

        public IEnumerable<Experience> Fetch(int profileId) {
            return _experienceRepository.Fetch(x => x.UserId == profileId);
        }

        public Experience Create(int userId, Action<Experience> initialize = null) {
            var experience = new Experience {UserId = userId};
            if (initialize != null)
                initialize(experience);
            _experienceRepository.Create(experience);
            return experience;
        }

        public Experience Get(int id) {
            return _experienceRepository.Get(id);
        }

        public void Delete(Experience experience) {
            _experienceRepository.Delete(experience);
        }
    }
}