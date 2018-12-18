/*
using GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    public class ValueGrain : Grain<ValueGrainState>, IValueGrain
    {

        public Task<string> GetValue()
        {
            return Task.FromResult(this.State.value);
        }

        public Task SetValue(string value)
        {
            this.State.value = value;

            return base.WriteStateAsync();
        }
    }

    public class ValueGrainState
    {
        public string value { get; set; }

    }
}
*/