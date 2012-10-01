using System.Security.Cryptography;

namespace dtHash
{
    class Md5HashAlgorithm : IHashAlgorithm
    {
        private readonly MD5 _md5;

        public Md5HashAlgorithm()
        {
            _md5 = MD5.Create();
        }

        public string Name
        {
            get { return "MD5"; }
        }

        public int HashedValuesLength
        {
            get { return 32; }
        }

        public HashAlgorithm CreateHashAlgorithm()
        {
            return MD5.Create();
        }
    }
}