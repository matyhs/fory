using System.Threading.Tasks;

namespace Fory.Core.Spec
{
    internal static class ForyRefMetaSpec
    {
        public enum ReferenceFlag : sbyte
        {
            // This flag indicates the object is a null value. We don't use another byte to indicate REF, so that we can save one byte.
            Null = -3,
            // This flag indicates the object is already serialized previously, and fory will write a ref id with unsigned varint format instead of serialize it again
            Ref = -2,
            // This flag indicates the object is a non-null value and fory doesn't track ref for this type of object.
            NotNull = -1,
            // This flag indicates the object is referencable and the first time to serialize.
            RefValue = 0
        }
    }

    public readonly struct ReferenceInfo
    {
        private readonly ForyRefMetaSpec.ReferenceFlag _bitmap;

        public bool IsNull => _bitmap == ForyRefMetaSpec.ReferenceFlag.Null;

        public bool IsNewReference => _bitmap == ForyRefMetaSpec.ReferenceFlag.RefValue;

        public bool HasReference => _bitmap == ForyRefMetaSpec.ReferenceFlag.Ref;

        internal ReferenceInfo(ForyRefMetaSpec.ReferenceFlag bitmap)
        {
            _bitmap = bitmap;
        }
    }
}
