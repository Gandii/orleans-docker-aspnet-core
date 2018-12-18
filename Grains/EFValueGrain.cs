using Data.Contexts;
using GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace Grains
{

    public class EFValueGrain : Grain, IValueGrain
    {

        private readonly AppDbContext _context;

        private Data.Contexts.Models.ValueGrainState state;

        public override async Task OnActivateAsync()
        {
            state = await _context.Values.FindAsync(this.GetPrimaryKeyLong());

            if (state == null)
            {
                //We need to create a new entity.
                state = new Data.Contexts.Models.ValueGrainState { ValueGrainId = this.GetPrimaryKeyLong() };
                _context.Add(state);

            }
        }

        public EFValueGrain(AppDbContext context)
        {
            this._context = context;
        }

        public Task<string> GetValue()
        {
            return Task.FromResult(this.state.Value);
        }

        public Task SetValue(string value)
        {
            this.state.Value = value;

            return _context.SaveChangesAsync();
        }
    }


}
