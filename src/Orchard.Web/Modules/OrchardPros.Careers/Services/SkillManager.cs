using System;
using System.Collections.Generic;
using Orchard.Data;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public class SkillManager : ISkillManager {
        private readonly IRepository<Skill> _skillRepository;

        public SkillManager(IRepository<Skill> skillRepository) {
            _skillRepository = skillRepository;
        }

        public IEnumerable<Skill> Fetch(int userId) {
            return _skillRepository.Fetch(x => x.UserId == userId);
        }

        public Skill Get(int id) {
            return _skillRepository.Get(id);
        }

        public Skill Create(int userId, Action<Skill> initialize) {
            var skill = new Skill {
                UserId = userId
            };
            if (initialize != null)
                initialize(skill);
            _skillRepository.Create(skill);
            return skill;
        }

        public void Delete(Skill skill) {
            _skillRepository.Delete(skill);
        }
    }
}