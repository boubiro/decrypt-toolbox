using System.Security.Cryptography;
using System.Text;

namespace dtCompareHash
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

        public string Hash(string input)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha1.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}