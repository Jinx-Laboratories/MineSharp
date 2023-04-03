using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using MineSharp.Components.Core.Types.Enums;
using MineSharp.Components.Protocol;
using MineSharp.Data.Protocol;

namespace MineSharp.Skeletons.Barebones.Client
{
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public class MinecraftClient
    {
        public delegate Task PacketReceivedHandler(IPacketPayload packet);
        public delegate Task PacketSentHandler(IPacketPayload packet);

        public event PacketReceivedHandler? PacketReceived;
        public event PacketSentHandler? PacketSent;
        
        private readonly Queue<(IPacketPayload Packet, GameState? State)> _packetQueue = new Queue<(IPacketPayload Packet, GameState? State)>();
        
        private TcpClient? Client { get; set; }
        
        public IPEndPoint EndPoint { get; }
        public CancellationTokenSource? CancellationTokenSource { get; private set; }
        public MinecraftStream? Stream { get; private set; }
        public GameState GameState { get; set; } = GameState.HANDSHAKING;
        
        public MinecraftClient(IPEndPoint endPoint)
        {
            this.EndPoint = endPoint;
        }

        public async Task Connect()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Client = new TcpClient();
            
            await this.Client.ConnectAsync(this.EndPoint, this.CancellationTokenSource.Token);
            this.Stream = new MinecraftStream(this.Client.GetStream());
            
            _ = Task.Run(this.Loop, this.CancellationTokenSource.Token);
        }

        public void Disconnect()
        {
            this.CancellationTokenSource?.Cancel();
            this.Stream?.Close();
            this.Client?.Close();
        }

        private async Task Loop()
        {
            while (!this.CancellationTokenSource!.IsCancellationRequested)
            {
                while (this.Stream!.IsAvailable)
                {
                    var incomingPacket = PacketFactory.ClientPacketFactory.BuildPacket(this.Stream!.ReadPacket(), this.GameState);

                    if (incomingPacket != null) _ = Task.Run(() => this.PacketReceived?.Invoke(incomingPacket), this.CancellationTokenSource.Token);
                    if (this.GameState != GameState.PLAY) await Task.Delay(1, this.CancellationTokenSource.Token);
                }

                await Task.Delay(1, this.CancellationTokenSource.Token);

                if (this._packetQueue.Count == 0) continue;

                var packet = this._packetQueue.Dequeue();
                var p = PacketFactory.ClientPacketFactory.WritePacket(packet.Packet, packet.State ?? this.GameState);
                this.Stream!.WritePacket(p);
                _ = Task.Run(() => this.PacketSent?.Invoke(packet.Packet), this.CancellationTokenSource.Token);
            }
        }
        
        public void SendPacket<T>(T packet, GameState? state = null) where T : IPacketPayload
        {
            this._packetQueue.Enqueue((packet, state));
        }
    }
} 