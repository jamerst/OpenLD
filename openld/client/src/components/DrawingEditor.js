import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import {
  Alert,
  Button, ButtonGroup,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink,
  Spinner } from 'reactstrap';
import { Layer, Line, Stage } from 'react-konva';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { HubConnectionBuilder } from "@aspnet/signalr";

import { DrawingUtils } from './drawing/DrawingUtils';
import { View, Grid, Tooltip } from "./drawing/DrawingComponents";
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
      snapGridSize: 0.1,
      stageScale: 50,
      stageWidth: 0,
      stageHeight: 0,
      stageX: 0,
      stageY: 0,
      stagePosition: {x: 0, y: 0},
      tooltipPos: {x: 0, y: 0},
      tooltipVisible: false,
      tooltipText: "",
      selectedTool: "none",
      isDrawing: false,
      newLinePoints: [0, 0],
      newLinePos: {x: 0, y: 0},
      lastLinePoint: [],
      nextLinePoint: [],
      drawingData: {},
      currentView: "",
      views: [],
      hub: null,
      alertContent: "",
      alertColour: "info",
      alertOpen: false,
      stageCursor: "grab"
    }

    this.initHubConnection = this.initHubConnection.bind(this);
    this.fetchDrawing = this.fetchDrawing.bind(this);
    this.sizeStage = this.sizeStage.bind(this);
    this.handleToolSelect = this.handleToolSelect.bind(this);
    this.handleMouseMove = this.handleMouseMove.bind(this);
    this.handleStageClick = this.handleStageClick.bind(this);
    this.handleStageDblClick = this.handleStageDblClick.bind(this);
    this.addStructure = this.addStructure.bind(this);
    this.insertNewStructure = this.insertNewStructure.bind(this);
    this.addStructureSuccess = this.addStructureSuccess.bind(this);
    this.addHubHandlers = this.addHubHandlers.bind(this);
    this.handleCreateView = this.handleCreateView.bind(this);
    this.insertNewView = this.insertNewView.bind(this);
    this.setAlertError = this.setAlertError.bind(this);
    this.switchView = this.switchView.bind(this);
    this.setTooltip = this.setTooltip.bind(this);
    this.updateStructurePoints = this.updateStructurePoints.bind(this);
    this.modifyStructurePoints = this.modifyStructurePoints.bind(this);
    this.setStageCursor = this.setStageCursor.bind(this);
    this.zoom = this.zoom.bind(this);
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
                <Button tool="polygon" outline color="primary" size="lg" onClick={this.handleToolSelect} active={this.state.selectedTool === "polygon"}>
                  <FontAwesomeIcon icon="draw-polygon" />
                </Button>

                <Button tool="eraser" outline color="danger" size="lg" onClick={this.handleToolSelect} active={this.state.selectedTool === "eraser"}>
                  <FontAwesomeIcon icon="eraser" />
                </Button>
              </ButtonGroup>
            </Col>
            <Col id="stage-container" className="p-0 m-0 bg-secondary">
              <div style={{position: "absolute", width: "100%", zIndex: "1000"}}>
                <Alert color={this.state.alertColour} isOpen={this.state.alertOpen}>{this.state.alertContent}</Alert>
              </div>
              <Stage
                x = {0}
                y = {0}
                width = {this.state.stageWidth}
                height = {this.state.stageHeight}
                scale = {{x: this.state.stageScale, y: -this.state.stageScale}}
                offsetY = {this.state.stageY}
                draggable
                position = {this.state.stagePosition}

                onWheel = {this.zoom}
                onMouseMove = {this.handleMouseMove}
                onClick = {this.handleStageClick}
                onDblClick = {this.handleStageDblClick}
                onDragStart = {() => this.setStageCursor("grabbing")}
                onDragEnd = {() => this.setStageCursor("grab")}

                style={{cursor: this.state.stageCursor}}
              >
                <Grid
                  enabled = {this.state.gridEnabled}
                  xLim = {this.state.drawingData.width}
                  yLim = {this.state.drawingData.height}
                  gridSize = {1}
                  lineWidth = {1 / this.state.stageScale}
                />
                <Layer>
                    <Line
                      key = "new-line"
                      points = {this.state.newLinePoints}
                      position = {this.state.newLinePos}
                      stroke = "#000"
                      strokeWidth = {0.05}
                    />
                    <Line
                      key = "line-preview"
                      points = {[...this.state.lastLinePoint, ...this.state.nextLinePoint]}
                      stroke = "#ddd"
                      strokeWidth = {0.05}
                    />
                </Layer>
                <View
                    data={this.state.views.find(view => view.id === this.state.currentView)}
                    snapGridSize = {this.state.snapGridSize}
                    updatePoints = {this.modifyStructurePoints}
                    setTooltip = {this.setTooltip}
                    setCursor = {this.setStageCursor}
                    scale = {this.state.stageScale}
                />
                <Layer>
                  <Tooltip
                    position = {this.state.tooltipPos}
                    visible = {this.state.tooltipVisible}
                    text = {this.state.tooltipText}
                    scale = {1.25 / this.state.stageScale}
                  />
                </Layer>
              </Stage>
            </Col>
            <Sidebar
              xs = "4"
              md = "3"
              lg = "1"

              drawingId = {this.state.drawingData.id}

              height = {this.state.stageHeight}
              views = {this.state.views}
              currentView = {this.state.currentView}

              onCreateView = {this.handleCreateView}
              onClickView = {this.switchView}
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

    this.state.hub.on("UpdateStructureGeometry", structure => this.modifyStructurePoints(structure.view.id, structure.id, structure.geometry.points));
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
        this.sizeStage(true);
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

  async addStructure() {
    let points = DrawingUtils.arrayPointsToObject(this.state.newLinePoints);

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

  async handleCreateView() {
    let name = prompt("Enter view name");

    this.state.hub.invoke(
      "CreateView",
      {
        drawing: { id: this.props.match.params.id },
        name: name
      }
    ).catch(err => console.log(err));
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

  switchView(id) {
    this.setState({currentView: id});
  }

  async updateStructurePoints(id, points) {
    this.state.hub.invoke(
      "UpdateStructureGeometry",
        id,
        {points: points}

    ).catch(err => console.log(err));
  }

  modifyStructurePoints(viewId, id, points) {
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
    }, () => {
      this.updateStructurePoints(id, points);
    });
  }

  handleStageClick(event) {
    if (this.state.selectedTool === "polygon") {
      const stage = event.target.getStage();
      const point = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.state.snapGridSize);
      if (this.state.isDrawing === true) {
        this.setState({
          newLinePoints: [...this.state.newLinePoints, ...[point.x, point.y]],
          lastLinePoint: [point.x, point.y]
        });
      } else {
        this.setState({
          isDrawing: true,
          newLinePoints: [point.x, point.y],
          newLinePos: [point.x, point.y],
          lastLinePoint: [point.x, point.y]
        });
      }
    }
  }

  handleStageDblClick(event) {
    if (this.state.selectedTool === "polygon") {
      this.setState({
        isDrawing: false,
        selectedTool: "none",
        lastLinePoint: [],
        nextLinePoint: [],
        tooltipVisible: false,
        stageCursor: "grab"
      })
      this.addStructure();
    }
  }

  handleMouseMove(event) {
    if (this.state.selectedTool === "polygon") {
      const stage = event.target.getStage();
      const snapPos = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.state.snapGridSize);
      this.setState({
        tooltipPos: {x: snapPos.x - 0.5, y: snapPos.y - 0.5},
        tooltipText: "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")",
        tooltipVisible: true,
        nextLinePoint: [snapPos.x, snapPos.y]
      });
    }
  }

  handleToolSelect(event) {
    const tool = event.target.getAttribute("tool");
    if (this.state.selectedTool === tool) {
      this.setState({selectedTool: "none", stageCursor: "grab"});
    } else {
      this.setState({selectedTool: tool, stageCursor: "crosshair"});
    }
  }

  sizeStage(setScale) {
    // set the stage size to be very small initially to prevent it from taking all the width before the other columns have resized
    this.setState({
      stageWidth: 0,
      stageHeight: 0
    }, () => {
      const container = document.getElementById("stage-container");
      this.setState({
        stageWidth: container.clientWidth,
        stageHeight: container.clientHeight,
        stageX: container.clientWidth / this.state.stageScale,
        stageY: container.clientHeight / this.state.stageScale,
      }, () => {
        if (setScale === true) {
          this.setState({
            stageScale: Math.min(
              this.state.stageWidth / (this.state.drawingData.width * 1.05),
              this.state.stageHeight / (this.state.drawingData.height * 1.05)
            )
          }, () => {
            this.setState({
              stagePosition: {
                x: (this.state.stageWidth - this.state.drawingData.width * this.state.stageScale) / 2,
                y: ((this.state.stageHeight - this.state.drawingData.height * this.state.stageScale) / 2) + ((this.state.drawingData.height * this.state.stageScale) / 2)
              }
            });
          });
        }
      });
    })
  }

  setStageCursor(cursor) {
    this.setState({stageCursor: cursor});
  }

  zoom(event) {
    event.evt.preventDefault();

    const newScale = event.evt.deltaY < 0 ? this.state.stageScale * 1.25 : this.state.stageScale / 1.25;
    this.setState({stageScale: newScale});
  }

  setTooltip(pos, visible, text) {
    this.setState({
      tooltipPos: pos,
      tooltipVisible: visible,
      tooltipText: text
    });
  }

  setAlertError(msg) {
    this.setState({
      alertColour: "danger",
      alertContent: "Error: " + msg,
      alertOpen: "true"
    })
  }
}