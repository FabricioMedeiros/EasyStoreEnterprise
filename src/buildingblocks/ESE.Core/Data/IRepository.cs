﻿using ESE.Core.DomainObjects;
using System;

namespace ESE.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
