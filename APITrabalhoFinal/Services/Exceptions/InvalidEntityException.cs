using System;

namespace APITrabalhoFinal.Services.Exceptions
{
    public class InvalidEntityException : Exception
    {
        public InvalidEntityException(string message) : base(message) { }
    }

}