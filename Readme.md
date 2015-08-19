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

### Reading Messages

For example, if you wish to read from a RabbitMQ server, all you need to do is install the [Overseer.RabbitMQ][overseer-rabbit] package, and then modify the `OnStart` method to use the `RabbitMessageReader` and `RabbitMessageConverter`:

```powershell
PM> Install-Package Overseer.RabbitMQ
```
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

### Validating Messages

Overseer has three Validation Sources built in: `InMemory`, `File` and `Url`.  There is also a `CachingValidatorSource`, which can decorate any other `IValidatorSource` to provide cached access.

The three validation sources all build up `JsonSchemaValidator` objects to do the actual validation, which use a json object containing two [JsonSchema][json-schema]s - one for the headers of the messages and one for the body of the messages.

For example, if we had the following object:

```csharp
public class PersonExactMatch
{
	public Guid ID { get; set; }
	public string Name { get; set; }
	public DateTime Birthday { get; set; }
	public IEnumerable<Address> Addresses { get; set; }
}
```

And we wanted to validate that:
* The message itself had a `CorrelationId` specified.
* The object has an `ID`.
* The object has a `Name`.
* The object has at least one `Address`, which has a `Line1` and `PostCode`.

The validator json would look like this:

```json
{
    "type": "PersonExactMatch",
    "header": {
        "$schema": "http://json-schema.org/draft-04/schema#",
        "type": "object",
        "properties": {
            "CorrelationId": { "type": "string" }
        },
        "required": [ "CorrelationId" ]
    },
    "body": {
        "$schema": "http://json-schema.org/draft-04/schema#",

        "definitions": {
            "address": {
                "type": "object",
                "properties": {
                    "Line1": { "type": "string" },
                    "PostCode": { "type": "string" }
                },
                "required": [ "PostCode" ]
            }
        },

        "type": "object",
        "properties": {
            "ID": { "type": "string" },
            "Name": { "type": "string" },
            "Addresses": { "type": "array", "items": { "$ref": "#/definitions/address" } }
        },
        "required": [ "ID", "Name", "Addresses" ]
    }
}
```
The `type` key is the Type Name of the message to validate, the `header` key is the JsonSchema to validate the message headers, and the `body` key is the JsonSchema to validate the body.

### Output Results



[overseer]: https://github.com/pondidum/overseer
[overseer-rabbit]: https://github.com/pondidum/overseer.rabbitmq
[json-schema]: https://json-schema.org
