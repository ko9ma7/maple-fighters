using Box2DX.Dynamics;
using Coroutines;
using Game.Application.Objects.Components;
using Game.Physics;

namespace Game.Application.Objects
{
    public class MobGameObject : GameObject
    {
        public MobGameObject(int id, string name, ICoroutineRunner coroutineRunner)
            : base(id, name)
        {
            Components.Add(new GameObjectGetter(this));
            Components.Add(new ProximityChecker());
            Components.Add(new PresenceMapProvider());
            Components.Add(new MobMoveBehaviour(coroutineRunner));
            Components.Add(new PhysicsBodyPositionSetter());
        }

        public NewBodyData CreateBodyData()
        {
            var bodyDef = new BodyDef();
            bodyDef.Position.Set(Transform.Position.X, Transform.Position.Y);

            var polygonDef = new PolygonDef();
            polygonDef.SetAsBox(0.3625f, 0.825f);
            polygonDef.Density = 0.0f;
            polygonDef.Filter = new FilterData()
            {
                GroupIndex = (short)LayerMask.Mob
            };
            polygonDef.UserData = new MobContactEvents(Id);

            return new NewBodyData(Id, bodyDef, polygonDef);
        }
    }
}