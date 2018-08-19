
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
    public class PointsOfInterestController : Controller
    {

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = GetPointOfInterest(city, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);

        }


        [HttpPost("{cityId}/pointsofinterest/")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreateDto pointOfInterest)
        {
            if (pointOfInterest == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            //Demo |  just used with in memory datastore to get the higher id from the point of interest list
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c =>
                    c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Description = pointOfInterest.Description,
                Name = pointOfInterest.Name
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                  new { cityId, finalPointOfInterest.Id },
                   finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!ModelState.IsValid || pointOfInterest == null)
            {
                return BadRequest(ModelState);
            }

            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = GetPointOfInterest(city, id);

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = GetPointOfInterest(city, id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Description = pointOfInterestFromStore.Description,
                    Name = pointOfInterestFromStore.Name
                };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            // Check whether ModelState is valid in patchDocument 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //Check if after applyTo operation the ModelState in the pointOfInterest is valid
            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = GetPointOfInterest(city, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterest);

            return NoContent();
        }

        /// <summary> Return the city from database by id </summary>
        private CityDto GetCity(int id)
        {
            return CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        }

        /// <summary> Return the city point of interest by id </summary>
        private PointOfInterestDto GetPointOfInterest(CityDto city, int id)
        {
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            return pointOfInterest;
        }

    }
}
