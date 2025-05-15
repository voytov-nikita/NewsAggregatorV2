namespace MessageQueue.Abstractions;

public interface IMessageProducer<TModel>
{
	void Publish(TModel obj);
}
