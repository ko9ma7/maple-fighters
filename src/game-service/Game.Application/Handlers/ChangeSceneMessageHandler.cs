﻿using Game.Application.Components;
using Game.Application.Messages;
using Game.Application.Network;
using Game.Application.Objects;
using Game.Application.Objects.Components;

namespace Game.Application.Handlers
{
    public class ChangeSceneMessageHandler : IMessageHandler
    {
        private IMessageSender messageSender;
        private IProximityChecker proximityChecker;
        private IGameSceneManager gameSceneManager;

        public ChangeSceneMessageHandler(
            IMessageSender messageSender,
            IProximityChecker proximityChecker,
            IGameSceneManager gameSceneManager)
        {
            this.messageSender = messageSender;
            this.proximityChecker = proximityChecker;
            this.gameSceneManager = gameSceneManager;
        }

        public void Handle(byte[] rawData)
        {
            var message =
                MessageUtils.DeserializeMessage<ChangeSceneMessage>(rawData);
            var portalId = message.PortalId;
            var portal = GetPortal(portalId);
            if (portal != null)
            {
                var portalData = portal.Components.Get<IPortalData>();
                var map = portalData.GetMap();
                var gameSceneExists =
                    gameSceneManager.TryGetGameScene((Map)map, out var gameScene);
                if (gameSceneExists)
                {
                    proximityChecker.ChangeScene(gameScene);

                    SendSceneChangedMessage(map);
                }
            }
        }

        private void SendSceneChangedMessage(byte map)
        {
            var message = new SceneChangedMessage
            {
                Map = map
            };

            messageSender.SendMessage((byte)MessageCodes.SceneChanged, message);
        }

        private IGameObject GetPortal(int id)
        {
            var nearbyGameObjects = proximityChecker.GetNearbyGameObjects();

            foreach (var gameObject in nearbyGameObjects)
            {
                if (gameObject.Id != id)
                {
                    continue;
                }

                return gameObject;
            }

            return null;
        }
    }
}