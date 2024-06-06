using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace APITrabalhoFinal.Services.Exceptions
{
    public class InvalidDataException : Exception
    {
        public IEnumerable<ValidationFailure> ValidationErrors { get; }

        public InvalidDataException(string message, IEnumerable<ValidationFailure> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }
}
