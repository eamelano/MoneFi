import React from "react";
import { useNavigate } from "react-router-dom";
import { Card, Row, Col, Image } from "react-bootstrap";
import PropTypes from "prop-types";
import { formatDate } from "../../utils/dateFormater";
import noimage from "./noimage.jpeg";
import { FaRegEdit } from "react-icons/fa";
import AverageRatingV2 from "components/ratings/AverageRatingV2";

function BlogsCard(props) {
  const { blog, currentUser, handleRating } = props;

  const navigate = useNavigate();
  const onEditClicked = (e) => {
    e.preventDefault();
    const blogCard = { type: "BLOG_CARD_OBJ", payload: blog };
    navigate(`/blogs/${blog.id}/edit`, { state: blogCard });
  };

  const onImageClicked = () => {
    navigate(`/blogs/${blog.id}`, { state: props.blog });
  };

  const handleRatingLocal = (e) => {
    handleRating(e, blog.id, blog.blogType.id);
  };

  return (
    <>
      <Card className="shadow-lg h-100 blogs-card-hover">
        <div>
          <Card.Img
            onClick={onImageClicked}
            variant="top"
            src={blog.imageUrl}
            className="rounded-top-md img-fluid blogs-card-image cursor-pointer"
            alt="no img"
            onError={({ currentTarget }) => {
              currentTarget.onerror = null;
              currentTarget.src = noimage;
            }}
          />
        </div>
        <Card.Body className="p-3">
          <h3 className="blogs-card-title-cont custom-card-title text-inherit">
            {blog.title}
          </h3>
          <p className="mb-3">{blog.subject}</p>

          <Row className="align-items-center g-0">
            <Col xs="auto">
              <Image
                src={blog.author.avatarUrl}
                alt=""
                className="rounded-circle blogs-avatar-sm me-2"
                onError={(e) => {
                  e.target.onerror = null;
                  e.target.src = { noimage };
                }}
              />
            </Col>
            <Col>
              <h5 className="mb-0">
                {blog.author.firstName} {blog.author.lastName}
              </h5>
              <p className="fs-6 mb-0">{formatDate(blog.dateModified)}</p>
            </Col>
          </Row>
          <Row>
            <Col>
              <AverageRatingV2
                cardRating={blog.rating}
                user={currentUser}
                handleRating={handleRatingLocal}
                entityId={blog.id}
                entityTypeId={blog.blogType.id}
              />
            </Col>
            <Col>
              {blog.authorId === currentUser?.id && (
                <FaRegEdit size={20} color="black" onClick={onEditClicked} />
              )}
            </Col>
          </Row>
        </Card.Body>
      </Card>
    </>
  );
}

BlogsCard.propTypes = {
  currentUser: PropTypes.shape({
    id: PropTypes.number,
    email: PropTypes.string,
    isLoggedIn: PropTypes.bool,
    roles: PropTypes.arrayOf(PropTypes.string),
  }),
  blog: PropTypes.shape({
    id: PropTypes.number.isRequired,
    title: PropTypes.string.isRequired,
    subject: PropTypes.string.isRequired,
    content: PropTypes.string.isRequired,
    imageUrl: PropTypes.string.isRequired,
    blogType: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
    }),
    author: PropTypes.shape({
      id: PropTypes.number.isRequired,
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
      mi: PropTypes.string,
      avatarUrl: PropTypes.string.isRequired,
    }).isRequired,
    authorId: PropTypes.number.isRequired,
    isPublished: PropTypes.bool.isRequired,
    dateCreated: PropTypes.string.isRequired,
    dateModified: PropTypes.string.isRequired,
    datePublish: PropTypes.string.isRequired,
    isDeleted: PropTypes.bool.isRequired,
    rating: PropTypes.number.isRequired,
  }).isRequired,
  handleRating: PropTypes.func.isRequired,
};
export default React.memo(BlogsCard);
