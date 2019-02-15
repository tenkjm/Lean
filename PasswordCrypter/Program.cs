using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoQuant;

namespace PasswordCrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("If you need to encode acess token enter: token encryptPass");

            var srt = Console.ReadLine();

            var parts = srt.Split(' ');

            if (parts.Length == 2)
            {
                Console.Out.WriteLine("Your encrypted string is:" + EncryptDecryptor.EncryptString(parts[0], parts[1]));
            }

            Console.ReadKey();
        }
    }
}
