import React, { Component } from "react";
import authService from '../api-authorization/AuthorizeService';
import { Alert, Collapse,
  Col, Container, Row,
  Card,
  Button, CustomInput, Form, FormGroup, Input, Label,
  InputGroup, InputGroupAddon, InputGroupText,
  Modal, ModalHeader, ModalBody, ModalFooter,
  Spinner } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { FixtureModesForm } from "./FixtureModesForm";

export class AddFixtureForm extends Component {
  constructor(props) {
    super(props);

    this.state = {
      newFixture: new NewFixture(),
      uploading: false,
      uploadAlertColour: "secondary",
      uploadAlertIcon: <Spinner size="sm" />,
      uploadAlertTitle: "Uploading file..",
      uploadAlertText: "",
      addFormError: false,
      addFormErrorMessage: "",
      dmx: false,
      modes: [{name: "Mode 1", channels: [""]}]
    }
  }

  render = () => {
    return (
      <Modal isOpen={this.props.isOpen} onOpened={this.handleOpen}>
        <ModalHeader>Add New Fixture</ModalHeader>
        <Form onSubmit={this.handleAddSubmit} onChange={this.handleAddChange}>
        <Alert color="danger" isOpen={this.state.addFormError}>
          <Container>
            <Row className="align-items-center">
              <Col xs="auto" className="p-0">
                <FontAwesomeIcon icon="exclamation" />
              </Col>
              <Col>
                <div className="font-weight-bold">Error</div>
                <div>{this.state.addFormErrorMessage}</div>
              </Col>
            </Row>
          </Container>
        </Alert>
          <ModalBody>
            <FormGroup>
              <Input type="text" value={this.state.newFixture.name} name="name" placeholder="Name" required/>
            </FormGroup>

            <FormGroup>
              <Input type="text" value={this.state.newFixture.manufacturer} name="manufacturer" placeholder="Manufacturer" required/>
            </FormGroup>

            <FormGroup>
              <Label for="addType">Fixture Type</Label>
              <CustomInput type="select" name="type" id="addType" value={this.state.newFixture.type.id} required>
                {this.props.renderedTypes}
              </CustomInput>
            </FormGroup>

            <FormGroup row>
              <Col xs="6">
                <Label for="addPower">Power</Label>
                <InputGroup>
                  <Input type="number" id="addPower" value={this.state.newFixture.power} name="power" min="0"/>
                  <InputGroupAddon addonType="append">
                    <InputGroupText>W</InputGroupText>
                  </InputGroupAddon>
                </InputGroup>
              </Col>
              <Col xs="6">
                <Label for="addWeight">Weight</Label>
                <InputGroup>
                  <Input type="number" id="addWeight" value={this.state.newFixture.weight} name="weight" step="0.1" min="0"/>
                  <InputGroupAddon addonType="append">
                    <InputGroupText>kg</InputGroupText>
                  </InputGroupAddon>
                </InputGroup>
              </Col>
            </FormGroup>

            <FormGroup check>
              <Label check for="dmx">
                <Input type="checkbox" id="dmx" checked={this.state.dmx} onChange={() => this.setState({dmx: !this.state.dmx})}/>
                Configure DMX Information
              </Label>
            </FormGroup>

            <Collapse isOpen={this.state.dmx}>
              <FixtureModesForm
                enabled = {this.state.dmx}
                modes = {this.state.modes}

                setModeName = {this.setModeName}
                setChannelName = {this.setChannelName}

                addMode = {this.addMode}
                addChannel = {this.addChannel}
              />
            </Collapse>

            <FormGroup>
              <Label for="addImage">Fixture Image</Label>
              <Alert isOpen={this.state.uploading} color={this.state.uploadAlertColour} style={{ fontSize: "11pt", padding: ".25rem 1rem" }}>
                <Container>
                  <Row className="align-items-center">
                    <Col xs="auto" className="p-0">
                      {this.state.uploadAlertIcon}
                    </Col>
                    <Col>
                      <div className="font-weight-bold">{this.state.uploadAlertTitle}</div>
                      <div>{this.state.uploadAlertText}</div>
                    </Col>
                  </Row>
                </Container>
              </Alert>
              <Input type="file" id="addImage" name="image" onChange={this.handleImageChange} required/>
            </FormGroup>
          </ModalBody>
          <ModalFooter>
            <Button type="submit"color="primary">Add</Button>
            <Button color="secondary" onClick={this.props.onCancel}>Cancel</Button>
          </ModalFooter>
        </Form>
      </Modal>
    );
  }

