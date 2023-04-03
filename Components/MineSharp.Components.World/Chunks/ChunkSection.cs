﻿using MineSharp.Components.Core.Types;
using MineSharp.Components.World.PalettedContainer;
using MineSharp.Data.Biomes;
using MineSharp.Data.Blocks;
using MineSharp.Data.Protocol;
using MineSharp.Data;

namespace MineSharp.Components.World.Chunks
{
    public class ChunkSection
    {
        private readonly BiomePalettedContainer BiomeStorage;
        private readonly BlockPalettedContainer BlockStorage;

        public ChunkSection(short blockCount, BlockPalettedContainer blockContainer, BiomePalettedContainer biomeContainer)
        {
            this.SolidBlockCount = blockCount;
            this.BlockStorage = blockContainer;
            this.BiomeStorage = biomeContainer;
        }


        public short SolidBlockCount { get; private set; }

        // Positions used in this Class should be local coordinates, relative to the chunk

        public static ChunkSection Read(PacketBuffer buffer)
        {
            var solidBlockCount = buffer.ReadI16();
            var blockContainer = BlockPalettedContainer.Read(buffer);
            var biomeContainer = BiomePalettedContainer.Read(buffer);

            return new ChunkSection(solidBlockCount, blockContainer, biomeContainer);
        }

        public void Update(long[] blocks)
        {
            for (var i = 0; i < blocks.Length; i++)
            {
                var stateId = (int)(blocks[i] >> 12);
                var x = (int)(blocks[i] >> 8 & 0xF);
                var z = (int)(blocks[i] >> 4 & 0xF);
                var y = (int)(blocks[i] >> 0 & 0xF);

                var blockId = BlockPalette.GetBlockIdByState(stateId);
                var block = new Block(BlockPalette.GetBlockInfoById(blockId), stateId, new Position(x, y, z));
                this.SetBlock(block);
            }
        }

        public Biome GetBiomeAt(Position pos) => this.GetBiomeAt(this.GetBiomeIndex(pos.X, pos.Y, pos.Z));

        public Block GetBlockAt(Position blockPos)
        {
            var state = this.BlockStorage.GetAt(this.GetBlockIndex(blockPos.X, blockPos.Y, blockPos.Z));
            var blockId = BlockPalette.GetBlockIdByState(state);
            var block = new Block(BlockPalette.GetBlockInfoById(blockId), state, blockPos);
            return block;
        }

        public void SetBlock(Block block)
        {
            var index = this.GetBlockIndex(block.Position!.X, block.Position!.Y, block.Position!.Z);

            var oldBlock = this.GetBlockAt(block.Position);

            if (oldBlock.IsSolid() && !block.IsSolid()) this.SolidBlockCount--;
            else if (!oldBlock.IsSolid() && block.IsSolid()) this.SolidBlockCount++;


            this.BlockStorage.SetAt(index, (int)block.State!);
        }

        private int GetBiomeIndex(int x, int y, int z) => y >> 2 << 2 | z >> 2 << 2 | x >> 2;

        private int GetBlockIndex(int x, int y, int z) => y << 8 | z << 4 | x;

        private Biome GetBiomeAt(int index)
        {
            var state = this.BiomeStorage.GetAt(index);
            var info = BiomePalette.GetBiomeInfoById(state);
            return new Biome(info);
        }

        public Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null)
        {

            return Task.Factory.StartNew(() =>
            {
                var blockInfo = type.GetInfo();

                if (!this.BlockStorage.Palette.HasState(blockInfo.MinStateId, blockInfo.MaxStateId)) return null;

                for (var y = 0; y < Chunk.ChunkSectionLength; y++)
                {
                    for (var z = 0; z < Chunk.ChunkSectionLength; z++)
                    {
                        for (var x = 0; x < Chunk.ChunkSectionLength; x++)
                        {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            var value = this.BlockStorage.GetAt(this.GetBlockIndex(x, y, z));
                            if (blockInfo.MinStateId <= value && value <= blockInfo.MaxStateId)
                                return this.GetBlockAt(new Position(x, y, z));
                        }
                    }
                }

                return null;
            });
        }

        public Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null)
        {

            return Task.Factory.StartNew(() =>
            {

                var blocks = new List<Block>();
                var blockInfo = type.GetInfo();

                if (!this.BlockStorage.Palette.HasState(blockInfo.MinStateId, blockInfo.MaxStateId)) return null;

                for (var y = 0; y < Chunk.ChunkSectionLength; y++)
                {
                    for (var z = 0; z < Chunk.ChunkSectionLength; z++)
                    {
                        for (var x = 0; x < Chunk.ChunkSectionLength; x++)
                        {
                            if (cancellation?.IsCancellationRequested ?? false) return null;
                            var value = this.BlockStorage.GetAt(this.GetBlockIndex(x, y, z));
                            if (blockInfo.MinStateId <= value && value <= blockInfo.MaxStateId)
                            {
                                blocks.Add(this.GetBlockAt(new Position(x, y, z)));
                                if (count > 0 && blocks.Count >= count)
                                {
                                    return blocks.Take(count).ToArray();
                                }
                            }
                        }
                    }
                }

                if (blocks.Count == 0) return null;
                return blocks.ToArray();
            });
        }
    }
}
