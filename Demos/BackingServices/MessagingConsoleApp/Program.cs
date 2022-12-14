using CoreXF.Messaging;
using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Channels.InProcess;
using CoreXF.Messaging.Messages;

namespace MessagingConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Program.FireAndForget();
            Program.PublishSubscribe();
            Program.RequestResponse();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrate (broadcast) fire-and-forget messaging.
        /// </summary>
        private static void FireAndForget()
        {
            Console.WriteLine("Demonstrating broadcast (Fire-and-Forget):");

            // use DI in more realistic scenario(s); usually the broker is registered as a singleton.
            var broker = new MessageBroker(new InProcessChannelFactory(default)) as IMessageBroker;
            var messageType = $"demo-{typeof(int).Name}";

            var message = new FireAndForgetMessage(messageType, 42);
            message.TimeToLive = new TimeSpan(0, 0, 1);
            broker.Fire(message); // nothing happens, there are

            Console.WriteLine($"Peek 'Fire-n-Forget message of type '{messageType}'.");
            var peeked = broker.Peek(messageType);
            Console.WriteLine($"Peeked: {peeked.Count()}.");
            foreach (var item in peeked)
            {
                var payload = item.GetPayload<int>();
                Console.WriteLine(payload);
            }

            Console.WriteLine("Waiting for 2 sec.");
            Thread.Sleep(2000);

            Console.WriteLine($"Peek 'Fire-n-Forget message of type '{messageType}'.");
            peeked = broker.Peek(messageType);
            Console.WriteLine($"Peeked: {peeked.Count()}.");
            foreach (var item in peeked)
            {
                var payload = item.GetPayload<int>();
                Console.WriteLine(payload);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrate (multicast) publishes-subscribe messaging.
        /// </summary>
        private static void PublishSubscribe()
        {
            Console.WriteLine("Demonstrating multicast (Publish-Subscribe):");

            // use DI in more realistic scenario(s); usually the broker is registered as a singleton.
            var broker = new MessageBroker(new InProcessChannelFactory(default)) as IMessageBroker;
            var messageType = $"demo-{typeof(int).Name}";
            broker.Register(messageType);

            // check if message of type messageType is registered
            var registered = broker.IsRegistered(messageType);
            Console.WriteLine($"Message of type '{messageType} is {(registered ? "registered" : "not registered")}. Broker is ready for subscriptions.");

            // create two subscribers
            broker.Subscribe(new Subscriber("subscriber-A"), messageType);
            broker.Subscribe(new Subscriber("subscriber-B"), messageType);

            var publisher = new Publisher(broker);
            publisher.Publish(new PublishedMessage(messageType, "Hello, subscribers"));

            broker.Unsubscribe("subscriber-A", messageType);
            broker.Unsubscribe("subscriber-B", messageType);
            Console.WriteLine($"Unregister message type : '{messageType}'.");
            broker.Unregister(messageType);

            publisher.Publish(new PublishedMessage(messageType, "Hello, subscribers to a non-registered message type"));
            Console.WriteLine();
        }

        /// <summary>
        /// The subscriber class. Receives messages sent by a publisher. A CoreXF extension implementing this interface can
        /// receive messages sent by publishers.
        /// </summary>
        public class Subscriber : ISubscriber
        {
            /// <summary>
            /// Gets the identity.
            /// </summary>
            public string Identity { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscriber"/> class.
            /// </summary>
            /// <param name="identity">The identity.</param>
            public Subscriber(string identity)
            {
                this.Identity = identity;
            }

            /// <summary>
            /// Receive a published message.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <exception cref="NotImplementedException"></exception>
            public void Receive(IPublishedMessage message)
            {
                Console.WriteLine($"Subscriber '{this.Identity}' received a message: '{message.GetPayload<string>()}'");
            }
        }

        /// <summary>
        /// The publisher.
        /// </summary>
        public class Publisher
        {
            private readonly IMessageBroker broker;

            /// <summary>
            /// Initializes a new instance of the <see cref="Publisher"/> class.
            /// </summary>
            /// <param name="broker">The broker.</param>
            public Publisher(IMessageBroker broker)
            {
                this.broker = broker;
            }

            /// <summary>
            /// Publish the message.
            /// </summary>
            /// <param name="message">The message.</param>
            public void Publish(PublishedMessage message)
            {
                this.broker.Publish(message);
            }
        }

        /// <summary>
        /// Demonstrating unicast (Request-Response) messaging.
        /// </summary>
        private static void RequestResponse()
        {
            Console.WriteLine("Demonstrating unicast (Request-Response):");

            // use DI in more realistic scenario(s); usually the broker is registered as a singleton.
            var broker = new MessageBroker(new InProcessChannelFactory(default)) as IMessageBroker;
            var messageTypeA = $"demo-{typeof(int).Name}";
            var messageTypeB = $"demo-{typeof(string).Name}";

            broker.AddRecipient(messageTypeA, new Recipient("recipient-A", typeof(int)));
            broker.AddRecipient(messageTypeB, new Recipient("recipient-B", typeof(string)));

            var responseA = broker.Request(new RequestMessage(messageTypeA, 123));
            Console.WriteLine($"The response from 'recipient-A' is: {responseA.StatusCode}.");

            var responseB = broker.Request(new RequestMessage(messageTypeB, "Hello, recipient"));
            Console.WriteLine($"The response from 'recipient-B' is: {responseB.StatusCode}.");
            Console.WriteLine();
        }

        /// <summary>
        /// The recipient.
        /// </summary>
        public class Recipient : IRecipient
        {
            private readonly Type type;

            /// <summary>
            /// Initializes a new instance of the <see cref="Recipient"/> class.
            /// </summary>
            /// <param name="identity">The identity.</param>
            public Recipient(string identity, Type type)
            {
                this.type = type;
                this.Identity = identity;
            }

            /// <summary>
            /// Gets the identity.
            /// </summary>
            public string Identity { get; }

            /// <summary>
            /// Receive the <see cref="IMessageResponse"/>.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <returns>An IMessageResponse.</returns>
            public IMessageResponse Receive(IRequestMessage message)
            {
                var payload = message.GetPayload(this.type);
                Console.WriteLine($"Recipient '{this.Identity}' received a message: '{payload}'");
                return new MessageResponse(this, StatusCode.Success);
            }
        }
    }
}