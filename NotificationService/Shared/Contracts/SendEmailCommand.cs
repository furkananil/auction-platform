namespace Shared.Contracts;

public record SendEmailCommand
{
    public string To { get; init; } = default!;
    public string Subject { get; init; } = default!;
    public string Body { get; init; } = default!;

    public SendEmailCommand() { }

    public SendEmailCommand(string to, string subject, string body)
        => (To, Subject, Body) = (to, subject, body);
}