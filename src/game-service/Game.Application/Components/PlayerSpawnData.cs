﻿using Common.ComponentModel;
using Common.MathematicsHelper;

namespace Game.Application.Components
{
    [ComponentSettings(ExposedState.Exposable)]
    public class PlayerSpawnData : ComponentBase, IPlayerSpawnData
    {
        private readonly Vector2 position;
        private readonly Vector2 size;

        public PlayerSpawnData(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public Vector2 GetSize()
        {
            return size;
        }
    }
}