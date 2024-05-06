using Rest_api_2.Models;

namespace Rest_api_2.Repositories;

public interface IDbRepository
{
    public Task<Warehouse> GetWarehouse(int id);
    public Task<Order> GetOrder(int id,int amount);
    public Task<Product_Warehouse> GetProductWarehouse(int id);
    public Task<Product> GetProduct(int id);
  
    Task<int> InsertProductWarehouse(int productIdProduct, int productIdWarehouse, int orderIdOrder, int productAmount, decimal? price, DateTime now);
    Task<int> GetProductWarehouse(int productIdProduct, int productIdWarehouse, int productAmount, decimal? price,
 DateTime dateTime);

    public Task<int> AddProductWerehouseWithProcedure(ProductDTO product);
}