using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public interface IExperienceManager : IDependency {
        IEnumerable<Experience> Fetch(int profileId);
        Experience Create(int profileId, Action<Experience> initialize = null);
        Experience Get(int id);
        void Delete(Experience experience);
    }
}