using MineSharp.Components.Core.Types.Enums;
using MineSharp.Components.Crypto;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Login.Clientbound;
using MineSharp.Skeletons.Barebones.Client;

namespace MineSharp.Skeletons.Minimal.MinimalClient.Handlers
{
	public class LoginPacketHandler : IPacketHandler
	{
		private static TaskCompletionSource? _enableEncryptionTsc;

		public Task HandleIncoming(IPacketPayload packet, MinecraftMinimalClient client)
		{
			return packet switch
			{
				PacketEncryptionBegin packetEncryptionBegin => HandleIncomingPacketEncryptionBegin(packetEncryptionBegin, client),
				PacketCompress packetCompress => HandleIncomingPacketCompress(packetCompress, client),
				PacketSuccess packetSuccess => HandleIncomingPacketSuccess(packetSuccess, client),
				PacketDisconnect packetDisconnect => HandleIncomingPacketDisconnect(packetDisconnect, client),
				_ => Task.CompletedTask
			};
		}

		public Task HandleOutgoing(IPacketPayload packet, MinecraftMinimalClient client) {
			return packet switch
			{
				Data.Protocol.Login.Serverbound.PacketEncryptionBegin packetEncryptionBegin => HandleOutgoingPacketEncryptionBegin(packetEncryptionBegin, client),
				_ => Task.CompletedTask
			};
		}

		private static async Task HandleIncomingPacketEncryptionBegin(PacketEncryptionBegin packet, MinecraftMinimalClient client)
		{
			var encryptionBeginResponse = HashHelper.GenerateEncryptionBegin(packet.ServerId, packet.PublicKey, packet.VerifyToken);
			await client.Session.JoinServer(encryptionBeginResponse.Hex);
			_enableEncryptionTsc = new TaskCompletionSource();
			SetEncryptionKey(encryptionBeginResponse.Key, _enableEncryptionTsc!.Task, client);
			client.SendPacket(encryptionBeginResponse.Packet);
		}
		
		private static Task HandleOutgoingPacketEncryptionBegin(Data.Protocol.Login.Serverbound.PacketEncryptionBegin packetEncryptionBegin, MinecraftMinimalClient client)
		{
			_enableEncryptionTsc!.SetResult();
			return Task.CompletedTask;
		}

		private static void SetEncryptionKey(byte[] key, Task enableEncryption, MinecraftClient client)
		{
			Task.Run(async () =>
			{
				await enableEncryption;
				client.Stream!.EnableEncryption(key);
			}, client.CancellationTokenSource!.Token);
		}
		
		private static Task HandleIncomingPacketCompress(PacketCompress packet, MinecraftClient client)
		{
			client.Stream!.SetCompressionThreshold(packet.Threshold);
			
			return Task.CompletedTask;
		}
		
		private static Task HandleIncomingPacketSuccess(PacketSuccess packetSuccess, MinecraftMinimalClient client)
		{
			client.GameState = GameState.PLAY;
			Task.Run(client.OnClientConnected);
			return Task.CompletedTask;
		}
		
		private static Task HandleIncomingPacketDisconnect(PacketDisconnect packetDisconnect, MinecraftMinimalClient client)
		{
			Task.Run(() => client.OnClientDisconnected(packetDisconnect));
			return Task.CompletedTask;
		}
	}
}
