using System.Security.Cryptography;
using System.Text;

namespace dtCompareHash
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

        public string Hash(string input)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha512.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}