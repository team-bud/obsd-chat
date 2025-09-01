using System.Collections.Concurrent;
using ToolBox.Ticket;

namespace ServerCore;

public class ChatRoom
{
    // core
    public ChatRoom(ChatServer.ID owner, ChatServer.CreateRoom ticket, string creatorEmail)
    {
        Owner = owner;
        Users = new() { creatorEmail };
        Ticket = ticket;
        
        ChatRoomManager.Register(this);
    }

    internal void Delete()
    {
        ChatRoomManager.Unregister(Id);
    }

    // state
    internal ID Id { get; } = ID.Random();
    internal ChatServer.ID Owner { get; }
    internal ChatServer.CreateRoom Ticket { get; }
    
    public HashSet<string> Users { get; } 
    
    public List<Message.ID> Messages { get; } = new();
    public Queue<CreateMessage> Tickets { get; } = new();
    public void AddTicket(CreateMessage ticket) => Tickets.Enqueue(ticket);
    
    
    
    // action
    public void CreateMessages()
    {
        while (Tickets.Count > 0)
        {
            var ticket = Tickets.Dequeue();

            var messageRef = new Message(Id, ticket, ticket.Body);
            Messages.Add(messageRef.Id);
        }
    }


    // value
    public readonly record struct ID
    {
        // core
        public required Guid RawValue { get; init; }

        public static ID Random() => new ID { RawValue = Guid.NewGuid() };

        // operator
        public bool IsExist()
        {
            return ChatRoomManager.Get(this) is not null;
        }
        public ChatRoom? Ref()
        {
            return ChatRoomManager.Get(this);
        }
    }

    public readonly record struct CreateMessage() : ITicket
    {
        // core
        public Guid Id { get; } = Guid.NewGuid();
        public required string Body { get; init; }
    }
}


// ObjectManager
internal static class ChatRoomManager
{
    // core
    internal static ConcurrentDictionary<ChatRoom.ID, ChatRoom> Container = new();

    internal static void Register(ChatRoom obj)
    {
        Container.TryAdd(obj.Id, obj);
    }

    internal static void Unregister(ChatRoom.ID id)
    {
        Container.TryRemove(id, out _);

    }

    internal static ChatRoom? Get(ChatRoom.ID id)
    {
        Container.TryGetValue(id, out ChatRoom? room);
        return room;
    }
}