using Domain.Results;

namespace Domain.ErrorMessages
{
    public static class AuthErrors
    {
        public static Error TokenNotExpired() => Error.Validation("Auth.TokenNotExpired", "This token hasn't expired yet.");
        public static Error InvalidToken() => Error.Unauthorized("Auth.InvalidToken", "Invalid token.");
        public static Error InvalidRefreshToken() => Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token.");
        public static Error RefreshTokenExpired() => Error.Unauthorized("Auth.RefreshTokenExpired", "Refresh token expired.");
        public static Error TokenRevoked() => Error.Unauthorized("Auth.TokenRevoked", "Token revoked.");
        public static Error InvalidTokenLinkage() => Error.Unauthorized("Auth.InvalidTokenLinkage", "Invalid token linkage.");
    }
}
