using System;
using Random = UnityEngine.Random;

namespace Twisted.Extensions
{
    public readonly struct RandomStateScope : IDisposable
    {
        private readonly Random.State State;

        public RandomStateScope(int seed)
        {
            State = Random.state;
            Random.InitState(seed);
        }

        public void Dispose()
        {
            Random.state = State;
        }
    }
}