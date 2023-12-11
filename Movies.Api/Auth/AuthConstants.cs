namespace Movies.Api.Auth
{
    public static class AuthConstants
    {
        public const string AdminUserPolicyName = "Admin";
        public const string AdminUserClaim = "admin";

        public const string TrustedMemberPolicyName = "TrustedUser";
        public const string TrustedMemberClaim = "trusted_user";
    }
}
