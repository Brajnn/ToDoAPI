using FluentValidation;
using ToDoAPI.Models;

namespace ToDoAPI.Validation
{
    public class ToDoItemValidator : AbstractValidator<ToDoItem>
    {
        public ToDoItemValidator()
        {

            RuleFor(todo => todo.Title)
                .NotEmpty().WithMessage("Title is required.") 
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
                .MaximumLength(500).WithMessage("Title cannot exceed 100 characters."); 


            RuleFor(todo => todo.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters."); 


            RuleFor(todo => todo.ExpiryDate)
                .GreaterThan(DateTime.Now).WithMessage("Expiry date must be in the future."); 

            RuleFor(todo => todo.PercentComplete)
                .InclusiveBetween(0, 100).WithMessage("PercentComplete must be between 0 and 100."); 

         
            RuleFor(todo => todo.IsDone)
                .Equal(false).WithMessage("A new task cannot be marked as completed upon creation.");
        }
    }
}
