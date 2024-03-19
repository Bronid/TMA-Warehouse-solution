using TMA_Warehouse_solution.Models.Item;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Order
{
    public enum OrderStatus
    {
        NEW,
        APPROVE,
        REJECT,
    }

}
