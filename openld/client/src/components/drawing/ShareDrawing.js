import React, { Component } from "react";
import {
  Alert,
  Container, Col, Row,
  Button, Form, FormGroup, Input, Label,
  ListGroup, ListGroupItem,
  Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import authService from "../api-authorization/AuthorizeService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export class ShareDrawing extends Component {
  constructor(props) {
    super(props)

    this.state = {
      userDrawings: [],
      loading: true,
      email: "",
      alertVisible: false,
      alertColour: "info",
      alertIcon: "exclamation",
      alertTitle: "",
      alertMsg: ""
    };
  }

  componentDidMount = () => {
    this.fetchSharedUsers()
  }

  render = () => {
    let users = <em className="text-center">No users</em>;
    if (this.state.userDrawings.length > 0) {
      users = this.state.userDrawings.map(ud => {
        return (
          <ListGroupItem key={`ud-${ud.user.id}`}>
            <div className="d-flex justify-content-between">
              <div>
                {ud.user.email}
              </div>
              <Button close onClick={() => this.handleRemoveUser(ud.id)}/>
            </div>
          </ListGroupItem>
        );
      })
    }

    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle}>
        <ModalHeader toggle={this.props.toggle}>Sharing Settings</ModalHeader>
        <Alert
          isOpen = {this.state.alertVisible}
          color = {this.state.alertColour}
        >
          <Container>
            <Row className="align-items-center">
              <Col xs="auto" className="p-0">
                <FontAwesomeIcon icon={this.state.alertIcon} />
              </Col>
              <Col>
                <div className="font-weight-bold">{this.state.alertTitle}</div>
                <div>{this.state.alertMsg}</div>
              </Col>
            </Row>
          </Container>
        </Alert>
        <ModalBody>
          This drawing is shared with:
          <ListGroup>
            {users}
          </ListGroup>
          <hr/>
          <h5>Share with user</h5>
          <Form onSubmit={this.handleAddUser}>
            <FormGroup row className="m-1 align-items-center">
              <Label for="email" className="m-0">Email</Label>
              <Col>
                <Input type="email" name="email" placeholder="Email" value={this.state.email} onChange={this.handleEmailChange}/>
              </Col>
              <Button xs="1" type="submit" color="primary">Share</Button>
            </FormGroup>
          </Form>
        </ModalBody>
        <ModalFooter>
          <Button color="secondary" onClick={this.props.toggle}>Close</Button>
        </ModalFooter>
      </Modal>
    );
  }

  handleAddUser = async (event) => {
    event.preventDefault();
    const response = await fetch("api/drawing/ShareWith", {
      headers: await authService.generateHeader({ 'Content-Type': 'application/json' }),
      method: "POST",
      body: JSON.stringify({
        user: {email: this.state.email},
        drawing: {id: this.props.drawingId}
      })
    });

    if (response.ok) {
      const data = await response.json();

      if (data.success) {
        this.setState({
          userDrawings: [...this.state.userDrawings, data.data],
          email: ""
        });
        this.showAlert("success", "check", "Success", "Drawing shared successfully");
      } else {
        this.showAlert("danger", "exclamation", "Error", data.msg);
      }
    } else {
      this.showAlert("danger", "exclamation", "Error", "Unknown error sharing with user");
    }
  }

  handleRemoveUser = async (id) => {
    const response = await fetch("api/drawing/UnshareWith", {
      headers: await authService.generateHeader({ 'Content-Type': 'application/json' }),
      method: "POST",
      body: JSON.stringify({
        id: id,
        drawing: {id: this.props.drawingId}
      })
    });

    if (response.ok) {
      const data = await response.json();

      if (data.success) {
        this.setState({
          userDrawings: this.state.userDrawings.filter(ud => ud.id !== data.data)
        });
        this.showAlert("success", "check", "Success", "Unshared with user successfully");
      } else {
        this.showAlert("danger", "exclamation", "Error", data.msg);
      }
    } else {
      this.showAlert("danger", "exclamation", "Error", "Unknown error unsharing with user");
    }
  }

  handleEmailChange = (event) => {
    this.setState({email: event.target.value});
  }

  fetchSharedUsers = async () => {
    const response = await fetch(`api/drawing/GetSharedUsers/${this.props.drawingId}`, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();
      if (data.success === true) {
        this.setState({loading: false, userDrawings: data.data});
      }
    }
  }

  showAlert = (colour, icon, title, msg) => {
    this.setState({
      alertVisible: true,
      alertColour: colour,
      alertIcon: icon,
      alertTitle: title,
      alertMsg: msg
    });
  }
}