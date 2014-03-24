using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public interface ISkillManager : IDependency {
        IEnumerable<Skill> Fetch(int userId);
        Skill Get(int id);
        Skill Create(int userId, Action<Skill> initialize);
        void Delete(Skill skill);
    }
}