﻿using MineSharp.Components.Core.Types;
using MineSharp.Data.Blocks;

namespace MineSharp.Components.Physics
{
    public static class PhysicsConst
    {

        public const double Gravity = 0.08f;
        public const double WaterGravity = Gravity / 16;
        public const double LavaGravity = Gravity / 4;
        public const double Airdrag = 0.98f;
        public const double YawSpeed = 3.0f;
        public const double PitchSpeed = 3.0f;
        public const double SprintSpeed = 0.3d;
        public const double SneakSpeed = 0.3f;
        public const double StepHeight = 0.6f;
        public const double NegligeableVelocity = 0.003f;
        public const double SoulsandSpeed = 0.4f;
        public const double HoneyblockSpeed = 0.4f;
        public const double HoneyblockJumpSpeed = 0.4f;
        public const double LadderMaxSpeed = 0.15f;
        public const double LadderClimbSpeed = 0.2f;
        public const double PlayerHalfWidth = 0.3f;
        public const double PlayerHeight = 1.8f;
        public const double WaterInertia = 0.8f;
        public const double LavaInertia = 0.5f;
        public const double LiquidAcceleration = 0.02f;
        public const double AirborneInertia = 0.91f;
        public const double AirborneAcceleration = 0.02f;
        public const double DefaultSlipperiness = 0.6f;
        public const double OutOfLiquidImpulse = 0.3f;
        public const int AutoJumpCooldown = 10;
        public const double SlowFalling = 0.125f;
        public const double SpeedEffect = 1.2f;
        public const double SlowEffect = 0.85f;
        public const string MovementSpeedAttribute = "generic.movement_speed";
        public const double PlayerSpeed = 0.1d;
        public const double MaxFallDistance = 0.625d;
        public static BubbleColumnDragC BubbleColumnSurfaceDrag = new BubbleColumnDragC(0.03f, -0.9f, 0.1f, 1.8f);
        public static BubbleColumnDragC BubbleColumnDrag = new BubbleColumnDragC(0.03f, -0.03f, 0.06f, 0.7f);
        public static readonly UUID SprintingUUID = UUID.Parse("662a6b8d-da3e-4c1c-8813-96ea6097278d");

        public static List<int> WaterLikeBlocks = new List<int> {
            (int)BlockType.Seagrass,
            (int)BlockType.TallSeagrass,
            (int)BlockType.Kelp,
            (int)BlockType.BubbleColumn
        };

        public static double GetBlockSlipperiness(int blockId)
        {
            switch (blockId)
            {
                case (int)BlockType.SlimeBlock:
                    return 0.8f;
                case (int)BlockType.Ice:
                    return 0.98f;
                case (int)BlockType.PackedIce:
                    return 0.98f;
                case (int)BlockType.FrostedIce:
                    return 0.98f;
                case (int)BlockType.BlueIce:
                    return 0.989f;
                default:
                    return DefaultSlipperiness;
            }
        }

        public static AABB GetPlayerBoundingBox(Vector3 pos)
        {
            var bb = new AABB(-PlayerHalfWidth, 0, -PlayerHalfWidth, PlayerHalfWidth, PlayerHeight, PlayerHalfWidth)
                .Offset(pos.X, pos.Y, pos.Z);
            return bb;
        }

        public class BubbleColumnDragC
        {

            public BubbleColumnDragC(double down, double maxDown, double up, double maxUp)
            {
                this.Down = down;
                this.MaxDown = maxDown;
                this.Up = up;
                this.MaxUp = maxUp;
            }

            public double Down { get; set; }
            public double MaxDown { get; set; }
            public double Up { get; set; }
            public double MaxUp { get; set; }
        }
    }
}
