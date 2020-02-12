import React, { Component, Fragment } from 'react';
import { Link } from 'react-router-dom';
import {
  Button, Container, Jumbotron, Col, Row, Card, CardBody, CardFooter, CardHeader, Spinner
} from 'reactstrap';
import Moment from "react-moment";
import authService from './api-authorization/AuthorizeService';
import { CreateDrawingForm } from './drawing/CreateDrawingForm';
import { DeleteDrawingModal } from "./drawing/DeleteDrawingModal";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);

    this.state = {
      actions: <div></div>,
      createModalOpen: false,
      ownedDrawings: [],
      sharedDrawings: [],
      authenticated: false,
      loadingOwned: true,
      loadingShared: true,
      deletedId: "",
      deletedTitle: "",
      deleteModalOpen: false
    }
  }

  componentDidMount = () => {
    document.title = "OpenLD";
    this.renderActions();
    this.fetchDrawings();
  }

  render = () => {
    let drawings;
    if (this.state.authenticated === true) {
      if (this.state.loadingOwned === true || this.state.loadingShared === true) {
        drawings = (
          <Container className="text-center">
            <Spinner style={{width: "5rem", height: "5rem"}}/>
          </Container>
        );
      } else {
        drawings = (
          <Container>
            <Row>
              <Col xs="12" md="6">
                <Row><h2>Your Drawings</h2></Row>
                {this.state.ownedDrawings.map(drawing => {
                  return (
                    <Card className="mb-3">
                      <Link to={`/drawing/${drawing.id}`} className="text-dark" key={drawing.id}>
                        <CardHeader><h4 className="mb-0">{drawing.title}</h4></CardHeader>
                        <CardBody>
                          <strong>Owner:</strong> {drawing.owner.userName}
                        </CardBody>
                      </Link>
                        <CardFooter className="d-flex small justify-content-between align-items-center">
                          <Button close size="sm" onClick={() => this.handleDelete(drawing.id, drawing.title)}><FontAwesomeIcon icon="trash"/></Button>
                          <em>Last modified <Moment date={drawing.lastModified} fromNow/></em>
                        </CardFooter>
                    </Card>
                  );
                })}
              </Col>
              <Col xs="12" md="6">
                <Row><h2>Drawings Shared With You</h2></Row>
                {this.state.sharedDrawings.map(drawing => {
                  return (
                    <Link to={`/drawing/${drawing.id}`} className="text-dark" key={drawing.id}>
                      <Card className="mb-3">
                        <CardHeader><h4 className="mb-0">{drawing.title}</h4></CardHeader>
                        <CardBody>
                          <strong>Owner:</strong> {drawing.owner.userName}
                        </CardBody>
                        <CardFooter className="text-right small">
                          <em>Last modified <Moment date={drawing.lastModified} fromNow/></em>
                        </CardFooter>
                      </Card>
                    </Link>
                  );
                })}
              </Col>
            </Row>
          </Container>
        );
      }
    }

    return (
      <Fragment>
        <Jumbotron>
          <Container>
            <h1 className="display-3">OpenLD</h1>
            <p>OpenLD is a free online tool for creating lighting designs collaboratively.</p>
          </Container>
          {this.state.actions}
        </Jumbotron>

        <CreateDrawingForm
          isOpen = {this.state.createModalOpen}
          onSubmitSuccess = {this.redirect}
          toggle = {this.toggleCreateModal}
        />

        <DeleteDrawingModal
          isOpen = {this.state.deleteModalOpen}
          toggle = {this.toggleDeleteModal}
          deleteDrawing = {this.deleteDrawing}
          drawingId = {this.state.deletedId}
          drawingTitle = {this.state.deletedTitle}
        />

        {drawings}
      </Fragment>
    );
  }

  toggleCreateModal = () => {
    this.setState({createModalOpen: !this.state.createModalOpen});
  }

  toggleDeleteModal = () => {
    this.setState({deleteModalOpen: !this.state.deleteModalOpen});
  }

  redirect = (id) => {
    this.props.history.push(`/drawing/${id}`);
  }

  renderActions = async () => {
    if (await authService.isAuthenticated()) {
      this.setState({actions: <Fragment><hr/><Button color="primary" size="lg" onClick={this.toggleCreateModal}>Create Drawing</Button></Fragment>});
    }
  }

  fetchDrawings = async () => {
    if (await authService.isAuthenticated()) {
      this.setState({authenticated: true});
      const ownedResponse = await fetch("api/drawing/GetOwnedDrawings", {
        headers: await authService.generateHeader()
      });

      if (ownedResponse.ok) {
        let data = await ownedResponse.json();

        if (data.success === true) {
          this.setState({
            ownedDrawings: data.data,
            loadingOwned: false
          });
        }
      }

      const sharedResponse = await fetch("api/drawing/GetSharedDrawings", {
        headers: await authService.generateHeader()
      });

      if (sharedResponse.ok) {
        let data = await sharedResponse.json();

        if (data.success === true) {
          this.setState({
            sharedDrawings: data.data,
            loadingShared: false
          });
        }
      }
    }
  }

  handleDelete = (id, title) => {
    this.setState({
      deletedId: id,
      deletedTitle: title,
      deleteModalOpen: true
    });
  }

  deleteDrawing = (id) => {
    this.setState(prevState => {
      let owned = [...prevState.ownedDrawings];
      const deletedIndex = owned.findIndex(drawing => drawing.id === id);

      owned.splice(deletedIndex, 1);

      return {
        ownedDrawings: owned
      };
    });
  }
}
