import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import {
  Alert,
  Button, ButtonGroup,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink,
  Spinner } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { HubConnectionBuilder } from "@aspnet/signalr";

import { DrawingUtils } from './drawing/DrawingUtils';
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
      selectedTool: "none",
      isDrawing: false,
      drawingData: {},
      currentView: "",
      views: [],
      hub: null,
      alertContent: "",
      alertColour: "info",
      alertOpen: false,
      stageCursor: "grab",
      selectedObjectType: "none",
      selectedObjectId: ""
    }

    this.initHubConnection = this.initHubConnection.bind(this);
    this.fetchDrawing = this.fetchDrawing.bind(this);
    this.sizeStage = this.sizeStage.bind(this);
    this.scaleStage = this.scaleStage.bind(this);
    this.handleToolSelect = this.handleToolSelect.bind(this);


    this.addStructure = this.addStructure.bind(this);
    this.insertNewStructure = this.insertNewStructure.bind(this);
    this.addStructureSuccess = this.addStructureSuccess.bind(this);
    this.addHubHandlers = this.addHubHandlers.bind(this);
    this.insertNewView = this.insertNewView.bind(this);
    this.setAlertError = this.setAlertError.bind(this);
    this.setAlert = this.setAlert.bind(this);
    this.toggleAlert = this.toggleAlert.bind(this);
    this.switchView = this.switchView.bind(this);

    this.moveStructure = this.moveStructure.bind(this);
    this.updateStructurePos = this.updateStructurePos.bind(this);

    this.getCurrentView = this.getCurrentView.bind(this);
    this.toggleGrid = this.toggleGrid.bind(this);
    this.setGridSize = this.setGridSize.bind(this);
    this.setScale = this.setScale.bind(this);
    this.setTool = this.setTool.bind(this);
    this.setIsDrawing = this.setIsDrawing.bind(this);
    this.setCursor = this.setCursor.bind(this);
  }

  componentDidMount() {
    this.fetchDrawing();
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.sizeStage);
  }

  render() {
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
          <NavLink tag={Link} to="/"><Button className="text-light" close/></NavLink>
        </Navbar>
        <Container fluid className="pl-0 d-flex flex-grow-1">
          <Row className="d-flex flex-grow-1">
            <Col xs="auto" className="pr-0 bg-light">
              <ButtonGroup vertical>
                <Button outline color="primary" size="lg" className="rounded-0" onClick={() => this.handleToolSelect("polygon")} active={this.state.selectedTool === "polygon"}>
                  <FontAwesomeIcon icon="draw-polygon"/>
                </Button>

                <Button outline color="danger" size="lg" className="rounded-0" onClick={() => this.handleToolSelect("eraser")} active={this.state.selectedTool === "eraser"}>
                  <FontAwesomeIcon icon="eraser"/>
                </Button>
              </ButtonGroup>
            </Col>
            <Col id="stage-container" className="p-0 m-0 bg-secondary" xs="10">
              <div style={{position: "absolute", width: "100%", zIndex: "1000"}}>
                <Alert color={this.state.alertColour} isOpen={this.state.alertOpen} toggle={this.toggleAlert}>{this.state.alertContent}</Alert>
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

                onCreateStructure = {this.addStructure}
                onMoveStructure = {this.moveStructure}

                gridEnabled = {this.state.gridEnabled}
                gridSize = {this.state.gridSize}
                snapGridSize = {this.state.snapGridSize}

                setScale = {this.setScale}
                setTool = {this.setTool}
                setIsDrawing = {this.setIsDrawing}
                setCursor = {this.setCursor}
              />
            </Col>
            <Sidebar
              drawingId = {this.state.drawingData.id}

              height = {this.state.stageHeight}
              views = {this.state.views}
              currentView = {this.state.currentView}
              hub = {this.state.hub}
              gridEnabled = {this.state.gridEnabled}
              gridSize = {this.state.gridSize}

              onClickView = {this.switchView}
              toggleGrid = {this.toggleGrid}
              setGridSize = {this.setGridSize}
            />
          </Row>
        </Container>
      </div>
    );
  }

  async initHubConnection() {
    let token = await authService.getAccessToken();

    this.setState({
      hub: new HubConnectionBuilder()
        .withUrl("/api/drawing/hub", { accessTokenFactory: () => token })
        .build()
    }, () => {
      this.state.hub
        .start()
        .then(() => {
          this.addHubHandlers();
          this.state.hub.invoke("OpenDrawing", this.state.drawingData.id)
            .catch(err => console.error(err.toString()));
        })
        .catch(err => console.log("Hub error: " + err));
    });
  }

  addHubHandlers() {
    this.state.hub.on("NewStructure", structure => this.insertNewStructure(structure));
    this.state.hub.on("AddStructureSuccess", structure => this.addStructureSuccess(structure));
    this.state.hub.on("AddStructureFailure", () => this.setAlertError("Failed to insert new structure"));

    this.state.hub.on("NewView", view => this.insertNewView(view));
    this.state.hub.on("CreateViewSuccess", view => this.insertNewView(view));
    this.state.hub.on("CreateViewFailure", () => this.setAlertError("Failed to create new view"));
    this.state.hub.on("DeleteView", view => this.deleteView(view));
    this.state.hub.on("DeleteViewSuccess", view => this.deleteView(view));
    this.state.hub.on("DeleteViewFailure", () => this.setAlertError("Failed to delete view"));

    this.state.hub.on("UpdateStructureGeometry", structure => this.updateStructurePos(structure.view.id, structure.id, structure.geometry.points));
    this.state.hub.on("UpdateStructureGeometryFailure", () => this.setAlertError("Failed to move structure"));
  }

  async fetchDrawing() {
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

  async addStructure(addedPoints) {
    let points = DrawingUtils.arrayPointsToObject(addedPoints);

    this.state.hub.invoke(
      "AddStructure",
      {
        view: {id: this.state.currentView},
        geometry: {points: points}
      }
    ).catch(err => console.log(err));
  }

  insertNewStructure(structure) {
    this.setState((prevState) => {
      let views = [...prevState.views];
      const modifiedIndex = views.findIndex(view => view.id === structure.view.id);
      const modifiedView = views[modifiedIndex]

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

  addStructureSuccess(structure) {
    this.insertNewStructure(structure);
    this.setState({newLinePoints: []});
  }

  insertNewView(view) {
    this.setState(prevState => {
      let views = [...prevState.views];
      views.push(view);

      return{
        views: views
      };
    })
  }

  deleteView(view) {
    this.setState(prevState => {
      let views = [...prevState.views];
      let currentView = prevState.currentView;
      const removedIndex = views.findIndex(v => v.id === view);

      if (removedIndex > -1) {
        views.splice(removedIndex, 1);
      }

      if (view === this.state.currentView) {
        this.setAlert("warning", "The view you were working on was deleted.")
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

  switchView(id) {
    this.setState({
      currentView: id
    }, this.scaleStage);
  }

  async moveStructure(viewId, id, points) {
    this.state.hub.invoke(
      "UpdateStructureGeometry",
        id,
        {points: points}
    ).catch(err => console.log(err))
    .then(() => this.updateStructurePos(viewId, id, points));
  }

  updateStructurePos(viewId, id, points) {
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
        geometry: {points: points}
      };

      modifiedView.structures[structureIndex] = newStructure;

      views[viewIndex] = modifiedView;

      return {
        views: views
      };
    });
  }

  handleToolSelect(tool) {
    if (this.state.selectedTool === tool) {
      if (this.state.isDrawing === true) {
        this.handleStageDblClick();
      }

      this.setState({selectedTool: "none", stageCursor: "grab", tooltipVisible: false});
    } else if (tool === "polygon") {
      this.setState({selectedTool: tool, stageCursor: "crosshair"});
    }
  }

  sizeStage(callback) {
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

  scaleStage() {
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

  setScale(scale) {
    this.setState({stageScale: scale});
  }

  setTool(tool) {
    this.setState({selectedTool: tool});
  }

  setIsDrawing(isDrawing) {
    this.setState({isDrawing: isDrawing});
  }

  setCursor(cursor) {
    this.setState({stageCursor: cursor});
  }

  setAlertError(msg) {
    this.setState({
      alertColour: "danger",
      alertContent: "Error: " + msg,
      alertOpen: "true"
    })
  }

  setAlert(colour, msg) {
    this.setState({
      alertColour: colour,
      alertContent: msg,
      alertOpen: "true"
    })
  }

  toggleGrid() {
    this.setState({gridEnabled: !this.state.gridEnabled});
  }

  setGridSize(size) {
    this.setState({gridSize: size});
  }

  toggleAlert() {
    this.setState({alertOpen: !this.state.alertOpen});
  }

  getCurrentView() {
    return this.state.views.find(v => v.id === this.state.currentView);
  }
}