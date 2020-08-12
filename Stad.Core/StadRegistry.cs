using System.Collections.Generic;
using Stad.Core.Model;

namespace Stad.Core
{
    public readonly struct Path
    {
        public Path(string value)
        {
            Value = value;
        }

        public readonly string Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public class StadRegistry
    {
        public Dictionary<Path, ListModel> ListModels = new Dictionary<Path,ListModel>();
        public Dictionary<Path, StadModel> SingleModels = new Dictionary<Path, StadModel>();
    }
}