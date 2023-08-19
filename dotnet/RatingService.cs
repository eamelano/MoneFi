using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Services.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Sabio.Models.Domain.Ratings;
using Sabio.Data;
using Sabio.Models.Requests.Ratings;

namespace Sabio.Services
{
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

        public void Delete(int ratingId)
        {
            string procName = "[dbo].[Ratings_Delete_ById]";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", ratingId);
            }, null
            );
        }

        public int Create(RatingAddRequest ratingModel, int userId)
        {
            int ratingId = 0;
            string procName = "[dbo].[Ratings_Insert]";

            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Rating", ratingModel.RatingVal);
                paramCollection.AddWithValue("@EntityTypeId", ratingModel.EntityTypeId);
                paramCollection.AddWithValue("@EntityId", ratingModel.EntityId);
                paramCollection.AddWithValue("@CreatedBy", userId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                paramCollection.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out ratingId);
            }
            );
            return ratingId;
        }

        public void Update(RatingUpdateRequest existingRating, int ratingId,int UserId)
        {
            string procName = "[dbo].[Ratings_Update]";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", ratingId);         
                paramCollection.AddWithValue("@Rating", existingRating.RatingVal);
                paramCollection.AddWithValue("@CommentId", existingRating.CommentId);
                paramCollection.AddWithValue("@EntityTypeId", existingRating.EntityTypeId);
                paramCollection.AddWithValue("@EntityId", existingRating.EntityId);
                paramCollection.AddWithValue("@ModifiedBy", UserId);
                paramCollection.AddWithValue("@isDeleted", existingRating.IsDeleted);

            }, null
            );
        }

        public Rating GetById(int ratingId)
        {
            Rating thisRating = null;
            string procName = "[dbo].[Ratings_Select_ById]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", ratingId);

            }, delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                LookUpService mapEntity = new LookUpService(_dataProvider);
                thisRating = MapRating(reader, ref startIndex, mapEntity);
            }
            );
            return thisRating;
        }

        public double GetAverageRatings(int entityTypeId, int entityId)
        {
            double ratingAverage = 0;
            string procName = "[dbo].[Ratings_SelectSummary_ByEntityId]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@EntityTypeId", entityTypeId);
                paramCollection.AddWithValue("@EntityId", entityId);

            }, delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                ratingAverage = reader.GetSafeDouble(startIndex++);

            }
            );
            return ratingAverage;
        }

        public Paged<Rating> GetRatings(int pageIndex, int pageSize, int createdById)
        {
            Paged<Rating> pagedList = null;
            List<Rating> ratingList = null;
            int totalCount = 0;
            string procName = "[dbo].[Ratings_Select_ByCreatedBy]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@CreatedById", createdById);

            }, delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                LookUpService mapEntity = new LookUpService(_dataProvider);
                Rating thisRating = MapRating(reader, ref startIndex, mapEntity);
                totalCount = reader.GetSafeInt32(startIndex++);

                if (ratingList == null)
                {
                    ratingList = new List<Rating>();
                }
                ratingList.Add(thisRating);
            }
            );

            if (ratingList != null)
            {
                pagedList = new Paged<Rating>(ratingList, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<Rating> GetRatings(int pageIndex, int pageSize, int entityTypeId, int entityId)
        {
            Paged<Rating> pagedList = null;
            List<Rating> ratingList = null;
            int totalCount = 0;
            string procName = "[dbo].[Ratings_Select_ByEntityId]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@EntityTypeId", entityTypeId);
                paramCollection.AddWithValue("@EntityId", entityId);

            }, delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                LookUpService mapEntity = new LookUpService(_dataProvider);
                Rating thisRating = MapRating(reader, ref startIndex, mapEntity);
                totalCount = reader.GetSafeInt32(startIndex++);


                if (ratingList == null)
                {
                    ratingList = new List<Rating>();
                }
                ratingList.Add(thisRating);
            }
            );

            if (ratingList != null)
            {
                pagedList = new Paged<Rating>(ratingList, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<Rating> GetRatings(int pageIndex, int pageSize)
        {
            Paged<Rating> pagedList = null;
            List<Rating> ratingList = null;
            int totalCount = 0;
            string procName = "[dbo].[Ratings_SelectAll]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);

            }, delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                LookUpService mapEntity = new LookUpService(_dataProvider);
                Rating thisRating = MapRating(reader, ref startIndex, mapEntity);
                totalCount = reader.GetSafeInt32(startIndex++);

                if (ratingList == null)
                {
                    ratingList = new List<Rating>();
                }
                ratingList.Add(thisRating);
            }
            );

            if (ratingList != null)
            {
                pagedList = new Paged<Rating>(ratingList, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Rating MapRating(IDataReader reader, ref int startIndex, LookUpService mapEntity)
        {
            Rating thisRating = new Rating();

            thisRating.Id = reader.GetSafeInt32(startIndex++);
            thisRating.RatingVal = reader.GetSafeByte(startIndex++);
            thisRating.EntityId = reader.GetSafeInt32(startIndex++);
            thisRating.DateCreated = reader.GetSafeDateTime(startIndex++);
            thisRating.DateModified = reader.GetSafeDateTime(startIndex++);
            thisRating.IsDeleted = reader.GetSafeBool(startIndex++);
            thisRating.CreatedBy = _userService.MapBaseUser(reader, ref startIndex);
            thisRating.ModifiedBy = _userService.MapBaseUser(reader, ref startIndex);
            thisRating.Entity = mapEntity.MapSingleLookUp(reader, ref startIndex);
            thisRating.Comment = _commentsService.MapSingleComment(reader, ref startIndex);

            if(thisRating.Id == 0)
            {
                return null;
            }

            return thisRating;
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
}
