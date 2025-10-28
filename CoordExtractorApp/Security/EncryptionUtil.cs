using System.Reflection.Metadata.Ecma335;

namespace CoordExtractorApp.Security
{
    public static class EncryptionUtil
    {
        //κρυπτογράφηση password
        public static string Encrypt(string clearText)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(clearText);
            return encryptedPassword;
        }

        //σύγκριση text χρηστη με encrypted password. one way function.
        public static bool IsValidPassword(string plainText, string cipherText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
        }
    }
}

