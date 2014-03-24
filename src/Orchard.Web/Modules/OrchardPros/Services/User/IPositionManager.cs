using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public interface IPositionManager : IDependency {
        IEnumerable<Position> Fetch(int userId);
        Position Get(int id);
        Position Create(int userId, Action<Position> initialize = null);
        void Archive(Position position);
    }
}