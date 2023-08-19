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
