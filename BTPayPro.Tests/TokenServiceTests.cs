using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using BTPayPro.Domaine;
using BTPayPro.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BTPayPro.Tests
{
    public class TokenServiceTests
    {
        private TokenService CreateTokenService(out IConfiguration config)
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "SuperSecretKeyForTests1234567890!" },
                { "Jwt:Issuer", "BTPayPro.TestIssuer" },
                { "Jwt:Audience", "BTPayPro.TestAudience" },
                { "Jwt:AccessTokenExpirationMinutes", "60" }
            };

            config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            return new TokenService(config);
        }

        private static User CreateFakeUser()
        {
            return new User
            {
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };
        }

        [Fact]
        public void GenerateAccessToken_Returns_NonEmpty_ValidJwt()
        {
            // Arrange
            var tokenService = CreateTokenService(out var config);
            var user = CreateFakeUser();

            // Act
            var token = tokenService.GenerateAccessToken(user);

            // Assert de base
            Assert.False(string.IsNullOrWhiteSpace(token));

            // Vérifier que c'est bien un JWT parsable
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal(config["Jwt:Issuer"], jwt.Issuer);
            Assert.Equal(config["Jwt:Audience"], jwt.Audiences.Single());
        }

        [Fact]
        public void GenerateRefreshToken_Returns_Base64_And_IsRandom()
        {
            // Arrange
            var tokenService = CreateTokenService(out _);

            // Act
            var token1 = tokenService.GenerateRefreshToken();
            var token2 = tokenService.GenerateRefreshToken();

            // Assert de base
            Assert.False(string.IsNullOrWhiteSpace(token1));
            Assert.False(string.IsNullOrWhiteSpace(token2));
            Assert.NotEqual(token1, token2);

            // Vérifier que c'est bien du Base64 valide
            var bytes = Convert.FromBase64String(token1);
            Assert.True(bytes.Length > 0);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_Returns_Principal_For_ValidToken()
        {
            // Arrange
            var tokenService = CreateTokenService(out _);
            var user = CreateFakeUser();

            // On réutilise le même token que GenerateAccessToken
            var token = tokenService.GenerateAccessToken(user);

            // Act
            var principal = tokenService.GetPrincipalFromExpiredToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.NotNull(principal.Identity);
            Assert.True(principal.Identity!.IsAuthenticated);
        }
    }
}
