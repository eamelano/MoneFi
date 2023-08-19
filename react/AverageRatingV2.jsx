import React from "react";
import PropTypes from "prop-types";
import { Rating } from "react-simple-star-rating";

const AverageRatingV2 = ({ cardRating, user, handleRating }) => {
  return (
    <div className="d-flex align-items-center">
      {cardRating !== null && (
        <>
          <Rating
            readonly={!user.isLoggedIn && true}
            disableFillHover={!user.isLoggedIn && true}
            allowFraction={true}
            initialValue={parseFloat(cardRating)}
            size={24}
            transition
            fillColor="#007bff"
            emptyColor="#ced4da"
            className="starRating"
            onClick={handleRating}
          />
          <span>{cardRating}/5</span>
        </>
      )}
    </div>
  );
};

AverageRatingV2.propTypes = {
  cardRating: PropTypes.number.isRequired,
  user: PropTypes.shape({
    id: PropTypes.number,
    email: PropTypes.string,
    isLoggedIn: PropTypes.bool,
    roles: PropTypes.arrayOf(PropTypes.string),
  }).isRequired,
  handleRating: PropTypes.func.isRequired,
};

export default AverageRatingV2;
