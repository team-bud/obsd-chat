using System.Collections.Concurrent;

namespace ServerCore;

public class Message
{
    // core
    public Message(ChatRoom.ID owner, ChatRoom.CreateMessage ticket, string body)
    {
        Ticket = ticket;
        Owner = owner;
        Body = body;

        MessageManager.Register(this);
    }
    internal void Delete()
    {
        MessageManager.Unregister(this.Id);
    }

    // state
    public ID Id { get; } = ID.Random();
    public ChatRoom.CreateMessage Ticket { get; }
    public ChatRoom.ID Owner { get; }
    
    public string Body { get; set; }
    
    
    // action
    public void RemoveMessage()
    {
        // capture
        var chatRoomRef = Owner.Ref()!;

        // muatate
        chatRoomRef.Messages.Remove(Id);
        this.Delete();
    }


    // value
    public readonly record struct ID
    {
        // core
        public required Guid RawValue { get; init; }

        public static ID With(Guid rawValue) => new ID { RawValue = rawValue };
        public static ID Random() => new ID { RawValue = Guid.NewGuid() };

        // operator
        public bool IsExist()
        {
            return MessageManager.Get(this) is not null;
        }
        public Message? Ref()
        {
            return MessageManager.Get(this);
        }
    }
}


// ObjectManager
internal static class MessageManager
{
    // core
    internal static ConcurrentDictionary<Message.ID, Message> Container = [];

    internal static void Register(Message obj)
    {
        Container.TryAdd(obj.Id, obj);
    }

    internal static void Unregister(Message.ID id)
    {
        Container.TryRemove(id, out _);
    }

    internal static Message? Get(Message.ID id)
    {
        Container.TryGetValue(id, out var messageRef);
        return messageRef;
    }
    
}