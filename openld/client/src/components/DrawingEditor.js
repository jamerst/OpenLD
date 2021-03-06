import React, { Component, Fragment } from 'react';
import { Link } from 'react-router-dom';
import {
  Alert,
  Button, ButtonGroup,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink,
  Spinner, UncontrolledTooltip } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { HubConnectionBuilder } from "@microsoft/signalr";

import { Drawing } from "./drawing/Drawing";
import { Sidebar } from "./drawing/Sidebar";
import { Ops } from "./drawing/DrawingUtils";
import authService from './api-authorization/AuthorizeService';

export class DrawingEditor extends Component {
  constructor(props) {
    super(props);

    this.state = {
      loadingData: true,
      loadingSymbols: true,
      error: false,
      errorTitle: "",
      errorMsg: "",
      errorIcon: "",

      gridEnabled: true,
      gridSize: 1,
      snapGridSize: 0.1,
      stageScale: 50,
      stageWidth: 0,
      stageHeight: 0,
      stageX: 0,
      stageY: 0,
      stagePosition: {x: 0, y: 0},
      stageCursor: "grab",
      tooltipVisible: false,

      selectedTool: "none",
      isDrawing: false,
      selectedObjectType: "none",
      selectedObjectId: "",
      selectedStructure: {id: "", name: "", type: "", rating: "", notes: ""},
      selectedFixture: {id: "", name: "", fixture: "", address: "", universe: "", mode: "", notes: "", colour: "", angle: ""},
      selectedLabel: {id: "", text: ""},
      modifiedCurrent: false,

      drawingData: {},
      currentView: "",
      views: [],
      connectedUsers: [],

      hubConnected: false,

      alertIcon: "",
      alertContent: "",
      alertColour: "info",
      alertOpen: false,

      hintText: "Use the buttons on the left to select a tool"
    }
    this.hub = null;
    this.history = [];
    this.undoHistory = [];

    this.randomMc = require("random-material-color");
  }

  componentDidMount = () => {
    this.fetchDrawing();
  }

  componentWillUnmount = async () => {
    window.removeEventListener("resize", this.sizeStage);
    if (this.state.hubConnected) {
      await this.hub.stop();
    }
  }

