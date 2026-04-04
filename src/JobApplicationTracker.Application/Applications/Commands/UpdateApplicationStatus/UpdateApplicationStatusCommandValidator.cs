using FluentValidation;

namespace JobApplicationTracker.Application.Applications.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
