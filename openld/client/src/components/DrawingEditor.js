import React, { Component } from 'react';
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
import authService from './api-authorization/AuthorizeService';

export class DrawingEditor extends Component {
  constructor(props) {
    super(props);

    this.state = {
      loading: true,
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
      modifiedCurrent: false,

      drawingData: {},
      currentView: "",
      views: [],
      connectedUsers: [],

      hub: null,
      hubConnected: false,

      alertIcon: "",
      alertContent: "",
      alertColour: "info",
      alertOpen: false,

      hintText: "Use the buttons on the left to select a tool",

      randomMc: require("random-material-color")
    }
  }

  componentDidMount = () => {
    this.fetchDrawing();
  }

  componentWillUnmount = () => {
    window.removeEventListener("resize", this.sizeStage);
    this.state.hub.stop();
  }

  render = () => {
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
    } else if (this.state.loading === true) {
      return (
      <Container className="h-100">
        <Col className="h-100">
          <Row className="align-items-center justify-content-center h-100">
            <Spinner style={{width: "10rem", height: "10rem"}}/>
          </Row>
        </Col>
      </Container>
      );
    }

    return (
      <div className="d-flex flex-column h-100">
        <Navbar color="dark">
          <NavbarBrand className="text-light">{this.state.drawingData.title}</NavbarBrand>
          <div className="d-flex">
            {this.state.connectedUsers.map(u => {
              return (
                <div
                  key={"userCircle-" + u.id}
                  className={"rounded-circle font-weight-bold d-flex align-items-center justify-content-center ml-2 " + (u.lightText ? "text-dark" : "text-light")}
                  style={{width: "2.5em", height: "2.5em", backgroundColor: u.colour}}
                >
                  <div id={"userCircle-" + u.id} style={{fontSize: "130%"}}>
                    {u.userName.charAt(0).toUpperCase()}
                  </div>
                  <UncontrolledTooltip placement="bottom" target={"userCircle-" + u.id} style={{zIndex: 1000}}>{u.userName}</UncontrolledTooltip>
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
                hub = {this.state.hub}
                hubConnected = {this.state.hubConnected}
                tooltipVisible = {this.state.tooltipVisible}

                onMoveStructure = {this.moveStructure}
                onSelectObject = {this.selectObject}
                deselectObject = {this.deselectObject}

                gridEnabled = {this.state.gridEnabled}
                gridSize = {this.state.gridSize}
                snapGridSize = {this.state.snapGridSize}

                setScale = {this.setScale}
                setTool = {this.setTool}
                setIsDrawing = {this.setIsDrawing}
                setCursor = {this.setCursor}
                setStructureColour = {this.setStructureColour}
                setFixtureColour = {this.setFixtureColour}
                setHintText = {this.setHintText}
                setTooltipVisible = {this.setTooltipVisible}
              />
            </Col>
            <Sidebar
              drawingId = {this.state.drawingData.id}

              height = {this.state.stageHeight}
              width = "22em"

              views = {this.state.views}
              currentView = {this.state.currentView}
              hub = {this.state.hub}
              hubConnected = {this.state.hubConnected}
              gridEnabled = {this.state.gridEnabled}
              gridSize = {this.state.gridSize}
              structure = {this.state.selectedStructure}
              fixture = {this.state.selectedFixture}
              selectedObjectId = {this.state.selectedObjectId}
              selectedObjectType = {this.state.selectedObjectType}
              modifiedCurrent = {this.state.modifiedCurrent}
              hintText = {this.state.hintText}

              onClickView = {this.switchView}
              toggleGrid = {this.toggleGrid}
              setGridSize = {this.setGridSize}
              getStructure = {this.getStructure}
              setModifiedCurrent = {this.setModifiedCurrent}
            />
          </Row>
        </Container>
      </div>
    );
  }

  initHubConnection = async () => {
    let token = await authService.getAccessToken();

    this.setState({
      hub: new HubConnectionBuilder()
        .withUrl("/api/drawing/hub", { accessTokenFactory: () => token })
        .withAutomaticReconnect([0, 1000, 1000, 2000, 2000, 5000, 5000, 5000, 10000, 10000, 20000])
        .build()
    }, () => {
      this.state.hub
        .start()
        .then(() => {
          this.addHubHandlers();
          this.setState({hubConnected: true});
          this.state.hub.invoke("OpenDrawing", this.state.drawingData.id)
            .catch(err => console.error(err.toString()));
        })
        .catch(err => console.error("Hub error: " + err));

      this.state.hub.onreconnecting(this.onHubReconnecting);
      this.state.hub.onreconnected(this.onHubReconnect);
      this.state.hub.onclose(this.onHubDisconnect)
    });
  }

  addHubHandlers = () => {
    this.state.hub.on("ConnectedUsers", users => this.setConnectedUsers(users));
    this.state.hub.on("UserJoined", user => this.userJoin(user));
    this.state.hub.on("UserLeft", user => this.userLeave(user));

    this.state.hub.on("NewView", view => this.insertNewView(view));
    this.state.hub.on("CreateViewSuccess", view => this.insertNewView(view));
    this.state.hub.on("CreateViewFailure", () => this.setAlertError("Failed to create new view"));
    this.state.hub.on("DeleteView", view => this.onDeleteView(view));
    this.state.hub.on("DeleteViewSuccess", view => this.deleteView(view));
    this.state.hub.on("DeleteViewFailure", () => this.setAlertError("Failed to delete view"));

    this.state.hub.on("SelectObject", (type, viewId, structureId, fixtureId, userId) => this.userSelectObject(type, viewId, structureId, fixtureId, userId));
    this.state.hub.on("DeselectObject", (type, viewId, structureId, fixtureId) => this.userDeselectObject(type, viewId, structureId, fixtureId));

    this.state.hub.on("NewStructure", structure => this.insertNewStructure(structure));
    this.state.hub.on("AddStructureSuccess", structure => this.setState({newLinePoints: []}));
    this.state.hub.on("AddStructureFailure", () => this.setAlertError("Failed to insert new structure"));

    this.state.hub.on("UpdateStructureGeometry", (structure, fixtures) => this.updateStructurePos(structure.view.id, structure.id, structure.geometry.points, fixtures));
    this.state.hub.on("UpdateStructureGeometryFailure", () => this.setAlertError("Failed to move structure"));
    this.state.hub.on("UpdateObjectProperty", (type, field, viewId, structure, fixture) => this.onObjectUpdate(type, field, viewId, structure, fixture));
    this.state.hub.on("UpdateObjectPropertySuccess", () => this.setModifiedCurrent(false));
    this.state.hub.on("DeleteObject", (type, viewId, structureId, fixtureId) => this.onObjectRemoved(type, viewId, structureId, fixtureId));
    this.state.hub.on("DeleteObjectSuccess", (type, viewId, structureId, fixtureId) => this.removeObject(type, viewId, structureId, fixtureId));
    this.state.hub.on("DeleteObjectFailure", (type) => this.setAlertError(`Failed to delete ${type}`));

    this.state.hub.on("AddFixture", (viewId, fixture) => this.insertNewFixture(viewId, fixture));
  }

  onHubDisconnect = () => {
    this.setAlertIcon("danger", "Lost connection to OpenLD. Please reconnect to make changes.", "exclamation");
    this.setState({hubConnected: false});
  }

  onHubReconnecting = () => {
    this.setAlertSpinner("info", "Reconnecting to OpenLD..");
    this.setState({hubConnected: false});
  }

  onHubReconnect = () => {
    this.setAlertIcon("success", "Successfully reconnected to OpenLD.", "check");
    window.setTimeout(() => this.setState({alertOpen: false}), 10000);
    this.setState({hubConnected: true});
    this.state.hub.invoke("OpenDrawing", this.state.drawingData.id)
      .catch(err => console.error(err.toString()));
  }

  fetchDrawing = async () => {
    const response = await fetch("api/drawing/GetDrawing/" + this.props.match.params.id, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();

      this.setState({
        drawingData: data.data,
        loading: false,
        currentView: data.data.views[0].id,
        views: data.data.views
      }, async () => {
        this.initHubConnection();
        this.sizeStage(this.scaleStage);
        window.addEventListener("resize", this.sizeStage);
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

  moveStructure = async (viewId, id, points, fixtures) => {
    this.state.hub.invoke(
      "UpdateStructureGeometry",
        id,
        {points: points},
        fixtures
    ).catch(err => console.error(err))
    .then(() => this.updateStructurePos(viewId, id, points, fixtures));
  }

  updateStructurePos = (viewId, id, points, fixtures) => {
    if (viewId === null) {
      viewId = this.state.currentView;
    }

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

  userSelectObject = (type, viewId, structureId, fixtureId, userId) => {
    if (type === "structure") {
      this.setStructureColour(viewId, structureId, this.getUserColour(userId));
    } else if (type === "fixture") {
      this.setFixtureColour(viewId, structureId, fixtureId, this.getUserColour(userId));
    }
  }

  userDeselectObject = (type, viewId, structureId, fixtureId) => {
    if (type === "structure") {
      if (this.state.selectedObjectType === "structure" && this.state.selectedObjectId === structureId) {
        this.setStructureColour(viewId, structureId, "#007bff");
      } else {
        this.setStructureColour(viewId, structureId, "#000");
      }
    } else if (type === "fixture") {
      if (this.state.selectedObjectType === "fixture" && this.state.selectedObjectId === fixtureId) {
        this.setFixtureColour(viewId, structureId, fixtureId, "#007bff");
      } else {
        this.setFixtureColour(viewId, structureId, fixtureId, "#000");
      }
    }
  }

  onObjectRemoved = (type, viewId, structureId, fixtureId) => {
    if (type === "structure") {
      if (this.state.selectedObjectType === type && this.state.selectedObjectId === structureId) {
        this.setAlertIcon("warning", "The structure you were working on was deleted.", "exclamation");
      }
      this.removeStructure(viewId, structureId);
    } else if (type === "fixture") {
      if (this.state.selectedObjectType === type && this.state.selectedObjectId === fixtureId) {
        this.setAlertIcon("warning", "The fixture you were working on was deleted.", "exclamation");
      }
      this.removeFixture(viewId, structureId, fixtureId);
    }
  }

  removeObject = (type, viewId, structureId, fixtureId) => {
    if (type === "structure") {
      this.removeStructure(viewId, structureId);
    } else if (type === "fixture") {
      this.removeFixture(viewId, structureId, fixtureId);
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

  selectObject = (type, structureId, fixtureId) => {
    if (type === "structure") {
      this.state.hub.invoke("SelectObject", type, structureId).catch(err => console.error(err));
      this.setState({
        selectedStructure: this.getStructure(this.state.currentView, structureId),
        selectedObjectId: structureId
      });

    } else if (type === "fixture") {
      this.state.hub.invoke("SelectObject", type, fixtureId).catch(err => console.error(err));
      this.setState({
        selectedFixture: this.getFixture(this.state.currentView, structureId, fixtureId),
        selectedObjectId: fixtureId
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

      this.state.hub.invoke("DeselectObject", "structure", this.state.selectedObjectId).catch(err => console.error(err));
    } else if (this.state.selectedObjectType === "fixture") {
      this.setFixtureColour(this.state.currentView, structureId, this.state.selectedObjectId);

      this.state.hub.invoke("DeselectObject", "fixture", this.state.selectedObjectId).catch(err => console.error(err));
    }

    this.setState({
      selectedObjectId: "",
      selectedObjectType: "none",
      modifiedCurrent: false
    })
  }

  onObjectUpdate = (type, field, viewId, structure, fixture) => {
    if (type === "structure") {
      this.setStructureField(viewId, field, structure);
    } else if (type === "fixture") {
      this.setFixtureField(viewId, structure.id, field, fixture);
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

      const colour = this.state.randomMc.getColor({text: user.id});
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

  setModifiedCurrent = (value) => {
    this.setState({modifiedCurrent: value});
  }

  getUserColour = (userId) => {
    return this.state.connectedUsers.find(u => u.id === userId).colour;
  }

  setAlertError = (msg) => {
    this.setState({
      alertColour: "danger",
      alertContent: "Error: " + msg,
      alertIcon: "exclamation",
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
}