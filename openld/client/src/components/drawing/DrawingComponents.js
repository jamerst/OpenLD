import React, { Component } from "react";
import { Layer, Line} from "react-konva";
import { DrawingUtils } from "./DrawingUtils";

export class View extends Component {
  render() {
    if (typeof(this.props.data) === "undefined" || typeof(this.props.data.structures) === "undefined") {
      return null;
    }

    return (<Layer>
      {this.props.data.structures.map(structure => {
        return (
          <Structure
            key={"s-" + structure.id}
            id={structure.id}
            points={structure.geometry.points}
            snapGridSize = {this.props.snapGridSize}
            setTooltip = {this.props.setTooltip}
            updatePoints = {this.props.updatePoints}
            onDragStart = {this.props.onDragStart}
            onDragMove = {this.props.onDragMove}
            onDragEnd = {this.props.onDragEnd}
          />
        )
      })}
    </Layer>);
  }
}

export class Structure extends Component {
  constructor(props) {
    super(props);

    this.state = {
      originalPos: {x: 0, y: 0}
    }

    this.handleDragStart = this.handleDragStart.bind(this);
    this.handleDrag = this.handleDrag.bind(this);
    this.handleDragEnd = this.handleDragEnd.bind(this);
  }

  render() {
    let points = typeof this.props.points !== "undefined" ?
    [].concat.apply([], this.props.points.map(p => [p.x, p.y]))
    : [];

    return (
      <Line
        key={"l-" + this.props.id}
        points={points}
        stroke = "#000"
        strokeWidth = {0.05}
        draggable
        onDragStart = {this.handleDragStart}
        onDragMove = {this.handleDrag}
        onDragEnd = {this.handleDragEnd}
      />
    );
  }

  handleDragStart(event) {
    this.setState({startPos: event.target.position()});
  }

  handleDrag(event) {
    const pos = event.target.position();
    const change = DrawingUtils.getDifference(pos, this.state.startPos);
    const points = event.target.points();
    const newPoint = {x: points[0] + change.x, y: points[1] + change.y};

    const snapPos = DrawingUtils.getNearestSnapPos(newPoint, this.props.snapGridSize);

    this.props.setTooltip({x: newPoint.x, y: newPoint.y + 0.5}, true, "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")");
  }

  handleDragEnd(event) {
    const snapPos = DrawingUtils.getNearestSnapPos(event.target.position(), this.props.snapGridSize);
    const change = DrawingUtils.getDifference(snapPos, this.state.startPos);

    const newPoints = DrawingUtils.movePoints(this.props.points, change);

    this.props.updatePoints(null, this.props.id, newPoints);
    event.target.position({x: 0, y: 0});

    event.target.getLayer().draw();
    this.props.setTooltip({x: 0, y: 0}, false, "");
  }
}

export class Grid extends Component {
  render() {
    if (!this.props.enabled) {
      return null;
    }

    let grid = [];
    for (let x = 0; x < this.props.xLim; x+=this.props.gridSize) {
      grid.push([x, 0, x, this.props.yLim]);
    }

    for (let y = 0; y < this.props.yLim; y+=this.props.gridSize) {
      grid.push([0, y, this.props.xLim, y]);
    }

    return (
      <Layer>
        {grid.map((line, index) => {
          return (
            <Line
              key = {"grid-" + index}
              points = {line}
              stroke = "#ddd"
              strokeWidth = {this.props.lineWidth}
            />
          );
        })}
      </Layer>
    );
  }
}