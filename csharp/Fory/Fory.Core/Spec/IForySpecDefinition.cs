using System.Threading.Tasks;

namespace Fory.Core.Spec
{
    /// <summary>
    /// Fory overall specification definition: | fory header | object ref meta | object type meta | object value data |
    /// </summary>
    internal interface IForySpecDefinition
    {
        Task Serialize<TValue>(in TValue value, SerializationContext context);
    }
}
