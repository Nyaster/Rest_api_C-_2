using Microsoft.AspNetCore.Http.HttpResults;
using Rest_api_2.Models;
using Rest_api_2.Repositories;

namespace Rest_api_2.Services;

public class WarehouseService : IWarehouseService
{
    private IDbRepository _orderRepository;

    public WarehouseService(IDbRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<int> AddProduct(ProductDTO product)
    {
        var productFromDB = await _orderRepository.GetProduct(product.IdProduct);
        if (productFromDB == null || !productFromDB.IdProduct.HasValue)
        {
            throw new Exception("Product not found.");
        }

        var warehouse = await _orderRepository.GetWarehouse(product.IdWarehouse);
        if (warehouse == null || !warehouse.IdWarehouse.HasValue)
        {
            throw new Exception("Warehouse not found.");
        }

        if (product.Amount <= 0)
        {
            throw new Exception("Amount should be greater than 0.");
        }

        var order = await _orderRepository.GetOrder(product.IdProduct, product.Amount);
        if (order == null || !order.IdOrder.HasValue || order.CreatedAt > product.CreatedAt)
        {
            throw new Exception("Order not found or created after current time.");
        }

        var productWarehouse = await _orderRepository.GetProductWarehouse(product.IdProduct);
        if (productWarehouse != null && productWarehouse.IdProduct.HasValue)
        {
            throw new Exception("Product already exists in the warehouse.");
        }
        decimal? price = productFromDB.Price * product.Amount;

        var dateTime = DateTime.Now;
        await _orderRepository.InsertProductWarehouse(product.IdProduct, product.IdWarehouse, order.IdOrder.Value, product.Amount, price, dateTime);
        var insertedId = await _orderRepository.GetProductWarehouse(product.IdProduct, product.IdWarehouse, product.Amount, price, dateTime);
        return insertedId;
    }

    public async Task<int> AddProductWithProcedure(ProductDTO product)
    {
        return await _orderRepository.AddProductWerehouseWithProcedure(product);
    }
}