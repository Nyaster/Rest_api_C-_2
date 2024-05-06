using System.Data.SqlClient;
using Rest_api_2.Models;

namespace Rest_api_2.Repositories;

public class DBRepository : IDbRepository
{
    private string _connectionString =
        "Server=db-mssql;Database=Server=db-mssql ;Database=2019SBD;Trusted_Connection=True;Trusted_Connection=True;";

    public async Task<Warehouse> GetWarehouse(int id)
    {
        Warehouse animal = new Warehouse();
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand();
            await sqlConnection.OpenAsync();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText =
                "Select IdWarehouse, Name,Address from Warehouse where IdWarehouse=@IdWarehouse";
            sqlCommand.Parameters.AddWithValue("@IdWarehouse", id);
            using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    animal.IdWarehouse = (int)sqlDataReader["IdWarehouse"];
                    animal.Name = sqlDataReader["Name"].ToString();
                    animal.Address = sqlDataReader["Address"].ToString();
                }
            }
        }

        return animal;
    }

    public async Task<Order> GetOrder(int id, int amount)
    {
        Order animal = new Order();
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand();
            await sqlConnection.OpenAsync();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText =
                "Select IdOrder, IdProduct,Amount,IdOrder,CreatedAt,FulfilledAt from \"Order\" where IdOrder=@IdOrder and Amount=@Amount";
            sqlCommand.Parameters.AddWithValue("@IdOrder", id);
            sqlCommand.Parameters.AddWithValue("@Amount", amount);
            using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    animal.IdProduct = (int)sqlDataReader["IdProduct"];
                    animal.Amount = (int)sqlDataReader["Amount"];
                    animal.IdOrder = (int)sqlDataReader["IdOrder"];
                    animal.CreatedAt = (DateTime)sqlDataReader["CreatedAt"];
                    if (sqlDataReader["FulfilledAt"] as System.DBNull != null)
                    {
                        animal.FulfilledAt = null;
                    }
                    else
                    {
                        animal.FulfilledAt = (DateTime)sqlDataReader["FulfilledAt"];
                    }
                }
            }
        }

        return animal;
    }

    public async Task<Product_Warehouse> GetProductWarehouse(int id)
    {
        Product_Warehouse animal = new Product_Warehouse();
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand();
            await sqlConnection.OpenAsync();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText =
                "Select IdProductWarehouse, IdWarehouse,IdProduct,IdOrder,Amount,Price,CreatedAt from Product_Warehouse where IdOrder=@IdProductWarehouse";
            sqlCommand.Parameters.AddWithValue("@IdProductWarehouse", id);
            using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    animal.IdProduct = (int)sqlDataReader["IdProduct"];
                    animal.Amount = (int)sqlDataReader["Amount"];
                    animal.IdOrder = (int)sqlDataReader["IdOrder"];
                    animal.CreatedAt = (DateTime)sqlDataReader["CreatedAt"];
                    animal.IdProductWarehouse = (int)sqlDataReader["IdProductWarehouse"];
                    animal.IdWarehouse = (int)sqlDataReader["IdWarehouse"];
                    animal.Price = (decimal)sqlDataReader["Price"];
                    animal.Amount = (int)sqlDataReader["Amount"];
                }
            }
        }

        return animal;
    }

    public async Task<Product> GetProduct(int id)
    {
        Product animal = new Product();
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand();
            await sqlConnection.OpenAsync();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText =
                "Select IdProduct, Name,Description,Price from Product where IdProduct=@IdProduct";
            sqlCommand.Parameters.AddWithValue("@IdProduct", id);
            using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    animal.IdProduct = (int)sqlDataReader["IdProduct"];
                    animal.Name = sqlDataReader["Name"].ToString();
                    animal.Description = sqlDataReader["Description"].ToString();
                    animal.Price = (decimal)sqlDataReader["Price"];
                }
            }
        }

        return animal;
    }

    public async Task<int> InsertProductWarehouse(int productIdProduct, int productIdWarehouse, int orderIdOrder,
        int productAmount,
        decimal? price, DateTime now)
    {
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();
            using (var transaction = sqlConnection.BeginTransaction())
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.Transaction = transaction;
                sqlCommand.CommandText =
                    "update\"Order\" set FulfilledAt=@FulfilledAt where IdOrder=@IdOrder";
                sqlCommand.Parameters.AddWithValue("@IdOrder", orderIdOrder);
                sqlCommand.Parameters.AddWithValue("@FulfilledAt", now);
                try
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Something happened here");
                }

                sqlCommand.Parameters.Clear();
                try
                {
                    sqlCommand.CommandText =
                        "INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt) VALUES (@IdProduct,@IdWarehouse, @IdOrder, @Amount, @Price, @CreatedAt)";
                    sqlCommand.Parameters.AddWithValue("@IdProduct", productIdProduct);
                    sqlCommand.Parameters.AddWithValue("@IdWarehouse", productIdWarehouse);
                    sqlCommand.Parameters.AddWithValue("@IdOrder", orderIdOrder);
                    sqlCommand.Parameters.AddWithValue("@Amount", productAmount);
                    sqlCommand.Parameters.AddWithValue("@Price", price);
                    sqlCommand.Parameters.AddWithValue("@CreatedAt", now);
                    int returnedId = await sqlCommand.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                    return returnedId;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    return -1;
                }
            }
        }
    }

    public async Task<int> GetProductWarehouse(int productIdProduct, int productIdWarehouse, int productAmount, decimal? price,
        DateTime dateTime)
    {
        int value = 0;
        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand();
            await sqlConnection.OpenAsync();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText =
                "Select IdProductWarehouse from Product_Warehouse where IdProduct=@IdProduct and IdWarehouse=@IdWarehouse and Price =@price and CreatedAt=@dateTime";
            sqlCommand.Parameters.AddWithValue("@IdProduct", productIdProduct);
            sqlCommand.Parameters.AddWithValue("@IdWarehouse", productIdWarehouse);
            sqlCommand.Parameters.AddWithValue("@productAmount", productAmount);
            sqlCommand.Parameters.AddWithValue("@price", price);
            sqlCommand.Parameters.AddWithValue("@dateTime", dateTime);
            using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    value = (int)sqlDataReader["IdProductWarehouse"];
                }
            }
        }
        return value;
    }
}