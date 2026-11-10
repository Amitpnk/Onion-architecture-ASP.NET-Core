using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OA.Service.Exceptions;
using OA.Service.Middleware;
using System.Net;

namespace OA.Test.Unit.Middleware;

public class CustomExceptionMiddlewareTests
{
    private static IEnumerable<TestCaseData> ExceptionCases()
    {
        yield return new TestCaseData(new ValidationException(new List<ValidationFailure> { new("Name", "Required") }), HttpStatusCode.BadRequest)
            .SetName("ValidationException maps to 400");
        yield return new TestCaseData(new BadRequestException("bad input"), HttpStatusCode.BadRequest)
            .SetName("BadRequestException maps to 400");
        yield return new TestCaseData(new DeleteFailureException("Customer", 1, "in use"), HttpStatusCode.BadRequest)
            .SetName("DeleteFailureException maps to 400");
        yield return new TestCaseData(new NotFoundException("Customer", 1), HttpStatusCode.NotFound)
            .SetName("NotFoundException maps to 404");
        yield return new TestCaseData(new ApiException("boom"), HttpStatusCode.InternalServerError)
            .SetName("ApiException maps to 500");
        yield return new TestCaseData(new InvalidOperationException("unexpected"), HttpStatusCode.InternalServerError)
            .SetName("Unhandled exception maps to 500");
    }

    [TestCaseSource(nameof(ExceptionCases))]
    public async Task Invoke_WhenNextThrows_WritesJsonErrorWithMappedStatusCode(Exception exception, HttpStatusCode expected)
    {
        var middleware = new CustomExceptionMiddleware(_ => throw exception, NullLogger<CustomExceptionMiddleware>.Instance);
        var context = CreateContext();

        await middleware.Invoke(context);

        var body = await ReadBodyAsync(context);
        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)expected));
            Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
            Assert.That(body["StatusCode"]!.Value<int>(), Is.EqualTo((int)expected));
            Assert.That(body["ErrorMessage"]!.Value<string>(), Is.EqualTo(exception.Message));
        });
    }

    [Test]
    public async Task Invoke_WhenNextSucceeds_LeavesResponseUntouched()
    {
        var middleware = new CustomExceptionMiddleware(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status201Created;
            return Task.CompletedTask;
        }, NullLogger<CustomExceptionMiddleware>.Instance);
        var context = CreateContext();

        await middleware.Invoke(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        Assert.That(context.Response.Body.Length, Is.EqualTo(0));
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<JObject> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return JObject.Parse(await reader.ReadToEndAsync());
    }
}
