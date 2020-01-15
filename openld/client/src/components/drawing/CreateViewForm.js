import React, { Component } from "react"
import {
  Alert,
  Container, Col, Row,
  Button, CustomInput, Form, FormGroup, Input, Label,
  Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export class CreateViewForm extends Component {
  constructor(props) {
    super(props);

    this.state = {
      name: "New View",
      width: 50,
      height: 50,
      type: 0,
      error: false,
      errorMsg: ""
    }
  }

  render = () => {
    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle}>
          <ModalHeader toggle={this.props.toggle}>New View</ModalHeader>
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
          <Form onSubmit={this.handleSubmit}>
            <ModalBody>
              <Row form>
                <Col xs="12">
                  <FormGroup>
                    <Label for="name">View Name</Label>
                    <Input type="text" value={this.state.name} onChange={this.handleNameChange} name="name" placeholder="Name"/>
                  </FormGroup>
                </Col>
              </Row>

              <Row form>
                <Col xs="12">
                  <FormGroup>
                    <Label for="type">View Type</Label>
                    <CustomInput type="select" name="type" id="type" value={this.state.type} onChange={this.handleTypeChange}>
                      <option value="0">Top-down</option>
                      <option value="1">Front-on</option>
                    </CustomInput>
                  </FormGroup>
                </Col>
              </Row>

              <Label className="mb-0">View Size</Label>
              <Row form>
                <Col xs="12" md="6">
                  <FormGroup>
                    <Label for="width">Width (m)</Label>
                    <Input type="number" value={this.state.width} onChange={this.handleWidthChange} name="width" placeholder="Width (m)" step="0.1" min="0"/>
                  </FormGroup>
                </Col>
                <Col xs="12" md="6">
                  <FormGroup>
                    <Label for="height">Height (m)</Label>
                    <Input type="number" value={this.state.height} onChange={this.handleHeightChange} name="height" placeholder="Height (m)" step="0.1" min="0"/>
                  </FormGroup>
                </Col>
              </Row>
            </ModalBody>
            <ModalFooter>
              <Button type="submit"color="primary">Create</Button>
              <Button color="secondary" onClick={this.props.toggle}>Cancel</Button>
            </ModalFooter>
          </Form>
        </Modal>
    )
  }

  handleSubmit = (event) => {
    event.preventDefault()

    this.props.hub.invoke(
      "CreateView",
      {
        drawing: { id: this.props.drawingId },
        name: this.state.name,
        type: this.state.type,
        width: this.state.width,
        height: this.state.height
      }
    ).catch(err => {
      console.error(err);
      this.setState({error: true, errorMsg: err});
    }).then(() => {
      this.props.toggle();
    });
  }

  handleNameChange = (event) => {
    this.setState({name: event.target.value});
  }

  handleTypeChange = (event) => {
    this.setState({type: event.target.value});
  }

  handleWidthChange = (event) => {
    this.setState({width: event.target.value});
  }

  handleHeightChange = (event) => {
    this.setState({height: event.target.value});
  }
}