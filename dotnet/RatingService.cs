    public class RatingService : IRatingService
    {
        private IDataProvider _dataProvider;
        private ICommentsService _commentsService;
        private IUserService _userService;  

        public RatingService(IDataProvider dataProvider
            , ICommentsService commentsServic
            , IUserService userService)
        {
            _dataProvider = dataProvider;
            _commentsService = commentsServic;
            _userService = userService;
        }

        public void MergeRating(RatingMergeRequest model, int userId)
        {
            string procName = "[dbo].[Ratings_MergeRating]";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", userId);
                paramCollection.AddWithValue("@Rating", model.Rating);
                paramCollection.AddWithValue("@EntityId", model.EntityId);
                paramCollection.AddWithValue("@EntityTypeId", model.EntityTypeId);
                paramCollection.AddWithValue("@isDeleted", model.IsDeleted);

            }, null
            );
        }
    }
