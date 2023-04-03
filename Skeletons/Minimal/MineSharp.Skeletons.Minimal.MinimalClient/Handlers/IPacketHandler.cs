using MineSharp.Data.Protocol;
namespace MineSharp.Skeletons.Minimal.MinimalClient.Handlers
{
	public interface IPacketHandler
	{
		public Task HandleIncoming(IPacketPayload packet, MinecraftMinimalClient client);
		public Task HandleOutgoing(IPacketPayload packet, MinecraftMinimalClient client);
	}
}
