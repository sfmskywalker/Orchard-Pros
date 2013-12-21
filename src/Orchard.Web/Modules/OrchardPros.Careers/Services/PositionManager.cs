using System;
using System.Collections.Generic;
using Orchard.Data;
using Orchard.Services;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public class PositionManager : IPositionManager {
        private readonly IRepository<Position> _positionRepository;
        private readonly IClock _clock;

        public PositionManager(IRepository<Position> positionRepository, IClock clock) {
            _positionRepository = positionRepository;
            _clock = clock;
        }

        public IEnumerable<Position> Fetch(int userId) {
            return _positionRepository.Fetch(x => x.UserId == userId && !x.IsArchived);
        }

        public Position Get(int id) {
            return _positionRepository.Get(id);
        }

        public Position Create(int userId, Action<Position> initialize = null) {
            var position = new Position {
                UserId = userId
            };
            if (initialize != null)
                initialize(position);
            _positionRepository.Create(position);
            return position;
        }

        public void Archive(Position position) {
            position.IsArchived = true;
            position.ArchivedUtc = _clock.UtcNow;
        }
    }
}