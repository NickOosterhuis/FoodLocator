using DataModelingService.Models;
using DTO;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataModelingService.Services
{
    public interface IGooglePlacesService
    {
        [Get("/place/nearbysearch/json")]
        Task<List<Restaurant>> GetRestaurants(QueryParameters parameters);
    }
}
