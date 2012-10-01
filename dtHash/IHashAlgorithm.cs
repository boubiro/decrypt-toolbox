using System.Security.Cryptography;

namespace dtHash
{
    interface IHashAlgorithm
    {
        string Name { get; }

        int HashedValuesLength { get; }

        HashAlgorithm CreateHashAlgorithm();
    }
}