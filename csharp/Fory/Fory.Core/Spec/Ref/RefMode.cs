namespace Fory.Core.Spec.Ref;

public enum RefMode : byte
{
    None = 0,
    NullOnly = 1 << 0,
    Tracking = 1 << 1
}
