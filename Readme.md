# Overseer.ServiceConsole

A **Console App** and **Windows Service** stub application for running [Overseer][overseer].

## Usage

You need to modify this application to do anything useful - all the code to modify is in `Service.cs`:

```csharp
protected override void OnStart(string[] args)
{
	// replace these with your own adapters
	IMessageReader reader = new InMemoryMessageReader();
	IMessageConverter converter = new DirectMessageConverter();
	IValidatorSource source = new FileValidatorSource(Path.Combine(_baseDirectory, "validators"));

	var output = new SerilogValidationOutput();

	_monitor = new QueueMonitor(reader, converter, new MessageValidator(source), output);
	_monitor.Start();
}
```

For example, to configure this to run against a RabbitMQ server:

> PM> Install-Package Overseer.RabbitMQ

Then modify the `OnStart` method:

```csharp
protected override void OnStart(string[] args)
{
	var reader = new RabbitMessageReader(new RabbitOptions
	{
		HostName = "192.168.59.103",
		ExchangeName = "DomainEvents"
	});

	var converter = new RabbitMessageConverter();

	var source = new FileValidatorSource(Path.Combine(_baseDirectory, "validators"));
	var output = new SerilogValidationOutput();

	_monitor = new QueueMonitor(reader, converter, new MessageValidator(source), output);
	_monitor.Start();
}
```



[overseer]: http://github.com/pondidum/overseer
