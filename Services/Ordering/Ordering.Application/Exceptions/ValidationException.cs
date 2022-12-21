﻿using FluentValidation.Results;
using System.Collections.Concurrent;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; set; }

        public ValidationException()
            :base("One or more validation failures have occurred")
        {
            Errors = new ConcurrentDictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            :this()
        {
            Errors = failures.GroupBy(s => s.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
