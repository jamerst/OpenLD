import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import {
  Alert,
  Button, ButtonGroup,
  Card, CardHeader, CardBody,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink,
  Spinner } from 'reactstrap';
import { Layer, Line, Stage } from 'react-konva';
import { Text } from './drawing/KonvaNodes';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { HubConnectionBuilder } from "@aspnet/signalr";

import { DrawingUtils } from './drawing/DrawingUtils';
import { View } from "./drawing/DrawingComponents";
import authService from './api-authorization/AuthorizeService';

export class Drawing extends Component {
  constructor(props) {
    super(props);
    this.state = {
      loading: true,
      grid: [],
      gridSize: 1,
      snapGridSize: 0.1,
      stageScale: 50,
      stageWidth: 0,
      stageHeight: 0,
      stageX: 0,
      stageY: 0,
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
      views: new Map(),
      hub: null,
      alertContent: "",
      alertColour: "info",
      alertOpen: false
    }

    this.fetchDrawing = this.fetchDrawing.bind(this);
    this.renderGrid = this.renderGrid.bind(this);
    this.sizeStage = this.sizeStage.bind(this);
    this.handleDrag = this.handleDrag.bind(this);
    this.handleDragEnd = this.handleDragEnd.bind(this);
    this.handleToolSelect = this.handleToolSelect.bind(this);
    this.handleMouseMove = this.handleMouseMove.bind(this);
    this.handleStageClick = this.handleStageClick.bind(this);
    this.handleStageDblClick = this.handleStageDblClick.bind(this);
    this.addStructure = this.addStructure.bind(this);
    this.insertNewStructure = this.insertNewStructure.bind(this);
    this.addStructureSuccess = this.addStructureSuccess.bind(this);
    this.addHubHandlers = this.addHubHandlers.bind(this);
    this.zoom = this.zoom.bind(this);
  }

  componentDidMount() {
    this.setState({
      hub: new HubConnectionBuilder()
        .withUrl("/api/drawing/hub")
        .build()
    }, () => {
      this.state.hub
        .start()
        .then(() => this.addHubHandlers())
        .catch(err => console.log("Hub error: " + err));
    });

    this.fetchDrawing();

    window.addEventListener("resize", this.sizeStage);
  }

