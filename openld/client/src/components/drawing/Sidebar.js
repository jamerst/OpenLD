import React, { Component } from "react";
import { Col, Row,
  Button, CustomInput, Form, Input, InputGroup, InputGroupAddon, InputGroupText,
  Card, CardHeader, CardBody,
  ListGroup, ListGroupItem, Label,
  Tooltip
} from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
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
      notes: ""
    };
  }

  componentDidMount() {
    this.fetchTypes();
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
    } else {
      return null;
    }
  }

  render = () => {
    return (
      <Col xs={this.props.xs} md={this.props.md} lg={this.props.lg} className="p-0 d-flex flex-column align-items-stretch bg-light" style={{maxHeight: this.props.height}}>
        <Card className="rounded-0" style={{minHeight: "15%"}}>
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

          <CardBody className="overflow-y-auto p-0">
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

        <Card className="rounded-0" style={{minHeight: "15%"}}>
          <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Selected Object</h5></CardHeader>
          <CardBody className="overflow-auto">
            {this.renderObjectProps()}
          </CardBody>
        </Card>

        <Card className="rounded-0" style={{minHeight: "15%"}}>
          <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Drawing Properties</h5></CardHeader>
          <CardBody className="overflow-auto">
            <Button color="primary" size="sm" onClick={this.toggleShareModal}>Sharing Settings</Button>
            <CustomInput type="switch" onChange={this.props.toggleGrid} checked={this.props.gridEnabled} id="grid-toggle" label="Show Grid"/>

            <Label for="grid-size">Grid Size</Label>
            <CustomInput
              type="range" min="1" max="20" step="1" name="gridSize" id="grid-size"
              value={this.props.gridSize}
              onChange={event => this.props.setGridSize(parseInt(event.target.value))}
            />
            <Tooltip isOpen={this.state.gridTooltipOpen} target="grid-size" toggle={this.toggleGridTooltip} hideArrow>
              {this.props.gridSize}m
            </Tooltip>
          </CardBody>
        </Card>
        <ShareDrawing
          isOpen = {this.state.shareModalOpen}
          drawingId = {this.props.drawingId}
          toggle = {this.toggleShareModal}
        />
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
          <div>This is a fixture</div>
        );
      case "none":
      default:
        return (
          <div className="text-center">
            <em>No object selected</em>
          </div>
        );
    }
  }

  handlePropertyChange = (event) => {
    this.props.setModifiedCurrent(true);
    this.setState({
      [event.target.name]: event.target.value
    });

    let data = {
      id: this.props.structure.id,
      [event.target.name]: event.target.value
    };

    if (event.target.name === "type") {
      data = {
        id: this.props.structure.id,
        [event.target.name]: {id: event.target.value}
      };
    }

    this.props.hub.invoke(
      "UpdateStructureProperty",
      data
    ).catch(err => console.error(err));
  }

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