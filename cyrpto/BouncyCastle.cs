//BOUNCY CASTLE IS THE OPENSOURCE EQUIVALENT OF THE NODEJS CRYPTO LIBRARIES AND INCLUDES ELLIPTICAL CURVE TECHNOLOGY.

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Text;

public static class BouncyKeyGenerator
{
    public static (string publicPem, string privatePem) GenerateRsa(int keySize)
    {
        var generator = new RsaKeyPairGenerator();
        generator.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
        var keyPair = generator.GenerateKeyPair();

        return (ToPem(keyPair.Public), ToPem(keyPair.Private));
    }

    public static (string publicPem, string privatePem) GenerateEc()
    {
        var gen = new ECKeyPairGenerator();
        var secureRandom = new SecureRandom();
        var ecParams = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256r1");
        var domainParams = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H);

        gen.Init(new ECKeyGenerationParameters(domainParams, secureRandom));
        var keyPair = gen.GenerateKeyPair();

        return (ToPem(keyPair.Public), ToPem(keyPair.Private));
    }

    public static (string publicPem, string privatePem) GenerateEd25519()
    {
        var gen = new Ed25519KeyPairGenerator();
        gen.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        var keyPair = gen.GenerateKeyPair();

        return (ToPem(keyPair.Public), ToPem(keyPair.Private));
    }

    public static string GenerateAes(int size)
    {
        var key = new byte[size / 8];
        new SecureRandom().NextBytes(key);
        return Convert.ToBase64String(key);
    }

    public static string GenerateDes()
    {
        var key = new byte[8];
        new SecureRandom().NextBytes(key);
        return Convert.ToBase64String(key);
    }

    public static string GenerateTripleDes()
    {
        var key = new byte[24];
        new SecureRandom().NextBytes(key);
        return Convert.ToBase64String(key);
    }

    private static string ToPem(object obj)
    {
        using var sw = new StringWriter();
        var pemWriter = new PemWriter(sw);
        pemWriter.WriteObject(obj);
        return sw.ToString();
    }
}
