using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for sales operations
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        request.UserId = GetCurrentUserId();
        var command = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Gets a list of sales based on filter criteria
    /// </summary>
    /// <param name="query">The query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales matching the criteria</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSales([FromQuery] GetSalesQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            sales => Ok(new ApiResponseWithData<IEnumerable<GetSaleResponse>>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = _mapper.Map<IEnumerable<GetSaleResponse>>(sales)
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Gets a sale by its unique identifier
    /// </summary>
    /// <param name="id">The sale's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSale(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSaleByIdQuery(id), cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            notFound => NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found"
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Updates a sale
    /// </summary>
    /// <param name="id">The sale's unique identifier</param>
    /// <param name="command">The update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Id mismatch"
            });

        command.UserId = GetCurrentUserId();
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            notFound => NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found"
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Cancels a sale
    /// </summary>
    /// <param name="id">The sale's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cancelled sale details</returns>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSale(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleCommand { Id = id }, cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale cancelled successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            notFound => NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found"
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Updates an item in a sale
    /// </summary>
    /// <param name="id">The sale's unique identifier</param>
    /// <param name="itemId">The item's unique identifier</param>
    /// <param name="command">The update item command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSaleItem(Guid id, Guid itemId, [FromBody] UpdateSaleItemCommand command, CancellationToken cancellationToken)
    {
        if (id != command.SaleId || itemId != command.ItemId)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Id mismatch"
            });

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Item updated successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            notFound => NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale or item not found"
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }

    /// <summary>
    /// Cancels an item in a sale
    /// </summary>
    /// <param name="id">The sale's unique identifier</param>
    /// <param name="itemId">The item's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPost("{id}/items/{itemId}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSaleItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleItemCommand { SaleId = id, ItemId = itemId }, cancellationToken);

        return result.Match<IActionResult>(
            sale => Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Item cancelled successfully",
                Data = _mapper.Map<GetSaleResponse>(sale)
            }),
            notFound => NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale or item not found"
            }),
            error => BadRequest(new ApiResponse
            {
                Success = false,
                Message = error.Message
            }));
    }
}