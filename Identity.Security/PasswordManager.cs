using System;
using System.Security.Cryptography;
using Identity.Domain.Managers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Identity.Security
{
    /// <summary>
    /// PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations.
    /// Format: { 0x01, prf (UInt32), iter count (UInt32), salt length (UInt32), salt, subkey }
    /// (All UInt32s are stored big-endian.)
    /// </summary>
    public class PasswordManager : IPasswordManager
    {
        private readonly int _iterationCount = 1000;
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentException(nameof(password));

            return Convert.ToBase64String(HashPassword(password, _rng));
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
                throw new ArgumentException(nameof(hashedPassword));

            if (providedPassword == null)
                throw new ArgumentException(nameof(providedPassword));

            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            try
            {
                // Read header information
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
                var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }

                var salt = new byte[saltLength];
                Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }

                var expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);

                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private byte[] HashPassword(string password, RandomNumberGenerator rng)
        {
            var saltSize = 128 / 8;
            var prf = KeyDerivationPrf.HMACSHA256;

            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, _iterationCount, 256 / 8);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_iterationCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                   | ((uint)(buffer[offset + 1]) << 16)
                   | ((uint)(buffer[offset + 2]) << 8)
                   | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}