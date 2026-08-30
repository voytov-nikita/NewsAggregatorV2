using System.Security.Cryptography;

using AuthService.BLL.Abstractions.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.BLL.Security;

/// <summary>
/// Owns the RSA key used to sign access tokens. The key is persisted to disk on first run:
/// generating a fresh one per startup would invalidate every outstanding access token and every
/// JWKS response other services have cached.
///
/// Production would use a managed store instead (Azure Key Vault, AWS KMS, a certificate from the
/// machine store); this file-based provider is a development convenience.
/// </summary>
public class SigningKeyProvider : ISigningKeyProvider, IDisposable
{
    private const string ActiveKeyFileName = "signing-key.pem";
    private const string RetiredDirectoryName = "retired";

    private readonly List<RSA> _ownedKeys = [];

    public SigningKeyProvider(JwtSettings settings, ILogger<SigningKeyProvider> logger)
    {
        Directory.CreateDirectory(settings.KeysPath);

        RSA activeKey = LoadOrCreateActiveKey(settings.KeysPath, logger);
        _ownedKeys.Add(activeKey);

        Kid = ComputeKid(activeKey);

        SigningCredentials = new SigningCredentials(
            new RsaSecurityKey(activeKey) { KeyId = Kid },
            SecurityAlgorithms.RsaSha256);

        PublicKeys = [ToPublicKey(activeKey, Kid), .. LoadRetiredPublicKeys(settings.KeysPath, logger)];
    }

    public string Kid { get; }

    public SigningCredentials SigningCredentials { get; }

    public IReadOnlyList<RsaSecurityKey> PublicKeys { get; }

    public void Dispose()
    {
        foreach (RSA key in _ownedKeys)
        {
            key.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Derives the key id from the public key itself rather than generating a random one. A random
    /// kid per startup is a subtle bug: tokens issued before a restart carry a kid that no longer
    /// appears in JWKS, and strict validators reject them even though the key material is identical.
    /// </summary>
    private static string ComputeKid(RSA key)
    {
        byte[] publicKeyInfo = key.ExportSubjectPublicKeyInfo();
        byte[] hash = SHA256.HashData(publicKeyInfo);

        return Base64UrlEncoder.Encode(hash)[..16];
    }

    private static RSA LoadOrCreateActiveKey(string keysPath, ILogger logger)
    {
        string path = Path.Combine(keysPath, ActiveKeyFileName);
        var key = RSA.Create(2048);

        if (File.Exists(path))
        {
            key.ImportFromPem(File.ReadAllText(path));

            return key;
        }

        File.WriteAllText(path, key.ExportPkcs8PrivateKeyPem());

        logger.LogWarning(
            "No signing key found at {Path} - generated a new one. Every previously issued access token is now invalid.",
            path);

        return key;
    }

    /// <summary>
    /// Retired keys stay published in JWKS so tokens signed before a rotation keep validating for
    /// the rest of their (short) lifetime. Rotating is therefore just:
    /// move signing-key.pem into retired/&lt;kid&gt;.pem and restart.
    /// </summary>
    private List<RsaSecurityKey> LoadRetiredPublicKeys(string keysPath, ILogger logger)
    {
        var retired = new List<RsaSecurityKey>();
        string retiredPath = Path.Combine(keysPath, RetiredDirectoryName);

        if (!Directory.Exists(retiredPath))
        {
            return retired;
        }

        foreach (string file in Directory.EnumerateFiles(retiredPath, "*.pem"))
        {
            try
            {
                var key = RSA.Create();
                key.ImportFromPem(File.ReadAllText(file));

                _ownedKeys.Add(key);
                retired.Add(ToPublicKey(key, ComputeKid(key)));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to load retired signing key {File} - skipping it.", file);
            }
        }

        return retired;
    }

    /// <summary>
    /// Re-imports only the public parameters, so the returned key physically cannot expose private
    /// material - the JWKS endpoint gets an object that has none to leak.
    /// </summary>
    private RsaSecurityKey ToPublicKey(RSA key, string kid)
    {
        var publicOnly = RSA.Create();
        publicOnly.ImportParameters(key.ExportParameters(includePrivateParameters: false));

        _ownedKeys.Add(publicOnly);

        return new RsaSecurityKey(publicOnly) { KeyId = kid };
    }
}
