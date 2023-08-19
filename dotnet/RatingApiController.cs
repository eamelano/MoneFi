using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Ratings;
using Sabio.Models.Requests.Ratings;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingApiController : BaseApiController
    {

        private IRatingService _service = null;
        private IAuthenticationService<int> _authService = null;

        public RatingApiController(ILogger<UserApiController> logger
            , IRatingService service
            , IAuthenticationService<int> authService) : base(logger)
        {
            _authService = authService;
            _service = service;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(RatingAddRequest model)
        {
            ObjectResult result = null;
            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                int id = _service.Create(model, user.Id);
                ItemResponse<int> response = new ItemResponse<int>();
                response.Item = id;

                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPut("{ratingId:int}")]
        public ActionResult<SuccessResponse> Update(RatingUpdateRequest model, int ratingId) 
        {
            int iCode = 200;
            BaseResponse response = null;

            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                _service.Update(model, ratingId, user.Id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}.");
            }
            return StatusCode(iCode, response);
        }

        [HttpDelete("{ratingId:int}")] 
        public ActionResult<SuccessResponse> Delete(int ratingId)
        {
            int iCode = 200;
            BaseResponse response = null;



            try
            {
                _service.Delete(ratingId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}.");
            }
            return StatusCode(iCode, response);
        }

        [HttpGet]
        public ActionResult<ItemResponse<Paged<Rating>>> GetPaginated(int pageIndex, int pageSize)
        {
            ActionResult result = null;

            try
            {

                Paged<Rating> list = _service.GetRatings(pageIndex, pageSize);
                
                if (list == null)
                {
                    ErrorResponse errorResponse = new ErrorResponse("Records Not Found");
                    result = NotFound404(errorResponse);
                }
                else
                {
                    ItemResponse<Paged<Rating>> response = new ItemResponse<Paged<Rating>>();
                    response.Item = list;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse($"Generic Error: {ex.Message}."));
            }
            return result;
        }

        [HttpGet("createdby/{createdBy:int}")]
        public ActionResult<ItemResponse<Paged<Rating>>> GetByCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            ActionResult result = null;

            try
            {

                Paged<Rating> list = _service.GetRatings(pageIndex, pageSize, createdBy);

                if (list == null)
                {
                    ErrorResponse errorResponse = new ErrorResponse("Records Not Found");
                    result = NotFound404(errorResponse);
                }
                else
                {
                    ItemResponse<Paged<Rating>> response = new ItemResponse<Paged<Rating>>();
                    response.Item = list;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse($"Generic Error: {ex.Message}."));
            }
            return result;
        }

        [HttpGet("entities/{entityTypeId:int}/{entityId:int}")]
        public ActionResult<ItemResponse<Paged<Rating>>> GetByEntity(int pageIndex, int pageSize, int entityTypeId, int entityId)
        {
            ActionResult result = null;

            try
            {

                Paged<Rating> list = _service.GetRatings(pageIndex, pageSize, entityTypeId, entityId);

                if (list == null)
                {
                    ErrorResponse errorResponse = new ErrorResponse("Records Not Found");
                    result = NotFound404(errorResponse);
                }
                else
                {
                    ItemResponse<Paged<Rating>> response = new ItemResponse<Paged<Rating>>();
                    response.Item = list;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse($"Generic Error: {ex.Message}."));
            }
            return result;
        }

        [HttpGet("average/{entityTypeId:int}/{entityId:int}")]
        public ActionResult<ItemResponse<double>> GetAverageByEntity(int entityTypeId, int entityId)
        {
            ActionResult result = null;

            try
            {

                double average = _service.GetAverageRatings(entityTypeId, entityId);

                if (average == 0)
                {
                    ErrorResponse errorResponse = new ErrorResponse("Records Not Found");
                    result = NotFound404(errorResponse);
                }
                else
                {
                    ItemResponse<double> response = new ItemResponse<double>();
                    response.Item = average;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse($"Generic Error: {ex.Message}."));
            }
            return result;
        }

        [HttpGet("{ratingId:int}")]
        public ActionResult<ItemResponse<Rating>> GetById(int ratingId)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Rating aRating = _service.GetById(ratingId); ;

                if (aRating == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Rating not found!");
                }
                else
                {
                    response = new ItemResponse<Rating> { Item = aRating };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}.");
            }
            return StatusCode(iCode, response);
        }

        [HttpPut("merge")]
        public ActionResult<SuccessResponse> MergeRating(RatingMergeRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                _service.MergeRating(model, user.Id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}.");
            }
            return StatusCode(code, response);
        }

    }
}
