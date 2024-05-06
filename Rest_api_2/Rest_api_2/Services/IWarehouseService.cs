using Rest_api_2.Models;

namespace Rest_api_2.Services;

public interface IWarehouseService
{
    public Task<int> AddProduct(ProductDTO product);
}