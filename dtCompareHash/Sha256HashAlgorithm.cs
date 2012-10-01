using System.Security.Cryptography;
using System.Text;

namespace dtCompareHash
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

        public string Hash(string input)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha256.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}