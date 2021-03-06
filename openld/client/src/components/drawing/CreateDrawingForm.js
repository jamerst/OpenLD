import React, { Component } from "react";
import {
  Alert,
  Container, Col, Row,
  Button, Form, FormGroup, Input, Label,
  Modal, ModalHeader, ModalBody, ModalFooter, CustomInput } from 'reactstrap';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import authService from '../api-authorization/AuthorizeService';


export class CreateDrawingForm extends Component {
  constructor(props) {
    super(props);

    this.state = {
      title: "New Drawing",
      width: 50,
      height: 50,
      error: false,
      errorMsg: "",
      useTemplate: false,
      template: "",
      templates: []
    }
  }

  componentDidMount = () => {
    this.fetchTemplates();
  }

  render = () => {
    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle}>
          <ModalHeader toggle={this.props.toggle}>New Drawing</ModalHeader>
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
                    <Label for="name">Drawing Title</Label>
                    <Input type="text" value={this.state.title} onChange={this.handleTitleChange} name="title" placeholder="Title"/>
                  </FormGroup>
                </Col>
              </Row>

              <Row form>
                <Col xs="12">
                  <FormGroup check>
                    <Label check>
                      <Input type="checkbox" value={this.state.useTemplate} onChange={this.handleUseTemplateChange} disabled={this.state.templates.length === 0}/>
                      Use a template
                    </Label>
                  </FormGroup>
                </Col>
              </Row>

              <Row form>
                <Col xs="12">
                  <FormGroup>
                    <Label for="template">Template</Label>
                    <CustomInput type="select" name="template" id="template" value={this.state.template} disabled={!this.state.useTemplate} onChange={this.handleTemplateChange}>
                      {
                        this.state.templates.map(template => {
                          return(<option key={template.id} value={template.id}>{template.drawing.title}</option>);
                        })
                      }
                    </CustomInput>
                  </FormGroup>
                </Col>
              </Row>

              <Label className="mb-0">Default View Size</Label>
              <Row form>
                <Col xs="12" md="6">
                  <FormGroup>
                    <Label for="width">Width (m)</Label>
                    <Input type="number" value={this.state.width} onChange={this.handleWidthChange} name="width" placeholder="Width (m)" step="0.1" min="0" disabled={this.state.useTemplate}/>
                  </FormGroup>
                </Col>
                <Col xs="12" md="6">
                  <FormGroup>
                    <Label for="height">Height (m)</Label>
                    <Input type="number" value={this.state.height} onChange={this.handleHeightChange} name="height" placeholder="Height (m)" step="0.1" min="0" disabled={this.state.useTemplate}/>
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

  handleSubmit = async (event) => {
    event.preventDefault();
    const response = await fetch("api/drawing/CreateDrawing", {
      method: "POST",
      headers: await authService.generateHeader({ 'Content-Type': 'application/json' }),
      body: JSON.stringify({
        template: {
          id: this.state.useTemplate ? this.state.template : null
        },
        drawing: {
          title: this.state.title,
          views: [{
            width: this.state.width,
            height: this.state.height
          }]
        }
      })
    });

    if (response.ok) {
      const data = await response.json();

      if (data.success === true) {
        this.props.onSubmitSuccess(data.data);
      } else {
        this.setState({error: true, errorMsg: data.msg});
      }
    } else {
      this.setState({error: true, errorMsg: "Unknown error creating drawing"});
    }
  }

  handleTitleChange = (event) => {
    this.setState({title: event.target.value});
  }

  handleWidthChange = (event) => {
    this.setState({width: event.target.value});
  }

  handleHeightChange = (event) => {
    this.setState({height: event.target.value});
  }

  handleUseTemplateChange = (event) => {
    this.setState({useTemplate: !this.state.useTemplate});
  }

  handleTemplateChange = (event) => {
    this.setState({template: event.target.value});
  }

  fetchTemplates = async () => {
    const response = await fetch("/api/drawing/GetTemplates", {
      method: "GET",
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();
      this.setState({
        templates: data,
        template: data.length > 0 ? data[0].id : ""
      });
    }
  }
}