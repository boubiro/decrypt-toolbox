using System.Security.Cryptography;
using System.Text;

namespace dtCompareHash
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

        public string Hash(string input)
        {
            SHA384 sha384 = SHA384.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha384.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}