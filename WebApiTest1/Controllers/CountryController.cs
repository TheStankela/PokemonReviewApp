﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using WebApiTest1.Dto;
using WebApiTest1.Interfaces;
using WebApiTest1.Models;

namespace WebApiTest1.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof (IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            return Ok(countries);
        }
        [HttpGet ("{id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetCountry(int id)
        {
            var country = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountry(id));
            return Ok(country);
        }
        [HttpGet("GetCountryOfAnOwner/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetCountryByOwnerID(int ownerId) 
        { 
            var country = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountryByOwnerID(ownerId));
            return Ok(country);
        }
        [HttpGet("GetOwnersFromCountry/{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwnersByCountry(int countryId)
        {
            var country = _countryRepository.GetOwnersByCountry(countryId);
            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest();

            var country = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists.");
                return StatusCode(422, ModelState);
            }

            var countryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return StatusCode(200, "Country created successfully.");
        }
        [HttpPut ("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry([FromBody] CountryDto updatedCountry, int countryId)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);
            if (updatedCountry.Id != countryId)
                return BadRequest();
            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Country does not exist.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(updatedCountry);

            if (!_countryRepository.UpdateCountry(countryMap))
                return StatusCode(500, "Error while updating the country");

            return StatusCode(200, "Successfully updated the country.");
        }

        [HttpDelete ("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult RemoveCountry(int countryId) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Country does not exist.");

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if (!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", "Error while deleting the country.");
                return StatusCode(500, ModelState);
            }

            return StatusCode(200, "Successfully deleted the country.");
        }
    }
}
