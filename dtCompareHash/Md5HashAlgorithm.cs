using System.Security.Cryptography;
using System.Text;

namespace dtCompareHash
{
    class Md5HashAlgorithm : IHashAlgorithm
    {
        public string Name
        {
            get { return "MD5"; }
        }

        public int HashedValuesLength
        {
            get { return 32; }
        }

        public string Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}