import React, { Component } from "react";
import { Layer, Line, Stage } from 'react-konva';

import { DrawingUtils } from './DrawingUtils';
import { View, Grid, Tooltip } from "./DrawingComponents";

export class Drawing extends Component {
  constructor(props) {
    super(props);

    this.state = {
      stageCursor: "grab",
      newLinePoints: [0, 0],
      newLinePos: {x: 0, y: 0},
      lastLinePoint: [],
      nextLinePoint: [],
      tooltipVisible: false,
      tooltipPos: {x: 0, y: 0},
      tooltipText: ""
    };

    this.handleKeyUp = this.handleKeyUp.bind(this);

    this.handleStageClick = this.handleStageClick.bind(this);
    this.handleStageDblClick = this.handleStageDblClick.bind(this);
    this.handleStageMouseMove = this.handleStageMouseMove.bind(this);
    this.handleStageWheel = this.handleStageWheel.bind(this);

    this.setTooltip = this.setTooltip.bind(this);
  }

  componentDidMount() {
    window.addEventListener("keyup", this.handleKeyUp);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.handleKeyUp);
  }

  render() {
    return (
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
        onDragStart = {() => this.props.setCursor("grabbing")}
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
              stroke = "#ddd"
              strokeWidth = {0.05}
            />
        </Layer>
        <View
            data={this.props.viewData}
            snapGridSize = {this.props.snapGridSize}
            updatePoints = {this.props.onMoveStructure}
            setTooltip = {this.setTooltip}
            setCursor = {this.props.setCursor}
            scale = {this.props.scale}
        />
        <Layer>
          <Tooltip
            position = {this.state.tooltipPos}
            visible = {this.state.tooltipVisible}
            text = {this.state.tooltipText}
            scale = {1.25 / this.props.scale}
          />
        </Layer>
      </Stage>
    );
  }

  handleStageClick(event) {
    if (this.props.selectedTool === "polygon") {
      const stage = event.target.getStage();
      const point = DrawingUtils.getNearestSnapPos(DrawingUtils.getRelativePointerPos(stage), this.props.snapGridSize);

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
    }
  }

  handleStageDblClick(event) {
    if (this.props.selectedTool === "polygon") {
      this.setState({
        lastLinePoint: [],
        nextLinePoint: [],
        tooltipVisible: false,
        stageCursor: "grab"
      }, () => {
        this.props.setIsDrawing(false);
        this.props.setTool("none");
      });

      if (this.state.newLinePoints.length > 4) {
        this.props.onCreateStructure(this.state.newLinePoints);
      }

      this.setState({
        newLinePoints: []
      });
    }
  }

  handleStageMouseMove(event) {
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
        tooltipVisible: true,
        nextLinePoint: [snapPos.x, snapPos.y]
      });
    }
  }

  handleStageWheel(event) {
    event.evt.preventDefault();

    const newScale = event.evt.deltaY < 0 ? this.props.scale * 1.25 : this.props.scale / 1.25;
    this.props.setScale(newScale);
  }

  handleKeyUp(event) {
    if (this.props.selectedTool !== "none" && event.keyCode === 27) {
      this.setState({
        lastLinePoint: [],
        nextLinePoint: [],
        tooltipVisible: false,
        stageCursor: "grab",
        newLinePoints: []
      }, () => {
        this.props.setIsDrawing(false);
        this.props.setTool("none");
      });
    }
  }

  setTooltip(pos, visible, text) {
    this.setState({
      tooltipPos: pos,
      tooltipVisible: visible,
      tooltipText: text
    });
  }
}