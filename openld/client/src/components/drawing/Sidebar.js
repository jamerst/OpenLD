import React, { Component } from "react";
import { Link } from 'react-router-dom';
import { Col, Row,
  Button, CustomInput, Form, Input, InputGroup, InputGroupAddon, InputGroupText,
  Card, CardHeader, CardBody,
  ListGroup, ListGroupItem, Label
} from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import AwesomeDebouncePromise from "awesome-debounce-promise";

import { ShareDrawing } from "./ShareDrawing";
import { CreateViewForm } from "./CreateViewForm";
import { DeleteViewModal } from "./DeleteViewModal";
import authService from "../api-authorization/AuthorizeService";

export class Sidebar extends Component {
  constructor(props) {
    super(props);

    this.state = {
      shareModalOpen: false,
      createViewOpen: false,
      deleteViewOpen: false,
      deletedViewId: "",
      deletedViewName: "",
      gridTooltipOpen: false,
      types: [],
      name: "",
      type: "",
      rating: "",
      notes: "",
      fixture: "",
      angle: "",
      address: "",
      universe: "",
      mode: "",
      colour: ""
    };
  }

  componentDidMount() {
    this.fetchTypes();
    this.setState({gridTooltipOpen: true});
  }

  static getDerivedStateFromProps = (nextProps) => {
    // update state from props if current state has not been modified
    if (!nextProps.modifiedCurrent && nextProps.selectedObjectType === "structure") {
      return {
        name: nextProps.structure.name,
        type: nextProps.structure.type.id,
        rating: nextProps.structure.rating,
        notes: nextProps.structure.notes
      };
    } else if (!nextProps.modifiedCurrent && nextProps.selectedObjectType === "fixture") {
      return {
        name: nextProps.fixture.name,
        fixture: nextProps.fixture.fixture,
        angle: nextProps.fixture.angle,
        address: nextProps.fixture.address,
        universe: nextProps.fixture.universe,
        mode: nextProps.fixture.mode.id,
        notes: nextProps.fixture.notes,
        colour: nextProps.fixture.colour
      }
    } else {
      return null;
    }
  }

  render = () => {
    return (
      <Col className="p-0 d-flex flex-column align-items-stretch bg-light" style={{maxHeight: this.props.height, minWidth: this.props.width, maxWidth: this.props.width}}>
        <Col className="p-0 overflow-auto">
          <Card className="rounded-0" style={{minHeight: "12em"}}>
            <CardHeader className="d-flex justify-content-between align-content-center pl-3 pr-3">
              <h5 className="mb-0">Views</h5>
              <Button onClick={this.toggleCreateView} close disabled={!this.props.hubConnected}><FontAwesomeIcon icon="plus-circle"/></Button>
              <CreateViewForm
                isOpen = {this.state.createViewOpen}
                toggle = {this.toggleCreateView}
                hub = {this.props.hub}
                drawingId = {this.props.drawingId}
              />
              <DeleteViewModal
                isOpen = {this.state.deleteViewOpen}
                toggle = {this.toggleDeleteView}
                hub = {this.props.hub}
                viewId = {this.state.deletedViewId}
                viewName = {this.state.deletedViewName}
              />
            </CardHeader>

            <CardBody className="p-0">
              <ListGroup>
                {this.props.views.map(view => {
                  let button;
                  if (this.props.views.length > 1) {
                    button = (
                    <Button close disabled={!this.props.hubConnected}>
                      <FontAwesomeIcon
                        icon="trash"
                        size="xs"
                        className={this.props.currentView === view.id ? "text-white" : ""}
                        onClick={event => {
                          event.stopPropagation();
                          this.handleClickDelete(view.id, view.name);
                        }}
                      />
                    </Button>);
                  }

                  return (
                    <ListGroupItem
                      key = {"list-" + view.id}
                      className="rounded-0 p-1 border-left-0 border-right-0 d-flex justify-content-between text-break"
                      onClick={() => this.props.onClickView(view.id)}
                      active={this.props.currentView === view.id}
                      style={{cursor: "pointer"}}
                    >
                      <div>{view.name}</div>
                      {button}
                    </ListGroupItem>
                  );
                })}
              </ListGroup>
            </CardBody>
          </Card>

          <Card className="rounded-0" style={{minHeight: "12em"}}>
            <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Selected Object</h5></CardHeader>
            <CardBody className="d-flex align-items-center justify-content-center">
              {this.renderObjectProps()}
            </CardBody>
          </Card>

          <Card className="rounded-0" style={{minHeight: "12em"}}>
            <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Drawing Properties</h5></CardHeader>
            <CardBody>
              <Button color="primary" size="sm" onClick={this.toggleShareModal} className="mb-3">Sharing Settings</Button>

              <CustomInput type="switch" onChange={this.props.toggleGrid} checked={this.props.gridEnabled} id="grid-toggle" label="Show Grid"/>

              <Label for="grid-size" className="mb-0">Grid Size</Label>
              <Row>
                <Col xs="10" className="d-flex align-items-center">
                  <CustomInput
                    type="range" min="1" max="20" step="1" name="gridSize" id="grid-size"
                    value={this.props.gridSize}
                    onChange={event => this.props.setGridSize(parseInt(event.target.value))}
                  />
                </Col>
                <Col xs="2" className="p-0">
                  {this.props.gridSize}m
                </Col>
              </Row>
              <Row className="mt-3">
                <Col>
                  <Link to={`/print/${this.props.drawingId}`}>Export to PDF</Link>
                </Col>
              </Row>
            </CardBody>
          </Card>
          <ShareDrawing
            isOpen = {this.state.shareModalOpen}
            drawingId = {this.props.drawingId}
            toggle = {this.toggleShareModal}
          />
        </Col>
        <Card className="rounded-0 bg-light" style={{justifySelf: "flex-end", maxHeight: "10%"}}>
          <CardBody className="overflow-auto small p-2" style={{whiteSpace: "pre-line"}}>
            {this.props.hintText}
          </CardBody>
        </Card>
      </Col>
    );
  }

