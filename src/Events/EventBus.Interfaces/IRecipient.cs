namespace EventBus.Interfaces
{
    public interface IRecipient
    {
        string Identity { get; set; }

        string Callback { get; set; }

        event ResponseEvent OnRecieve;

        IMessageResponse Recieve(IRequestResponseMessage message);
    }
}