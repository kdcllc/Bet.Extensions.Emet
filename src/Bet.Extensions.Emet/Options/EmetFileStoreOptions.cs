using System.Collections.Generic;

namespace Bet.Extensions.Emet.Options;

public class EmetFileStoreOptions
{
    /// <summary>
    /// File names of location to the file.
    /// </summary>
    public List<string> FileNames { get; set; } = new List<string>();
}
