﻿using Game.Common;
using Scripts.Gameplay.Entity;
using UnityEngine;

namespace Scripts.Gameplay.Player
{
    [RequireComponent(typeof(SpawnCharacter))]
    public class CharacterOrientation : MonoBehaviour
    {
        private Transform character;
        private ISpawnedCharacter spawnedCharacter;

        private void Awake()
        {
            spawnedCharacter = GetComponent<ISpawnedCharacter>();

            if (spawnedCharacter == null)
            {
                character = transform;
            }
        }

        private void Start()
        {
            if (spawnedCharacter != null)
            {
                spawnedCharacter.CharacterSpawned += OnCharacterSpawned;
            }

            var positionSetter = GetComponent<PositionSetter>();
            if (positionSetter != null)
            {
                positionSetter.DirectionChanged += OnDirectionChanged;
            }
        }

        private void OnDestroy()
        {
            if (spawnedCharacter != null)
            {
                spawnedCharacter.CharacterSpawned -= OnCharacterSpawned;
            }

            var positionSetter = GetComponent<PositionSetter>();
            if (positionSetter != null)
            {
                positionSetter.DirectionChanged -= OnDirectionChanged;
            }
        }

        private void OnCharacterSpawned()
        {
            character = spawnedCharacter.GetCharacterGameObject().transform;
        }

        private void OnDirectionChanged(Directions direction)
        {
            SetDirection(direction);
        }

        private void SetDirection(Directions direction)
        {
            var x = Mathf.Abs(character.localScale.x);

            switch (direction)
            {
                case Directions.Left:
                {
                    character.localScale = new Vector3(
                        x,
                        character.localScale.y,
                        character.localScale.z);
                    break;
                }

                case Directions.Right:
                {
                    character.localScale = new Vector3(
                        -x,
                        character.localScale.y,
                        character.localScale.z);
                    break;
                }
            }
        }

        public Directions GetDirection()
        {
            var direction = 
                character.localScale.x > 0 ? Directions.Left : Directions.Right;

            return direction;
        }
    }
}