using System;
using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        [ConcurrencyCheck]
        public DateTime UpdatedAt { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        protected Entity(Guid id, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUpdatedAt(DateTime updatedAt)
        {
            UpdatedAt = updatedAt;
        }
    }
}