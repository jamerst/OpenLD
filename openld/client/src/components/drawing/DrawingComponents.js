import React, { Component } from "react";
import { Group, Layer, Line, Rect, Label, Tag, Text as KonvaText, Circle } from "react-konva";
import { DrawingUtils } from "./DrawingUtils";
import { Text } from "./KonvaNodes";

export class View extends Component {
  render = () => {
    if (typeof(this.props.data) === "undefined" || !this.props.data.structures) {
      return null;
    }

    return (<Layer>
      {this.props.data.structures.map(structure => {
        return (
          <Structure
            key={"s-" + structure.id}
            id={structure.id}
            points={structure.geometry.points}
            name={structure.name}
            snapGridSize = {this.props.snapGridSize}
            setTooltip = {this.props.setTooltip}
            setHintText = {this.props.setHintText}
            updatePoints = {this.props.updatePoints}
            onDragStart = {this.props.onDragStart}
            onDragMove = {this.props.onDragMove}
            onDragEnd = {this.props.onDragEnd}
            setCursor = {this.props.setCursor}
            scale = {this.props.scale}
            onStructureSelect = {this.props.onStructureSelect}
            onStructureDelete = {this.props.onStructureDelete}
            deselectObject = {this.props.deselectObject}
            selected = {this.props.selectedObjectId === structure.id && this.props.selectedObjectType === "structure"}
            setStructureColour = {this.setStructureColour}
            colour = {structure.colour}
            hubConnected = {this.props.hubConnected}
            selectedTool = {this.props.selectedTool}
            setTool = {this.props.setTool}
            setValidPosition = {this.props.setValidPosition}
            setHoveredStructure = {this.props.setHoveredStructure}
          />
        )
      })}
    </Layer>);
  }

  setStructureColour = (id, colour) => {
    this.props.setStructureColour(this.props.data.id, id, colour);
  }
}

export class Structure extends Component {
  constructor(props) {
    super(props);

    this.state = {
      startPos: {x: 0, y: 0},
      singlePoint: this.props.points !== null && this.props.points.length === 1
    }
  }

  render = () => {
    const points = this.props.points !== null && typeof this.props.points !== "undefined" ?
      [].concat.apply([], this.props.points.map(p => [p.x, p.y]))
      : [];

    const angle = this.props.points !== null && typeof this.props.points !== "undefined" ?
      DrawingUtils.lineAngle(this.props.points[1], this.props.points[0])
      : 0;

    let structure;

    if (this.state.singlePoint) {
      structure = (
        <Circle
          key = {"c-" + this.props.id}
          x = {points[0]}
          y = {points[1]}
          fill = {this.props.colour}
          radius = {this.props.selected ? 0.3 : 0.2}
          strokeWidth = {0}
          hitStrokeWidth = {1}
          onMouseOver = {this.onMouseOver}
          onMouseOut = {this.onMouseOut}
          onClick = {this.onClick}
        />
      )
    } else {
      structure = (
        <Line
          key = {"l-" + this.props.id}
          points = {points}
          stroke = {this.props.colour}
          strokeWidth = {this.props.selected ? 0.1 : 0.06}
          hitStrokeWidth = {1}
          onMouseOver = {this.onMouseOver}
          onMouseOut = {this.onMouseOut}
          onClick = {this.onClick}
        />
      )
    }

    return (
      <Group
        draggable = {this.props.hubConnected}
        onDragStart = {this.handleDragStart}
        onDragMove = {this.handleDrag}
        onDragEnd = {this.handleDragEnd}
      >
        {structure}
        <Text
          key={"sl-" + this.props.id}
          x = {points[0]}
          y = {points[1] - 0.1}
          rotation = {angle}
          padding = {2}
          text = {this.props.name}
          textScale = {0.05}
          fill = "#000"
        />
      </Group>
    );
  }

  handleDragStart = (event) => {
    this.setState({startPos: event.target.position()});
    this.props.setHintText("Release to confirm position");
  }

  handleDrag = (event) => {
    const pos = event.target.position();
    const change = DrawingUtils.getDifference(pos, this.state.startPos);
    const newPoint = {x: this.props.points[0].x + change.x, y: this.props.points[0].y + change.y};

    const snapPos = DrawingUtils.getNearestSnapPos(newPoint, this.props.snapGridSize);

    this.props.setTooltip({x: newPoint.x, y: newPoint.y + 25 / this.props.scale}, true, "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")");
  }

  handleDragEnd = (event) => {
    const snapPos = DrawingUtils.getNearestSnapPos(event.target.position(), this.props.snapGridSize);
    const change = DrawingUtils.getDifference(snapPos, this.state.startPos);

    const newPoints = DrawingUtils.movePoints(this.props.points, change);

    this.props.updatePoints(null, this.props.id, newPoints);
    event.target.position({x: 0, y: 0});

    event.target.getLayer().draw();
    this.props.setTooltip({x: 0, y: 0}, false, "");
    this.props.setHintText("");
  }

  onMouseOver = (event) => {
    if (this.state.singlePoint) {
      event.target.radius(0.3);
    } else {
      event.target.strokeWidth(0.1);
    }

    event.target.draw();
    this.props.setCursor("pointer");
    if (this.props.selectedTool === "none") {
      this.props.setHintText("Click to select structure.\nClick and hold to move structure.")
    } else if (this.props.selectedTool === "add-fixture") {
      console.log(event);
      this.props.setValidPosition(true);
      this.props.setHoveredStructure(this.props.id);
    } else if (this.props.selectedTool === "eraser") {
      this.props.setHintText("Click to remove structure.")
    }
  }

  onMouseOut = (event) => {
    if (!this.props.selected) {
      if (this.state.singlePoint) {
        event.target.radius(0.2);
      } else {
        event.target.strokeWidth(0.06);
      }

      // prevent exception when object deleted whilst hovering on
      try {
        event.target.draw();
      } catch {}
      this.props.setHintText("");
    }

    if (this.props.selectedTool === "add-fixture") {
      this.props.setValidPosition(false);
      this.props.setHoveredStructure("");
    } else {
      this.props.setCursor("grab");
    }

  }

  onClick = (event) => {
    if (this.props.hubConnected) {
      event.cancelBubble = true;
      this.props.deselectObject();
      this.props.onStructureSelect(this.props.id);

      if (this.props.selectedTool === "eraser") {
        this.props.onStructureDelete();
        this.props.setTool("none");
      } else if (this.props.selectedTool === "none") {
        this.props.setStructureColour(this.props.id, "#007bff");
        this.props.setHintText("Modify structure properties above.\nPress delete to remove structure.")
      }
    }
  }
}

export class Grid extends Component {
  render = () => {
    if (!this.props.enabled) {
      return (
        <Layer>
          <Rect
            x = {0}
            y = {0}
            width = {this.props.xLim}
            height = {this.props.yLim}
            fill = "#fff"
          />
        </Layer>
      )
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
        <Rect
          x = {0}
          y = {0}
          width = {this.props.xLim}
          height = {this.props.yLim}
          fill = "#fff"
        />
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

export class Tooltip extends Component {
  render = () => {
    return (
      <Label
        position = {this.props.position}
        scale = {{x: this.props.scale, y: -this.props.scale}}
        visible = {this.props.visible}
      >
        <Tag fill="#ddd"/>
        <KonvaText
          key = "tooltip"
          text = {this.props.text}
          fill = "#000"
          padding = {2}
        />
      </Label>
    )
  }
}