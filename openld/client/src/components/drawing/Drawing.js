import React, { Component, Fragment } from "react";
import { Layer, Line, Stage } from 'react-konva';

import { DrawingUtils, Ops } from './DrawingUtils';
import { View } from "./View";
import { Grid } from "./Grid";
import { Tooltip } from "./Tooltip";
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

    this.stageRef = React.createRef();
    this.copiedObject = {type: "", data: null}
    this.lodash = require('lodash/lang');
  }

  componentDidMount = () => {
    window.addEventListener("keydown", this.handleKeyDown);
  }

  componentWillUnmount = () => {
    window.removeEventListener("keydown", this.handleKeyDown);
  }

  render = () => {
    return (
      <Fragment>
        <Stage
          ref = {this.stageRef}
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
              onMoveStructure = {this.props.onMoveStructure}
              onMoveFixture = {this.props.onMoveFixture}
              onMoveLabel = {this.props.onMoveLabel}
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
              setLabelColour = {this.props.setLabelColour}
              selectedFixtureStructure = {this.state.selectedFixtureStructure}
              setSelectedFixtureStructure = {this.setSelectedFixtureStructure}
              hubConnected = {this.props.hubConnected}
              selectedTool = {this.props.selectedTool}
              setTool = {this.props.setTool}
              onFixturePlace = {this.onFixturePlace}
              onSymbolLoad = {this.props.onSymbolLoad}
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
      } else if (this.props.selectedTool === "add-label") {
        const stage = event.target.getStage();
        const point = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize);

        this.addLabel(point);
      } else if (this.props.selectedObjectId !== "") {
        this.props.deselectObject(this.state.selectedFixtureStructure);
        this.props.setHintText("");
      }
    }
  }

  handleStageDblClick = async (event) => {
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

      let result = {success: false};
      result = await this.props.hub.invoke(
        "AddStructure",
        {
          view: {id: this.props.viewData.id},
          geometry: {points: points}
        }
      ).catch(err => {console.error(err); result.success = false});

      if (result && result.success) {
        this.props.pushHistoryOp({type: Ops.ADD_STRUCTURE, data: result.data});
      } else {
        this.props.setAlertError("Failed to insert new structure")
      }

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
          tooltipText: `(${snapPos.x.toFixed(1)},${snapPos.y.toFixed(1)})`,
          nextLinePoint: [snapPos.x, snapPos.y]
        });
        this.props.setTooltipVisible(true);
      } else if (this.props.selectedTool === "add-label") {
        const stage = event.target.getStage();
        const snapPos = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize);

        if (snapPos.x < 0 || snapPos.x > this.props.viewData.width ||
        snapPos.y < 0 || snapPos.y > this.props.viewData.height) {
            this.props.setCursor("not-allowed");
        } else {
          this.props.setCursor("text");
        }

        this.setState({
          tooltipPos: {x: snapPos.x - 0.5, y: snapPos.y - 0.5},
          tooltipText: `(${snapPos.x.toFixed(1)},${snapPos.y.toFixed(1)})`
        });
        this.props.setTooltipVisible(true);
      }
    }
  }

  handleStageWheel = (event) => {
    event.evt.preventDefault();

    let newScale = event.evt.deltaY < 0 ? this.props.scale * 1.25 : this.props.scale / 1.25;
    if (newScale < 8) {
      newScale = 8;
    }
    this.props.setScale(newScale);
  }

  handleKeyDown = async (event) => {
    if (this.props.hubConnected) {
      if (this.props.selectedTool !== "none" && event.keyCode === 27) { // ESC and no tool selected
        this.cancelCreateStructure();
      } else if ((this.props.selectedObjectType === "structure" || this.props.selectedObjectType === "fixture" || this.props.selectedObjectType === "label") && event.keyCode === 46) { // DELETE and object selected
        this.deleteObject();
      } else if (event.keyCode === 67 && event.ctrlKey) { // ctrl+c
        this.copyObject();
      } else if (event.keyCode === 86 && event.ctrlKey) { // ctrl+v
        this.pasteObject();
      }
    }
  }

  cancelCreateStructure = () => {
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
  }

  deleteObject = async () => {
    if (this.props.hubConnected === true) {
      let result = {success: false};
      result = await this.props.hub.invoke(
        "DeleteObject",
        this.props.selectedObjectType,
        this.props.selectedObjectId
      ).catch(err => {console.error(err); result.success = false});

      if (result && result.success) {
        this.props.onRemoveObject(result.data.type, result.data.id, result.data.viewId, result.data.structureId, false);
      } else {
        this.props.setAlertError(`Failed to delete ${result.data ? result.data : "object"}`)
      }
    }
  }

  copyObject = () => {
    if (this.props.selectedObjectType === "structure") {
      this.copiedObject = {type: this.props.selectedObjectType, data: this.props.getStructure(this.props.currentView, this.props.selectedObjectId)};
    } else if (this.props.selectedObjectType === "fixture") {
      this.copiedObject = {type: this.props.selectedObjectType, data: this.props.getFixture(this.props.currentView, this.state.selectedFixtureStructure, this.props.selectedObjectId)};
    } else if (this.props.selectedObjectType === "label") {
      this.copiedObject = {type: this.props.selectedObjectType, data: this.props.getLabel(this.props.currentView, this.props.selectedObjectId)};
    } else {
      this.props.setAlertIcon("info", "Select an object to copy", "info");
    }
  }

  pasteObject = async () => {
    if (this.copiedObject.type === "structure") {
      let newStructure = this.lodash.cloneDeep(this.copiedObject.data);
      // find nearest snap point to current pointer location, and change required to move original to that point
      const snapPos = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(this.stageRef.current), this.props.snapGridSize);
      const change = DrawingUtils.getDifference(snapPos, newStructure.geometry.points[0]);

      newStructure.geometry.points = DrawingUtils.movePoints(newStructure.geometry.points, change);
      newStructure.fixtures = DrawingUtils.moveFixtures(newStructure.fixtures, change);
      newStructure.view = {id: this.props.currentView}
      newStructure.id = null;
      newStructure.fixtures.forEach((f, i) => {
        newStructure.fixtures[i].id = null;
      });


      let result = {success: false};
      result = await this.props.hub.invoke(
        "AddStructure",
        newStructure
      ).catch(err => {console.error(err); result.success = false});

      if (result && result.success) {
        this.props.pushHistoryOp({type: Ops.ADD_STRUCTURE, data: result.data});
      } else {
        this.props.setAlertError("Error inserting copied structure")
      }

    } else if (this.copiedObject.type === "fixture") {
      if (this.props.selectedObjectType === "structure") {
        let newFixture = this.lodash.cloneDeep(this.copiedObject.data);
        // get original fixture, and currently selected structure
        let selectedStructure = this.props.getStructure(this.props.currentView, this.props.selectedObjectId);

        // find nearest snap point on selected structure
        const nearestPoint = DrawingUtils.nearestLinePoint(selectedStructure.geometry.points, DrawingUtils.getRelativePointerPos(this.stageRef.current));
        const snapPos = DrawingUtils.getNearestSnapPos(nearestPoint, this.props.snapGridSize);

        newFixture.id = null;
        newFixture.position = snapPos;
        newFixture.structure = {id: this.props.selectedObjectId};

        let result = {success: false};
        result = await this.props.hub.invoke(
          "AddFixture",
          newFixture
        ).catch(err => {console.error(err); result.success = false});

        if (result && result.success) {
          this.props.pushHistoryOp({type: Ops.ADD_FIXTURE, data: result.data});
        } else {
          this.props.setAlertError("Error inserting copied fixture")
        }
      } else {
        this.props.setAlertIcon("info", "Select a structure to add copied fixture", "info");
      }
    } else if (this.copiedObject.type === "label") {
      let newLabel = this.lodash.cloneDeep(this.copiedObject.data);
      // find nearest snap point to current pointer location
      newLabel.position = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(this.stageRef.current), this.props.snapGridSize);
      newLabel.id = null;
      newLabel.view = {id: this.props.currentView};

      let result = {success: false};
      result = await this.props.hub.invoke(
        "AddLabel",
        newLabel
      ).catch(err => {console.error(err); result.success = false});

      if (result.success === true) {
        this.props.pushHistoryOp({type: Ops.ADD_LABEL, data: result.data});
      }
    }
  }

  onFixturePlace = (structureId, position) => {
    this.setState({newFixtureStructure: structureId, newFixturePos: position, addFixtureModalOpen: true});
  }

  addFixture = async (fixtureId) => {
    if (this.props.hubConnected) {
      let result = {success: false};
      result = await this.props.hub.invoke(
        "AddFixture",
        {
          fixture: {id: fixtureId},
          structure: {id: this.state.newFixtureStructure},
          position: this.state.newFixturePos
        }
      ).catch(err => {console.error(err); result.success = false});

      if (result && result.success) {
        this.props.pushHistoryOp({type: Ops.ADD_FIXTURE, data: result.data});
      } else {
        this.props.setAlertError("Failed to insert new fixture");
      }
    }
  }

  addLabel = async (point) => {
    if (this.props.hubConnected) {
      let result = {success: false};
      result = await this.props.hub.invoke(
        "AddLabel",
        {
          view: {id: this.props.currentView},
          position: point,
          text: "Testing"
        }
      ).catch(err => {console.error(err); result.success = false});

      if (result.success === true) {
        this.props.pushHistoryOp({type: Ops.ADD_LABEL, data: result.data});
      }
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