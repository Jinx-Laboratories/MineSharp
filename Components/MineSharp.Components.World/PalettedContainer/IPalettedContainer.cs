using MineSharp.Components.Core.Types;
using MineSharp.Components.World.PalettedContainer.Palettes;
using MineSharp.Data.Protocol;

namespace MineSharp.Components.World.PalettedContainer
{
    public interface IPalettedContainer
    {

        public IPalette Palette { get; set; }
        public int Capacity { get; }
        public IntBitArray Data { get; set; }

        public static void Read(PacketBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public int GetAt(int index);

        public void SetAt(int index, int state);
    }
}
