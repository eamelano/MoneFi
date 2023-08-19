import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import locale from "rc-pagination/lib/locale/en_US";
import "./blogs.css";
import blogsService from "services/blogsService";
import lookUpService from "services/lookUpService";
import ratingsService from "services/ratingsService";
import toastr from "toastr";
import BlogsCard from "./BlogsCard";
import PropTypes from "prop-types";
import { FaSearch, FaTimesCircle, FaPlus } from "react-icons/fa";
import { Tooltip } from "react-tooltip";
import { Button, Col, FormControl, InputGroup, Row } from "react-bootstrap";

function BlogsMain(props) {
  //#region Variables | useState | useEffect

  const navigateToForm = useNavigate();
  const roles = props.currentUser.roles;

  const [blogsType, setBlogsType] = useState({
    blogTypes: [],
    blogTypesComponent: [],
    selectedBlogTypeId: "",
  });

  const [blogs, setBlogs] = useState({
    blogsResponse: [],
    blogsComponents: [],
    pageIndex: 0,
    pageSize: 8,
    totalCount: 0,
  });

  const [searchBar, setSearchBar] = useState({
    query: "",
    searchActive: false,
  });

  useEffect(() => {
    lookUpService
      .getTypes(["BlogTypes"])
      .then(blogsTypesGetAllSuccess)
      .catch(blogsTypesGetAllError);
  }, []);

  useEffect(() => {
    if (searchBar.searchActive === true) {
      blogsService
        .search(blogs.pageIndex, blogs.pageSize, searchBar.query)
        .then(onSearchBlogsSuccess)
        .catch(onSearchBlogsError);
    } else if (
      blogsType.selectedBlogTypeId &&
      blogsType.selectedBlogTypeId !== "all"
    ) {
      blogsService
        .getByBlogTypeId(
          blogs.pageIndex,
          blogs.pageSize,
          blogsType.selectedBlogTypeId
        )
        .then(blogsGetByTypeIdSuccess)
        .catch(blogsGetByTypeIdError);
    } else {
      blogsService
        .getAll(blogs.pageIndex, blogs.pageSize)
        .then(blogsGetAllSuccess)
        .catch(blogsGetAllError);
    }
  }, [blogs.pageIndex, searchBar.searchActive, blogsType.selectedBlogTypeId]);

  //#endregion

  //#region Functionality

  const handleReset = () => {
    setSearchBar((prevState) => {
      let ps = { ...prevState };
      ps.query = "";
      ps.searchActive = false;
      return ps;
    });

    setBlogsType((prevState) => {
      let ps = { ...prevState };
      ps.selectedBlogTypeId = "";
      return ps;
    });

    setBlogs((prevState) => {
      return {
        ...prevState,
        pageIndex: 0,
      };
    });
  };

  const blogsTypeOptionSelected = (e) => {
    const { value } = e.target;

    setBlogsType((prevState) => {
      return {
        ...prevState,
        selectedBlogTypeId: value,
      };
    });
    setBlogs((prevState) => {
      return {
        ...prevState,
        pageIndex: 0,
      };
    });
  };

  const searchValueChange = (e) => {
    const { value } = e.target;

    setSearchBar((prevState) => {
      let ps = { ...prevState };
      ps.query = value;
      return ps;
    });
  };

  const searchClicked = () => {
    performSearch();
  };

  const handleKeyPress = (e) => {
    if (e.key === "Enter") {
      performSearch();
    }
  };

  const performSearch = () => {
    if (searchBar.query) {
      setBlogs((prevState) => {
        let ps = { ...prevState };
        ps.pageIndex = 0;
        return ps;
      });

      setSearchBar((prevState) => {
        let ps = { ...prevState };
        ps.searchActive = true;
        return ps;
      });

      blogsService
        .search(blogs.pageIndex, blogs.pageSize, searchBar.query)
        .then(onSearchBlogsSuccess)
        .catch(onSearchBlogsError);
    }
  };

  const onPaginationClick = (pageNumber) => {
    setBlogs((prevState) => {
      const ps = { ...prevState };
      ps.pageIndex = pageNumber - 1;
      return ps;
    });
  };

  const mapABlog = (aBlog) => {
    return (
      <div
        className="col-lg-3 col-md-6 col-sm-12 mb-4"
        key={`blogId-${aBlog.id}_authorId-${aBlog.authorId}`}
      >
        <BlogsCard
          blog={aBlog}
          currentUser={props.currentUser}
          handleRating={submitRating}
        />
      </div>
    );
  };

  const mapBlogsTypesOptions = (blogType) => {
    return (
      <option key={blogType.id} value={blogType.id}>
        {blogType.name}
      </option>
    );
  };

  const noBlogsFound = () => (
    <div className="text-center">No blogs found...</div>
  );

  const addNewBlogClicked = () => {
    navigateToForm(`/blogs/new`);
  };

  const submitRating = (e, id, typeId) => {
    const payload = {
      rating: e,
      entityId: id,
      entityTypeid: typeId,
    };

    ratingsService.merge(payload).then(ratingSuccess).catch(ratingError);
  };

  //#endregion

  //#region Handlers

  const blogsGetAllSuccess = (response) => {
    setBlogs((prevState) => {
      let ps = { ...prevState };
      ps.blogsComponents = response.item.pagedItems.map(mapABlog);
      ps.blogsResponse = response.item.pagedItems;
      ps.totalCount = response.item.totalCount;
      ps.blogRating = response.item.pagedItems.rating;
      return ps;
    });
  };

  const blogsGetAllError = (err) => {
    toastr.error(err.response.data.errors[0], "Error");
  };

  const blogsTypesGetAllSuccess = (response) => {
    setBlogsType((prevState) => {
      let ps = { ...prevState };
      ps.blogTypes = response.item.blogTypes;
      ps.blogTypesComponent = response.item.blogTypes.map(mapBlogsTypesOptions);
      return ps;
    });
  };

  const blogsTypesGetAllError = (err) => {
    toastr.error(err.response.data.errors[0], "Error");
  };

  const blogsGetByTypeIdSuccess = (response) => {
    setBlogs((prevState) => {
      let ps = { ...prevState };
      ps.blogsResponse = response.item.pagedItems;
      ps.blogsComponents = response.item.pagedItems.map(mapABlog);
      ps.totalCount = response.item.totalCount;
      return ps;
    });
  };

  const blogsGetByTypeIdError = () => {
    setBlogs((prevState) => {
      let ps = { ...prevState };
      ps.blogsComponents = "";
      ps.totalCount = 0;
      return ps;
    });
    noBlogsFound();
  };

  const onSearchBlogsSuccess = (response) => {
    setBlogs((prevState) => {
      const ps = { ...prevState };
      ps.blogsComponents = response.item.pagedItems.map(mapABlog);
      ps.blogsResponse = response.item.pagedItems;
      ps.totalCount = response.item.totalCount;
      return ps;
    });
  };

  const onSearchBlogsError = () => {
    setBlogs((prevState) => {
      let ps = { ...prevState };
      ps.blogsComponents = "";
      return ps;
    });

    noBlogsFound();
  };

  const ratingSuccess = () => {
    toastr.success("Rating updated");
  };

  const ratingError = () => {
    toastr.error("Unable to update rating");
  };

  //#endregion

  return (
    <>
      <div className="blogs-main-container">
        <div className="container mw-100">
          <Row className="mb-3 align-items-start">
            <Col>
              <Pagination
                total={blogs.totalCount}
                current={blogs.pageIndex + 1}
                pageSize={blogs.pageSize}
                onChange={onPaginationClick}
                locale={locale}
              />
            </Col>

            <Col className="blogs-search-bar-container">
              <InputGroup>
                <FormControl
                  type="text"
                  placeholder="Search..."
                  value={searchBar.query}
                  onChange={searchValueChange}
                  onKeyDown={handleKeyPress}
                />
                <Button
                  className="btn blogs-button mx-2"
                  onClick={searchClicked}
                  data-tooltip-id="search-tooltip"
                  data-tooltip-content="Search"
                  data-tooltip-place="top"
                >
                  <FaSearch />
                </Button>
              </InputGroup>
              <Tooltip id="search-tooltip" />

              <Button
                className="btn blogs-button"
                onClick={handleReset}
                data-tooltip-id="reset-tooltip"
                data-tooltip-content="Reset"
                data-tooltip-place="top"
              >
                <FaTimesCircle />
              </Button>
              <Tooltip id="reset-tooltip" />

              {(roles.includes("Admin") || roles.includes("Merchant")) && (
                <Button
                  className="blogs-button-add mx-2"
                  onClick={addNewBlogClicked}
                  data-tooltip-id="add-blog-tooltip"
                  data-tooltip-content="Add blog"
                  data-tooltip-place="top"
                >
                  <FaPlus />
                </Button>
              )}
              <Tooltip id="add-blog-tooltip" />
            </Col>

            <Col className="blogs-filter-container">
              <label htmlFor="category-select" className="mx-2">
                Filter by Topic:
              </label>
              <select
                id="blogType"
                className="form-select text-bg-white p-2 width-65"
                onChange={blogsTypeOptionSelected}
              >
                <option value="all">All</option>
                {blogsType.blogTypesComponent}
              </select>
            </Col>
          </Row>

          <Row>
            {blogs.blogsComponents === ""
              ? noBlogsFound()
              : blogs.blogsComponents}
          </Row>
        </div>
      </div>
    </>
  );
}

BlogsMain.propTypes = {
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    email: PropTypes.string.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string),
  }).isRequired,
};
export default BlogsMain;