  addHubHandlers() {
    this.state.hub.on("NewStructure", structure => this.insertNewStructure(structure));
    this.state.hub.on("AddStructureSuccess", structure => this.addStructureSuccess(structure));
    this.state.hub.on("AddStructureFailure", () => this.setState({ alertColour: "danger", alertContent: "Error: failed to add structure", alertOpen: "true" }));
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.sizeStage);
  }

  render() {
    if (this.state.loading === true) {
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
        <Navbar color="light">
          <NavbarBrand>{this.state.drawingData.title}</NavbarBrand>
          <NavLink tag={Link} to="/"><Button close/></NavLink>
        </Navbar>
        <Container fluid className="pl-0 d-flex flex-grow-1">
          <Row className="d-flex flex-grow-1">
            <Col xs="auto" className="pr-0">
              <ButtonGroup vertical>
                <Button tool="polygon" outline size="lg" onClick={this.handleToolSelect} active={this.state.selectedTool === "polygon"}>
                  <FontAwesomeIcon icon="draw-polygon" />
                </Button>

                <Button tool="eraser" outline size="lg" onClick={this.handleToolSelect} active={this.state.selectedTool === "eraser"}>
                  <FontAwesomeIcon icon="eraser" />
                </Button>
              </ButtonGroup>
            </Col>
            <Col id="stage-container" className="p-0 m-0 flex-grow-1">
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

                onWheel = {this.zoom}
                onMouseMove = {this.handleMouseMove}
                onClick = {this.handleStageClick}
                onDblClick = {this.handleStageDblClick}
              >
                <Layer>
                  {this.state.grid.map((line, index) => {
                    return (
                      <Line
                        key = {index}
                        points = {line}
                        stroke = "#ddd"
                        strokeWidth = {0.01}
                      />
                    );
                  })}
                </Layer>
                <Layer>
                  <Line
                    key = "scale"
                    points = {[
                      Math.floor(this.state.stageX) - 1, 1,
                      Math.floor(this.state.stageX), 1
                    ]}
                    stroke = "#000"
                    strokeWidth = {0.025}
                  />
                  <Text
                    key = "scaleLabel"
                    x = {(Math.floor(this.state.stageX) - 1)}
                    y = {1.25}
                    text = "1m"
                    textScale = {1 / this.state.stageScale}
                  />
                </Layer>
                <Layer>
                  <Text
                    key = "tooltip"
                    position = {this.state.tooltipPos}
                    visible = {this.state.tooltipVisible}
                    text = {this.state.tooltipText}
                    textScale = {1 / this.state.stageScale}
                  />
                </Layer>
                <Layer>
                    <Line
                      key = "new-line"
                      points = {this.state.newLinePoints}
                      position = {this.state.newLinePos}
                      stroke = "#000"
                      strokeWidth = {0.05}
                      draggable
                      onDragEnd = {this.handleDragEnd}
                      onDragMove = {this.handleDrag}
                    />
                    <Line
                      key = "line-preview"
                      points = {[...this.state.lastLinePoint, ...this.state.nextLinePoint]}
                      stroke = "#ddd"
                      strokeWidth = {0.05}
                    />
                </Layer>
                <View
                    data={this.state.views.get(this.state.currentView)}
                    onDragEnd = {this.handleDragEnd}
                    onDragMove = {this.handleDrag}
                />
              </Stage>
            </Col>
            <Col xs="1" className="p-0">
              <Card className="rounded-0">
                <CardHeader>Testing</CardHeader>
                <CardBody>Hello</CardBody>
              </Card>
            </Col>
          </Row>
        </Container>
      </div>
    );
  }

  async fetchDrawing() {
    const response = await fetch("api/drawing/GetDrawing/" + this.props.match.params.id, {
      headers: await authService.generateHeader()
    });
    if (response.ok) {
      const data = await response.json();

      this.state.hub.invoke("OpenDrawing", data.data.id);

      this.setState({
        drawingData: data.data,
        loading: false,
        currentView: data.data.views[0].id,
        views: new Map(data.data.views.map(view => {return [view.id, view]}))
      }, () => {
        this.sizeStage();
      });
    } else if (response.status === 401) {
      alert("Access denied");
    }

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
        tooltipVisible: false
      })
      this.addStructure();
    }
  }

  async addStructure() {
    let points = [];
    for (let i = 0; i < this.state.newLinePoints.length; i+=2) {
      points.push({x: this.state.newLinePoints[i], y: this.state.newLinePoints[i+1]})
    }

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
      const nextViews = new Map(prevState.views);
      const curView = nextViews.get(structure.view.id)

      const newView = {
        ...curView,
        structures: [...curView.structures, structure]
      };

      return {
        views: nextViews.set(structure.view.id, newView)
      };
    });
  }

  addStructureSuccess(structure) {
    this.insertNewStructure(structure);
    this.setState({newLinePoints: []});
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
      this.setState({selectedTool: "none"});
    } else {
      this.setState({selectedTool: tool});
    }
  }

  sizeStage() {
    const container = document.getElementById("stage-container");
    this.setState({
      stageWidth: container.clientWidth,
      stageHeight: container.clientHeight,
      stageX: container.clientWidth / this.state.stageScale,
      stageY: container.clientHeight / this.state.stageScale,
    }, () => {
      this.renderGrid();
    });

  }

  handleDrag(event) {
    console.log("DRAGGING");
    const pos = event.target.position();
    const snapPos = DrawingUtils.getNearestSnapPos(pos, this.state.snapGridSize);
    this.setState({
      tooltipPos: {x: pos.x - 0.5, y: pos.y - 0.5},
      tooltipText: "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")",
      tooltipVisible: true
    });
  }

  handleDragEnd(event) {
    const pos = event.target.position();
    event.target.position(DrawingUtils.getNearestSnapPos(pos, this.state.snapGridSize));
    event.target.getLayer().draw();
    this.setState({tooltipVisible: false});
  }

  zoom(event) {
    event.evt.preventDefault();

    const newScale = event.evt.deltaY < 0 ? this.state.stageScale * 1.25 : this.state.stageScale / 1.25;
    this.setState({stageScale: newScale});
  }

  renderGrid() {
    let grid = [];
    for (let x = 0; x < 2 * this.state.stageX; x+=this.state.gridSize) {
      grid.push([x, 0, x, 2 * this.state.stageY]);
    }

    for (let y = 0; y < 2 * this.state.stageY; y+=this.state.gridSize) {
      grid.push([0, y, 2 * this.state.stageX, y]);
    }
    this.setState({grid: grid});
  }
}