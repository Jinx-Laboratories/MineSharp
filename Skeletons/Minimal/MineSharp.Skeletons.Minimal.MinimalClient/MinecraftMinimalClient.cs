using System.Net;
using MineSharp.Components.Core.Types.Enums;
using MineSharp.Components.MojangAuth;
using MineSharp.Data.Protocol.Handshaking.Serverbound;
using MineSharp.Data.Protocol.Login.Clientbound;
using MineSharp.Data.Protocol.Login.Serverbound;
using MineSharp.Data.Protocol.Status.Serverbound;
using MineSharp.Skeletons.Barebones.Client;
using MineSharp.Skeletons.Minimal.MinimalClient.Handlers;
namespace MineSharp.Skeletons.Minimal.MinimalClient
{
	public class MinecraftMinimalClient : MinecraftClient
	{
		public Session Session { get; }
		public int Protocol { get; set; }

		public delegate Task ClientConnectedHandler();
		public delegate Task ClientDisconnectedHandler(PacketDisconnect packet);

		public event ClientConnectedHandler? ClientConnected;
		public event ClientDisconnectedHandler? ClientDisconnected;
		
		public MinecraftMinimalClient(IPEndPoint endPoint, Session session, int protocol) : base(endPoint)
		{
			this.Session = session;
			this.Protocol = protocol;
			this.PacketReceived += (packet) => PacketHandler.HandleIncoming(packet, this);
			this.PacketSent += (packet) => PacketHandler.HandleOutgoing(packet, this);
		}
		
		public async Task Connect(GameState nextState)
		{
			await base.Connect();
			this.SendPacket(new PacketSetProtocol(this.Protocol, this.EndPoint.Address.ToString(), (ushort) this.EndPoint.Port, (int) nextState), GameState.HANDSHAKING);
			switch (nextState)
			{
				case GameState.LOGIN:
					this.SendPacket(new PacketLoginStart(this.Session.Username, null, this.Session.Uuid), GameState.LOGIN); // TODO: Add support for signatures
					break;
				case GameState.STATUS:
					this.SendPacket(new PacketPingStart(), GameState.STATUS);
					break;
				case GameState.HANDSHAKING:
				case GameState.PLAY:
				default:
					throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
			}
			this.GameState = nextState;
		}
		public Task OnClientConnected()
		{
			this.ClientConnected?.Invoke();
			return Task.CompletedTask;
		}
		
		public Task OnClientDisconnected(PacketDisconnect packet)
		{
			this.Disconnect();
			this.ClientDisconnected?.Invoke(packet);
			return Task.CompletedTask;
		}
	}
}
