namespace Game.Application.Objects.Components
{
    public interface IMessageSender
    {
        void SendMessage<TMessage>(byte code, TMessage message)
            where TMessage : class;

        void SendMessageToNearbyGameObjects<TMessage>(byte code, TMessage message)
            where TMessage : class;
    }
}