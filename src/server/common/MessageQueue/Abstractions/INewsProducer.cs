using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface INewsProducer: IMessageProducer<NewsQueueModel[]>
{
    
}