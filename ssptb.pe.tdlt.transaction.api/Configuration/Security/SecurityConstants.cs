namespace ssptb.pe.tdlt.transaction.api.Configuration.Security;

public struct ContentTypeOptionsConstants
{
    public const string Header = "X-Content-Type-Options";
    public const string NoSniff = "nosniff";
}

public struct FrameOptionsConstants
{
    public const string Header = "X-Frame-Options";
    public const string Deny = "DENY";
    public const string SameOrigin = "SAMEORIGIN";
    public const string AllowFromUri = "ALLOW-FROM {0}";
}

public struct ServerConstants
{
    public const string Header = "Server";
}

public struct StrictTransportSecurityConstants
{
    public const string Header = "Strict-Transport-Security";
    public const string MaxAge = "max-age={0}";
    public const string MaxAgeIncludeSubdomains = "max-age=631138519; includeSubDomains";
    public const string NoCache = "max-age=0";
}

public struct XssProtectionConstants
{
    public const string Header = "X-XSS-Protection";
    public const string Enabled = "1";
    public const string Disabled = "0";
    public const string Block = "1; mode=block";
    public const string Report = "1; report={0}";
}

public struct ContentSecurityConstants
{
    public const string Header = "Content-Security-Policy";
    public const string Default = "object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self'; font-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' data: 'unsafe-eval'";
}

public struct RefererPolicyConstants
{
    public const string Header = "Referrer-Policy";
    public const string Default = "same-origin";
}

public struct FeaturePolicyContants
{
    public const string Header = "Feature-Policy";
    public const string Default = "geolocation 'self'; sync-xhr 'self'";
}