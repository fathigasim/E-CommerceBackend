using EcommerceApplication.Interfaces;
using EcommerceInfrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceInfrastructure.Services
{
    public class OrderNumberService : IOrderNumberService
    {
        private readonly AppDbContext _context;

        public OrderNumberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var outputParam = new Microsoft.Data.SqlClient.SqlParameter
            {
                ParameterName = "@nextNumber",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            // Execute the command directly
            await _context.Database.ExecuteSqlRawAsync(
                "SET @nextNumber = NEXT VALUE FOR OrderNumbers",
                outputParam);

            var nextNumber = (int)outputParam.Value;
            var year = DateTime.UtcNow.Year;

            return $"ORD-{year}-{nextNumber:D3}";
        }
    }
}
