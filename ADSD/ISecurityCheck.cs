namespace ADSD
{
    /// <summary>
    /// Protocol for validating a security token
    /// </summary>
    public interface ISecurityCheck
    {
        /// <summary>
        /// Validate a raw token.
        /// This could be from a cookie or an Authorization header
        /// </summary>
        /// <param name="token">JWT token string</param>
        /// <returns>Validation result</returns>
        SecurityOutcome Validate(string token);
    }
}