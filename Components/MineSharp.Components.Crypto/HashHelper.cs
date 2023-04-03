using MineSharp.Data.Protocol.Login.Serverbound;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
namespace MineSharp.Components.Crypto
{
	public class HashHelper
	{
		public static EncryptionBeginResponse GenerateEncryptionBegin(string serverId, byte[] publicKey, byte[] verifyToken)
		{
			var aes = Aes.Create();
			aes.KeySize = 128;
			aes.GenerateKey();

			byte[] hash = SHA1.HashData(Encoding.ASCII.GetBytes(serverId).Concat(aes.Key).Concat(publicKey).ToArray());
			Array.Reverse(hash);
			var b = new BigInteger(hash);
			string hex;
			if (b < 0)
			{
				hex = "-" + BigInteger.Negate(b).ToString("x").TrimStart('0');
			} else
			{
				hex = b.ToString("x").TrimStart('0');
			}

			var rsa = RSAHelper.DecodePublicKey(publicKey);
			if (rsa == null)
			{
				throw new Exception("Could not decode public key");
			}
			byte[] encrypted = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
			byte[] encVerTok = rsa.Encrypt(verifyToken, RSAEncryptionPadding.Pkcs1);

			var response = new PacketEncryptionBegin(encrypted, true, new PacketEncryptionBegin.CryptoSwitch(new PacketEncryptionBegin.CryptoSwitch.CryptoSwitchStatetrueContainer(encVerTok)));
			
			return new EncryptionBeginResponse(response, hex, aes.Key);
		}
		
		public record EncryptionBeginResponse(PacketEncryptionBegin Packet, string Hex, byte[] Key);
	}
}
