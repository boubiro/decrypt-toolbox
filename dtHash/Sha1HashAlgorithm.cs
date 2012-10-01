using System.Security.Cryptography;

namespace dtHash
{
    class Sha1HashAlgorithm : IHashAlgorithm
    {
        public string Name
        {
            get { return "SHA1"; }
        }

        public int HashedValuesLength
        {
            get { return 40; }
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            return SHA1.Create();
        }
    }
}