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
      sharedUsers: [],
      loading: true,
      email: ""
    };

    this.fetchSharedUsers = this.fetchSharedUsers.bind(this);
    this.handleEmailChange = this.handleEmailChange.bind(this);
    this.handleAddUser = this.handleAddUser.bind(this);
  }

  componentDidMount() {
    this.fetchSharedUsers()
  }

  render() {
    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle}>
        <ModalHeader toggle={this.props.toggle}>Sharing Settings</ModalHeader>
        <ModalBody>
          This drawing is shared with:
          <ListGroup>
            {this.state.sharedUsers.map(user => {
              return (
                <ListGroupItem key={"ud-" + user.user.id}>
                  <div className="d-flex justify-content-between">
                    <div>
                      {user.user.email}
                    </div>
                    <Button close/>
                  </div>
                </ListGroupItem>
              );
            })}
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

  async handleAddUser(event) {
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
          sharedUsers: [...this.state.sharedUsers, data.data],
          email: ""
        });
      }
    }
  }

  handleEmailChange(event) {
    this.setState({email: event.target.value});
  }

  async fetchSharedUsers() {
    const response = await fetch("api/drawing/GetSharedUsers/" + this.props.drawingId, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();
      if (data.success === true) {
        this.setState({loading: false, sharedUsers: data.data});
      }
    }
  }
}