  renderObjectProps = () => {
    switch(this.props.selectedObjectType) {
      case "structure":
        return (
          <Form>
            <Row form>
              <Col xs="12">
                <Label for="name" className="mb-0">Name</Label>
                <Input type="text" value={this.state.name} name="name" id="name" bsSize="sm" onChange={this.handlePropertyChange}/>
              </Col>

              <Col xs="12" xl="8">
                <Label for="type" className="mb-0 mt-2">Type</Label>
                <CustomInput type="select" value={this.state.type} name="type" id="type" bsSize="sm" onChange={this.handlePropertyChange}>
                  {this.state.types.map(type => {
                    return (
                      <option key={type.id} value={type.id}>{type.name}</option>
                    )
                  })}
                </CustomInput>
              </Col>
              <Col xs="12" xl="4">
                <Label for="rating" className="mb-0 mt-2">Load Rating</Label>
                <InputGroup size="sm">
                  <Input type="number" value={this.state.rating} name="rating" id="rating" step="0.1" min="0" onChange={this.handlePropertyChange}/>
                  <InputGroupAddon addonType="append">
                    <InputGroupText>kg</InputGroupText>
                  </InputGroupAddon>
                </InputGroup>
              </Col>

              <Col xs="12">
                <Label for="notes" className="mb-0 mt-2">Notes</Label>
                <Input type="textarea" value={this.state.notes} name="notes" id="notes" rows="4" onChange={this.handlePropertyChange}/>
              </Col>
            </Row>
          </Form>
        );
      case "fixture":
        return (
          <Form>
            <Row form>
              <Col xs="12">
                <Row>
                  <Col xs="4">
                    <img src={ "/api/fixture/GetImage/" + this.state.fixture.id } style={{maxWidth: "100%"}} alt={this.state.fixture.name}/>
                  </Col>
                  <Col xs="8" className="pl-0">
                    <ListGroup>
                      <ListGroupItem className="pt-1 pb-1 font-weight-bold">{this.state.fixture.manufacturer} {this.state.fixture.name}</ListGroupItem>
                      <ListGroupItem className="pt-1 pb-1 small">{this.state.fixture.power}W</ListGroupItem>
                      <ListGroupItem className="pt-1 pb-1 small">{this.state.fixture.weight}kg</ListGroupItem>
                    </ListGroup>
                  </Col>
                </Row>
              </Col>
              <Col xs="12">
                <Label for="name" className="mb-0">Name</Label>
                <Input type="text" value={this.state.name} name="name" id="name" bsSize="sm" onChange={this.handlePropertyChange}/>
              </Col>

              <Col xs="12" xl="8">
                <Label for="mode" className="mb-0 mt-2">Mode</Label>
                <CustomInput type="select" value={this.state.mode} name="mode" id="mode" bsSize="sm" onChange={this.handlePropertyChange}>
                  {this.state.fixture.modes.map(mode => {
                    return (
                      <option key={mode.id} value={mode.id}>{mode.name} ({mode.channels.length} channel)</option>
                    )
                  })}
                </CustomInput>
              </Col>
              <Col xs="12" xl="4">
                <Label for="angle" className="mb-0 mt-2">Angle</Label>
                <InputGroup size="sm">
                  <Input type="number" value={this.state.angle} name="angle" id="angle" step="1" min="0" max="360" onChange={this.handlePropertyChange}/>
                  <InputGroupAddon addonType="append">
                    <InputGroupText>&deg;</InputGroupText>
                  </InputGroupAddon>
                </InputGroup>
              </Col>

              <Col xs="6">
                  <Label for="universe" className="mb-0 mt-2">Universe</Label>
                  <Input type="number" value={this.state.universe} name="universe" id="universe" step="1" min="1" onChange={this.handlePropertyChange}/>
              </Col>
              <Col xs="6">
                  <Label for="universe" className="mb-0 mt-2">Address</Label>
                  <Input type="number" value={this.state.address} name="address" id="address" step="1" min="1" onChange={this.handlePropertyChange}/>
              </Col>

              <Col xs="12">
                  <Label for="colour" className="mb-0 mt-2">Colour</Label>
                  <Input type="text" value={this.state.colour} name="colour" id="colour" onChange={this.handlePropertyChange}/>
              </Col>

              <Col xs="12">
                <Label for="notes" className="mb-0 mt-2">Notes</Label>
                <Input type="textarea" value={this.state.notes} name="notes" id="notes" rows="4" onChange={this.handlePropertyChange}/>
              </Col>
            </Row>
          </Form>
        );
      case "none":
      default:
        return (
          <em>No object selected</em>
        );
    }
  }

