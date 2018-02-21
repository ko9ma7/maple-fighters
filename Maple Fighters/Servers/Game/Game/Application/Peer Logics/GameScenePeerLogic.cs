﻿using Characters.Client.Common;
using CommonTools.Log;
using Game.Application.Components;
using Game.Application.PeerLogic.Components;
using Game.Application.PeerLogic.Operations;
using Game.InterestManagement;
using PeerLogic.Common;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationInterfaces;
using Shared.Game.Common;

namespace Game.Application.PeerLogics
{
    internal class GameScenePeerLogic : PeerLogicBase<GameOperations, GameEvents>
    {
        private readonly CharacterFromDatabaseParameters character;
        private readonly ISceneObject sceneObject;

        public GameScenePeerLogic(CharacterFromDatabaseParameters character)
        {
            this.character = character;

            sceneObject = CreateSceneObject(character);
        }

        public override void Initialize(IClientPeerWrapper<IClientPeer> peer)
        {
            base.Initialize(peer);

            AddCommonComponents();
            AddComponents();

            AddHandlerForEnterSceneOperation();
            AddHandlerForUpdatePositionOperation();
            AddHandlerForUpdatePlayerStateOperation();
            AddHandlerForChangeSceneOperation();
        }

        private void AddComponents()
        {
            sceneObject.Components.AddComponent(new PeerIdGetter(PeerWrapper.PeerId));

            Components.AddComponent(new SceneObjectGetter(sceneObject));
            Components.AddComponent(new InterestManagementNotifier());
            Components.AddComponent(new CharactersSender());
            Components.AddComponent(new PositionChangesListener());
        }

        private void AddHandlerForEnterSceneOperation()
        {
            OperationRequestHandlerRegister.SetHandler(GameOperations.EnterScene, new EnterSceneOperationHandler(sceneObject, character));
        }

        private void AddHandlerForUpdatePositionOperation()
        {
            var transform = sceneObject.Components.GetComponent<ITransform>().AssertNotNull();
            OperationRequestHandlerRegister.SetHandler(GameOperations.PositionChanged, new UpdatePositionOperationHandler(transform));
        }

        private void AddHandlerForUpdatePlayerStateOperation()
        {
            OperationRequestHandlerRegister.SetHandler(GameOperations.PlayerStateChanged, new UpdatePlayerStateOperationHandler(sceneObject));
        }

        private void AddHandlerForChangeSceneOperation()
        {
            OperationRequestHandlerRegister.SetHandler(GameOperations.ChangeScene, new ChangeSceneOperationHandler(sceneObject));
        }

        public override void Dispose()
        {
            base.Dispose();

            sceneObject.Dispose();
        }

        private ISceneObject CreateSceneObject(CharacterFromDatabaseParameters character)
        {
            var characterSceneObjectCreator = Server.Components.GetComponent<ICharacterCreator>().AssertNotNull();
            var characterSceneObject = characterSceneObjectCreator.Create(character);
            return characterSceneObject;
        }
    }
}