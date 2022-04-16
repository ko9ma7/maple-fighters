using System.Collections.Generic;
using Game.Application.Objects;
using InterestManagement;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Game.Application.Components
{
    public class GameSceneManager : ComponentBase
    {
        private IIdGenerator idGenerator;
        private IGameSceneCollection gameSceneCollection;

        protected override void OnAwake()
        {
            idGenerator = Components.Get<IIdGenerator>();
            gameSceneCollection = Components.Get<IGameSceneCollection>();

            var config = GetConfig();
            var SceneCollectionData = GetSceneCollectionData(config);

            CreateGameScene(SceneCollectionData);
        }

        protected override void OnRemoved()
        {
            gameSceneCollection.Dispose();
        }

        private void CreateGameScene(SceneCollectionData sceneCollection)
        {
            foreach (var sceneData in sceneCollection.Scenes)
            {
                var sceneName = sceneData.Key;
                var scene = sceneData.Value;
                var sceneSize = new Vector2(scene.SceneSize.X, scene.SceneSize.Y);
                var regionSize = new Vector2(scene.RegionSize.X, scene.RegionSize.Y);
                var playerSpawnPosition = new Vector2(scene.PlayerSpawn.Position.X, scene.PlayerSpawn.Position.Y);
                var playerSpawnSize = new Vector2(scene.PlayerSpawn.Size.X, scene.PlayerSpawn.Size.Y);
                var playerSpawnDirection = scene.PlayerSpawn.Direction;
                var objects = scene.Objects;

                var gameScene = new GameScene(sceneSize, regionSize);
                gameScene.PlayerSpawnData.SetPosition(playerSpawnPosition);
                gameScene.PlayerSpawnData.SetSize(playerSpawnSize);
                gameScene.PlayerSpawnData.SetDirection(playerSpawnDirection);

                foreach (var gameObject in CreateGameSceneObject(objects))
                {
                    var presenceMapProvider = gameObject.Components.Get<IPresenceMapProvider>();
                    presenceMapProvider.SetMap(gameScene);

                    gameScene.GameObjectCollection.Add(gameObject);
                }

                gameSceneCollection.Add(sceneName, gameScene);
            }
        }

        private IEnumerable<IGameObject> CreateGameSceneObject(ObjectData[] objects)
        {
            foreach (var objectData in objects)
            {
                var name = objectData.Name;
                var type = (ObjectTypes)objectData.Type;
                var position = new Vector2(objectData.Position.X, objectData.Position.Y);
                var size = new Vector2(objectData.Size.X, objectData.Size.Y);

                var id = idGenerator.GenerateId();
                var gameObject = GetGameObject(type, id, name);
                gameObject.Transform.SetPosition(position);
                gameObject.Transform.SetSize(size);

                yield return gameObject;
            }
        }

        private SceneCollectionData GetSceneCollectionData(string data)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<SceneCollectionData>(data);
        }

        private IGameObject GetGameObject(ObjectTypes type, int id, string name)
        {
            if (type == ObjectTypes.Npc) return new NpcGameObject(id, name);
            else if (type == ObjectTypes.Portal) return new PortalGameObject(id, name);
            else if (type == ObjectTypes.Mob) return new MobGameObject(id, name);
            else throw new System.Exception($"Unknown object type: {type}");
        }

        private string GetConfig()
        {
            return @"
scenes:
  lobby:
    sceneSize:
      x: 40
      y: 5
    regionSize: &regionSize
      x: 10
      y: 5
    playerSpawn:
      position:
        x: 18
        y: -1.86
      size: *regionSize
      direction: 1
    objects:
    - name: Guardian
      type: 0 # NPC
      position:
        x: -14.24
        y: -2.025
      size: *regionSize
    - name: Portal
      type: 1 # Portal
      position:
        x: -17.125
        y: -1.5
      size: *regionSize
  thedarkforest:
    sceneSize:
      x: 30
      y: 30
    regionSize: &regionSize
      x: 10
      y: 5
    playerSpawn:
      position:
        x: -12.8
        y: -12.95
      size: *regionSize
      direction: -1
    objects:
    - name: BlueSnail
      type: 0 # NPC
      position:
        x: -2.5
        y: -8.15
      size: *regionSize
    - name: BlueSnail
      type: 2 # Mob
      position:
        x: -2.85
        y: -3.05
      size: *regionSize
    - name: BlueSnail
      type: 2 # Mob
      position:
        x: -3.5
        y: -3.05
      size: *regionSize
    - name: Mushroom
      type: 2 # Mob
      position:
        x: -6.5
        y: -3.75
      size: *regionSize
    - name: Mushroom
      type: 2 # Mob
      position:
        x: -0.8
        y: -3.75
      size: *regionSize
    - name: Portal
      type: 1 # Portal
      position:
        x: -12.5
        y: -1.125
      size: *regionSize
            ";
        }
    }
}