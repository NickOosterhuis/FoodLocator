using DataModelingService.Models;
using DTO;
using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.Photos.Request;
using GoogleApi.Entities.Places.Search.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataModelingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleTestController : ControllerBase
    {
        public string GoogleApiKey { get; }
        public List<Restaurant> Restaurants { get; }

        public GoogleTestController(IConfiguration configuration)
        {
            GoogleApiKey = configuration["AppSettings:Google:ApiKey"];
            Restaurants = new List<Restaurant>();
        }

        [HttpPost]
        public async Task<IActionResult> GetRestaurantsByParameters(QueryParameters parameters)
        {
            var response = await GooglePlaces.NearBySearch.QueryAsync(new PlacesNearBySearchRequest
            {
                Key = GoogleApiKey,
                Location = new GoogleApi.Entities.Places.Search.NearBy.Request.Location(parameters.Location.Latitude, parameters.Location.Longitude),
                Radius = parameters.Radius,
                Type = SearchPlaceType.Restaurant, 
                PageToken = parameters.PageToken
            });

            if (!response.Status.HasValue || response.Status.Value != Status.Ok || !response.Results.Any())
                return new BadRequestResult();                     

            var restaurants = await AddRestaurants(response.Results);

            var nextPage = response.NextPageToken;

            if (nextPage != null)
            {
                parameters.PageToken = nextPage;
                await GetRestaurantsByParameters(parameters);
            }

            return new JsonResult(restaurants);          
        }

        private async Task<List<Restaurant>> AddRestaurants(IEnumerable<NearByResult> results)
        {          
            foreach (var item in results)
            {
                if (item.PermanentlyClosed)
                    continue;

                var restaurant = new Restaurant
                {
                    PlaceId = item.PlaceId,
                    PhotoReference = item.Photos.FirstOrDefault().PhotoReference,
                    Name = item.Name,
                    OpeningHours = item.OpeningHours,                    
                    Rating = item.Rating,
                    Adress = item.Vicinity,
                    Latitude = item.Geometry.Location.Latitude,
                    Longitude = item.Geometry.Location.Longitude,
                };

                var details = await AddPlaceDetails(restaurant.PlaceId);
                restaurant.PhoneNumber = details.FormattedPhoneNumber;
                restaurant.Website = details.Website;
                //restaurant.OpenNow = details.OpeningHours.OpenNow;
                //restaurant.OpeningPeriods = details.OpeningHours.Periods;
                
                var photo = await AddPlacePhoto(restaurant.PhotoReference);
                restaurant.PhotoReference = photo; 


                Restaurants.Add(restaurant);
            }

            return Restaurants;
        }

        private async Task<string> AddPlacePhoto(string photoRefference)
        {
            var response = await GooglePlaces.Photos.QueryAsync(new PlacesPhotosRequest
            {
                Key = GoogleApiKey,
                PhotoReference = photoRefference,
                MaxWidth = 400
            });

            if (!response.Status.HasValue || response.Status.Value != Status.Ok)
                return null;

            return Convert.ToBase64String(response.Buffer);
        }

        private async Task<DetailsResult> AddPlaceDetails(string placeId)
        {
            var response = await GooglePlaces.Details.QueryAsync(new PlacesDetailsRequest
            {
                Key = GoogleApiKey,
                PlaceId = placeId,
                
            });

            if (!response.Status.HasValue || response.Status.Value != Status.Ok)
                return null;            

            return response.Result;

        }
    }
}
