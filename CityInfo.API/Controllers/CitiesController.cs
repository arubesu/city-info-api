using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        [HttpGet()]
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}", Name = "GetCity")]
        public IActionResult GetCity(int id)
        {
            var city = GetCityById(id);

            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost()]
        public IActionResult CreateCity(
            [FromBody] CityForCreateDto cityForCreate)
        {

            if (!ModelState.IsValid || cityForCreate == null)
            {
                return BadRequest(ModelState);
            }

            var cityautoId = CitiesDataStore.Current.Cities.Max(c => c.Id);

            var finalCity = new CityDto()
            {
                Id = ++cityautoId,
                Description = cityForCreate.Description,
                Name = cityForCreate.Name
            };

            CitiesDataStore.Current.Cities.Add(finalCity);

            return CreatedAtRoute("GetCity", new { finalCity.Id }, finalCity);
        }

        [HttpPut("{id}")]
        public IActionResult updateCity(int id,
            [FromBody] CityForUpdateDto cityForUpdate)
        {
            if (cityForUpdate == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = GetCityById(id);

            if (city == null)
            {
                return NotFound();
            }

            city.Name = cityForUpdate.Name;
            city.Description = cityForUpdate.Description;

            return NoContent();
        }


        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateCity(int id,
            [FromBody] JsonPatchDocument<CityForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var city = GetCityById(id);

            if (city == null)
            {
                return NotFound();
            }

            var cityToPatch = new CityForUpdateDto()
            {
                Description = city.Description,
                Name = city.Name
            };

            patchDocument.ApplyTo(cityToPatch, ModelState);

            // Check whether ModelState is valid in patchDocument 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //Check if after applyTo operation the ModelState in the city is valid
            TryValidateModel(cityToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            city.Name = cityToPatch.Name;
            city.Description = cityToPatch.Description;

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteCity(int id)
        {
            var city = GetCityById(id);

            if (city == null)
            {
                return NotFound();
            }

            CitiesDataStore.Current.Cities.Remove(city);

            return NoContent();
        }

        private CityDto GetCityById(int id)
        {
            return CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        }
    }
}
