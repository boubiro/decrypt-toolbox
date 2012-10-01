using System.Security.Cryptography;

namespace dtHash
{
    class Sha256HashAlgorithm : IHashAlgorithm
    {
        public string Name
        {
            get { return "SHA256"; }
        }

        public int HashedValuesLength
        {
            get { return 64; }
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            return SHA256.Create();
        }
    }
}