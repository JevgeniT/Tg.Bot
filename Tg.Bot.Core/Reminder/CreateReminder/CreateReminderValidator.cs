using ErrorOr;
using FluentValidation;
using MediatR;

namespace Tg.Bot.Core.Reminder.CreateReminder;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validator is null) return await next();
   
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid) return await next();
     
        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));
        
        return (dynamic)errors;
    }
}
public class CreateReminderValidator : AbstractValidator<CreateReminderCommand> 
{
    public CreateReminderValidator()
    {
        RuleFor(x => x.Text).MinimumLength(5);
        RuleFor(x => x.RemindOn).GreaterThan( x=> DateTime.Today.AddMinutes(30));
    }
}