  render = () => {
    let loadingSymbols;
    if (this.state.error === true) {
      return (
        <Container className="h-100">
          <Col className="h-100">
            <Row className="align-items-center justify-content-center flex-column h-100">
              {this.state.errorIcon}
              <h1 className="mt-3">{this.state.errorTitle}</h1>
              <h3>{this.state.errorMsg}</h3>
              <Link to="/">Return Home</Link>
            </Row>
          </Col>
        </Container>
        );
    } else if (this.state.loadingData === true) {
      return (
      <Container className="h-100">
        <Col className="h-100">
          <Row className="align-items-center justify-content-center h-100 flex-column">
            <Spinner style={{width: "10rem", height: "10rem"}}/>
            <h3 className="mt-3">Loading Data</h3>
          </Row>
        </Col>
      </Container>
      );
    } else if (this.state.loadingSymbols === true) {
      loadingSymbols = (
        <Container className="h-100 z4">
          <Col className="h-100">
            <Row className="align-items-center justify-content-center h-100 flex-column">
              <Spinner style={{width: "10rem", height: "10rem"}}/>
              <h3 className="mt-3">Loading Symbols</h3>
            </Row>
          </Col>
        </Container>
      )
    }

    return (
      <Fragment>
        {loadingSymbols}
        <div className="d-flex flex-column h-100">
          <Navbar color="dark">
            <NavbarBrand className="text-light">{this.state.drawingData.title}</NavbarBrand>
            <div className="d-flex">
              {this.state.connectedUsers.map(u => {
                return (
                  <div
                    key={`userCircle-${u.id}`}
                    className={"rounded-circle font-weight-bold d-flex align-items-center justify-content-center ml-2 " + (u.lightText ? "text-dark" : "text-light")}
                    style={{width: "2.5em", height: "2.5em", backgroundColor: u.colour}}
                  >
                    <div id={`userCircle-${u.id}`} style={{fontSize: "130%"}}>
                      {u.userName.charAt(0).toUpperCase()}
                    </div>
                    <UncontrolledTooltip placement="bottom" target={`userCircle-${u.id}`} style={{zIndex: 1000}}>{u.userName}</UncontrolledTooltip>
                  </div>
                );
              })}
              <NavLink tag={Link} to="/"><Button className="text-light" close/></NavLink>
            </div>
          </Navbar>
          <Container fluid className="pl-0 d-flex flex-grow-1">
            <Row className="d-flex flex-grow-1">
              <Col xs="auto" className="pr-0 bg-light">
                <ButtonGroup vertical>
                  <Button
                    outline
                    color="primary"
                    size="lg"
                    className="rounded-0"
                    onClick={() => this.handleToolSelect("polygon")}
                    active={this.state.selectedTool === "polygon"}
                    disabled={!this.state.hubConnected}
                    onMouseEnter = {() => this.setHintText("Insert new structure")}
                    onMouseLeave = {this.onToolButtonLeave}
                  >
                    <FontAwesomeIcon icon="draw-polygon"/>
                  </Button>

                  <Button
                    outline
                    color="primary"
                    size="lg"
                    className="rounded-0"
                    onClick={() => this.handleToolSelect("add-fixture")}
                    active={this.state.selectedTool === "add-fixture"}
                    disabled={!this.state.hubConnected}
                    onMouseEnter = {() => this.setHintText("Insert new fixture")}
                    onMouseLeave = {this.onToolButtonLeave}
                  >
                    <FontAwesomeIcon icon="plus-circle"/>
                  </Button>

                  <Button
                    outline
                    color="primary"
                    size="lg"
                    className="rounded-0"
                    onClick={() => this.handleToolSelect("add-label")}
                    active={this.state.selectedTool === "add-label"}
                    disabled={!this.state.hubConnected}
                    onMouseEnter = {() => this.setHintText("Add a label")}
                    onMouseLeave = {this.onToolButtonLeave}
                  >
                    <FontAwesomeIcon icon="i-cursor"/>
                  </Button>

                  <Button
                    outline
                    color="danger"
                    size="lg"
                    className="rounded-0"
                    onClick={() => this.handleToolSelect("eraser")}
                    active={this.state.selectedTool === "eraser"}
                    disabled={!this.state.hubConnected}
                    onMouseEnter = {() => this.setHintText("Remove object")}
                    onMouseLeave = {this.onToolButtonLeave}
                  >
                    <FontAwesomeIcon icon="eraser"/>
                  </Button>
                </ButtonGroup>
              </Col>
              <Col id="stage-container" className="p-0 m-0 bg-secondary">
                <div style={{position: "absolute", width: "100%", zIndex: "1000"}}>
                  <Alert color={this.state.alertColour} isOpen={this.state.alertOpen} toggle={this.toggleAlert} className="d-flex justify-content-center align-items-center">
                    <span className="mr-3">{this.state.alertIcon}</span>
                    <span className="h5 m-0">{this.state.alertContent}</span>
                  </Alert>
                </div>
                <Drawing
                  width = {this.state.stageWidth}
                  height = {this.state.stageHeight}
                  scale = {this.state.stageScale}
                  x = {this.state.stageX}
                  y = {this.state.stageY}
                  position = {this.state.stagePosition}

                  viewData = {this.getCurrentView()}
                  isDrawing = {this.state.isDrawing}
                  selectedTool = {this.state.selectedTool}
                  cursor = {this.state.stageCursor}
                  selectedObjectId = {this.state.selectedObjectId}
                  selectedObjectType = {this.state.selectedObjectType}
                  hub = {this.hub}
                  hubConnected = {this.state.hubConnected}
                  tooltipVisible = {this.state.tooltipVisible}
                  currentView = {this.state.currentView}

                  onMoveStructure = {this.moveStructure}
                  onMoveFixture = {this.moveFixture}
                  onMoveLabel = {this.moveLabel}
                  onSelectObject = {this.selectObject}
                  deselectObject = {this.deselectObject}
                  onRemoveObject = {this.onRemoveObject}
                  onSymbolLoad = {this.onSymbolLoad}

                  gridEnabled = {this.state.gridEnabled}
                  gridSize = {this.state.gridSize}
                  snapGridSize = {this.state.snapGridSize}

                  setScale = {this.setScale}
                  setPosition = {this.setPosition}
                  setTool = {this.setTool}
                  setIsDrawing = {this.setIsDrawing}
                  setCursor = {this.setCursor}
                  setStructureColour = {this.setStructureColour}
                  setFixtureColour = {this.setFixtureColour}
                  setLabelColour = {this.setLabelColour}
                  setHintText = {this.setHintText}
                  setTooltipVisible = {this.setTooltipVisible}
                  pushHistoryOp = {this.pushHistoryOp}
                  setAlertError = {this.setAlertError}
                  setAlertIcon = {this.setAlertIcon}
                  getStructure = {this.getStructure}
                  getFixture = {this.getFixture}
                  getLabel = {this.getLabel}
                />
              </Col>
              <Sidebar
                drawingId = {this.state.drawingData.id}

                height = {this.state.stageHeight}
                width = "22em"

                views = {this.state.views}
                currentView = {this.state.currentView}
                hub = {this.hub}
                hubConnected = {this.state.hubConnected}
                gridEnabled = {this.state.gridEnabled}
                gridSize = {this.state.gridSize}
                structure = {this.state.selectedStructure}
                fixture = {this.state.selectedFixture}
                label = {this.state.selectedLabel}
                selectedObjectId = {this.state.selectedObjectId}
                selectedObjectType = {this.state.selectedObjectType}
                modifiedCurrent = {this.state.modifiedCurrent}
                hintText = {this.state.hintText}
                owner = {this.state.drawingData.owner}

                onClickView = {this.switchView}
                toggleGrid = {this.toggleGrid}
                setGridSize = {this.setGridSize}
                getStructure = {this.getStructure}
                setModifiedCurrent = {this.setModifiedCurrent}
                setAlertIcon = {this.setAlertIcon}
                pushHistoryOp = {this.pushHistoryOp}
                setAlertError = {this.setAlertError}
                deleteView = {this.deleteView}
              />
            </Row>
          </Container>
        </div>
      </Fragment>
    );
  }

