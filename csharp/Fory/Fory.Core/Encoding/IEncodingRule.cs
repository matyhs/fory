namespace Fory.Core.Encoding
{
    internal interface IEncodingRule
    {
        // Note: Would've been nicer with static interface method since we don't need the instance of the encoding rule to validate
        bool Evaluate(StringStatistics stats, string value);
    }
}
