import React, { Component, Fragment } from "react";
import { Layer, Line, Stage } from 'react-konva';

import { DrawingUtils } from './DrawingUtils';
import { View } from "./View";
import { Grid } from "./Grid";
import { Tooltip } from "./Tooltip";
import { DeleteObjectModal } from "./DeleteObjectModal";
import { AddFixtureModal } from "./AddFixtureModal";

export class Drawing extends Component {
  constructor(props) {
    super(props);

    this.state = {
      stageCursor: "grab",
      newLinePoints: [0, 0],
      newLinePos: {x: 0, y: 0},
      lastLinePoint: [],
      nextLinePoint: [],
      tooltipPos: {x: 0, y: 0},
      tooltipText: "",
      deleteModalOpen: false,
      validPosition: false,
      addFixtureModalOpen: false,
      newFixtureStructure: "",
      newFixturePos: {x: 0, y: 0},
      selectedFixtureStructure: ""
    };
  }

  componentDidMount = () => {
    window.addEventListener("keyup", this.handleKeyUp);
  }

  componentWillUnmount = () => {
    window.removeEventListener("keyup", this.handleKeyUp);
  }

  render = () => {
    return (
      <Fragment>
        <Stage
          x = {0}
          y = {0}
          width = {this.props.width}
          height = {this.props.height}
          scale = {{x: this.props.scale, y: -this.props.scale}}
          offsetY = {this.props.y}
          draggable
          position = {this.props.position}

          onWheel = {this.handleStageWheel}
          onMouseMove = {this.handleStageMouseMove}
          onClick = {this.handleStageClick}
          onDblClick = {this.handleStageDblClick}
          onDragStart = {() => this.props.setCursor("move")}
          onDragEnd = {() => this.props.setCursor("grab")}

          style={{cursor: this.props.cursor}}
        >
          <Grid
            enabled = {this.props.gridEnabled}
            xLim = {this.props.viewData.width}
            yLim = {this.props.viewData.height}
            gridSize = {this.props.gridSize}
            lineWidth = {1 / this.props.scale}
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
                stroke = "#999"
                strokeWidth = {0.05}
              />
          </Layer>
          <View
              data={this.props.viewData}
              snapGridSize = {this.props.snapGridSize}
              updatePoints = {this.props.onMoveStructure}
              setTooltip = {this.setTooltip}
              setCursor = {this.props.setCursor}
              setHintText = {this.props.setHintText}
              scale = {this.props.scale}
              onSelectObject = {this.props.onSelectObject}
              onStructureDelete = {this.toggleDeleteModal}
              deselectObject = {this.props.deselectObject}
              selectedObjectId = {this.props.selectedObjectId}
              selectedObjectType = {this.props.selectedObjectType}
              setStructureColour = {this.props.setStructureColour}
              setFixtureColour = {this.props.setFixtureColour}
              selectedFixtureStructure = {this.state.selectedFixtureStructure}
              setSelectedFixtureStructure = {this.setSelectedFixtureStructure}
              hubConnected = {this.props.hubConnected}
              selectedTool = {this.props.selectedTool}
              setTool = {this.props.setTool}
              onFixturePlace = {this.onFixturePlace}
              onLoad = {() => void(0)}
          />
          <Layer>
            <Tooltip
              position = {this.state.tooltipPos}
              visible = {this.props.tooltipVisible}
              text = {this.state.tooltipText}
              scale = {1.25 / this.props.scale}
            />
          </Layer>
        </Stage>
        <DeleteObjectModal
          selectedId = {this.props.selectedObjectId}
          type = {this.props.selectedObjectType}
          hub = {this.props.hub}
          isOpen = {this.state.deleteModalOpen}
          toggle = {this.toggleDeleteModal}
        />
        <AddFixtureModal
          isOpen = {this.state.addFixtureModalOpen}
          toggle = {this.toggleAddFixtureModal}
          onChoose = {this.addFixture}
        />
      </Fragment>
    );
  }

