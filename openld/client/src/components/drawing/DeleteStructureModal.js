import React, { Component } from "react";
import {
  Alert,
  Container, Col, Row,
  Button,
  Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export class DeleteStructureModal extends Component {
  constructor(props) {
    super(props);

    this.state = {
      error: false,
      errorMsg: ""
    }
  }

  render = () => {
    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle} autoFocus={false} centered fade={false}>
        <ModalHeader toggle={this.props.toggle}>Delete Structure</ModalHeader>
        <Alert color="danger" isOpen={this.state.error}>
          <Container>
            <Row className="align-items-center">
              <Col xs="auto" className="p-0">
                <FontAwesomeIcon icon="exclamation" />
              </Col>
              <Col>
                <div className="font-weight-bold">Error</div>
                <div>{this.state.errorMsg}</div>
              </Col>
            </Row>
          </Container>
        </Alert>
        <ModalBody className="text-center">
          Are you sure you want to delete this structure?
          <p className="font-weight-bold h5 text-danger">
            This cannot be undone, all data will be lost permanently!
          </p>
        </ModalBody>
        <ModalFooter>
          <Button color="danger" onClick={this.handleConfirm} autoFocus={true}>Delete</Button>
          <Button color="secondary" onClick={this.props.toggle}>Cancel</Button>
        </ModalFooter>
      </Modal>
    )
  }

  handleConfirm = () => {
    this.props.hub.invoke(
      "DeleteStructure",
      this.props.structureId,
      this.props.viewId
    ).catch(err => {
      this.setState({error: true, errorMsg: err});
    }).then(() => {
      this.props.toggle();
    });
  }
}