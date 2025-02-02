﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiTest1.Dto;
using WebApiTest1.Interfaces;
using WebApiTest1.Models;

namespace WebApiTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IReviewerRepository _reviewerRepository;

        public ReviewerController(IMapper mapper, IReviewerRepository reviewerRepository)
        {
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers().ToList());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewers);
        }

        [HttpGet ("{id}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        public IActionResult GetReviewer(int id)
        {
            if(!_reviewerRepository.ReviewerExists(id))
                return NotFound();

            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewer);
        }
        [HttpGet ("GetReviewsOfReviewer/{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviewsOfReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsOfReviewer(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            if (reviewerCreate == null)
                return BadRequest();

            var reviewer = _reviewerRepository.GetReviewers()
                .Where(r => r.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper()).FirstOrDefault();

            if(reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists.");
                return StatusCode(422, ModelState);
            }

            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);
            
            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return StatusCode(200, "Reviewer successfully created.");
        }

        [HttpPut ("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer([FromBody] ReviewerDto updatedReviewer, int reviewerId) 
        {
            if(updatedReviewer == null)
                return BadRequest(ModelState);
            if(updatedReviewer.Id != reviewerId)
                return BadRequest();
            if (_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound("Reviewer does not exist.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);

            if(!_reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Error while updating the reviewer.");
                return StatusCode(500, ModelState);
            }

            return StatusCode(200, "Successfully updated the reviewer.");
        }

        [HttpDelete ("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer (int reviewerId)
        {
            if (reviewerId == 0)
                return BadRequest(ModelState);
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound("Reviewer does not exist.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", "Error while deleting reviewer.");
                return StatusCode(500, ModelState);
            }

            return StatusCode(200, "Successfully deleted the reviewer.");
        }
    }
}
