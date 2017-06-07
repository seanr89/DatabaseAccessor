
using System.Collections.Generic;

namespace MyGenericContext.DataLayer.Interfaces
{
    /// <summary>
    /// Interface class to provide the READ data operation method header calls
    /// </summary>
    public interface IReadDataStore
    {
        IEnumerable<T> ReadObjectCollection<T>(Dictionary<string, object> parameters);
    }
}