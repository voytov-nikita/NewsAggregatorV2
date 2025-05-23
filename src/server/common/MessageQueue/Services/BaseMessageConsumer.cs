using System.Text.Json;
using MessageQueue.Abstractions;
using MessageQueue.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageQueue.Services;

public abstract class BaseMessageConsumer<TModel>: IDisposable
{
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _queueName;
	private readonly string _errorQueueName;

	protected BaseMessageConsumer(Uri serverAddress, string queueName)
	{
		var factory = new ConnectionFactory
		{
			Uri = serverAddress,
			DispatchConsumersAsync = true
		};
		_connection = factory.CreateConnection();

		_channel = _connection.CreateModel();

		_queueName = queueName;
		_errorQueueName = queueName + ".error";

		_channel.QueueDeclare(queueName, exclusive: false);
		_channel.QueueDeclare(_errorQueueName, exclusive: false);
	}

	public void AddSubscription(Func<TModel, Task> callback)
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);

		// add the message receive event
		consumer.Received += async (model, deliveryEventArgs) =>
		{
			try
			{
				byte[] body = deliveryEventArgs.Body.ToArray();
				// convert the message back from byte[] to a string
				TModel message = JsonSerializer.Deserialize<TModel>(body)!;

				await callback(message);
			}
			catch (Exception e)
			{
				// Messages that can't be processed will be moved to error queue
				SendErrorModel(e, deliveryEventArgs, _queueName);
			}
			finally
			{
				// ack the message, ie. confirm that we have processed it
				// otherwise it will be requeued a bit later
				_channel.BasicAck(deliveryEventArgs.DeliveryTag, false);

			}
		};

		// start consuming
		_ = _channel.BasicConsume(consumer, _queueName);
	}

	private void SendErrorModel(Exception exception, BasicDeliverEventArgs deliveryEventArgs, string queueName)
	{
		ConsumeErrorModel errorModel = new ConsumeErrorModel
		{
			OriginalQueue = queueName,
			OrigimalMeggage = deliveryEventArgs.Body.ToArray(),
			
			ExceptionTypeName = exception.GetType()
				.FullName,
			ExceptionMessage = exception.Message
		};
		using (MemoryStream ms = new MemoryStream())
		{
			JsonSerializer.Serialize(ms, errorModel);

			_channel.BasicPublish(string.Empty, _errorQueueName, null, ms.ToArray());
		}
	}

	public void Dispose()
	{
		_connection.Dispose();
		_channel.Dispose();
	}
}
