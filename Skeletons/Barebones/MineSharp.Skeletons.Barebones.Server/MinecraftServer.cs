using System.Net;
using System.Net.Sockets;
namespace MineSharp.Skeletons.Barebones.Server
{
	public class MinecraftServer
	{
		public delegate Task ClientConnectedHandler(MinecraftRemoteClient client);
		
		public event ClientConnectedHandler? ClientConnected;
		
		public TcpListener? Listener { get; private set; }
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _loopTask;

        public Task Start(IPEndPoint endPoint)
        {
            this.Listener = new TcpListener(endPoint);
            this._cancellationTokenSource = new CancellationTokenSource();
            this.Listener.Start();

            this._loopTask = Task.Run(this.Loop, this._cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            this._cancellationTokenSource?.Cancel();
            this._loopTask!.Dispose();
            this.Listener.Stop();

            return Task.CompletedTask;
        }

        private async Task Loop()
        {
            while (!this._cancellationTokenSource!.IsCancellationRequested)
            {
	            while (this.Listener.Pending())
	            {
		            var client = await this.Listener.AcceptTcpClientAsync();
		            var remoteClient = new MinecraftRemoteClient(client);
		            await this.ClientConnected!.Invoke(remoteClient);
	            }
            }
        }
	}
}
