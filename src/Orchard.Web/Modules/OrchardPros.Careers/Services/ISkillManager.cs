using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public interface ISkillManager : IDependency {
        IEnumerable<Skill> Fetch(int profileId);
        Skill Get(int id);
        Skill Create(int profileId, Action<Skill> initialize);
        void Delete(Skill skill);
    }
}