  handleStageClick = (event) => {
    if (this.props.hubConnected === true && event.evt.detail === 1) {
      if (this.props.selectedTool === "polygon") {
        const stage = event.target.getStage();
        const point = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize);

        if (point.x === this.state.lastLinePoint[0] && point.y === this.state.lastLinePoint[1]) {
          return;
        }

        if (point.x >= 0 && point.x <= this.props.viewData.width &&
        point.y >= 0 && point.y <= this.props.viewData.height) {
          if (this.props.isDrawing === true) {
            this.setState({
              newLinePoints: [...this.state.newLinePoints, ...[point.x, point.y]],
              lastLinePoint: [point.x, point.y]
            });
          } else {
            this.setState({
              newLinePoints: [point.x, point.y],
              newLinePos: [point.x, point.y],
              lastLinePoint: [point.x, point.y]
            }, () => {this.props.setIsDrawing(true)});
          }
        }
      } else if (this.props.selectedObjectId !== "") {
        this.props.deselectObject(this.state.selectedFixtureStructure);
        this.props.setHintText("");
      }
    }
  }

  handleStageDblClick = (event) => {
    if (this.props.selectedTool === "polygon" && this.props.hubConnected) {
      this.setState({
        lastLinePoint: [],
        nextLinePoint: [],
        stageCursor: "grab"
      }, () => {
        this.props.setTooltipVisible(false);
        this.props.setIsDrawing(false);
        this.props.setTool("none");
      });

      let points;

      if (this.state.newLinePoints.length > 2) {
        points = DrawingUtils.arrayPointsToObject(this.state.newLinePoints);
      } else {
        const stage = event.target.getStage();
        points = [DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize)];
      }

      this.props.hub.invoke(
        "AddStructure",
        {
          view: {id: this.props.viewData.id},
          geometry: {points: points}
        }
      ).catch(err => console.error(err));

      this.setState({
        newLinePoints: []
      });
      this.props.setHintText("");
    }
  }

  handleStageMouseMove = (event) => {
    if (this.props.hubConnected === true) {
      if (this.props.selectedTool === "polygon") {
        const stage = event.target.getStage();
        const snapPos = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize);

        if (snapPos.x < 0 || snapPos.x > this.props.viewData.width ||
        snapPos.y < 0 || snapPos.y > this.props.viewData.height) {
            this.props.setCursor("not-allowed");
        } else {
          this.props.setCursor("crosshair");
        }
        this.setState({
          tooltipPos: {x: snapPos.x - 0.5, y: snapPos.y - 0.5},
          tooltipText: "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")",
          nextLinePoint: [snapPos.x, snapPos.y]
        });
        this.props.setTooltipVisible(true);
      }
    }
  }

  handleStageWheel = (event) => {
    event.evt.preventDefault();

    const newScale = event.evt.deltaY < 0 ? this.props.scale * 1.25 : this.props.scale / 1.25;
    this.props.setScale(newScale);
  }

  handleKeyUp = (event) => {
    if (this.props.selectedTool !== "none" && event.keyCode === 27) {
      this.setState({
        lastLinePoint: [],
        nextLinePoint: [],
        newLinePoints: []
      }, () => {
        this.props.setTooltipVisible(false);
        this.props.setIsDrawing(false);
        this.props.setTool("none");
        this.props.setCursor("grab");
        this.props.setHintText("");
      });
    } else if ((this.props.selectedObjectType === "structure" || this.props.selectedObjectType === "fixture") && event.keyCode === 46) {
      if (this.props.hubConnected === true && this.state.deleteModalOpen === false) {
        this.toggleDeleteModal();
      }
    }
  }

  onFixturePlace = (structureId, position) => {
    this.setState({newFixtureStructure: structureId, newFixturePos: position, addFixtureModalOpen: true});
  }

  addFixture = (fixtureId) => {
    if (this.props.hubConnected) {
      this.props.hub.invoke(
        "AddFixture",
        {
          fixture: {id: fixtureId},
          structure: {id: this.state.newFixtureStructure},
          position: this.state.newFixturePos
        }
      ).catch(err => console.error(err));
    }
  }

  getStructure = (structureId) => {
    return this.props.viewData.structures.find(s => s.id === structureId);
  }

  setTooltip = (pos, visible, text) => {
    this.props.setTooltipVisible(visible);
    this.setState({
      tooltipPos: pos,
      tooltipText: text
    });
  }

  setSelectedFixtureStructure = (id) => {
    this.setState({selectedFixtureStructure: id});
  }

  toggleDeleteModal = () => {
    this.setState({deleteModalOpen: !this.state.deleteModalOpen});
  }

  toggleAddFixtureModal = () => {
    this.setState({addFixtureModalOpen: !this.state.addFixtureModalOpen});
  }
}