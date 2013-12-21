using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public interface ISkillManager : IDependency {
        IEnumerable<Skill> Fetch(int userId);
        Skill Get(int id);
        Skill Create(int userId, Action<Skill> initialize);
        void Delete(Skill skill);
    }
}