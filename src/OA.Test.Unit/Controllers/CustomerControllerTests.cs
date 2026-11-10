using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using OA.Controllers;
using OA.Domain.Entities;
using OA.Service.Features.CustomerFeatures.Commands;
using OA.Service.Features.CustomerFeatures.Queries;

namespace OA.Test.Unit.Controllers;

public class CustomerControllerTests
{
    private Mock<IMediator> _mediator = null!;
    private CustomerController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = new Mock<IMediator>();
        var services = new ServiceCollection().AddSingleton(_mediator.Object).BuildServiceProvider();
        _controller = new CustomerController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = services }
            }
        };
    }

    [Test]
    public async Task Create_SendsCommand_AndReturnsOkWithId()
    {
        var command = new CreateCustomerCommand { CustomerName = "Acme" };
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(15);

        var result = await _controller.Create(command);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(15));
    }

    [Test]
    public async Task GetAll_ReturnsOkWithCustomers()
    {
        IEnumerable<Customer> customers = new List<Customer> { new() { Id = 1 }, new() { Id = 2 } };
        _mediator.Setup(m => m.Send(It.IsAny<GetAllCustomerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(customers);

        var result = await _controller.GetAll();

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(customers));
    }

    [Test]
    public async Task GetById_SendsQueryWithRouteId()
    {
        var customer = new Customer { Id = 8 };
        _mediator.Setup(m => m.Send(It.Is<GetCustomerByIdQuery>(q => q.Id == 8), It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        var result = await _controller.GetById(8);

        Assert.That(((OkObjectResult)result).Value, Is.SameAs(customer));
    }

    [Test]
    public async Task Delete_SendsCommandWithRouteId()
    {
        _mediator.Setup(m => m.Send(It.Is<DeleteCustomerByIdCommand>(c => c.Id == 3), It.IsAny<CancellationToken>())).ReturnsAsync(3);

        var result = await _controller.Delete(3);

        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(3));
    }

    [Test]
    public async Task Update_RouteIdMatchesBody_SendsCommand()
    {
        var command = new UpdateCustomerCommand { Id = 4, CustomerName = "Renamed" };
        _mediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(4);

        var result = await _controller.Update(4, command);

        Assert.That(((OkObjectResult)result).Value, Is.EqualTo(4));
    }

    [Test]
    public async Task Update_RouteIdDiffersFromBody_ReturnsBadRequest_WithoutCallingMediator()
    {
        var result = await _controller.Update(1, new UpdateCustomerCommand { Id = 2 });

        Assert.That(result, Is.TypeOf<BadRequestResult>());
        _mediator.VerifyNoOtherCalls();
    }
}
