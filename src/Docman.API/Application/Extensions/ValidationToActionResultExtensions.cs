using System;
using Docman.Domain;
using Docman.Domain.Errors;
using Docman.Domain.Extensions;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Application.Extensions
{
    public static class ValidationToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Validation<Error, T> validation, Func<T, IActionResult> success) =>
            validation.Match(
                Succ: success,
                Fail: errors =>
                {
                    if (errors.Any(e => e is UserForbidError))
                        return new ForbidResult();

                    if (errors.Any(e => e is UserUnauthorizedError))
                        return new UnauthorizedResult();

                    if (errors.Any(e => e is DocumentNotFoundError || e is FileNotFoundError))
                        return new NotFoundResult();

                    return new BadRequestObjectResult(new { Errors = errors.Join() });
                });
    }
}