  handleAddChange = (event) => {
    if (event.target.name !== "image") {
      let values = this.state.newFixture;

      if (event.target.name === "type") {
        values.type.id = event.target.value;
      } else {
        values[event.target.name] = event.target.value;
      }
      this.setState({ newFixture: values });
    }
  }

  handleImageChange = async (event) => {
    this.setState({
      uploading: true,
      uploadAlertColor: "secondary",
      uploadAlertIcon: <Spinner size="sm"/>,
      uploadAlertTitle: "Uploading file.."
    });

    let formData = new FormData();
    formData.append("file", event.target.files[0]);

    const response = await fetch('api/library/UploadFixtureImage', {
      method: "POST",
      headers: await authService.generateHeader(),
      body: formData
    });

    const data = await response.json();
    if (data.success) {
      let fixtureData = this.state.newFixture;
      fixtureData.image.id = data.data;
      this.setState({
        newFixture: fixtureData,
        uploadAlertColour: "success",
        uploadAlertIcon: <FontAwesomeIcon icon="check" />,
        uploadAlertTitle: "File uploaded successfully",
        uploadAlertText: ""
      });
    } else {
      this.setState({
        uploadAlertColour: "danger",
        uploadAlertIcon: <FontAwesomeIcon icon="exclamation" />,
        uploadAlertTitle: "Error",
        uploadAlertText: data.msg
      });
    }
  }

  handleAddSubmit = async (event) => {
    event.preventDefault();

    this.setState({ addFormError: false });

    let fixtureData = this.state.newFixture;
    if (this.state.dmx) {
      fixtureData = {...fixtureData, ...{modes: this.state.modes}};
    }

    const response = await fetch('api/library/CreateFixture', {
      method: "POST",
      headers: await authService.generateHeader({ 'Content-Type': 'application/json' }),
      body: JSON.stringify(fixtureData)
    });

    if (response.ok) {
      const data = await response.json();
      if (data.success === true) {
        this.setState({
          newFixture: new NewFixture(),
          uploading: false,
          uploadAlertColor: "secondary",
          uploadAlertIcon: <Spinner size="sm"/>,
          uploadAlertTitle: "Uploading file.."
        });

        this.props.onSubmitSuccess();
      } else {
        this.setState({
          addFormError: true,
          addFormErrorMessage: data.msg
        });
      }
    } else {
      this.setState({
        addFormError: true,
        addFormErrorMessage: `Unknown error adding fixture (code ${response.status})`
      });
    }
  }

  handleOpen = () => {
    let fixture = new NewFixture();
    fixture.type.id = this.props.types[0].id

    this.setState({
      newFixture: fixture
    });
  }

  setModeName = (index, name) => {
    this.setState(() => {
      let modes = [...this.state.modes];
      let mode = modes[index];

      mode.name = name;
      modes[index] = mode;

      return {
        modes: modes
      };
    })
  }

  setChannelName = (mIndex, cIndex, name) => {
    this.setState(() => {
      let modes = [...this.state.modes];
      let mode = modes[mIndex];

      let channels = [...mode.channels];
      channels[cIndex] = name;

      mode.channels = channels;

      modes[mIndex] = mode;

      return {
        modes: modes
      };
    })
  }

  addMode = () => {
    this.setState(prevState => {
      let modes = [...prevState.modes];

      modes.push({name: "Mode", channels: [""]});

      return {
        modes: modes
      };
    });
  }

  addChannel = (modeIndex) => {
    this.setState(prevState => {
      let modes = [...prevState.modes];
      let mode = modes[modeIndex];

      mode.channels.push("");
      modes[modeIndex] = mode;

      return {
        modes: modes
      };
    });
  }
}

export class NewFixture {
  name = "";
  manufacturer = "";
  releaseDate = new Date();
  type = {
      id: ""
  };
  power = 0;
  weight = 0;
  image = {
      id: ""
  }
}