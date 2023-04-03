using System.Diagnostics;
using MineSharp.Components.Core.Types.Enums;
using MineSharp.Data.Protocol;
namespace MineSharp.Skeletons.Minimal.MinimalClient.Handlers
{
	public static class PacketHandler
	{
		public static Task HandleIncoming(IPacketPayload packet, MinecraftMinimalClient client)
		{
			return client.GameState switch
			{
				GameState.HANDSHAKING => new HandshakePacketHandler().HandleIncoming(packet, client),
				GameState.STATUS => new StatusPacketHandler().HandleIncoming(packet, client),
				GameState.LOGIN => new LoginPacketHandler().HandleIncoming(packet, client),
				GameState.PLAY => new PlayPacketHandler().HandleIncoming(packet, client),
				_ => throw new UnreachableException()
			};
		}
		
		public static Task HandleOutgoing(IPacketPayload packet, MinecraftMinimalClient client)
		{
			return client.GameState switch
			{
				GameState.HANDSHAKING => new HandshakePacketHandler().HandleOutgoing(packet, client),
				GameState.STATUS => new StatusPacketHandler().HandleOutgoing(packet, client),
				GameState.LOGIN => new LoginPacketHandler().HandleOutgoing(packet, client),
				GameState.PLAY => new PlayPacketHandler().HandleOutgoing(packet, client),
				_ => throw new UnreachableException()
			};
		}
	}
}
