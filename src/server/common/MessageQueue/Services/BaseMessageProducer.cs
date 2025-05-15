using System.Text.Json;
using MessageQueue.Abstractions;
using MessageQueue.Constants;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using RabbitMQ.Client;

namespace MessageQueue.Services;

public abstract class BaseMessageProducer<TModel>: IMessageProducer<TModel>, IDisposable
{
	private IConnection? _connection;
	private IModel? _channel;
	private readonly string _exchangeName;
	private readonly string _routingKey;
	private readonly Uri _serverAddress;
	private readonly string _queueName;

	private readonly ResiliencePipelineProvider<string> _pipelineProvider;
	private readonly ILogger<BaseMessageProducer<TModel>> _logger;

	protected BaseMessageProducer(Uri serverAddress, string queueName, ResiliencePipelineProvider<string> pipelineProvider, ILogger<BaseMessageProducer<TModel>> logger)
	{
		_serverAddress = serverAddress;
		_queueName = queueName;
		_pipelineProvider = pipelineProvider;
		_logger = logger;
		_exchangeName = string.Empty;
		_routingKey = queueName;
	}

	public void Publish(TModel obj)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			JsonSerializer.Serialize(ms, obj);
			var pipeline = _pipelineProvider.GetPipeline(MessageQueuePipelineConstants.ResiliencePipelineRabbitMq);

			try
			{
				pipeline.Execute(cancellationToken =>
				{
					GetOrCreateChannel()
						.BasicPublish(_exchangeName, _routingKey, null, ms.ToArray());
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error has occurred while publish \"{obj}\".", obj);
				throw;
			}
		}
	}

	private IModel? GetOrCreateChannel()
	{
		if (_channel != null)
		{
			return _channel;
		}

		ConnectionFactory factory = new ConnectionFactory
		{
			Uri = _serverAddress
		};

		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		_channel.QueueDeclare(_queueName, exclusive: false);

		return _channel;
	}

	public void Dispose()
	{
		_connection?.Dispose();
		_channel?.Dispose();
	}
}
