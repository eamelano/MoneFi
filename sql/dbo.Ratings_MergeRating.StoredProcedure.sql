﻿USE [MoneFi_V2]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: Edgar Melano
-- Create date: 16 AUG 2023
-- Description: Updates rating if exists, if not inserts
-- Code Reviewer: 
-- =============================================
CREATE PROC [dbo].[Ratings_MergeRating]
	@UserId int
	,@Rating decimal(2,1)
	,@EntityId int
	,@EntityTypeId int
	,@IsDeleted bit = NULL

AS

/*
	DECLARE 
		@UserId Int = 138
		,@Rating DECIMAL(2,1) = 4.0
		,@EntityId INT = 1
		,@EntityTypeId INT = 1
		,@IsDeleted BIT = 0

	EXECUTE
		dbo.Ratings_MergeRating
			@UserId
			,@Rating
			,@EntityId
			,@EntityTypeId
			,@IsDeleted

	SELECT 
		Rating
	FROM	
		Ratings 
	WHERE	
		CreatedBy = @UserId
*/

BEGIN

	IF EXISTS 
		(SELECT 
			1 
		FROM 
			Ratings 
		WHERE 
			CreatedBy = @UserId 
		AND 
			EntityId = @EntityId
		AND 
			EntityTypeId = @EntityTypeId)

	BEGIN

		DECLARE 
			@DateModified datetime2(7) = GETUTCDATE()

		UPDATE	
			Ratings
		SET		
			Rating = @Rating
			,RatingDate = @DateModified
			,IsDeleted = ISNULL(@IsDeleted, IsDeleted)
		WHERE	
			CreatedBy = @UserId
		AND		
			EntityId = @EntityId

	END
	ELSE
	BEGIN

		INSERT INTO 
			Ratings(Rating, EntityId, EntityTypeId, CreatedBy, IsDeleted)
		VALUES		
			(@Rating, @EntityId, @EntityTypeId, @UserId, 0)

	END

 END
GO
