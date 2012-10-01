using System.Security.Cryptography;

namespace dtHash
{
    class Sha512HashAlgorithm : IHashAlgorithm
    {
        public string Name
        {
            get { return "SHA512"; }
        }

        public int HashedValuesLength
        {
            get { return 128; }
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            return SHA512.Create();
        }
    }
}