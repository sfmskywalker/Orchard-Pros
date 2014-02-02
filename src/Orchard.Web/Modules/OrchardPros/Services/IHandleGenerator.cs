using System;
using Orchard;

namespace OrchardPros.Services {
    public interface IHandleGenerator : IDependency {
        string Generate();
    }
}