using MineSharp.Components.Core.Logging;
using MineSharp.Components.Core.Types.Enums;
using MineSharp.Data.Protocol;
using System.Diagnostics;
namespace MineSharp.Components.Protocol
{
	public static class PacketFactory
	{
		public static class ClientPacketFactory
		{
			private static readonly Logger Logger = Logger.GetLogger();

			public static IPacketPayload? BuildPacket(PacketBuffer packetBuffer, GameState gameState)
			{
				try
				{
					var packet = gameState switch
					{
						GameState.HANDSHAKING => Data.Protocol.Handshaking.Clientbound.HandshakingPacketFactory.ReadPacket(packetBuffer),
						GameState.LOGIN => Data.Protocol.Login.Clientbound.LoginPacketFactory.ReadPacket(packetBuffer),
						GameState.PLAY => Data.Protocol.Play.Clientbound.PlayPacketFactory.ReadPacket(packetBuffer),
						GameState.STATUS => Data.Protocol.Status.Clientbound.StatusPacketFactory.ReadPacket(packetBuffer),
						_ => throw new UnreachableException()
					};

					if (packetBuffer.ReadableBytes > 0)
						Logger.Debug3($"PacketBuffer should be empty after reading ({packet.Name})"); //throw new Exception("PacketBuffer must be empty after reading");

					return packet switch
					{
						Data.Protocol.Handshaking.Clientbound.Packet chPacket => (IPacketPayload)chPacket.Params.Value!,
						Data.Protocol.Status.Clientbound.Packet csPacket => (IPacketPayload)csPacket.Params.Value!,
						Data.Protocol.Login.Clientbound.Packet clPacket => (IPacketPayload)clPacket.Params.Value!,
						Data.Protocol.Play.Clientbound.Packet cpPacket => (IPacketPayload)cpPacket.Params.Value!,
						_ => throw new Exception()
					};
				} catch (Exception e)
				{
					Logger.Error("Error reading packet!");
					Logger.Error(e.ToString());
					return null;
				}
			}

			public static PacketBuffer WritePacket(IPacketPayload packet, GameState gameState)
			{
				try
				{
					var packetBuffer = new PacketBuffer();

					switch (gameState)
					{
						case GameState.HANDSHAKING:
							Data.Protocol.Handshaking.Serverbound.HandshakingPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.LOGIN:
							Data.Protocol.Login.Serverbound.LoginPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.PLAY:
							Data.Protocol.Play.Serverbound.PlayPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.STATUS:
							Data.Protocol.Status.Serverbound.StatusPacketFactory.WritePacket(packetBuffer, packet);
							break;
						default:
							throw new UnreachableException();
					}

					return packetBuffer;
				} catch (Exception ex)
				{
					Logger.Error($"Error while writing packet of type {packet.GetType().FullName}: " + ex);
					throw new Exception($"Error while writing packet of type {packet.GetType().FullName}", ex);
				}
			}
		}

		public static class ServerPacketFactory
		{
			private static readonly Logger Logger = Logger.GetLogger();

			public static IPacketPayload? BuildPacket(PacketBuffer packetBuffer, GameState gameState)
			{
				try
				{
					var packet = gameState switch
					{
						GameState.HANDSHAKING => Data.Protocol.Handshaking.Serverbound.HandshakingPacketFactory.ReadPacket(packetBuffer),
						GameState.LOGIN => Data.Protocol.Login.Serverbound.LoginPacketFactory.ReadPacket(packetBuffer),
						GameState.PLAY => Data.Protocol.Play.Serverbound.PlayPacketFactory.ReadPacket(packetBuffer),
						GameState.STATUS => Data.Protocol.Status.Serverbound.StatusPacketFactory.ReadPacket(packetBuffer),
						_ => throw new UnreachableException()
					};

					if (packetBuffer.ReadableBytes > 0)
						Logger.Debug3($"PacketBuffer should be empty after reading ({packet.Name})"); //throw new Exception("PacketBuffer must be empty after reading");

					return packet switch
					{
						Data.Protocol.Handshaking.Serverbound.Packet chPacket => (IPacketPayload)chPacket.Params.Value!,
						Data.Protocol.Status.Serverbound.Packet csPacket => (IPacketPayload)csPacket.Params.Value!,
						Data.Protocol.Login.Serverbound.Packet clPacket => (IPacketPayload)clPacket.Params.Value!,
						Data.Protocol.Play.Serverbound.Packet cpPacket => (IPacketPayload)cpPacket.Params.Value!,
						_ => throw new Exception()
					};
				} catch (Exception e)
				{
					Logger.Error("Error reading packet!");
					Logger.Error(e.ToString());
					return null;
				}
			}

			public static PacketBuffer WritePacket(IPacketPayload packet, GameState gameState)
			{
				try
				{
					var packetBuffer = new PacketBuffer();

					switch (gameState)
					{
						case GameState.HANDSHAKING:
							Data.Protocol.Handshaking.Clientbound.HandshakingPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.LOGIN:
							Data.Protocol.Login.Clientbound.LoginPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.PLAY:
							Data.Protocol.Play.Clientbound.PlayPacketFactory.WritePacket(packetBuffer, packet);
							break;
						case GameState.STATUS:
							Data.Protocol.Status.Clientbound.StatusPacketFactory.WritePacket(packetBuffer, packet);
							break;
					}

					return packetBuffer;
				} catch (Exception ex)
				{
					Logger.Error($"Error while writing packet of type {packet.GetType().FullName}: " + ex);
					throw new Exception($"Error while writing packet of type {packet.GetType().FullName}", ex);
				}
			}
		}
	}
}
