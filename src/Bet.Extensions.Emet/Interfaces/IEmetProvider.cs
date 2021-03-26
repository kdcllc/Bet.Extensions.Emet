using System;
using System.Threading.Tasks;

using RulesEngine.Interfaces;

namespace Bet.Extensions.Emet
{
    /// <summary>
    /// The main container interface for managing <see cref="IRulesEngine"/>.
    /// </summary>
    public interface IEmetProvider
    {
        /// <summary>
        /// An instance of the <see cref="IEmetProvider"/>.
        /// </summary>
        Lazy<Task<IRulesEngine>> RulesEngine { get; }

        /// <summary>
        /// Emet Store provider.
        /// </summary>
        IEmetStore Store { get; }

        /// <summary>
        /// The name for the <see cref="IEmetProvider"/>.
        /// </summary>
        string Name { get; }
    }
}
