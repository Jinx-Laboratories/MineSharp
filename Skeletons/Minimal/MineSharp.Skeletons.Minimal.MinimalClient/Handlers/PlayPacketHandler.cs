using MineSharp.Data.Protocol;
namespace MineSharp.Skeletons.Minimal.MinimalClient.Handlers
{
	public class PlayPacketHandler : IPacketHandler
	{
		public Task HandleIncoming(IPacketPayload packet, MinecraftMinimalClient client) => throw new NotImplementedException();
		public Task HandleOutgoing(IPacketPayload packet, MinecraftMinimalClient client) => throw new NotImplementedException();
	}
}
