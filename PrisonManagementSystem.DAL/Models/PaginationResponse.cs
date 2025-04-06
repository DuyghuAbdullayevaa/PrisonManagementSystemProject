using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Models
{
    public class PaginationResponse<T>
    {
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public IEnumerable<T> Data { get; init; }

        public PaginationResponse(int totalCount, IEnumerable<T> data, int pageNumber, int pageSize)
        {
            TotalCount = totalCount;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
