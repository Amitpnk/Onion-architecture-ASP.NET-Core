using FluentValidation.Results;
using NUnit.Framework;
using OA.Service.Exceptions;

namespace OA.Test.Unit.Exceptions;

public class ExceptionTests
{
    [Test]
    public void ValidationException_GroupsFailuresByPropertyName()
    {
        var failures = new List<ValidationFailure>
        {
            new("Email", "Email is required."),
            new("Email", "Email is invalid."),
            new("Password", "Password is too short.")
        };

        var exception = new ValidationException(failures);

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("One or more validation failures have occurred."));
            Assert.That(exception.Failures.Keys, Is.EquivalentTo(new[] { "Email", "Password" }));
            Assert.That(exception.Failures["Email"], Is.EqualTo(new[] { "Email is required.", "Email is invalid." }));
            Assert.That(exception.Failures["Password"], Is.EqualTo(new[] { "Password is too short." }));
        });
    }

    [Test]
    public void ValidationException_WithoutFailures_HasEmptyDictionary()
    {
        var exception = new ValidationException();

        Assert.That(exception.Failures, Is.Empty);
    }

    [Test]
    public void NotFoundException_FormatsEntityNameAndKey()
    {
        var exception = new NotFoundException("Customer", 42);

        Assert.That(exception.Message, Is.EqualTo("Entity \"Customer\" (42) was not found."));
    }

    [Test]
    public void DeleteFailureException_FormatsEntityNameKeyAndReason()
    {
        var exception = new DeleteFailureException("Customer", 7, "It has open orders.");

        Assert.That(exception.Message, Is.EqualTo("Deletion of entity \"Customer\" (7) failed. It has open orders."));
    }

    [Test]
    public void ApiException_FormatsMessageArguments()
    {
        var exception = new ApiException("User {0} has {1} errors", "bob", 3);

        Assert.That(exception.Message, Is.EqualTo("User bob has 3 errors"));
    }
}
