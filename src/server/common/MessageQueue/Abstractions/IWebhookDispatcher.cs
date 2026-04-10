namespace MessageQueue.Abstractions;

public interface IWebhookDispatcher
{
    void Dispatch<T>(string eventType, T data);
}
