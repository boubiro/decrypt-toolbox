namespace dtCompareHash
{
    interface IHashAlgorithm
    {
        string Name { get; }

        int HashedValuesLength { get; }

        string Hash(string input);
    }
}