  handlePropertyChange = (event) => {
    this.props.setModifiedCurrent(true);
    this.setState({
      [event.target.name]: event.target.value
    });

    this.sendChangesDebounce(event.target.name, event.target.value);
  }

  sendChanges = (field, value) => {
    let structureData, fixtureData = null;

    if (this.props.selectedObjectType === "structure") {
      structureData = {
        id: this.props.structure.id,
        [field]: value
      };

      if (field === "type") {
        structureData = {
          id: this.props.structure.id,
          [field]: {id: value}
        };
      }

    } else if (this.props.selectedObjectType === "fixture") {
      fixtureData = {
        id: this.props.fixture.id,
        [field]: value
      }

      if (field === "mode") {
        fixtureData = {
          id: this.props.fixture.id,
          [field]: {id: value}
        }
      }
    }

    this.props.hub.invoke(
      "UpdateObjectProperty",
      this.props.selectedObjectType,
      field,
      structureData,
      fixtureData
    ).catch(err => console.error(err));
  }

  sendChangesDebounce = AwesomeDebouncePromise(this.sendChanges, 250);

  fetchTypes = async () => {
    const response = await fetch("api/structure/GetStructureTypes", {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();

      if (data.success) {
        this.setState({types: data.data});
      }
    }
  }

  toggleShareModal = () => {
    this.setState({shareModalOpen: !this.state.shareModalOpen});
  }

  toggleCreateView = () => {
    this.setState({createViewOpen: !this.state.createViewOpen});
  }

  toggleDeleteView = () => {
    this.setState({deleteViewOpen: !this.state.deleteViewOpen});
  }

  handleClickDelete = (id, name) => {
    this.setState({
      deletedViewId: id,
      deletedViewName: name,
      deleteViewOpen: true
    })
  }

  toggleGridTooltip = () => {
    this.setState({gridTooltipOpen: !this.state.gridTooltipOpen});
  }
}