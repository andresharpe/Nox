using Nox.Core.Interfaces.Messaging;

namespace Nox.Messaging.AmazonSQS;

public class AmazonSqsMessageBusProvider: IMessageBusProvider
{
    private string _connectionString;


    public AmazonSqsMessageBusProvider(IServiceMessageBus serviceBus)
    {
        _connectionString = serviceBus.ConnectionString!;
    }
}