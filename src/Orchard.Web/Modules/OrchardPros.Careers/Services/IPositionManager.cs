﻿using System;
using System.Collections.Generic;
using Orchard;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public interface IPositionManager : IDependency {
        IEnumerable<Position> Fetch(int userId);
        Position Get(int id);
        Position Create(int profileId, Action<Position> initialize = null);
        void Archive(Position position);
    }
}