using Xunit;

namespace Identity.Security.Test
{
    public class PasswordManagerTests
    {
        private readonly PasswordManager _sut = new PasswordManager();

        [Theory]
        [InlineData("abc123")]
        [InlineData("a")]
        [InlineData("1234")]
        public void HashAndVerify_ReturnsTrueForSamePassword(string providedPassword1)
        {
            var hash = _sut.HashPassword(providedPassword1);
            var result = _sut.VerifyHashedPassword(hash, providedPassword1);

            Assert.True(result);
        }

        [Theory]
        [InlineData("abc123", "Abc123")]
        [InlineData("a", "b")]
        [InlineData("1234", "12345")]
        public void HashAndVerify_ReturnsFalseForDifferentPassword(string providedPassword1, string providedPassword2)
        {
            var hash = _sut.HashPassword(providedPassword1);
            var result = _sut.VerifyHashedPassword(hash, providedPassword2);

            Assert.False(result);
        }

        [Theory]
        [InlineData("abc123")]
        [InlineData("a")]
        [InlineData("1234")]
        public void Hash_ReturnsDifferentHashForSamePassword(string providedPassword)
        {
            var hash1 = _sut.HashPassword(providedPassword);
            var hash2 = _sut.HashPassword(providedPassword);

            Assert.NotEqual(hash1, hash2);
        }
    }
}