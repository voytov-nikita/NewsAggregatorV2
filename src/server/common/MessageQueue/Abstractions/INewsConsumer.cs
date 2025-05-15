using MessageQueue.Models;

namespace MessageQueue.Abstractions;

public interface INewsConsumer: IMessageConsumer<NewsQueueModel[]>
{
    
}