  initHubConnection = async () => {
    let token = await authService.getAccessToken();

    this.hub = new HubConnectionBuilder()
      .withUrl("/api/drawing/hub", { accessTokenFactory: () => token })
      .withAutomaticReconnect([0, 1000, 1000, 2000, 2000, 5000, 5000, 5000, 10000, 10000, 20000])
      .build();

    this.hub
      .start()
      .then(() => {
        this.addHubHandlers();
        this.setState({hubConnected: true});
        this.hub.invoke("OpenDrawing", this.state.drawingData.id)
          .catch(err => console.error(err.toString()));
      })
      .catch(err => console.error(`Hub error: ${err}`));

      this.hub.onreconnecting(this.onHubReconnecting);
      this.hub.onreconnected(this.onHubReconnect);
      this.hub.onclose(this.onHubDisconnect);
  }

  addHubHandlers = () => {
    this.hub.on("ConnectedUsers", this.setConnectedUsers);
    this.hub.on("UserJoined", this.userJoin);
    this.hub.on("UserLeft", this.userLeave);

    this.hub.on("NewView", this.insertNewView);
    this.hub.on("DeleteView", this.onDeleteView);

    this.hub.on("SelectObject", this.userSelectObject);
    this.hub.on("DeselectObject", this.userDeselectObject);

    this.hub.on("NewStructure", this.insertNewStructure);

    this.hub.on("UpdateStructureGeometry", (structure, fixtures) => this.updateStructurePos(structure.view.id, structure.id, structure.geometry.points, fixtures));
    this.hub.on("UpdateObjectProperty", this.onObjectUpdate);
    this.hub.on("DeleteObject", this.onObjectRemoved);

    this.hub.on("AddFixture", this.insertNewFixture);
    this.hub.on("UpdateFixturePosition", this.updateFixturePos);

    this.hub.on("AddLabel", this.insertNewLabel);
    this.hub.on("UpdateLabelPosition", this.updateLabelPos);
  }

  onHubDisconnect = () => {
    this.setAlertIcon("danger", "Lost connection to OpenLD. Please reconnect to make changes.", "exclamation");
    this.setState({hubConnected: false, connectedUsers: []});
  }

  onHubReconnecting = () => {
    this.setAlertSpinner("info", "Reconnecting to OpenLD..");
    this.setState({hubConnected: false, connectedUsers: []});
  }

  onHubReconnect = () => {
    this.setAlertIcon("success", "Successfully reconnected to OpenLD.", "check");
    window.setTimeout(() => this.setState({alertOpen: false}), 10000);
    this.setState({hubConnected: true});
    this.hub.invoke("OpenDrawing", this.state.drawingData.id)
      .catch(err => console.error(err.toString()));
  }

  fetchDrawing = async () => {
    const response = await fetch(`api/drawing/GetDrawing/${this.props.match.params.id}`, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();

      this.setState({
        drawingData: data.data,
        loadingData: false,
        currentView: data.data.views[0].id,
        views: data.data.views
      }, async () => {
        this.initHubConnection();
        this.sizeStage(this.scaleStage);
        window.addEventListener("resize", this.sizeStage);
        window.addEventListener("keydown", this.handleKeyDown);
      });
    } else if (response.status === 401) {
      this.setState({
        error: true,
        errorTitle: "Access Denied",
        errorMsg: "Sorry, you don't have permission to view this drawing.",
        errorIcon: <FontAwesomeIcon icon="ban" style={{width: "15rem", height: "15rem", color: "#B71C1C"}}/>
      });
    } else if (response.status === 404) {
      this.setState({
        error: true,
        errorTitle: "Not Found",
        errorMsg: "The drawing you requested couldn't be found.",
        errorIcon: <FontAwesomeIcon icon={["far", "question-circle"]} style={{width: "15rem", height: "15rem", color: "#1565C0"}}/>
      });
    }
  }

  onSymbolLoad = () => {
    this.setState({
      loadingSymbols: false
    });
  }

  insertNewStructure = (structure) => {
    this.setState((prevState) => {
      let views = [...prevState.views];
      const modifiedIndex = views.findIndex(view => view.id === structure.view.id);
      const modifiedView = views[modifiedIndex]

      structure.colour = "#000";
      const newView = {
        ...modifiedView,
        structures: [...modifiedView.structures, structure]
      };

      views[modifiedIndex] = newView;

      return {
        views: views
      };
    });
  }

  moveStructure = async (id, points, fixtures, prevPoints, prevFixtures) => {
    let result;
    result = await this.hub.invoke(
      "UpdateStructureGeometry",
        id,
        {points: points},
        fixtures
    ).catch(err => {console.error(err); result = false;});

    if (result === true) {
      this.pushHistoryOp({
        type: Ops.MOVE_STRUCTURE,
        data: {
          id: id,
          prevValue: {points: prevPoints, fixtures: prevFixtures},
          newValue: {points: points, fixtures: fixtures}
        }
      });
    } else {
      this.setAlertError("Failed to move structure");
    }
  }

