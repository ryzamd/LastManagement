
using LastManagement.Api.Constants;
using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Utilities.Constants.Global;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Application.Features.InventoryStocks.Commands;

public class BatchAdjustStockCommand
{
    private readonly AdjustStockCommand _adjustCommand;
    private readonly IInventoryStockRepository _stockRepository;

    public BatchAdjustStockCommand(AdjustStockCommand adjustCommand, IInventoryStockRepository inventoryStockRepository)
    {
        _adjustCommand = adjustCommand;
        _stockRepository = inventoryStockRepository;
    }

    public async Task<InventoryStockBatchOperationResult> ExecuteAsync(BatchAdjustmentRequest request, string adminUser, CancellationToken cancellationToken = default)
    {
        var result = new InventoryStockBatchOperationResult
        {
            Successful = 0,
            Failed = 0,
            Results = new List<InventoryStockBatchItemResult>()
        };

        foreach (var operation in request.Operations)
        {
            try
            {
                var adjustResult = await _adjustCommand.ExecuteAsync(
                    operation.StockId,
                    new AdjustStockRequest
                    {
                        AdjustmentType = operation.AdjustmentType,
                        Quantity = operation.Quantity,
                        Reason = operation.Reason,
                        ReferenceNumber = request.ReferenceNumber
                    },
                    adminUser,
                    cancellationToken
                );

                result.Successful++;
                result.Results.Add(new InventoryStockBatchItemResult
                {
                    StockId = operation.StockId,
                    Status = StatusContants.SUCCESS,
                    MovementId = adjustResult.MovementId,
                    NewQuantity = adjustResult.NewQuantityGood
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                result.Failed++;
                result.Results.Add(new InventoryStockBatchItemResult
                {
                    StockId = operation.StockId,
                    Status = StatusContants.ERROR,
                    Error = new InventoryStockBatchError
                    {
                        Type = ProblemDetailsConstants.Types.PRECONDITION_FAILED,
                        Title = ProblemDetailsConstants.Titles.CONCURRENCY_CONFLICT,
                        Status = 412,
                        Detail = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new InventoryStockBatchItemResult
                {
                    StockId = operation.StockId,
                    Status = StatusContants.ERROR,
                    Error = new InventoryStockBatchError
                    {
                        Type = ex is InvalidOperationException ? ProblemDetailsConstants.Types.INSUFFICIENT_STOCK : ProblemDetailsConstants.Types.VALIDATION_ERROR,
                        Title = ex is InvalidOperationException ? ProblemDetailsConstants.Titles.INSUFFICIENT_STOCK : ProblemDetailsConstants.Titles.VALIDATION_ERROR,
                        Status = 400,
                        Detail = ex.Message
                    }
                });
            }
        }

        return result;
    }
}