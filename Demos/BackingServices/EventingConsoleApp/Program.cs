using CoreXF.Eventing;
using CoreXF.Eventing.Abstractions;

namespace EventingConsoleApp
{
    /// <summary>
    /// This console app demonstrates CoreXF event services.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Program's entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            // use DI in more realistic scenario(s); usually the aggregator is registered as a singleton.
            var agregator = new EventAggregator() as IEventAggregator;

            var recipient = new Recipient();

            // recipient has to subscribed so as to receive notifications
            agregator.Subscribe<Event>(recipient);

            var sender = new Sender();
            sender.Publish(new Event($"Payload-{123}"), agregator);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// The sender which publishes information about an event by using the event aggregator. A CoreXF extension implementing 
    /// this interface can notify other extensions about happening of event.
    /// </summary>
    public class Sender : ISender
    {
        /// <summary>
        /// Publish the event using the event aggregator.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evt">The event.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public void Publish<TEvent>(TEvent evt, IEventAggregator eventAggregator) where TEvent : IEvent
        {
            Console.WriteLine($"Sender publishes: {evt}");
            eventAggregator.Publish(this, evt);
        }
    }

    /// <summary>
    /// The recipient class. Receives a notification about an event. A CoreXF extension implementing this interface can 
    /// receive notifications about an event sent by senders.
    /// </summary>
    public class Recipient : IRecipient
    {
        /// <summary>
        /// Handle the event sent by the sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        public void Handle(ISender sender, IEvent @event)
        {
            Console.WriteLine($"Recipient received: {@event.Payload}");
        }
    }

    /// <summary>
    /// This is the event notification which the sender sends and the recipient receives.
    /// </summary>
    public class Event : IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public Event(string text) => this.Payload = text;

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public object Payload { get; } = null!;

        /// <inheridtoc/>
        public override string ToString() => this.Payload.ToString() ?? string.Empty;
    }
}