  updateStructurePos = (viewId, id, points, fixtures) => {
    this.setState((prevState) => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(view => view.id === viewId);
      const modifiedView = views[viewIndex];

      const structureIndex = modifiedView.structures.findIndex(structure => structure.id === id);
      const modifiedStructure = modifiedView.structures[structureIndex];

      const newStructure = {
        ...modifiedStructure,
        geometry: {points: points},
        fixtures: fixtures
      };

      modifiedView.structures[structureIndex] = newStructure;

      views[viewIndex] = modifiedView;

      return {
        views: views
      };
    });
  }

  moveFixture = async (id, pos, prevPos) => {
    let result = false;
    result = await this.hub.invoke(
      "UpdateFixturePosition",
      {
        id: id,
        position: pos
      }
    ).catch(err => {console.error(err); result = false});

    if (result === true) {
      this.pushHistoryOp({
        type: Ops.MOVE_FIXTURE,
        data: {
          id: id,
          prevValue: prevPos,
          newValue: pos
        }
      });
    } else {
      this.setAlertError("Failed to move fixture");
    }
  }

  updateFixturePos = (fixture) => {
    this.setState((prevState) => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === fixture.structure.view.id);
      if (viewIndex < 0) {
        console.error(`updateFixturePos error: view ID "${fixture.structure.view.id}" not found`);
        return;
      }
      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === fixture.structure.id);
      if (structureIndex < 0) {
        console.error(`updateFixturePos error: structure ID "${fixture.structure.id}" not found`);
        return;
      }
      let structure = structures[structureIndex];

      let fixtures = [...structure.fixtures];
      const fixtureIndex = fixtures.findIndex(f => f.id === fixture.id);
      if (fixtureIndex < 0) {
        console.error(`updateFixturePos error: fixture ID "${fixture.id}" not found`);
        return;
      }

      fixtures[fixtureIndex].position = fixture.position;

      structures[structureIndex].fixtures = fixtures;
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views,
      };
    });
  }

  moveLabel = async (id, pos, prevPos) => {
    let result = false;
    result = await this.hub.invoke(
      "UpdateLabelPosition",
      {
        id: id,
        position: pos
      }
    ).catch(err => {console.error(err); result = false});

    if (result === true) {
      this.pushHistoryOp({
        type: Ops.MOVE_LABEL,
        data: {
          id: id,
          prevValue: prevPos,
          newValue: pos
        }
      });
    } else {
      this.setAlertError("Failed to move label");
    }
  }

  updateLabelPos = (label) => {
    this.setState((prevState) => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(view => view.id === label.view.id);
      const modifiedView = views[viewIndex];

      const labelIndex = modifiedView.labels.findIndex(l => l.id === label.id);

      modifiedView.labels[labelIndex].position = label.position;

      views[viewIndex] = modifiedView;

      return {
        views: views
      };
    });
  }

  userSelectObject = (type, id, userId, viewId, structureId) => {
    if (type === "structure") {
      this.setStructureColour(viewId, id, this.getUserColour(userId));
    } else if (type === "fixture") {
      this.setFixtureColour(viewId, structureId, id, this.getUserColour(userId));
    } else if (type === "label") {
      this.setLabelColour(viewId, id, this.getUserColour(userId));
    }
  }

  userDeselectObject = (type, id, viewId, structureId) => {
    if (type === "structure") {
      if (this.state.selectedObjectType === "structure" && this.state.selectedObjectId === id) {
        this.setStructureColour(viewId, id, "#007bff");
      } else {
        this.setStructureColour(viewId, id, "#000");
      }
    } else if (type === "fixture") {
      if (this.state.selectedObjectType === "fixture" && this.state.selectedObjectId === id) {
        this.setFixtureColour(viewId, structureId, id, "#007bff");
      } else {
        this.setFixtureColour(viewId, structureId, id, "#000");
      }
    } else if (type === "label") {
      if (this.state.selectedObjectType === "label" && this.state.selectedObjectId === id) {
        this.setLabelColour(viewId, id, "#007bff");
      } else {
        this.setLabelColour(viewId, id, "#fff");
      }
    }
  }

  onObjectRemoved = (type, id, viewId, structureId) => {
    if (this.state.selectedObjectType === type && this.state.selectedObjectId === id) {
      this.setAlertIcon("warning", `The ${type} you were working on was deleted.`, "exclamation");
    }

    if (type === "structure") {
      this.removeStructure(viewId, id);
    } else if (type === "fixture") {
      this.removeFixture(viewId, structureId, id);
    } else if (type === "label") {
      this.removeLabel(viewId, id);
    }
  }

  onRemoveObject = (type, id, viewId, structureId, undo) => {
    if (type === "structure") {
      if (!undo) {
        this.pushHistoryOp({type: Ops.REMOVE_STRUCTURE, data: {...this.getStructure(viewId, id), ...{view: {id: viewId}}}});
      }
      this.removeStructure(viewId, id);
    } else if (type === "fixture") {
      if (!undo) {
        this.pushHistoryOp({type: Ops.REMOVE_FIXTURE, data: {...this.getFixture(viewId, structureId, id), ...{structure: {id: structureId}}}});
      }
      this.removeFixture(viewId, structureId, id);
    } else if (type === "label") {
      if (!undo) {
        this.pushHistoryOp({type: Ops.REMOVE_LABEL, data: {...this.getLabel(viewId, id), ...{view: {id: viewId}}}});
      }
      this.removeLabel(viewId, id);
    }
  }

  removeStructure = (viewId, structureId) => {
    if (this.state.selectedObjectType === "structure" && this.state.selectedObjectId === structureId) {
      this.setState({selectedObjectType: "none", selectedObjectId: ""});
      this.setHintText("");
    }

    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        return;
      }

      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structureId);
      if (structureIndex < 0) {
        return;
      }

      structures.splice(structureIndex, 1);

      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  removeFixture = (viewId, structureId, fixtureId) => {
    if (this.state.selectedObjectType === "fixture" && this.state.selectedObjectId === fixtureId) {
      this.setState({selectedObjectType: "none", selectedObjectId: ""});
      this.setHintText("");
    }

    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        return;
      }
      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structureId);
      if (structureIndex < 0) {
        return;
      }
      let structure = structures[structureIndex];

      let fixtures = [...structure.fixtures];
      const fixtureIndex = fixtures.findIndex(f => f.id === fixtureId);
      if (fixtureIndex < 0) {
        return;
      }

      fixtures.splice(fixtureIndex, 1);

      structure.fixtures = fixtures;
      structures[structureIndex] = structure;
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views
      };
    });
  }

  removeLabel = (viewId, labelId) => {
    if (this.state.selectedObjectType === "label" && this.state.selectedObjectId === labelId) {
      this.setState({selectedObjectType: "none", selectedObjectId: ""});
      this.setHintText("");
    }

    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        return;
      }


      let view = views[viewIndex];

      let labels = [...view.labels];
      const labelIndex = labels.findIndex(l => l.id === labelId);
      if (labelIndex < 0) {
        return;
      }

      labels.splice(labelIndex, 1);

      view.labels = labels;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  selectObject = (type, id, structureId) => {
    if (type === "structure") {
      this.hub.invoke("SelectObject", type, id).catch(err => console.error(err));
      this.setState({
        selectedStructure: this.getStructure(this.state.currentView, id),
        selectedObjectId: id
      });

    } else if (type === "fixture") {
      this.hub.invoke("SelectObject", type, id).catch(err => console.error(err));
      this.setState({
        selectedFixture: this.getFixture(this.state.currentView, structureId, id),
        selectedObjectId: id
      });
    } else if (type === "label") {
      this.hub.invoke("SelectObject", type, id).catch(err => console.error(err));
      this.setState({
        selectedLabel: this.getLabel(this.state.currentView, id),
        selectedObjectId: id
      });
    }

    this.setState({
      selectedObjectType: type,
      modifiedCurrent: false
    });

  }

  deselectObject = (structureId) => {
    if (this.state.selectedObjectType === "structure") {
      this.setStructureColour(this.state.currentView, this.state.selectedObjectId, "#000");

      this.hub.invoke("DeselectObject", "structure", this.state.selectedObjectId).catch(err => console.error(err));
    } else if (this.state.selectedObjectType === "fixture") {
      this.setFixtureColour(this.state.currentView, structureId, this.state.selectedObjectId);

      this.hub.invoke("DeselectObject", "fixture", this.state.selectedObjectId).catch(err => console.error(err));
    } else if (this.state.selectedObjectType === "label") {
      this.setLabelColour(this.state.currentView, this.state.selectedObjectId, "#fff");

      this.hub.invoke("DeselectObject", "label", this.state.selectedObjectId).catch(err => console.error(err));
    }

    this.setState({
      selectedObjectId: "",
      selectedObjectType: "none",
      modifiedCurrent: false
    })
  }

  onObjectUpdate = (type, field, viewId, structure, fixture, label) => {
    if (type === "structure") {
      this.setStructureField(viewId, field, structure);
    } else if (type === "fixture") {
      this.setFixtureField(viewId, structure.id, field, fixture);
    } else if (type === "label") {
      this.setLabelField(viewId, field, label);
    }
  }

  insertNewView = (view) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      views.push(view);

      return{
        views: views
      };
    })
  }

  onDeleteView = (view) => {
    if (view === this.state.currentView) {
      this.setAlertIcon("warning", "The view you were working on was deleted.", "exclamation")
    }

    this.deleteView(view);
  }

  deleteView = (view) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      let currentView = prevState.currentView;
      const removedIndex = views.findIndex(v => v.id === view);

      if (removedIndex > -1) {
        views.splice(removedIndex, 1);
      }

      if (view === currentView) {
        currentView = views[0].id;
      }

      return {
        views: views,
        currentView: currentView
      }
    }, () => {
      this.scaleStage();
    });
  }

  switchView = (id) => {
    this.deselectObject();
    this.setState({
      currentView: id
    }, this.scaleStage);
  }

  insertNewFixture = (viewId, fixture) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);

      if (viewIndex < 0) {
        return;
      }

      let view = views[viewIndex];
      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === fixture.structure.id);

      if (structureIndex < 0) {
        return;
      }

      let structure = structures[structureIndex];
      structure.fixtures.push(fixture);
      structures[structureIndex] = structure;

      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  insertNewLabel = (viewId, label) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);

      if (viewIndex < 0) {
        return;
      }
      views[viewIndex].labels.push(label);

      return {
        views: views
      };
    });
  }

  handleToolSelect = (tool) => {
    if (this.state.selectedTool === tool) {
      if (this.state.isDrawing === true) {
        this.handleStageDblClick();
      }

      this.setState({selectedTool: "none", stageCursor: "grab", tooltipVisible: false});
    } else if (tool === "polygon") {
      this.setState({selectedTool: tool, stageCursor: "crosshair", hintText: "Click to create a point.\nDouble click to create final point.\nPress escape to cancel."});
    } else if (tool === "add-fixture") {
      this.setState({selectedTool: tool, hintText: "Click to place fixture on structure"});
    } else if (tool === "add-label") {
      this.setState({selectedTool: tool, stageCursor: "text", hintText: "Click to place label"});
    } else if (tool === "eraser") {
      this.setState({selectedTool: tool, stageCursor: "copy", hintText: "Click an object to remove."});
    }
  }

  onToolButtonLeave = () => {
    if (this.state.selectedTool === "none") {
      this.setHintText("");
    }
  }

  userJoin = (user) => {
    this.setState(prevState => {
      let users = [...prevState.connectedUsers];

      const colour = this.randomMc.getColor({text: user.id});
      const rgb = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(colour);

      user.colour = colour;
      user.lightText = false;
      if (parseInt(rgb[1], 16) * 0.299 + parseInt(rgb[2], 16) * 0.587 + parseInt(rgb[3], 16) * 0.114 > 186) {
        user.lightText = true;
      }

      users.push(user);

      return {connectedUsers: users};
    })
  }

  userLeave = (user) => {
    this.setState(prevState => {
      const removedIndex = prevState.connectedUsers.findIndex(u => u.id === user);

      let users = [...prevState.connectedUsers];
      if (removedIndex > -1) {
        users.splice(removedIndex, 1);
      }

      return {
        connectedUsers: users
      };
    })
  }

  sizeStage = (callback) => {
    // set the stage size to be very small initially to prevent it from taking all the width before the other columns have resized
    this.setState({
      stageWidth: 0,
      stageHeight: 0
    }, () => {
      const container = document.getElementById("stage-container");
      this.setState({
        stageWidth: container.offsetWidth,
        stageHeight: container.offsetHeight,
        stageX: container.offsetWidth / this.state.stageScale,
        stageY: container.offsetHeight / this.state.stageScale,
      }, () => {
        if (callback && typeof callback === "function") {
          callback();
        }
      });
    })
  }

  scaleStage = () => {
    // set the scale such that the entire view can be seen
    this.setState({
      stageScale: Math.min(
        this.state.stageWidth / (this.getCurrentView().width * 1.05),
        this.state.stageHeight / (this.getCurrentView().height * 1.05)
      )
    }, () => {
      // center the drawing
      this.setState({
        stagePosition: {
          x: (this.state.stageWidth - this.getCurrentView().width * this.state.stageScale) / 2,
          y: (this.state.stageHeight + this.getCurrentView().height * this.state.stageScale) / 2 - this.state.stageY * this.state.stageScale
        }
      });
    });
  }

  setConnectedUsers = (users) => {
    users.forEach(user => this.userJoin(user));
  }

  setScale = (scale) => {
    this.setState({stageScale: scale});
  }

  setPosition = (position) => {
    this.setState({stagePosition: position});
  }

  setTool = (tool) => {
    this.setState({selectedTool: tool});
  }

  setIsDrawing = (isDrawing) => {
    this.setState({isDrawing: isDrawing});
  }

  setCursor = (cursor) => {
    this.setState({stageCursor: cursor});
  }

  setStructureColour = (viewId, structureId, colour) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setStructureColour error: view ID "${viewId}" not found`);
        return;
      }

      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structureId);
      if (structureIndex < 0) {
        console.error(`setStructureColour error: structure ID "${structureId}" not found`);
        return;
      }

      structures[structureIndex].colour = colour;
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  setStructureField = (viewId, field, structure) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setStructureField error: view ID "${viewId}" not found`);
        return;
      }
      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structure.id);
      if (structureIndex < 0) {
        console.error(`setStructureField error: structure ID "${structure.id}" not found`);
        return;
      }

      structures[structureIndex][field] = structure[field];
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views,
      };
    }, () => {
      if (this.state.selectedObjectType === "structure" && this.state.selectedObjectId === structure.id) {
        this.setState({
          selectedStructure: this.getStructure(viewId, structure.id)
        });
      }
    });
  }

  setFixtureField = (viewId, structureId, field, fixture) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setFixtureField error: view ID "${viewId}" not found`);
        return;
      }
      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structureId);
      if (structureIndex < 0) {
        console.error(`setFixtureField error: structure ID "${structureId}" not found`);
        return;
      }
      let structure = structures[structureIndex];

      let fixtures = [...structure.fixtures];
      const fixtureIndex = fixtures.findIndex(f => f.id === fixture.id);
      if (fixtureIndex < 0) {
        console.error(`setFixtureField error: fixture ID "${fixture.id}" not found`);
        return;
      }

      fixtures[fixtureIndex][field] = fixture[field];

      structures[structureIndex].fixtures = fixtures;
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views,
      };
    }, () => {
      if (this.state.selectedObjectType === "fixture" && this.state.selectedObjectId === fixture.id) {
        this.setState({
          selectedFixture: this.getFixture(viewId, structureId, fixture.id)
        });
      }
    });
  }

  setFixtureColour = (viewId, structureId, fixtureId, colour) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setFixtureColour error: view ID "${viewId}" not found`);
        return;
      }
      let view = views[viewIndex];

      let structures = [...view.structures];
      const structureIndex = structures.findIndex(s => s.id === structureId);
      if (structureIndex < 0) {
        console.error(`setFixtureColour error: structure ID "${structureId}" not found`);
        return;
      }
      let structure = structures[structureIndex];

      let fixtures = [...structure.fixtures];
      const fixtureIndex = fixtures.findIndex(f => f.id === fixtureId);
      if (fixtureIndex < 0) {
        console.error(`setFixtureColour error: fixture ID "${fixtureId}" not found`);
        return;
      }
      fixtures[fixtureIndex].highlightColour = colour;

      structure.fixtures = fixtures;
      structures[structureIndex] = structure;
      view.structures = structures;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  setLabelField = (viewId, field, label) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setLabelField error: view ID "${viewId}" not found`);
        return;
      }
      let view = views[viewIndex];

      let labels = [...view.labels];
      const labelIndex = labels.findIndex(l => l.id === label.id);
      if (labelIndex < 0) {
        console.error(`setLabelField error: label ID "${label.id}" not found`);
        return;
      }

      labels[labelIndex][field] = label[field];
      view.labels = labels;
      views[viewIndex] = view;

      return {
        views: views,
      };
    }, () => {
      if (this.state.selectedObjectType === "label" && this.state.selectedObjectId === label.id) {
        this.setState({
          selectedStructure: this.getStructure(viewId, label.id)
        });
      }
    });
  }

  setLabelColour = (viewId, labelId, colour) => {
    this.setState(prevState => {
      let views = [...prevState.views];
      const viewIndex = views.findIndex(v => v.id === viewId);
      if (viewIndex < 0) {
        console.error(`setLabelColour error: view ID "${viewId}" not found`);
        return;
      }

      let view = views[viewIndex];

      let labels = [...view.labels];
      const labelIndex = labels.findIndex(s => s.id === labelId);
      if (labelIndex < 0) {
        console.error(`setLabelColour error: label ID "${labelId}" not found`);
        return;
      }

      labels[labelIndex].colour = colour;
      view.labels = labels;
      views[viewIndex] = view;

      return {
        views: views
      };
    })
  }

  setModifiedCurrent = (value) => {
    this.setState({modifiedCurrent: value});
  }

  getUserColour = (userId) => {
    return this.state.connectedUsers.find(u => u.id === userId).colour;
  }

  setAlertError = (msg) => {
    this.setState({
      alertColour: "danger",
      alertContent: `Error: ${msg}`,
      alertIcon: <FontAwesomeIcon icon="exclamation" className="h5 m-0"/>,
      alertOpen: "true"
    })
  }

  setAlertIcon = (colour, msg, icon) => {
    this.setState({
      alertColour: colour,
      alertContent: msg,
      alertIcon: <FontAwesomeIcon icon={icon} className="h5 m-0"/>,
      alertOpen: "true"
    });
  }

  setAlertSpinner = (colour, msg) => {
    this.setState({
      alertColour: colour,
      alertContent: msg,
      alertIcon: <Spinner style={{width: "1.5em", height: "1.5em"}}/>,
      alertOpen: "true"
    });
  }

  setHintText = (msg) => {
    if (msg === "") {
      this.setState({hintText: "Use the buttons on the left to select a tool"})
    } else {
      this.setState({hintText: msg});
    }
  }

  setTooltipVisible = (visible) => {
    this.setState({tooltipVisible: visible});
  }

  toggleGrid = () => {
    this.setState({gridEnabled: !this.state.gridEnabled});
  }

  setGridSize = (size) => {
    this.setState({gridSize: size});
  }

  toggleAlert = () => {
    this.setState({alertOpen: !this.state.alertOpen});
  }

  getCurrentView = () => {
    return this.state.views.find(v => v.id === this.state.currentView);
  }

  getStructure = (viewId, structureId) => {
    const view = this.state.views.find(v => v.id === viewId);

    return view.structures.find(s => s.id === structureId);
  }

  getFixture = (viewId, structureId, fixtureId) => {
    const structure = this.getStructure(viewId, structureId);

    return structure.fixtures.find(f => f.id === fixtureId);
  }

  getLabel = (viewId, labelId) => {
    const view = this.state.views.find(v => v.id === viewId);

    return view.labels.find(l => l.id === labelId);
  }

  handleKeyDown = (event) => {
    if (event.keyCode === 90 && event.ctrlKey && !event.shiftKey) { // ctrl+z
      if (this.history.length === 0) {
        return;
      }
      const op = this.history.pop();
      this.undoOperation(op, false);
    } else if (event.keyCode === 90 && event.ctrlKey && event.shiftKey) { // ctrl+shift+z
      if (this.undoHistory.length === 0) {
        return;
      }
      const op = this.undoHistory.pop();
      this.undoOperation(op, true);
    }
  }

  undoOperation = async (op, redo) => {
    let result, op2;
    // perform inverse for each operation, and add inverse operation to undo history stack
    switch (op.type) {
      case Ops.ADD_FIXTURE:
        result = {success: false}
        result = await this.hub.invoke(
          "DeleteObject",
          "fixture",
          op.data.id
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          this.onRemoveObject(result.data.type, result.data.id, result.data.viewId, result.data.structureId, true);
          op2 = {
            type: Ops.REMOVE_FIXTURE,
            data: op.data
          }
        } else {
          this.setAlertError("Failed to undo fixture add");
        }
        break;
      case Ops.MOVE_FIXTURE:
        result = await this.hub.invoke(
          "UpdateFixturePosition",
            {
              id: op.data.id,
              position: op.data.prevValue
            }
        ).catch(err => {console.error(err); result = false});

        if (result) {
          const temp = op.data.prevValue;
          op.data.prevValue = op.data.newValue;
          op.data.newValue = temp;
          op2 = {
            type: Ops.MOVE_FIXTURE,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo fixture move");
        }
        break;
      case Ops.REMOVE_FIXTURE:
        result = {success: false};
        result = await this.hub.invoke(
          "AddFixture",
          op.data
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          op2 = {
            type: Ops.ADD_FIXTURE,
            data: result.data
          }
        } else {
          this.setAlertError("Failed to undo fixture delete");
        }
        break;

      case Ops.ADD_LABEL:
        result = {success: false}
        result = await this.hub.invoke(
          "DeleteObject",
          "label",
          op.data.id
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          this.onRemoveObject(result.data.type, result.data.id, result.data.viewId, result.data.structureId, true);
          op2 = {
            type: Ops.REMOVE_LABEL,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo label add");
        }
        break;
      case Ops.MOVE_LABEL:
        result = await this.hub.invoke(
          "UpdateLabelPosition",
            {
              id: op.data.id,
              position: op.data.prevValue
            }
        ).catch(err => {console.error(err); result = false});

        if (result) {
          const temp = op.data.prevValue;
          op.data.prevValue = op.data.newValue;
          op.data.newValue = temp;
          op2 = {
            type: Ops.MOVE_LABEL,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo label move");
        }
        break;
      case Ops.REMOVE_LABEL:
        result = {success: false}
        result = await this.hub.invoke(
          "AddLabel",
          op.data
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          op2 = {
            type: Ops.ADD_LABEL,
            data: result.data
          };
        } else {
          this.setAlertError("Failed to undo label remove");
        }
        break;

      case Ops.ADD_STRUCTURE:
        result = {success: false}
        result = await this.hub.invoke(
          "DeleteObject",
          "structure",
          op.data.id
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          this.onRemoveObject(result.data.type, result.data.id, result.data.viewId, result.data.structureId, true);
          op2 = {
            type: Ops.REMOVE_STRUCTURE,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo structure add");
        }
        break;
      case Ops.MOVE_STRUCTURE:
        result = await this.hub.invoke(
          "UpdateStructureGeometry",
            op.data.id,
            {points: op.data.prevValue.points},
            op.data.prevValue.fixtures
        ).catch(err => {console.error(err); result = false});

        if (result) {
          const temp = op.data.prevValue;
          op.data.prevValue = op.data.newValue;
          op.data.newValue = temp;
          op2 = {
            type: Ops.MOVE_STRUCTURE,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo structure move");
        }
        break;
      case Ops.REMOVE_STRUCTURE:
        result = {success: false}
        result = await this.hub.invoke(
          "AddStructure",
          op.data
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          op2 = {
            type: Ops.ADD_STRUCTURE,
            data: result.data
          };
        } else {
          this.setAlertError("Failed to undo structure remove");
        }
        break;

      case Ops.UPDATE_PROPERTY:
        let structureData, fixtureData, labelData = null;
        if (op.data.prevValue === null) {
          op.data.prevValue = "";
        }

        if (op.data.type === "structure") {
          structureData = {
            id: op.data.id,
            [op.data.field]: op.data.prevValue
          };

          if (op.data.field === "type") {
            structureData = {
              id: op.data.id,
              rating: -1,
              [op.data.field]: {id: op.data.prevValue}
            };
          }
        } else if (op.data.type === "fixture") {
          fixtureData = {
            id: op.data.id,
            angle: -1,
            channel: -1,
            universe: -1,
            address: -1,
            [op.data.field]: op.data.prevValue
          }

          if (op.data.field === "mode") {
            fixtureData = {
              id: op.data.id,
              [op.data.field]: {id: op.data.prevValue}
            }
          }
        } else if (op.data.type === "label") {
          labelData = {
            id: op.data.id,
            [op.data.field]: op.data.prevValue
          };
        }

        result = await this.hub.invoke(
          "UpdateObjectProperty",
          op.data.type,
          op.data.field,
          structureData,
          fixtureData,
          labelData
        ).catch(err => {console.error(err); result = false});

        if (result) {
          const temp = op.data.prevValue;
          op.data.prevValue = op.data.newValue;
          op.data.newValue = temp;
          op2 = {
            type: Ops.UPDATE_PROPERTY,
            data: op.data
          };
        } else {
          this.setAlertError("Failed to undo property change");
        }
        break;
      default:
        return;
    }

    if (op2) {
      // push reverse operation to correct stack whether currently undoing or redoing
      if (!redo) {
        this.undoHistory.push(op2);
      } else {
        this.history.push(op2)
      }
    }
  }

  pushHistoryOp = (op) => {
    this.history.push(op);
    this.undoHistory = [];
  }
}