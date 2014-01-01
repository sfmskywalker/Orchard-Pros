using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IExperienceManager : IDependency {
        IEnumerable<Experience> Fetch(int profileId);
        Experience Create(int userId, Action<Experience> initialize = null);
        Experience Get(int id);
        void Delete(Experience experience);
    }
}