namespace MessageQueue.Abstractions;

public interface IMessageConsumer<TModel>
{
	void AddSubscription(Func<TModel, Task> callback);
}
