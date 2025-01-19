using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Queries;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Requests;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers.v1
{
    /// <summary>
    /// Order Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class OrderController : BaseApiController
    {
        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="request">Request body.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
            => CreatedAtAction(nameof(Create), await Mediator.Send(new CreateOrderCommand(request)));


        /// <summary>
        /// Edit order
        /// </summary>
        /// <param name="request">Request body.</param>
        /// <param name="id">Id</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditOrderRequest request)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the order ID.");
            }
            return Ok(await Mediator.Send(new EditOrderCommand(request)));
        }

        /// <summary>
        /// Delete order
        /// </summary>
        /// <param name="id">Id</param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteOrderCommand(id)));


        /// <summary>
        /// Get order by Id
        /// </summary>
        /// <param name="id">Id</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) 
            => Ok(await Mediator.Send(new GetOrderByIdQuery(id)));
    }
}
