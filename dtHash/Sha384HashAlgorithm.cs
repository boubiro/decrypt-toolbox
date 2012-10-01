using System.Security.Cryptography;

namespace dtHash
{
    class Sha384HashAlgorithm : IHashAlgorithm
    {
        public string Name
        {
            get { return "SHA384"; }
        }

        public int HashedValuesLength
        {
            get { return 96; }
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            return SHA384.Create();
        }
    }
}