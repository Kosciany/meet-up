﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Application.Meals.Models;
using WebApplication.Application.Movies.Models;
using WebApplication.Application.Recomendations.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [EnableCors("VueCorsPolicy")]
    //[Authorize]
    [Route("api/meet-up/recomendation")]
    public class RecomendationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecomendationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("movie")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<MovieDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMovieRecomendationsAsync([FromQuery] GetMovieRecomendationQuery getMovieRecomendationQuery)
        {
            return Ok(await _mediator.Send(getMovieRecomendationQuery));
        }

        [HttpGet]
        [Route("meal")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MealDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMealRecomendationAsync([FromQuery] GetMealRecomendationQuery getMealRecomendationQuery)
        {
            return Ok(await _mediator.Send(getMealRecomendationQuery));
        }

    }
}