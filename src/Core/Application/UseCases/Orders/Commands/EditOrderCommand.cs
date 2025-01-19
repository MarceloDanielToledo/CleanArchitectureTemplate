using Application.Constants;
using Application.Interfaces;
using Application.UseCases.OrderItems.Responses;
using Application.UseCases.OrderItems.Specifications;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.UseCases.Orders.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Orders.Commands
{
    public class EditOrderCommand(EditOrderRequest request) : IRequest<Response<OrderResponse>>
    {
        public EditOrderRequest Request { get; } = request;
    }
    internal class EditOrderCommandHandler(IRepositoryAsync<Order> orderRespositoryAsync, IMapper mapper) : IRequestHandler<EditOrderCommand, Response<OrderResponse>>
    {
        private readonly IRepositoryAsync<Order> _orderRespositoryAsync = orderRespositoryAsync;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<OrderResponse>> Handle(EditOrderCommand command, CancellationToken cancellationToken)
        {
            var record = await _orderRespositoryAsync.FirstOrDefaultAsync(new GetOrderByIdSpecification(command.Request.Id), cancellationToken) ?? throw new KeyNotFoundException(ResponseMessages.NotFoundMessage);
            record.Comment = command.Request.Comment;
            await _orderRespositoryAsync.UpdateAsync(record, cancellationToken);
            return Response<OrderResponse>.Success(_mapper.Map<OrderResponse>(record), ResponseMessages.UpdatedSuccessfullyMessage);
        }
    }
}
