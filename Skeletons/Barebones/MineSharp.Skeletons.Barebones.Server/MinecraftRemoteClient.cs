using System.Net;
using System.Net.Sockets;
using MineSharp.Components.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Components.Protocol;
namespace MineSharp.Skeletons.Barebones.Server
{
	public class MinecraftRemoteClient
	{
        public delegate Task PacketReceivedHandler(IPacketPayload packet);

        public event PacketReceivedHandler? PacketReceived;

        private readonly TcpClient _client;
        private readonly Queue<(IPacketPayload Packet, GameState? State)> _packetQueue = new Queue<(IPacketPayload Packet, GameState? State)>();
        private readonly CancellationTokenSource? _cancellationTokenSource = new CancellationTokenSource();
        private readonly MinecraftStream? _stream;
        private readonly Task? _loopTask;
        
        public IPEndPoint RemoteEndPoint => (IPEndPoint) this._client.Client.RemoteEndPoint!;
        public GameState GameState { get; set; } = GameState.HANDSHAKING;

        public MinecraftRemoteClient(TcpClient client)
        {
            this._client = client;
            this._stream = new MinecraftStream(this._client.GetStream());
            
            this._loopTask = Task.Run(this.Loop, this._cancellationTokenSource.Token);
        }
        
        public void Disconnect()
        {
            this._packetQueue.Clear();
            this._cancellationTokenSource?.Cancel();
            this._loopTask!.Wait();
            this._client.Close();
        }

        private async Task Loop()
        {
            while (!this._cancellationTokenSource!.IsCancellationRequested)
            {
                while (this._stream!.IsAvailable)
                {
                    var incomingPacket = PacketFactory.ServerPacketFactory.BuildPacket(this._stream!.ReadPacket(), this.GameState);

                    if (incomingPacket != null) await this.PacketReceived?.Invoke(incomingPacket)!;
                    if (this.GameState != GameState.PLAY) await Task.Delay(1, this._cancellationTokenSource.Token);
                }

                await Task.Delay(1, this._cancellationTokenSource.Token);

                if (this._packetQueue.Count == 0) continue;

                var packet = this._packetQueue.Dequeue();
                var p = PacketFactory.ServerPacketFactory.WritePacket(packet.Packet, packet.State ?? this.GameState);
                this._stream!.WritePacket(p);
            }
        }
        
        public void SendPacket(IPacketPayload packet, GameState? state = null)
        {
            this._packetQueue.Enqueue((packet, state));
        }
	}
}
