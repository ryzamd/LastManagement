
using LastManagement.Application.Features.InventoryStocks.DTOs;

namespace LastManagement.Application.Features.InventoryStocks.Commands;

public class BatchAdjustStockCommand
{
    private readonly AdjustStockCommand _adjustCommand;

    public BatchAdjustStockCommand(AdjustStockCommand adjustCommand)
    {
        _adjustCommand = adjustCommand;
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
                    Status = "success",
                    MovementId = adjustResult.MovementId,
                    NewQuantity = adjustResult.NewQuantityGood
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new InventoryStockBatchItemResult
                {
                    StockId = operation.StockId,
                    Status = "error",
                    Error = new InventoryStockBatchError
                    {
                        Type = ex is InvalidOperationException
                            ? "http://localhost:5000/problems/insufficient-stock"
                            : "http://localhost:5000/problems/validation-error",
                        Title = ex is InvalidOperationException ? "Insufficient Stock" : "Validation Error",
                        Status = 400,
                        Detail = ex.Message
                    }
                });
            }
        }

        return result;
    }
}